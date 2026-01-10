import ws from 'k6/ws';
import { check, sleep } from 'k6';

const RECORD_SEPARATOR = '\u001e';

export const options = {
  vus: 50,
  duration: '30s',
};

export default function () {
  const url = 'wss://localhost:8081/hubs/aquisitions/customers';

  const res = ws.connect(url, {}, function (socket) {
    socket.on('open', () => {
      socket.send(JSON.stringify({ protocol: 'json', version: 1 }) + RECORD_SEPARATOR);
      sleep(10);
      socket.close();
    });

    socket.on('message', (raw) => {
      const frames = raw.split(RECORD_SEPARATOR).filter(Boolean);

      for (const f of frames) {
        let msg;
        try { msg = JSON.parse(f); } catch { continue; }

        if (msg.type === 1 && msg.target === 'CreateCustomerAquisitionRequestedNotification') {
            console.log("recebido conforme esperado.");
        }
      }
    });

    socket.on('error', (e) => {
      console.log('ws error', e);
    });
  });

  check(res, { 'upgraded to websocket': (r) => r && r.status === 101 });
}