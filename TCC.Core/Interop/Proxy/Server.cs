using System;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCC.Interop.JsonRPC;

namespace TCC.Interop.Proxy
{
    public class Server
    {
        public event Action<Response> ResponseReceived;
        public event Action<Request> RequestReceived;
        private readonly HttpListener _server;
        private bool _listening = false;
        public Server()
        {
            _server = new HttpListener { Prefixes = { "http://127.0.0.51:9551/" } };
        }

        public void Start()
        {
            _listening = true;
            _server.Start();
            new Thread(Listen).Start();
        }

        public void Stop()
        {
            _listening = false;
            _server.Stop();
        }
        private void Listen()
        {
            while (_listening)
            {
                try
                {
                    var ctx = _server.GetContext();
                    using (var reader = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding))
                    {
                        var stringReq = reader.ReadToEnd();
                        var jsonReq = JObject.Parse(stringReq);
                        if (jsonReq.ContainsKey(Request.MethodKey)) RequestReceived?.Invoke(new Request(jsonReq));
                        else if (jsonReq.ContainsKey(Response.ErrorKey) || jsonReq.ContainsKey(Response.ResultKey)) ResponseReceived?.Invoke(new Response(jsonReq));
                        else Log.CW("[Server] Unknown message type.");
                        ctx.Response.StatusCode = 200;
                        ctx.Response.Close();
                    }
                }
                catch (Exception e)
                {
                    Log.CW($"[Server] Error while parsing request: {e}");
                }
            }
        }
    }
}
