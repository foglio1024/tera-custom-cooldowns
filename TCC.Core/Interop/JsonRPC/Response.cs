using Newtonsoft.Json.Linq;

namespace TCC.Interop.JsonRPC
{
    public class Response : JObject
    {
        public const string ResultKey = "result";
        public const string ErrorKey = "error";

        public JToken Result => this[ResultKey];
        public JToken Error => this[ErrorKey];

        public Response(JObject jObj)
        {
            this["jsonrpc"] = jObj["jsonrpc"];
            if (jObj.ContainsKey(ResultKey)) this[ResultKey] = jObj[ResultKey];
            else if (jObj.ContainsKey(ErrorKey)) this[ErrorKey] = jObj[ErrorKey];
            this["id"] = jObj["id"];
        }
    }
}
