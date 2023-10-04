using Nostrum.WPF.ThreadSafe;
using TCC.Utils;

namespace TCC.Data.Chat;

public class ChatChannelOnOff : ThreadSafeObservableObject
{
    bool _enabled;
    ChatChannel _channel;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled == value) return;
            _enabled = value;
            N();
        }
    }
    public ChatChannel Channel
    {
        get => _channel;
        set
        {
            if (_channel == value) return;
            _channel = value;
            N();
        }
    }

    public ChatChannelOnOff()
    {
    }
    public ChatChannelOnOff(ChatChannel ch, bool en = true) : this()
    {
        Channel = ch;
        Enabled = en;
    }
}