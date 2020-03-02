using System.Threading.Tasks;

namespace TCC.Interop.Proxy
{
    public class StubInterface
    {
        private static StubInterface _instance;
        public static StubInterface Instance => _instance ?? new StubInterface();

        public readonly StubClient StubClient;
        private readonly RpcServer StubServer;
        private readonly StubMessageHandler MessageHandler;

        public bool IsFpsUtilsAvailable { get; set; }
        public bool IsStubAvailable { get; set; }

        private StubInterface()
        {
            _instance = this;

            StubServer = new RpcServer();
            StubClient = new StubClient();
            MessageHandler = new StubMessageHandler();
            
            StubServer.RequestReceived += MessageHandler.HandleRequest;
            StubServer.ResponseReceived += MessageHandler.HandleResponse;
        }

        public async Task Init()
        {
            if (!App.Settings.EnableProxy) return;
            IsStubAvailable = await StubClient.PingStub();
            if (!IsStubAvailable) return;
            StubClient.Initialize();
            StubServer.Start();
            IsFpsUtilsAvailable = await StubClient.GetIsModAvailable("fps-utils");
        }

        public void Disconnect()
        {
            StubServer.Stop();
            StubServer.RequestReceived -= MessageHandler.HandleRequest;
            StubServer.ResponseReceived -= MessageHandler.HandleResponse;

        }
    }
}