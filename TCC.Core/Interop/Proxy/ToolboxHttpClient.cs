using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TCC.Interop.JsonRPC;
using TCC.Utils;

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
                Log.F($"Error while sending data to Toolbox: {e}");
                return null;
            }
        }


        public async Task<Response> CallAsync(string methodName, JObject parameters = null)
        {
            return await CallAsync(new Request(methodName, parameters));
        }
        private async Task<Response> CallAsync(Request req)
        {
            var resp = await Send(req);
            if (resp?.Error != null)
            {
                Log.F($"Toolbox RPC call generated an error: {resp.Error["message"]}");
            }
            return resp;
        }
        //public void SendResponse(Response resp)
        //{
        //    Send(resp);
        //}


    }
}