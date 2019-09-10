using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TCC.Interop.JsonRPC;
using TCC.Utilities;

namespace TCC.Interop.Proxy
{
    public class ToolboxHttpClient
    {
        private readonly HttpClient _client;
        private readonly string _address;
        public ToolboxHttpClient(string address)
        {
            _address = address;
            _client = new HttpClient();
        }

        private async Task<Response> Send(JObject obj)
        {
            var req = new StringContent(obj.ToString(), Encoding.UTF8, "application/json");

            try
            {
                var resp = await _client.PostAsync(_address, req);
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
        //public void SendResponse(Response resp)
        //{
        //    Send(resp);
        //}


    }
}