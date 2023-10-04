using Newtonsoft.Json.Linq;

namespace TeraPacketParser.Sniffing.JsonRPC;

public class Response : JObject
{
    public const string ResultKey = "result";
    public const string ErrorKey = "error";

    public JToken? Result => this[ResultKey];
    public JToken? Error => this[ErrorKey];

    public Response(JObject jObj)
    {
        this["jsonrpc"] = jObj["jsonrpc"];
        if (jObj.TryGetValue(ResultKey, out var resVal)) this[ResultKey] = resVal;
        else if (jObj.TryGetValue(ErrorKey, out var errVal)) this[ErrorKey] = errVal;
        this["id"] = jObj["id"];
    }
}