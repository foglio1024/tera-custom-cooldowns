using System.Threading.Tasks;

namespace TCC.Interop.Proxy
{
    public class ProxyInterface
    {
        private static ProxyInterface _instance;
        public static ProxyInterface Instance => (_instance ?? new ProxyInterface());
        private Server Server { get; }
        public TccStubInterface Stub { get; }
        private ProxyMessageHandler MessageHandler { get; }
        public bool IsFpsUtilsAvailable { get; set; }
        public bool IsStubAvailable { get; set; }

        private ProxyInterface()
        {
            _instance = this;
            Server = new Server();
            MessageHandler = new ProxyMessageHandler();
            Stub = new TccStubInterface();
            Server.RequestReceived += MessageHandler.HandleRequest;
            Server.ResponseReceived += MessageHandler.HandleResponse;
        }


        public async Task Init()
        {
            if (!App.Settings.EnableProxy) return;
            IsStubAvailable = await Stub.PingStub();
            if (!IsStubAvailable) return;
            Stub.Initialize();
            Server.Start();
            IsFpsUtilsAvailable = await Stub.GetIsModAvailable("fps-utils");
        }

        public void Disconnect()
        {
            Server.Stop();
        }
    }
}