using System;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;
using TCC.Interop.JsonRPC;
using TCC.Utils;

namespace TCC.Interop.Proxy
{
    /// <summary>
    /// Listens to tcc-stub commands using a <see cref="HttpListener"/> and fires related events.
    /// </summary>
    public class RpcServer
    {
        public event Action<Response> ResponseReceived;
        public event Action<Request> RequestReceived;

        private readonly HttpListener _server;
        private bool _listening;
        public RpcServer()
        {
            _server = new HttpListener { Prefixes = { "http://127.0.0.51:9551/" } };
        }

        public void Start()
        {
            if (_listening) return;
            _listening = true;
            _server.Start();
            new Thread(Listen).Start();
        }

        public void Stop()
        {
            try
            {
                _server.Stop();
            }
            catch (ObjectDisposedException ex)
            {
                Log.F($"[ObjectDisposedException] {ex.ObjectName} disposed, skipping Stop()..."); 
            }
            _listening = false;
        }
        private void Listen()
        {
            while (_listening)
            {
                try
                {
                    var ctx = _server.GetContext();
                    using var reader = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding);
                    var stringReq = reader.ReadToEnd();
                    var jsonReq = JObject.Parse(stringReq);
                    if (jsonReq.ContainsKey(Request.MethodKey))
                    {
                        RequestReceived?.Invoke(new Request(jsonReq));
                    }
                    else if (jsonReq.ContainsKey(Response.ErrorKey) || jsonReq.ContainsKey(Response.ResultKey))
                    {
                        ResponseReceived?.Invoke(new Response(jsonReq));
                    }
                    else
                    {
                        Log.F($"Received unknown message type: \n{jsonReq}", "http_server.log");
                    }
                    ctx.Response.StatusCode = 200;
                    ctx.Response.Close();
                }
                catch (Exception e)
                {
                    Log.F($"Error while parsing request: {e}", "http_server.log");
                }
            }
            _listening = false;
        }
    }
}
