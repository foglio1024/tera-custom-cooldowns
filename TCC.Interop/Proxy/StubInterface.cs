using System.Threading.Tasks;
using TCC.Utils;

namespace TCC.Interop.Proxy
{
    public class StubInterface
    {
        private static StubInterface? _instance;
        public static StubInterface Instance => _instance ?? new StubInterface();

        public readonly StubClient StubClient;
        private readonly RpcServer2 StubServer;
        private readonly StubMessageParser _messageParser;

        public bool IsFpsModAvailable { get; set; }
        public bool IsStubAvailable { get; private set; }

        private StubInterface()
        {
            _instance = this;

            StubServer = new RpcServer2();
            StubClient = new StubClient();
            _messageParser = new StubMessageParser();
        }

        public async Task InitAsync(bool useLfg, bool enablePlayerMenu, bool enableProxy, bool showIngameChat, bool tccChatEnabled)
        {
            StubServer.RequestReceived += _messageParser.HandleRequest;
            StubServer.ResponseReceived += _messageParser.HandleResponse;

            if (!enableProxy) return;
            IsStubAvailable = await StubClient.PingStub();
            if (!IsStubAvailable)
            {
                Log.F("Stub not found");
                return;
            }
            StubClient.Initialize(useLfg, enablePlayerMenu, enableProxy, showIngameChat, tccChatEnabled);
            StubServer.Start();
            IsFpsModAvailable = await StubClient.GetIsModAvailable("fps-utils") || await StubClient.GetIsModAvailable("fps-manager");
        }

        public void Disconnect()
        {
            StubServer.Stop();
            StubServer.RequestReceived -= _messageParser.HandleRequest;
            StubServer.ResponseReceived -= _messageParser.HandleResponse;
        }
    }
}