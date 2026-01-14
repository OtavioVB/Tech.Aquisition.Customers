import http from "k6/http";
import ws from "k6/ws";
import { check, sleep } from "k6";
import { Trend, Rate } from "k6/metrics";

const handshakeTrend = new Trend("signalr_handshake_ms");
const connectFailRate = new Rate("signalr_connect_fail");
const negotiateFailRate = new Rate("signalr_negotiate_fail");

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
    signalr_connect_fail: ["rate<0.01"],
    signalr_negotiate_fail: ["rate<0.01"],
  },
};

const BASE_URL = "ws://localhost:8080";
const HUB_PATH = "/hubs/aquisitions/customers";
const TOKEN = "6962dad3-7404-8325-a0d0-d5c8a8079ae8";

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
  negotiateFailRate.add(0);

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
    headers: {
      "Authorization": `${TOKEN}`
    },
  };

  const start = Date.now();

  const res = ws.connect(wsUrl, params, (socket) => {
    let handshaked = false;

    socket.on("open", () => {
      socket.send('{"protocol":"json","version":1}\x1e');
    });

    socket.on("message", (data) => {
      if (!handshaked) {
        handshaked = true;
        handshakeTrend.add(Date.now() - start);
      }
    });

    socket.on("error", (e) => {
      connectFailRate.add(1);
    });

    socket.on("close", (code, reason) => {
        const normal = code === 1000 || code === 1001;

        if (!normal) {
            connectFailRate.add(1);
        }
    });

    socket.setTimeout(() => {
      socket.close();
    }, HOLD_MS);
  });

  const ok = check(res, {
    "ws connected (status 101)": (r) => r && r.status === 101,
  });

  if (!ok) connectFailRate.add(1);
  else connectFailRate.add(0);

  sleep(1);
}
