using System.Threading.Tasks;
using TCC.Utils;

namespace TCC.Interop.Proxy;

public class StubInterface
{
    private static StubInterface? _instance;
    public static StubInterface Instance => _instance ?? new StubInterface();

    public readonly StubClient StubClient;
    private readonly RpcServer2 _stubServer;
    private readonly StubMessageParser _messageParser;

    public bool IsFpsModAvailable { get; set; }
    public bool IsStubAvailable { get; private set; }
    public bool IsConnected => _stubServer.Connected;

    private StubInterface()
    {
        _instance = this;

        _stubServer = new RpcServer2();
        StubClient = new StubClient();
        _messageParser = new StubMessageParser();

    }

    public async Task InitAsync(bool useLfg, bool enablePlayerMenu, bool enableProxy, bool showIngameChat, bool tccChatEnabled)
    {
        _stubServer.Stop();

        _stubServer.RequestReceived += _messageParser.HandleRequest;
        _stubServer.ResponseReceived += _messageParser.HandleResponse;
        _stubServer.ConnectionChanged += OnStubConnectionChanged;

        if (!enableProxy) return;
        IsStubAvailable = await StubClient.PingStub();
        if (!IsStubAvailable)
        {
            Log.F("Stub not found");
            Log.N("tcc-stub", 
                "Failed to connect to tcc-stub. Some functionalities will be unavailable.",
                NotificationType.Warning
                );
            return;
        }
        StubClient.Initialize(useLfg, enablePlayerMenu, enableProxy, showIngameChat, tccChatEnabled);
        _stubServer.Start();
        IsFpsModAvailable = await StubClient.GetIsModAvailable("fps-utils") || await StubClient.GetIsModAvailable("fps-manager");
        Log.Chat("Successfully connected to tcc-stub.");

    }

    private void OnStubConnectionChanged(bool connected)
    {
        if(!connected)
        {
            IsStubAvailable = false;
            Log.Chat("tcc-stub disconnected.");
        }
    }

    public void Disconnect()
    {
        _stubServer.Stop();
        _stubServer.RequestReceived -= _messageParser.HandleRequest;
        _stubServer.ResponseReceived -= _messageParser.HandleResponse;
        _stubServer.ConnectionChanged -= OnStubConnectionChanged;
    }
}