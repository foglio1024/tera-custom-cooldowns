using Newtonsoft.Json.Linq;

namespace TCC.Interop.JsonRPC
{
    public class Request : JObject
    {
        public const string MethodKey = "method";
        public const string IdKey = "id";
        public const string ParametersKey = "params";

        public string Method => ContainsKey(MethodKey) ? this[MethodKey].Value<string>() : null;
        public string Id => ContainsKey(IdKey) ? this[IdKey].Value<string>() : null;
        public JObject Parameters => ContainsKey(ParametersKey) ? this[ParametersKey] as JObject : null;

        private static uint _nextId;
        public Request(string methodName, JObject parameters = null)
        {
            this["jsonrpc"] = "2.0";
            this[MethodKey] = methodName;
            if (parameters != null) this[ParametersKey] = parameters;
            this[IdKey] = GetNextId();
        }

        public Request(JObject j)
        {
            this["jsonrpc"] = j["jsonrpc"];
            this[MethodKey] = j[MethodKey];
            if (j.ContainsKey(ParametersKey)) this[ParametersKey] = j[ParametersKey];
            this[IdKey] = j[IdKey];
        }
        private static uint GetNextId()
        {
            return _nextId++;
        }
    }
}
