using Newtonsoft.Json.Linq;
using Nostrum.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using TCC.Utils;
using TeraPacketParser.Analysis;
using TeraPacketParser.TeraCommon.Game;
using TeraPacketParser.TeraCommon.Sniffing;

namespace TeraPacketParser.Sniffing;

/// <summary>
/// Uses a <see cref="TcpListener"/> to receive TERA packets sent by <c>ttb-interface-data</c>.
/// Uses a <see cref="ToolboxControlInterface"/> to interact with <c>ttb-interface-control</c>.
/// </summary>
public class ToolboxSniffer : ITeraSniffer
{
    /// <summary>
    /// Used to send HTTP requests to Toolbox.
    /// </summary>
    public class ToolboxControlInterface
    {
        private readonly ToolboxHttpClient _client;

        public ToolboxControlInterface(string address)
        {
            _client = new ToolboxHttpClient(address);
        }

        public readonly record struct ServerInfo(uint id, string category, string name, string address, uint port);

        /// <summary>
        /// Requests <c>ttb-interface-control</c> to return current server id.
        /// </summary>
        /// <returns>server id</returns>
        public async Task<ServerInfo> GetServer()
        {
            var resp = await _client.CallAsync("getServerInfo"); // todo: update after meter
            try
            {
                return resp?.Result?.Value<JObject>()?.ToObject<ServerInfo>() ?? default;
            }
            catch 
            {
                return default;                
            }
        }
        /// <summary>
        /// Requests <c>ttb-interface-control</c> to return current release version.
        /// </summary>
        /// <returns>release version</returns>
        public async Task<int> GetReleaseVersion()
        {
            var resp = await _client.CallAsync("getReleaseVersion");
            return resp?.Result?.Value<int>() ?? 0;
        }
        /// <summary>
        /// Requests <c>ttb-interface-control</c> to dump a map to file.
        /// </summary>
        /// <param name="path">path the map will be dumped to</param>
        /// <param name="mapType">type of the map:
        ///     <list type="bullet">
        ///         
        ///         <item>
        ///             <term>"protocol"</term>
        ///             <description>opcode map</description>
        ///         </item>
        ///         <item>
        ///             <term>"sysmsg"</term>
        ///             <description>system messages map</description>
        ///         </item>
        ///     </list>
        /// </param>
        /// <returns>true if successful</returns>
        public async Task<bool> DumpMap(string path, string mapType)
        {
            var resp = await _client.CallAsync("dumpMapSync", new JObject
            {
                { "path", path},
                { "mapType", mapType }
            });
            return resp?.Result != null && resp.Result.Value<bool>();
        }
        /// <summary>
        /// Requests <c>ttb-interface-control</c> to install hooks for the provided list of opcodes.
        /// </summary>
        /// <param name="opcodes">list of opcode names</param>
        /// <returns>true if successful</returns>
        public async Task<bool> AddHooks(List<string> opcodes)
        {
            var jArray = new JArray();
            foreach (var opc in opcodes) jArray.Add(opc);
            var resp = await _client.CallAsync("addHooks", new JObject
            {
                {"hooks", jArray}
            });
            return resp?.Result != null && resp.Result.Value<bool>();
        }

        public async Task<bool> AddDefinition(string opcodeName, uint version, string def)
        {
            var resp = await _client.CallAsync("addDefinition", new JObject
            {
                {"opcodeName", opcodeName},
                {"version", version},
                {"def", def}
            });
            return resp?.Result != null && resp.Result.Value<bool>();
        }
        public async Task<bool> AddOpcode(string opcodeName, ushort opcode)
        {
            var resp = await _client.CallAsync("addOpcode", new JObject
            {
                {"opcodeName", opcodeName},
                {"opcode", opcode}
            });
            return resp?.Result != null && resp.Result.Value<bool>();
        }
        /// <summary>
        /// Requests <c>ttb-interface-control</c> to uninstall hooks for the provided list of opcodes.
        /// </summary>
        /// <param name="opcodes">list of opcode names</param>
        /// <returns>true if successful</returns>
        public async Task<bool> RemoveHooks(List<string> opcodes)
        {
            var jArray = new JArray();
            foreach (var opc in opcodes) jArray.Add(opc);
            var resp = await _client.CallAsync("removeHooks", new JObject
            {
                {"hooks", jArray}
            });
            return resp?.Result != null && resp.Result.Value<bool>();
        }
        /// <summary>
        /// Requests <c>ttb-interface-control</c> to perform a DC query.
        /// </summary>
        /// <param name="query">query string</param>
        /// <returns>json object containing query result</returns>
        public async Task<JObject?> Query(string query)
        {
            var resp = await _client.CallAsync("query", new JObject
            {
                { "query" , query }
            });

            return resp?.Result?.Value<JObject>();
        }
        /// <summary>
        /// Requests <c>ttb-interface-control</c> to return current protocol version.
        /// </summary>
        /// <returns>protocol version</returns>
        public async Task<uint> GetProtocolVersion()
        {
            var resp = await _client.CallAsync("getProtocolVersion");
            return resp?.Result?.Value<uint>() ?? 0U;
        }

