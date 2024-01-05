using Nostrum.WPF.ThreadSafe;
using TCC.Utils;

namespace TCC.Data.Chat;

public class ChatChannelOnOff : ThreadSafeObservableObject
{
    bool _enabled;

    public bool Enabled
    {
        get => _enabled;
        set => RaiseAndSetIfChanged(value, ref _enabled);
    }
    public ChatChannel Channel { get; }

    public ChatChannelOnOff(ChatChannel ch, bool en = true)
    {
        Channel = ch;
        Enabled = en;
    }
}