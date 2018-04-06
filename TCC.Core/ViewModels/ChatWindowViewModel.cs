using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.Windows;

namespace TCC.ViewModels
{
    public class ChatWindowViewModel : TccWindowViewModel
    {
        private DispatcherTimer hideTimer;
        private static ChatWindowViewModel _instance;

        private bool isChatVisible;
        private SynchronizedObservableCollection<ChatMessage> _chatMessages;
        private ConcurrentQueue<ChatMessage> _queue;
        public List<SimpleUser> Friends;
        public List<string> BlockedUsers;
        public LFG LastClickedLfg;
        private SynchronizedObservableCollection<LFG> _lfgs;
        public readonly PrivateChatChannel[] PrivateChannels = new PrivateChatChannel[8];
        public Tab CurrentTab { get; set; }
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
        public double ChatWindowOpacity => SettingsManager.ChatWindowOpacity;

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

        public List<ChatChannelOnOff> VisibleChannels => SettingsManager.EnabledChatChannels;
        private readonly object _lock = new object();
        private SynchronizedObservableCollection<Tab> _tabs;

        public ChatWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _scale = SettingsManager.ChatWindowSettings.Scale;
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
            if (SettingsManager.ChatFadeOut)
            {
                IsChatVisible = false;
            }
            hideTimer.Stop();
        }
        private void ChatMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshTimer();
            NotifyPropertyChanged("NewItem");
            IsChatVisible = true;
        }
        public void AddChatMessage(ChatMessage chatMessage)
        {
            if (!SettingsManager.ChatWindowSettings.Enabled) return;
            if (BlockedUsers.Contains(chatMessage.Author)) return;
            var vch = VisibleChannels.FirstOrDefault(x => x.Channel == chatMessage.Channel);
            if (vch == null)
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.Append("TIME: ");
                    sb.Append(DateTime.UtcNow);
                    sb.Append("\n");
                    sb.Append("FROM: ");
                    sb.Append(chatMessage.Author);
                    sb.Append("\n");
                    sb.Append("CHANNEL: ");
                    sb.Append(chatMessage.Channel);
                    sb.Append("\n");
                    sb.Append("TEXT: ");
                    sb.Append(chatMessage.RawMessage);

                    File.WriteAllText("chat-message-error.txt", sb.ToString());
                    var err = new ChatMessage(ChatChannel.Error, "TCC", "Failed to display chat message. Please send chat-message-error.txt to the developer via Discord or GitHub issue.");
                    AddChatMessage(err);
                } catch { }
            }
            if (!vch.Enabled) return;
            if (ChatMessages.Count < SettingsManager.SpamThreshold)
            {
                for (int i = 0; i < ChatMessages.Count - 1; i++)
                {
                    var m = ChatMessages[i];
                    if (!Pass(chatMessage, m)) return;
                }
            }
            else
            {

                for (int i = 0; i < SettingsManager.SpamThreshold; i++)
                {
                    if (i > ChatMessages.Count - 1) continue;

                    var m = ChatMessages[i];
                    if (!Pass(chatMessage, m)) return;
                }
            }

            ChatMessage.SplitSimplePieces(chatMessage);

            if (CurrentTab != null && !CurrentTab.Filter(chatMessage))
            {
                chatMessage.Animate = false; //set animate to false if the message is not going in the active tab
                if (chatMessage.ContainsPlayerName || chatMessage.Channel == ChatChannel.ReceivedWhisper)
                {
                    var t = Tabs.FirstOrDefault(x => x.Channels.Contains(chatMessage.Channel));
                    if (t != null)
                    {
                        t.Attention = true;
                    }
                    else
                    {
                        t = Tabs.FirstOrDefault(x => !x.ExcludedChannels.Contains(chatMessage.Channel));
                        if (t != null) t.Attention = true;
                    }
                }
            }
            if (!Paused) ChatMessages.Insert(0, chatMessage);
            else _queue.Enqueue(chatMessage);

            if (ChatMessages.Count > SettingsManager.MaxMessages)
            {
                ChatMessages.RemoveAt(ChatMessages.Count - 1);
            }
            NotifyPropertyChanged(nameof(MessageCount));
        }

        public void AddTccMessage(string message)
        {
            var msg = new ChatMessage(ChatChannel.TCC, "System", "<FONT>" + message + "</FONT>");
            AddChatMessage(msg);
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
                LFGs.Add(new LFG(x.Id, x.Name, x.Message, x.Raid));
            }
        }
        public void RemoveLfg(LFG lfg)
        {
            lfg.Dispose();
            LFGs.Remove(lfg);
        }
        private bool TryGetLfg(int id, string msg, string name, out LFG lfg)
        {
            lfg = LFGs.ToSyncArray().FirstOrDefault(x => x.Id == id);
            if (lfg == null)
            {
                lfg = LFGs.ToSyncArray().FirstOrDefault(x => x.Name == name);
                if (lfg == null)
                {
                    lfg = LFGs.ToSyncArray().FirstOrDefault(x => x.Message == msg);
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
        public void RefreshTimer()
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
            Tabs.Add(new Tab("ALL", new ChatChannel[] { }, new ChatChannel[] { }, new string[] { }, new string[] { "System" }));
            Tabs.Add(new Tab("GUILD", new ChatChannel[] { ChatChannel.Guild, ChatChannel.GuildNotice, }, new ChatChannel[] { }, new string[] { }, new string[] { }));
            Tabs.Add(new Tab("GROUP", new ChatChannel[]{ChatChannel.Party, ChatChannel.PartyNotice,
                ChatChannel.RaidLeader, ChatChannel.RaidNotice,
                ChatChannel.Raid, ChatChannel.Ress,ChatChannel.Death,
                ChatChannel.Group, ChatChannel.GroupAlerts  }, new ChatChannel[] { }, new string[] { }, new string[] { }));
            Tabs.Add(new Tab("WHISPERS", new ChatChannel[] { ChatChannel.ReceivedWhisper, ChatChannel.SentWhisper, }, new ChatChannel[] { }, new string[] { }, new string[] { }));
            Tabs.Add(new Tab("SYSTEM", new ChatChannel[] { }, new ChatChannel[] { }, new string[] { "System" }, new string[] { }));
            CurrentTab = Tabs[0];
        }

        public void ScrollToBottom()
        {
            WindowManager.ChatWindow.ScrollToBottom();
        }

        private bool Pass(ChatMessage current, ChatMessage old)
        {
            if (current.Author == SessionManager.CurrentPlayer.Name) return true;
            if (old.RawMessage == current.RawMessage)
            {
                if (old.Author == current.Author)
                {
                    if (current.Channel == ChatChannel.Money) return true;
                    if (current.Channel == ChatChannel.Loot) return true;
                    if (current.Channel == ChatChannel.Bargain) return true;
                    if (old.Channel == ChatChannel.SentWhisper && current.Channel == ChatChannel.ReceivedWhisper) return true;
                    if (old.Channel == ChatChannel.ReceivedWhisper && current.Channel == ChatChannel.SentWhisper) return true;
                    return false;
                }
                return false;
            }
            return true;
        }
        public void NotifyOpacityChange()
        {
            NotifyPropertyChanged(nameof(ChatWindowOpacity));
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
        private bool _attention;

        public bool Attention
        {
            get => _attention;
            set
            {
                if (_attention == value) return;
                _attention = value;
                NotifyPropertyChanged(nameof(Attention));
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
                NotifyPropertyChanged(nameof(Messages));
            }
        }

        public void Refresh()
        {
            Messages.Refresh();
        }

        public Tab(string n, ChatChannel[] ch, ChatChannel[] ex, string[] a, string[] exa)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            TabName = n;
            Messages = new ListCollectionView(ChatWindowViewModel.Instance.ChatMessages);
            Authors = new SynchronizedObservableCollection<string>(_dispatcher);
            ExcludedAuthors = new SynchronizedObservableCollection<string>(_dispatcher);
            Channels = new SynchronizedObservableCollection<ChatChannel>(_dispatcher);
            ExcludedChannels = new SynchronizedObservableCollection<ChatChannel>(_dispatcher);
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
                //if (Authors.Count == 0 && Channels.Count != 0)
                //{
                //    if (ExcludedChannels.Count != 0)
                //    {
                //        return Channels.Contains(m.Channel) && !ExcludedChannels.Contains(m.Channel);
                //    }
                //    return Channels.Contains(m.Channel);
                //}
                //if (Channels.Count == 0 && Authors.Count != 0)
                //{
                //    if (ExcludedChannels.Count != 0)
                //    {
                //        return Authors.Contains(m.Author) && !ExcludedChannels.Contains(m.Channel);
                //    }
                //    return Authors.Contains(m.Author);
                //}

            };
        }
    }
}
