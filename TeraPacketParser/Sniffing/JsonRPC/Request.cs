using Newtonsoft.Json.Linq;

namespace TeraPacketParser.Sniffing.JsonRPC;

public class Request : JObject
{
    public const string MethodKey = "method";
    public const string IdKey = "id";
    public const string ParametersKey = "params";

    public string Id => ContainsKey(IdKey) ? this[IdKey]?.Value<string>() ?? "" : "";
    public string Method => ContainsKey(MethodKey) ? this[MethodKey]?.Value<string>() ?? "" : "";
    public JObject? Parameters => ContainsKey(ParametersKey) ? this[ParametersKey] as JObject : null;

    static uint _nextId;
    public Request(string methodName, JObject? parameters = null)
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
        if (j.TryGetValue(ParametersKey, out var parameters)) this[ParametersKey] = parameters;
        this[IdKey] = j[IdKey];
    }

    static uint GetNextId()
    {
        return _nextId++;
    }
}