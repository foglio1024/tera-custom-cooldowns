using System.Windows.Threading;

namespace TCC.Data
{
    public class ChatChannelOnOff : TSPropertyChanged
    {
        private bool enabled;
        private ChatChannel channel;

        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled == value) return;
                enabled = value;
                NPC(nameof(Enabled));
            }
        }
        public ChatChannel Channel
        {
            get => channel;
            set
            {
                if (channel == value) return;
                channel = value;
                NPC(nameof(Channel));
            }
        }

        public ChatChannelOnOff()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }
        public ChatChannelOnOff(ChatChannel ch, bool en = true) : this()
        {
            Channel = ch;
            Enabled = en;
        }
    }
}
