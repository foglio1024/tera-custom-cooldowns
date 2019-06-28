using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using Newtonsoft.Json.Linq;
using TCC.Annotations;
using TCC.Interop.Proxy;
using TCC.TeraCommon.Sniffing;
using TeraPacketParser;
using Server = TCC.TeraCommon.Game.Server;

namespace TCC.Parsing
{
    internal class ToolboxControlInterface
    {
        private readonly ToolboxHttpClient _client;

        public ToolboxControlInterface(string address)
        {
            _client = new ToolboxHttpClient(address);
        }

        public async Task<uint> GetServer()
        {
            var resp = await _client.CallAsync("getServer");
            return resp?.Result.Value<uint>() ?? 0;
        }
        public async Task<bool> DumpMap([NotNull] string path, [NotNull] string mapType)
        {
            var resp = await _client.CallAsync("dumpMap", new JObject
            {
                { "path", path},
                { "mapType", mapType }
            });
            return resp?.Result != null && resp.Result.Value<bool>();
        }

        public async Task<bool> AddHooks(List<string> opcodes)
        {
            var jArray = new JArray();
            opcodes.ForEach(opc => jArray.Add(opc));
            var resp = await _client.CallAsync("addHooks", new JObject
            {
                {"hooks", jArray}
            });
            return resp?.Result != null && resp.Result.Value<bool>();
        }
        public async Task<bool> RemoveHooks(List<string> opcodes)
        {
            var jArray = new JArray();
            opcodes.ForEach(opc => jArray.Add(opc));
            var resp = await _client.CallAsync("removeHooks", new JObject
            {
                {"hooks", jArray}
            });
            return resp?.Result != null && resp.Result.Value<bool>();
        }
        public async Task<JObject> Query([NotNull] string query)
        {
            var resp = await _client.CallAsync("query", new JObject
            {
                { "query" , query }
            });

            return resp?.Result?.Value<JObject>();
        }
        public async Task<uint> GetProtocolVersion()
        {
            var resp = await _client.CallAsync("getProtocolVersion");
            return resp?.Result?.Value<uint>() ?? 0;
        }
    }

    internal class ToolboxSniffer : ITeraSniffer
    {
        private readonly TcpListener _dataConnection;
        public readonly ToolboxControlInterface ControlConnection;
        private bool _enabled;
        private bool _connected;

        public ToolboxSniffer()
        {
            _dataConnection = new TcpListener(IPAddress.Parse("127.0.0.60"), 5200);
            _dataConnection.Start();
            ControlConnection = new ToolboxControlInterface("http://127.0.0.61:5200");
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                if (_enabled) new Thread(Listen).Start();
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

        public event Action<Message> MessageReceived;
        public event Action<Server> NewConnection;
        public event Action EndConnection;

        private async void Listen()
        {
            while (Enabled)
            {
                var client = _dataConnection.AcceptTcpClient();
                await ProxyInterface.Instance.Init();
                var resp = await ControlConnection.GetServer();
                if (resp != 0)
                {
                    NewConnection?.Invoke(Session.DB.ServerDatabase.GetServer(resp));
                    await ControlConnection.AddHooks(PacketAnalyzer.Factory.OpcodesList);
                }

                var stream = client.GetStream();
                while (client.Connected)
                {
                    Connected = true;
                    try
                    {
                        var lenBuf = new byte[2];
                        stream.Read(lenBuf, 0, 2);
                        var length = BitConverter.ToUInt16(lenBuf, 0) - 2;
                        var dataBuf = new byte[length];
                        stream.Read(dataBuf, 0, length);

                        MessageReceived?.Invoke(new Message(DateTime.Now, dataBuf));
                    }
                    catch (Exception)
                    {
                        Connected = false;
                        client.Close();
                    }

                }
                Connected = false;
                client.Close();
            }
        }

    }
}