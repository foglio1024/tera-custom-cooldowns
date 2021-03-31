const net = require("net");
const { RpcServer } = require("./lib/rpc-server");
// TODO: move these to settings ------
const address = "127.0.0.51";
const port = 9551;
// -----------------------------------

const connectedClients = [];

class TccInterface {
    constructor(mod) {
        this.nextId = 1;
        this.mod = mod;
        this.server = new RpcServer(mod);
        this.server.start();

        this.interface = new net.Server();
        this.interface.listen(port, address);
        this.mod.log("Listening on " + address + ":" + port);

        this.interface.on("connection", (socket) => {
            connectedClients.push(socket);

            this.mod.log(socket.remoteAddress + ":" + socket.remotePort + " connected to tcc-stub!");
            socket.on("end", () => {
                this.mod.log(socket.remoteAddress + ":" + socket.remotePort + " disconnected from tcc-stub!");
                connectedClients.splice(connectedClients.indexOf(socket), 1);
            });
            socket.on("error", (err) => {
                this.mod.error(err);
            });

        });
    }

    call(method, params) {
        const request = {
            'jsonrpc': "2.0",
            'method': method,
            'params': params,
            'id': this.nextId++
        };

        const strReq = JSON.stringify(request);

        const len = strReq.length;
        const data = Buffer.alloc(len + 2);
        data.writeUInt16LE(len);
        data.write(strReq, 2);
        //this.mod.log("Sending " + data.length + " bytes: " + strReq + "\n" + data);
        connectedClients.forEach(socket => {
            socket.write(data);
        });
    }

    destructor() {
        connectedClients.forEach(socket => {
            socket.destroy();
        });
        this.interface.close();
        this.server.stop();
        connectedClients.splice(0, connectedClients.length);
    }
}
exports.TccInterface = TccInterface;
