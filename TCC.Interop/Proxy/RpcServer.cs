using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nostrum.Extensions;
using TCC.Utils;
using TeraPacketParser.Sniffing.JsonRPC;

namespace TCC.Interop.Proxy;

/// <summary>
/// Listens to tcc-stub commands using a <see cref="HttpListener"/> and fires related events.
/// </summary>
public class RpcServer
{
    public event Action<Response>? ResponseReceived;
    public event Action<Request>? RequestReceived;

    readonly HttpListener _server;
    bool _listening;
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

    void Listen()
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
/// <summary>
/// Listens to tcc-stub commands using a <see cref="HttpListener"/> and fires related events.
/// </summary>
public class RpcServer2
{
    public event Action<Response>? ResponseReceived;
    public event Action<Request>? RequestReceived;

    TcpClient _socket;
    bool _listening;
    bool _connected;
    public RpcServer2()
    {
        _socket = new TcpClient();
    }

    public void Start()
    {
        if (_listening) return;
        _listening = true;
        Task.Run(Listen);
    }

    public void Stop()
    {
        try
        {
            _socket.Dispose();
        }
        catch (ObjectDisposedException ex)
        {
            Log.F($"[ObjectDisposedException] {ex.ObjectName} disposed, skipping Stop()...");
        }
        _listening = false;
    }

    async Task Listen()
    {
        _socket = new TcpClient();

        while (_listening)
        {
            try
            {
                if (_socket.Client == null)
                {
                    Stop();
                    continue;
                }
                    
                await _socket.ConnectAsync("127.0.0.51", 9551);

                var stream = _socket.GetStream();

                _connected = true;

                while (_connected)
                {
                    var lenBuf = new byte[2];
                    _ = stream.Read(lenBuf, 0, 2);
                    var len = BitConverter.ToUInt16(lenBuf, 0);
                    if (len <= 2)
                    {
                        if (!_socket.IsConnected())
                        {
                            _socket.Close();
                            _connected = false;
                        }
                        continue;
                    }
                    var dataBuf = new byte[len];
                    var progress = 0;
                    while (progress < len)
                    {
                        progress += stream.Read(dataBuf, progress, len - progress);
                    }

                    try
                    {
                        var jsonReq = JObject.Parse(Encoding.UTF8.GetString(dataBuf));
                        if (jsonReq.ContainsKey(Request.MethodKey))
                        {
                            RequestReceived?.Invoke(new Request(jsonReq));
                            //Log.CW("Received request: " + jsonReq);
                        }
                        else if (jsonReq.ContainsKey(Response.ErrorKey) || jsonReq.ContainsKey(Response.ResultKey))
                        {
                            ResponseReceived?.Invoke(new Response(jsonReq));
                        }
                        else
                        {
                            Log.F($"Received unknown message type: \n{jsonReq}");
                        }
                    }
                    catch (Exception e)
                    {
                        Log.F($"Error while parsing request: {e}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.F($"Socket error: {e}");
            }
        }
        _listening = false;
    }
}