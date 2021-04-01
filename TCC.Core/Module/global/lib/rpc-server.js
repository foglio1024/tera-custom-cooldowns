const { Helpers } = require("./helpers");
const { RpcHandler } = require("./rpc-handler");
const http = require("http");

class RpcServer
{
    debug(msg)
    {
        //if (!this.mod.settings.debug) return;
        //this.mod.command.message(`<font color="#fff1b5">${msg}</font>`);
        //this.mod.log(`${msg}`);
    }

    setNetworkMod(networkMod) {
        this.handler.setNetworkMod(networkMod);
    }

    constructor(mod)
    {
        this.mod = mod;
        
        this.debug("Creating rpc server");
        this.isRunning = false;
        this.handler = new RpcHandler(mod);
        this.server = http.createServer((req, res) =>
        {
            if (req.method !== "POST") return;
            let body = "";
            req.on("data", chunk => body += chunk.toString());
            req.on("error", err => {
                if(err.errno === "ECONNREFUSED"){
                    this.mod.error(`Failed to send data to TCC (error:${err.errno}). Make sure that:\n\t1. TCC is running and working correctly\n\t2. Toolbox is enabled in TCC System settings.`);
                }
                else {
                    this.mod.error(`Failed to send data to TCC (error:${err.errno}).`);
                }
            });
            req.on("end", async () =>
            {
                let rpcRequest = JSON.parse(body);
                var rpcResult = null;
                var respType = "result";
                try
                {
                    rpcResult = await this.handler.handle(rpcRequest);
                }
                catch (error)
                {
                    respType = "error";
                    rpcResult = {
                        'code': -1,
                        'message': error
                    };
                    this.mod.error(`Error while executing command ${JSON.stringify(rpcRequest)}; error: ${error}`);
                }
                let jsonResponse = Helpers.buildResponse(rpcResult, rpcRequest.id, respType);
                let stringResponse = JSON.stringify(jsonResponse);
                //this.mod.log("Sending response of "+ rpcRequest.method +": " + stringResponse);
                res.writeHead(200, Helpers.buildHeaders(stringResponse.length));
                res.write(stringResponse);
                res.end();
            });
        });
        this.server.on("error", err =>
        {
            if (err.errno == "EADDRINUSE")
            {
                this.mod.error(`Cannot connect to TCC (${err}). This might be caused by multi-clienting.`);
            }
            else if (err.errno == "ECONNREFUSED")
            {
                this.mod.error(`Cannot connect to TCC (${err}). TCC didn't might not be running.`);
            }
        });

    }

    start()
    {
        this.debug("Starting rpc server");
        this.server.listen(9550, "127.0.0.52", () => { });
    }
    stop()
    {
        this.debug("Stopping rpc server");
        this.server.removeAllListeners();
        this.server.close();
    }
}
exports.RpcServer = RpcServer;
