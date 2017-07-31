using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TCC.Data
{
    public class ChatChannelOnOff : TSPropertyChanged
    {
        private bool enabled;
        private ChatChannel channel;

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value) return;
                enabled = value;
                NotifyPropertyChanged(nameof(Enabled));
            }
        }
        public ChatChannel Channel
        {
            get { return channel; }
            set
            {
                if (channel == value) return;
                channel = value;
                NotifyPropertyChanged(nameof(Channel));
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
