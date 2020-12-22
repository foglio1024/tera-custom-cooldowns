const http = require('http');
const net = require('net');
const { RpcServer } = require("./rpc-server");
// TODO: move these to settings ------
const address = '127.0.0.51';
const port = 9551;
// -----------------------------------

const connectedClients = [];

class TccInterface
{
    constructor(mod)
    {
        this.nextId = 1;
        this.mod = mod;
        this.server = new RpcServer(mod);
        this.server.start();

        this.interface = new net.Server();
        this.interface.listen(port, address);
        this.interface.on("connection", (socket) => {
            connectedClients.push(socket);

            this.mod.log("[tcc-stub] " + socket.remoteAddress + ":" + socket.remotePort + " connected!");
            socket.on("end", () => {
                this.mod.log("[tcc-stub] " + socket.remoteAddress + ":" + socket.remotePort + " disconnected!")
                connectedClients.splice(connectedClients.indexOf(socket), 1);
            });
            socket.on("error", (err) => {
                this.mod.error("[tcc-stub] " + err);
            });

        });
    }

    call(method, params)
    {
        const request = {
            'jsonrpc': '2.0',
            'method': method,
            'params': params,
            'id': this.nextId++
        };

        const strReq = JSON.stringify(request);

        const len = strReq.length;
        const data = Buffer.alloc(len + 2);
        data.writeUInt16LE(len);
        data.write(strReq,2);
        this.mod.log("Sending " + data.length + " bytes: " + strReq + "\n" + data);
        connectedClients.forEach(socket => {
            socket.write(data);
        });

        //const options = {
        //    hostname: '127.0.0.51',
        //    port: 9551,
        //    method: 'POST',
        //    path: '/',
        //    headers: {
        //        'Content-Type': 'application/json',
        //        'Content-Length': JSON.stringify(request).length
        //    }
        //};
        //try
        //{
        //    const req = http.request(options, (res) => { });
        //    req.on('error', (err) =>
        //    {
        //        if (err.errno === 'ECONNREFUSED')
        //        {
        //            this.mod.error(`Failed to send data to TCC (error:${err.errno}). Make sure that:\n\t1. TCC is running and working correctly\n\t2. Toolbox is enabled in TCC System settings.`);
        //        }
        //        else
        //        {
        //            this.mod.error(`Failed to send data to TCC (error:${err.errno}).`)
        //        }
        //    });
        //    req.write(JSON.stringify(request));
        //    req.end();
        //}
        //catch (err)
        //{
        //    this.mod.log(`Error in TCC interface: ${err}`);
        //}
    }

    stopServer()
    {
        connectedClients.forEach(socket => {
            socket.destroy();
        });
        this.interface.close();
        this.server.stop();
        connectedClients.splice(0, connectedClients.length);
    }
}
exports.TccInterface = TccInterface;
