using Nostrum.WPF.ThreadSafe;
using TCC.Utils;

namespace TCC.Data.Chat;

public class ChatChannelOnOff : ThreadSafeObservableObject
{
    bool _enabled;

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
    public ChatChannel Channel { get; }

    public ChatChannelOnOff(ChatChannel ch, bool en = true)
    {
        Channel = ch;
        Enabled = en;
    }
}