using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
using TCC.Interop.JsonRPC;

namespace TCC.Interop.Proxy
{
    public class Client
    {
        private readonly HttpClient _client;
        private const string Address = "http://127.0.0.52:9550";
        public Client()
        {
            _client = new HttpClient();
            //_client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            //_client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=1, max=100");
            //_client.DefaultRequestHeaders.Add("User-Agent", "TCC");
        }

        private async Task<Response> Send(JObject obj)
        {
            var req = new StringContent(obj.ToString(), Encoding.UTF8, "application/json");

            try
            {
                var resp = await _client.PostAsync(Address, req);
                return new Response(JObject.Parse(await resp.Content.ReadAsStringAsync()));
            }
            catch (Exception e)
            {
                Log.CW(e.ToString());
                return null;
            }
        }


        public async Task<Response> CallAsync(Request req)
        {
            //Log.All($"[RPC] {req.Method}()");
            return await Send(req);
        }
        public async Task<Response> CallAsync(string methodName, JObject parameters = null)
        {
            var req = new Request(methodName, parameters);
            return await CallAsync(req);
        }
        public void SendResponse(Response resp)
        {
            Send(resp);
        }


    }
}