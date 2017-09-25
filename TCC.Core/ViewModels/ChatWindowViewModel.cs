using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Data;
using TCC.Parsing.Messages;

namespace TCC.ViewModels
{
    public class ChatWindowViewModel : TSPropertyChanged
    {
        private DispatcherTimer hideTimer;
        private static ChatWindowViewModel _instance;
        private double scale = SettingsManager.ChatWindowSettings.Scale;
        private bool isChatVisible;
        private SynchronizedObservableCollection<ChatMessage> _chatMessages;
        private ConcurrentQueue<ChatMessage> _queue;
        public List<SimpleUser> Friends;
        public List<string> BlockedUsers;
        public LFG LastClickedLfg;
        private SynchronizedObservableCollection<LFG> _lfgs;
        public readonly PrivateChatChannel[] PrivateChannels = new PrivateChatChannel[8];
        private bool paused;

        public static ChatWindowViewModel Instance => _instance ?? (_instance = new ChatWindowViewModel());
        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
        public bool IsChatVisible
        {
            get => isChatVisible;
            set
            {
                if (isChatVisible == value) return;
                isChatVisible = value;
                NotifyPropertyChanged(nameof(IsChatVisible));
            }
        }
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
        public double ChatWindowOpacity
        {
            get { return SettingsManager.ChatWindowOpacity; }
            set
            {
                NotifyPropertyChanged(nameof(ChatWindowOpacity));
            }
        }
        public bool LfgOn
        {
            get => SettingsManager.LfgOn;
            set
            {
                NotifyPropertyChanged(nameof(LfgOn));
            }
        }
        public int MessageCount
        {
            get => ChatMessages.Count;
        }
        public bool Paused
        {
            get { return paused; }
            set
            {
                if (paused == value) return;
                paused = value;
                NotifyPropertyChanged(nameof(Paused));
            }
        }
        public bool IsQueueEmpty
        {
            get
            {
                return _queue.Count == 0 ? true : false;
            }
        }
        public TooltipInfo TooltipInfo { get; set; }
        public SynchronizedObservableCollection<ChatMessage> ChatMessages
        {
            get => _chatMessages; set
            {
                if (_chatMessages == value) return;
                _chatMessages = value;
            }
        }
        public SynchronizedObservableCollection<LFG> LFGs
        {
            get => _lfgs;
            set
            {
                if (_lfgs == value) return;
                _lfgs = value;
            }
        }

        public SynchronizedObservableCollection<Tab> Tabs
        {
            get => _tabs;
            set
            {
                if (_tabs == value) return;
                _tabs = value;
                NotifyPropertyChanged(nameof(Tabs));
            }
        }

        public List<ChatChannelOnOff> VisibleChannels { get => SettingsManager.EnabledChatChannels; }
        private readonly object _lock = new object();
        private SynchronizedObservableCollection<Tab> _tabs;

