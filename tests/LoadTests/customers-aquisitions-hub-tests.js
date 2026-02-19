import http from "k6/http";
import ws from "k6/ws";
import { check, sleep } from "k6";
import { Trend, Rate } from "k6/metrics";

const handshakeTrend = new Trend("signalr_handshake_ms");
const connectFailRate = new Rate("signalr_connect_fail");
const negotiateFailRate = new Rate("signalr_negotiate_fail");

const wsUpgradeFailRate = new Rate("signalr_ws_upgrade_fail"); // Falha ao receber o Ack do Serviço
const handshakeTimeoutFailRate = new Rate("signalr_handshake_timeout_fail"); // Timeout por não receber a resposta do handshake de mensagem do WebSocket.

const HOLD_MS = 180 * 1000;

export const options = {
    scenarios: {
        capacity_ws: {
            executor: "ramping-vus",
            startVUs: 0,
            stages: [
                { duration: "15s", target: 840 },
                { duration: "30s", target: 840 },

                { duration: "15s", target: 860 },
                { duration: "30s", target: 860 },

                { duration: "15s", target: 880 },
                { duration: "30s", target: 880 },

                { duration: "15s", target: 900 },
                { duration: "30s", target: 900 },

                { duration: "30s", target: 0 },
            ],
            gracefulRampDown: "30s",
        },
    },
    thresholds: {
        signalr_negotiate_fail: ["rate<0.01"],
        signalr_ws_upgrade_fail: ["rate<0.01"],
        signalr_handshake_timeout_fail: ["rate<0.01"],
        signalr_connect_fail: ["rate<0.01"],
    },
};

const BASE_URL = "ws://localhost:8080";
const HUB_PATH = "/hubs/aquisitions/customers";
const TOKEN = "6962dad3-7404-8325-a0d0-d5c8a8079ae8";
const HANDSHAKE_TIMEOUT_MS = 5000;

function negotiate() {
    const url = `http://localhost:8080${HUB_PATH}/negotiate?negotiateVersion=1`;

    const headers = {
        "Content-Type": "application/json",
        "Authorization": `${TOKEN}`
    };

    const res = http.post(url, null, { headers });

    const ok = check(res, {
        "[CreateCustomerAquisitionRequestedNotification][Negotiate] Status 200": (r) => r.status === 200,
    });

    if (!ok) {
        negotiateFailRate.add(1);
        return null;
    }

    return res.json();
}

function buildWsUrl(neg) {
    const wsBase = `${BASE_URL}${HUB_PATH}`;
    return `${wsBase}?id=${encodeURIComponent(neg.connectionToken)}`;
}

export default function () {
    if (__ITER > 0) { sleep(1); return; }

    const neg = negotiate();
    if (!neg) {
        sleep(1);
        return;
    }

    const wsUrl = buildWsUrl(neg);

    const params = {
        headers: { "Authorization": `${TOKEN}` },
    };

    const start = Date.now();

    const res = ws.connect(wsUrl, params, (socket) => {
        let handshakeAcked = false;

        // Configuração de Timeout do Handshake
        const hsTimer = socket.setTimeout(() => {
            if (!handshakeAcked) {
                handshakeTimeoutFailRate.add(1);
                connectFailRate.add(1);
                socket.close(4000, "handshake-timeout");
            }
        }, HANDSHAKE_TIMEOUT_MS);

        socket.on("open", () => {
            // Envio do Ack pelo Cliente
            socket.send('{"protocol":"json","version":1}\x1e');
        });

        socket.on("message", (data) => {
            if (handshakeAcked) return;

            // SignalR delimita mensagens por 0x1e (record separator)
            const frames = String(data).split("\x1e").filter(Boolean);

            // Verifica se recebemos algum Ack do SignalR
            const gotAck = frames.some((f) => f.trim() === "{}");

            if (gotAck) {
                handshakeAcked = true;
                handshakeTrend.add(Date.now() - start);
                handshakeTimeoutFailRate.add(0);
                // conexão está realmente estabelecida.
            }
        });

        socket.on("error", () => {
            wsUpgradeFailRate.add(1);
            connectFailRate.add(1);
        });

        socket.on("close", (code) => {
            const normal = code === 1000 || code === 1001;

            // Se fechou antes do handshake ACK, conta como falha de estabelecer conexão
            if (!normal && !handshakeAcked) {
                wsUpgradeFailRate.add(1);
                connectFailRate.add(1);
            }
        });

        socket.setTimeout(() => socket.close(), HOLD_MS);
    });

    const upgraded = check(res, {
        "[CreateCustomerAquisitionRequestedNotification][SignalR][Handshake] (Status 101)": (r) => r && r.status === 101,
    });

    if (!upgraded) {
        wsUpgradeFailRate.add(1);
        connectFailRate.add(1);
    } else {
        wsUpgradeFailRate.add(0);
    }

    sleep(1);
}
