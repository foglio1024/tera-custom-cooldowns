using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Chat;

namespace TCC.ViewModels
{
    public class Tab : TSPropertyChanged
    {
        // needed for combobox in settings
        public List<ChatChannelOnOff> AllChannels => Utils.GetEnabledChannelsList();

        private ICollectionView _messages;
        private string _tabName;
        private ChatMessage _pinnedMessage;

        public string TabName
        {
            get => _tabName;
            set
            {
                if (_tabName == value) return;
                _tabName = value;
                N(nameof(TabName));
            }
        }
        private bool _attention;

        public bool Attention
        {
            get => _attention;
            set
            {
                if (_attention == value) return;
                _attention = value;
                N(nameof(Attention));
            }
        }

        public SynchronizedObservableCollection<string> Authors { get; set; }
        public SynchronizedObservableCollection<string> ExcludedAuthors { get; set; }
        public SynchronizedObservableCollection<ChatChannel> Channels { get; set; }
        public SynchronizedObservableCollection<ChatChannel> ExcludedChannels { get; set; }
        public ICollectionView Messages
        {
            get => _messages;
            set
            {
                if (_messages == value) return;
                _messages = value;
                N(nameof(Messages));
            }
        }
        public ChatMessage PinnedMessage
        {
            get => _pinnedMessage;
            set
            {
                if (_pinnedMessage == value) return;
                _pinnedMessage = value;
                N();
            }
        }
        public void Refresh()
        {
            Messages.Refresh();
        }

        public Tab(string n, ChatChannel[] ch, ChatChannel[] ex, string[] a, string[] exa)
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            TabName = n;
            Messages = new ListCollectionView(ChatWindowManager.Instance.ChatMessages);
            Authors = new SynchronizedObservableCollection<string>(Dispatcher);
            ExcludedAuthors = new SynchronizedObservableCollection<string>(Dispatcher);
            Channels = new SynchronizedObservableCollection<ChatChannel>(Dispatcher);
            ExcludedChannels = new SynchronizedObservableCollection<ChatChannel>(Dispatcher);
            foreach (var auth in a)
            {
                Authors.Add(auth);
            }
            foreach (var auth in exa)
            {
                ExcludedAuthors.Add(auth);
            }
            foreach (var chan in ch)
            {
                Channels.Add(chan);
            }
            foreach (var chan in ex)
            {
                ExcludedChannels.Add(chan);
            }
            if (Channels.Count == 0 && Authors.Count == 0 && ExcludedChannels.Count == 0 && ExcludedAuthors.Count == 0)
            {
                Messages.Filter = null;
                return;
            }
            ApplyFilter();
        }

        public bool Filter(ChatMessage m)
        {
            return (Authors.Count == 0 || Authors.Any(x => x == m.Author)) &&
                   (Channels.Count == 0 || Channels.Any(x => x == m.Channel)) &&
                   (ExcludedChannels.Count == 0 || ExcludedChannels.All(x => x != m.Channel)) &&
                   (ExcludedAuthors.Count == 0 || ExcludedAuthors.All(x => x != m.Author));

        }
        public void ApplyFilter()
        {
            Messages.Filter = f =>
            {
                var m = f as ChatMessage;
                return Filter(m);
            };
        }
    }
}
