using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Data;
using TCC.Parsing;

namespace TCC.ViewModels
{
    public class ChatWindowViewModel : TSPropertyChanged
    {
        static ChatWindowViewModel _instance;
        public static ChatWindowViewModel Instance => _instance ?? (_instance = new ChatWindowViewModel());
        const int MESSAGE_CAP = 300;
        const int SPAM_THRESHOLD = 3;

        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
        private double scale = SettingsManager.ChatWindowSettings.Scale;
        public double Scale
        {
            get { return scale; }
            set
            {
                if (scale == value) return;
                scale = value;
                NotifyPropertyChanged("Scale");
            }
        }
        public int MessageCount
        {
            get => ChatMessages.Count;
        }

        public TooltipInfo TooltipInfo { get; set; }

        private SynchronizedObservableCollection<ChatMessage> _chatMessages;
        public SynchronizedObservableCollection<ChatMessage> ChatMessages
        {
            get => _chatMessages; set
            {
                if (_chatMessages == value) return;
                _chatMessages = value;
            }
        }
        private ICollectionView _allMessages;
        private ICollectionView _guildMessages;
        private ICollectionView _groupMessages;
        private ICollectionView _systemMessages;

        public ICollectionView AllMessages
        {
            get => _allMessages;
        }
        public ICollectionView GuildMessages
        {
            get => _guildMessages;
        }
        public ICollectionView GroupMessages
        {
            get => _groupMessages;
        }
        public ICollectionView SystemMessages
        {
            get => _systemMessages;
        }

        public List<ChatChannel> VisibleChannels;
        public List<string> BlockedUsers;
        public List<string> Friends;
        public PrivateChatChannel[] PrivateChannels = new PrivateChatChannel[8];

        public ChatWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _chatMessages = new SynchronizedObservableCollection<ChatMessage>(_dispatcher);
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                NotifyPropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    _dispatcher.Invoke(() =>
                    {
                        WindowManager.ChatWindow.Topmost = false;
                        WindowManager.ChatWindow.Topmost = true;
                    });
                }
            };
            ChatMessages.CollectionChanged += ChatMessages_CollectionChanged;
            VisibleChannels = new List<ChatChannel>
            {
                ChatChannel.Global,
                ChatChannel.Guild,
                ChatChannel.GuildNotice,
                ChatChannel.Party,
                ChatChannel.Trade,
                ChatChannel.Loot,
                ChatChannel.Raid,
                ChatChannel.RaidNotice,
                ChatChannel.Say
            };
            BlockedUsers = new List<string>();
            Friends = new List<string>();
            TooltipInfo = new TooltipInfo("", "", 1);

            _allMessages = new CollectionViewSource { Source = _chatMessages }.View;
            _guildMessages = new CollectionViewSource { Source = _chatMessages }.View;
            _groupMessages = new CollectionViewSource { Source = _chatMessages }.View;
            _systemMessages = new CollectionViewSource { Source = _chatMessages }.View;

            _allMessages.Filter = null;
            _guildMessages.Filter = p => ((ChatMessage)p).Channel == ChatChannel.Guild ||
                                        ((ChatMessage)p).Channel == ChatChannel.GuildNotice;
            _groupMessages.Filter = p => ((ChatMessage)p).Channel == ChatChannel.Party ||
                                        ((ChatMessage)p).Channel == ChatChannel.PartyNotice ||
                                        ((ChatMessage)p).Channel == ChatChannel.Group ||
                                        ((ChatMessage)p).Channel == ChatChannel.GroupAlerts ||
                                        ((ChatMessage)p).Channel == ChatChannel.Raid ||
                                        ((ChatMessage)p).Channel == ChatChannel.RaidNotice;
            _systemMessages.Filter = p => ((ChatMessage)p).Author == "System";
        }

        private void ChatMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("NewItem");
            //NotifyPropertyChanged(nameof(GuildMessages));
            //NotifyPropertyChanged(nameof(GroupMessages));
            //NotifyPropertyChanged(nameof(SystemMessages));
        }

        public void AddChatMessage(ChatMessage chatMessage)
        {
            if (chatMessage.RawMessage.Contains("W W W.M M O O K.C 0 M")) return;
            if (BlockedUsers.Contains(chatMessage.Author)) return;
            if (ChatMessages.Count < SPAM_THRESHOLD)
            {
                for (int i = 0; i < ChatMessages.Count - 1; i++)
                {
                    var m = ChatMessages[i];
                    if (m.RawMessage == chatMessage.RawMessage && m.Channel == chatMessage.Channel && m.Author == chatMessage.Author && !VisibleChannels.Contains(chatMessage.Channel)) return;
                }
            }
            else
            {

                int offset = 0;
                for (int i = 0; i < SPAM_THRESHOLD; i++)
                {
                    if (1 + i + offset > ChatMessages.Count) continue;

                    var m = ChatMessages[ChatMessages.Count - 1 - i - offset];
                    //if (!VisibleChannels.Contains(m.Channel))
                    //{
                    //    i--;
                    //    offset++;
                    //    continue;
                    //}
                    if (m.RawMessage == chatMessage.RawMessage && m.Channel == chatMessage.Channel && m.Author == chatMessage.Author) return;
                }
            }

            ChatMessages.Add(chatMessage);

            if (ChatMessages.Count >= MESSAGE_CAP)
            {
                ChatMessages.RemoveAt(0);
            }
            NotifyPropertyChanged(nameof(MessageCount));
        }
    }
}