        internal async Task<string> GetLanguage()
        {
            var resp = await _client.CallAsync("getLanguage");
            return resp?.Result?.Value<string>() ?? "";
        }
    }

#if SERVER
        private readonly TcpListener _dataConnection;
#else
    private TcpClient _dataConnection;
#endif
    public readonly ToolboxControlInterface ControlConnection;
    private readonly bool _failed;
    private bool _enabled;
    private bool _connected;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled == value) return;
            _enabled = value;
            if (_enabled)
            {
#if SERVER
                    new Thread(Listen).Start();
#else
                Task.Run(ReceiveAsync);
#endif
            }
        }
    }
    public bool Connected
    {
        get => _connected;
        set
        {
            if (_connected == value) return;
            _connected = value;
            if (!_connected) EndConnection?.Invoke();
        }
    }

    public event Action<Message>? MessageReceived;
    public event Action<Server>? NewConnection;
    public event Action? EndConnection;

    public async Task<bool> RetrieveSysMsgIfNeeded(string destPath)
    {
        if (!Connected) return false;
        return await ControlConnection.DumpMap(destPath, "sysmsg");
    }

    public ToolboxSniffer()
    {
#if SERVER
            _dataConnection = new TcpListener(IPAddress.Parse("127.0.0.60"), 5200);
#else
        _dataConnection = new TcpClient();
#endif
        ControlConnection = new ToolboxControlInterface("http://127.0.0.61:5300");
        try
        {
#if SERVER
                _dataConnection.Start();
#endif
        }
        catch (Exception e)
        {
            Log.F($"Failed to start Toolbox sniffer: {e}");
            _failed = true;
        }
    }
#if SERVER

        private async void Listen()
        {
            if (_failed) return;
            while (Enabled)
            {
                var client = _dataConnection.AcceptTcpClient();
                var resp = await ControlConnection.GetServerId();
                if (resp != 0)
                {
                    await ControlConnection.AddHooks(PacketAnalyzer.Factory.OpcodesList);
                    PacketAnalyzer.Factory.ReleaseVersion = await ControlConnection.GetReleaseVersion();
                    NewConnection?.Invoke(Game.DB.ServerDatabase.GetServer(resp));
                }
                var stream = client.GetStream();
                while (true)
                {
                    Connected = true;
                    try
                    {
                        var lenBuf = new byte[2];
                        stream.Read(lenBuf, 0, 2);
                        var len = BitConverter.ToUInt16(lenBuf, 0);
                        if (len <= 2)
                        {
                            if (!client.IsConnected())
                            {
                                client.Close();
                                Connected = false;
                                break;
                            }
                            continue;
                        }
                        var length = len - 2;
                        var dataBuf = new byte[length];

                        var progress = 0;
                        while (progress < length)
                        {
                            progress += stream.Read(dataBuf, progress, length - progress);
                        }

                        MessageReceived?.Invoke(new Message(DateTime.UtcNow, dataBuf));

                    }
                    catch
                    {
                        Connected = false;
                        client.Close();
                        //Log.F($"Disconnected: {e}");
                    }
                }
            }
        }
#else
    private async Task ReceiveAsync()
    {
        if (_failed) return;
        while (Enabled)
        {
            var resp = await ControlConnection.GetServer();
            if (resp != default)
            {
                Connected = true;
                await ControlConnection.AddHooks(PacketAnalyzer.Factory!.OpcodesList);
                PacketAnalyzer.Factory.ReleaseVersion = await ControlConnection.GetReleaseVersion();
                NewConnection?.Invoke(new Server(resp.name, await ControlConnection.GetLanguage(), resp.address, resp.id));
                _dataConnection = new TcpClient();
                await _dataConnection.ConnectAsync("127.0.0.60", 5301);
            }
            else
            {
                await Task.Delay(100);
                continue;
            }
            var stream = _dataConnection.GetStream();

            Connected = true;
            while (Connected)
            {
                try
                {
                    var lenBuf = new byte[2];
                    _ = stream.Read(lenBuf, 0, 2);
                    var len = BitConverter.ToUInt16(lenBuf, 0);
                    if (len <= 2)
                    {
                        if (!_dataConnection.IsConnected())
                        {
                            _dataConnection.Close();
                            Connected = false;
                        }
                        continue;
                    }
                    var length = len - 2;
                    var dataBuf = new byte[length];

                    var progress = 0;
                    while (progress < length)
                    {
                        progress += stream.Read(dataBuf, progress, length - progress);
                    }
                    MessageReceived?.Invoke(new Message(DateTime.UtcNow, dataBuf));

                }
                catch
                {
                    _dataConnection.Close();
                    Connected = false;
                }
            }
        }
    }
#endif
}