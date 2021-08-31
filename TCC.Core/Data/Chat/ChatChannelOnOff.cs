using System.Windows.Threading;
using Nostrum;
using Nostrum.WPF.ThreadSafe;
using TCC.Utils;

namespace TCC.Data.Chat
{
    public class ChatChannelOnOff : ThreadSafePropertyChanged
    {
        private bool _enabled;
        private ChatChannel _channel;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value) return;
                _enabled = value;
                N(nameof(Enabled));
            }
        }
        public ChatChannel Channel
        {
            get => _channel;
            set
            {
                if (_channel == value) return;
                _channel = value;
                N(nameof(Channel));
            }
        }

        public ChatChannelOnOff()
        {
            SetDispatcher(Dispatcher.CurrentDispatcher);
        }
        public ChatChannelOnOff(ChatChannel ch, bool en = true) : this()
        {
            Channel = ch;
            Enabled = en;
        }
    }
}