        public ChatWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _chatMessages = new SynchronizedObservableCollection<ChatMessage>(_dispatcher);
            _queue = new ConcurrentQueue<ChatMessage>();
            _lfgs = new SynchronizedObservableCollection<LFG>(_dispatcher);
            hideTimer = new DispatcherTimer();
            hideTimer.Interval = TimeSpan.FromSeconds(15);
            hideTimer.Tick += HideTimer_Tick;
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                NotifyPropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    WindowManager.ChatWindow.RefreshTopmost();
                }
            };

            ChatMessages.CollectionChanged += ChatMessages_CollectionChanged;
            BindingOperations.EnableCollectionSynchronization(ChatMessages, _lock);

            //get bool from settings
            //
            //
            BlockedUsers = new List<string>();
            Friends = new List<SimpleUser>();
            TooltipInfo = new TooltipInfo("", "", 1);
            PrivateChannels[7] = new PrivateChatChannel(uint.MaxValue - 1, "Proxy", 7);

        }

        public void RemoveDeadLfg()
        {
            if (LastClickedLfg != null)
            {
                RemoveLfg(LastClickedLfg);
                LastClickedLfg = null;
            }
        }
        public void AddFromQueue(int itemsToAdd)
        {
            for (int i = 0; i < itemsToAdd; i++)
            {
                ChatMessage msg;
                if (_queue.TryDequeue(out msg))
                {
                    ChatMessages.Insert(0, msg);
                    if (ChatMessages.Count > SettingsManager.MaxMessages)
                    {
                        ChatMessages.RemoveAt(ChatMessages.Count - 1);
                    }
                }
            }
        }
        private void HideTimer_Tick(object sender, EventArgs e)
        {
            IsChatVisible = false;
            hideTimer.Stop();
        }
        private void ChatMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshHideTimer();
            NotifyPropertyChanged("NewItem");
            IsChatVisible = true;
        }
        public void AddChatMessage(ChatMessage chatMessage)
        {
            if (!SettingsManager.ChatWindowSettings.Enabled) return;
            if (BlockedUsers.Contains(chatMessage.Author)) return;
            if (!VisibleChannels.FirstOrDefault(x => x.Channel == chatMessage.Channel).Enabled) return;
            if (ChatMessages.Count < SettingsManager.SpamThreshold)
            {
                for (int i = 0; i < ChatMessages.Count - 1; i++)
                {
                    var m = ChatMessages[i];
                    if (m.RawMessage == chatMessage.RawMessage &&
                        m.Channel == chatMessage.Channel &&
                        m.Author == chatMessage.Author
                        ) return;
                }
            }
            else
            {

                for (int i = 0; i < SettingsManager.SpamThreshold; i++)
                {
                    if (i > ChatMessages.Count - 1) continue;

                    var m = ChatMessages[i];

                    if (m.RawMessage == chatMessage.RawMessage &&
                        m.Author == chatMessage.Author &&
                        m.Channel != ChatChannel.Money &&
                        m.Channel != ChatChannel.Loot &&
                        m.Channel != ChatChannel.Bargain) return;
                }
            }

            ChatMessage.SplitSimplePieces(chatMessage);

            if (!Paused) ChatMessages.Insert(0, chatMessage);
            else _queue.Enqueue(chatMessage);

            if (ChatMessages.Count > SettingsManager.MaxMessages)
            {
                ChatMessages.RemoveAt(ChatMessages.Count - 1);
            }
            NotifyPropertyChanged(nameof(MessageCount));
        }
        public void AddOrRefreshLfg(S_PARTY_MATCH_LINK x)
        {
            LFG lfg;
            if (TryGetLfg(x.Id, x.Message, x.Name, out lfg))
            {
                lfg.Message = x.Message;
                lfg.Refresh();
            }
            else
            {
                _lfgs.Add(new LFG(x.Id, x.Name, x.Message, x.Raid));
            }
        }
        public void RemoveLfg(LFG lfg)
        {
            lfg.Dispose();
            LFGs.Remove(lfg);
        }
        private bool TryGetLfg(int id, string msg, string name, out LFG lfg)
        {
            lfg = _lfgs.FirstOrDefault(x => x.Id == id);
            if (lfg == null)
            {
                lfg = _lfgs.FirstOrDefault(x => x.Name == name);
                if (lfg == null)
                {
                    lfg = _lfgs.FirstOrDefault(x => x.Message == msg);
                    if (lfg == null)
                    {
                        return false;
                    }
                    return true;
                }
                return false; ;
            }
            else
            {
                return true;
            }
        }
        public void UpdateLfgMembers(S_PARTY_MEMBER_INFO p)
        {
            LFG lfg;
            if (TryGetLfg(p.Id, "", "", out lfg))
            {
                lfg.MembersCount = p.MembersCount;
            }
        }
        public void RefreshHideTimer()
        {
            hideTimer.Stop();
            hideTimer.Start();
        }
        public void StopHideTimer()
        {
            hideTimer.Stop();
            IsChatVisible = true;

        }
        public void ClearAll()
        {
            ChatMessages.Clear();
            LFGs.Clear();
        }

        public void LoadTabs(IEnumerable<Tab> tabs)
        {

            Tabs = new SynchronizedObservableCollection<Tab>();
            foreach (var chatTabsSetting in tabs)
            {
                Tabs.Add(chatTabsSetting);
            }
            if (Tabs.Count != 0) return;
            Tabs.Add(new Tab("ALL", new ChatChannel[] { }, new string[] { }));
            Tabs.Add(new Tab("GUILD", new ChatChannel[] { ChatChannel.Guild, ChatChannel.GuildNotice, }, new string[] { }));
            Tabs.Add(new Tab("GROUP", new ChatChannel[]{ChatChannel.Party, ChatChannel.PartyNotice,
                ChatChannel.RaidLeader, ChatChannel.RaidNotice,
                ChatChannel.Raid, ChatChannel.Ress,ChatChannel.Death,
                ChatChannel.Group, ChatChannel.GroupAlerts  }, new string[] { }));
            Tabs.Add(new Tab("WHISPERS", new ChatChannel[] { ChatChannel.ReceivedWhisper, ChatChannel.SentWhisper, }, new string[] { }));
            Tabs.Add(new Tab("SYSTEM", new ChatChannel[] { }, new string[] { "System" }));
        }
    }

    public class Tab : TSPropertyChanged
    {
        public List<ChatChannelOnOff> AllChannels => Utils.GetEnabledChannelsList();
        private ICollectionView _messages;
        private string _tabName;

        public string TabName
        {
            get => _tabName;
            set
            {
                if (_tabName == value) return;
                _tabName = value;
                NotifyPropertyChanged(nameof(TabName));
            }
        }

        public SynchronizedObservableCollection<string> Authors { get; set; }
        public SynchronizedObservableCollection<ChatChannel> Channels { get; set; }
        public ICollectionView Messages
        {
            get => _messages;
            set
            {
                if (_messages == value) return;
                _messages = value;
                NotifyPropertyChanged(nameof(Messages));
            }
        }

        public void Refresh()
        {
            Messages.Refresh();
        }

        public Tab(string n, ChatChannel[] ch, string[] a)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            TabName = n;
            Messages = new ListCollectionView(ChatWindowViewModel.Instance.ChatMessages);
            Authors = new SynchronizedObservableCollection<string>(_dispatcher);
            Channels = new SynchronizedObservableCollection<ChatChannel>(_dispatcher);
            foreach (var auth in a)
            {
                Authors.Add(auth);
            }
            foreach (var chan in ch)
            {
                Channels.Add(chan);
            }
            if (Channels.Count == 0 && Authors.Count == 0)
            {
                Messages.Filter = null;
                return;
            }
            ApplyFilter();


        }

        public void ApplyFilter()
        {
            Messages.Filter = f =>
            {
                var m = f as ChatMessage;
                if (Authors.Count == 0 && Channels.Count != 0)
                {
                    return Channels.Contains(m.Channel);
                }
                if (Channels.Count == 0 && Authors.Count != 0)
                {
                    return Authors.Contains(m.Author);
                }
                return Authors.Contains(m.Author) && Channels.Contains(m.Channel);
            };
        }
    }
}
