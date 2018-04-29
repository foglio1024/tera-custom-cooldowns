using Dragablz;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.Windows;

namespace TCC.ViewModels
{
    public class ChatWindowManager : TccWindowViewModel
    {
        private DispatcherTimer hideTimer;
        private static ChatWindowManager _instance;
        private bool isChatVisible;
        private SynchronizedObservableCollection<ChatMessage> _chatMessages;
        private ConcurrentQueue<ChatMessage> _queue;

        public event Action<ChatMessage> NewMessage;

        internal void LockTooltip(bool v)
        {
            throw new NotImplementedException();
        }
        internal void CloseTooltip()
        {
            ChatWindows[0].CloseTooltip();
        }
        internal void RemoveTab(Tab dc)
        {
            ChatWindows.ToList().ForEach(x =>
            {
                if (x.VM.Tabs.Contains(dc)) x.VM.RemoveTab(dc);
            });
        }

        public List<SimpleUser> Friends;
        public List<string> BlockedUsers;
        public LFG LastClickedLfg;
        private SynchronizedObservableCollection<LFG> _lfgs;
        public readonly PrivateChatChannel[] PrivateChannels = new PrivateChatChannel[8];

        public static ChatWindowManager Instance => _instance ?? (_instance = new ChatWindowManager());

        internal void SetPaused(bool v, ChatMessage dc)
        {
            ChatWindows.ToList().ForEach(w =>
            {
                if (w.VM.CurrentTab.Messages.Contains(dc)) w.VM.Paused = v;
            });
        }

        public bool IsChatVisible
        {
            get => isChatVisible;
            set
            {
                if (isChatVisible == value) return;
                isChatVisible = value;
                NPC(nameof(IsChatVisible));
            }
        }

        internal void SetPaused(bool v)
        {
            ChatWindows.ToList().ForEach(w =>
            {
                w.VM.Paused = v;
            });
        }

        internal void ScrollToBottom()
        {
            throw new NotImplementedException();
        }

        public int MessageCount
        {
            get => ChatMessages.Count;
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
        public SynchronizedObservableCollection<ChatWindow> ChatWindows { get; set; }

        public List<ChatChannelOnOff> VisibleChannels => SettingsManager.EnabledChatChannels;

        public object CurrentSender { get; internal set; }

        private readonly object _lock = new object();

        public ChatWindowManager()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            //_scale = SettingsManager.ChatWindowSettings.Scale; TODO
            _chatMessages = new SynchronizedObservableCollection<ChatMessage>(_dispatcher);
            _queue = new ConcurrentQueue<ChatMessage>();
            _lfgs = new SynchronizedObservableCollection<LFG>(_dispatcher);
            ChatWindows = new SynchronizedObservableCollection<ChatWindow>();
            hideTimer = new DispatcherTimer();
            hideTimer.Interval = TimeSpan.FromSeconds(15);
            hideTimer.Tick += HideTimer_Tick;

            ChatMessages.CollectionChanged += ChatMessages_CollectionChanged;
            BindingOperations.EnableCollectionSynchronization(ChatMessages, _lock);

            //get bool from settings
            //
            //
            BlockedUsers = new List<string>();
            Friends = new List<SimpleUser>();
            TooltipInfo = new TooltipInfo("", "", 1);
            PrivateChannels[7] = new PrivateChatChannel(uint.MaxValue - 1, "Proxy", 7);
            ChatWindows.CollectionChanged += ChatWindows_CollectionChanged;
            //TODO: create windows based on settings

        }

        private void ChatWindows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Console.WriteLine($"Chat windows list changed; count: {ChatWindows.Count}");


            //ChatWindows.ToList().ForEach(x =>
            //{
            //    //Console.WriteLine($"\t - {x.VM.TabVMs.Count} tabs");
            //});
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
            NPC("NewItem");
            IsChatVisible = true;
        }
        public void AddChatMessage(ChatMessage chatMessage)
        {
            if (!SettingsManager.ChatEnabled) return;
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
                }
                catch { }
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

            if (ChatWindows.Any(x => !x.IsPaused))
            {
                ChatMessages.Insert(0, chatMessage);
            }
            else _queue.Enqueue(chatMessage);

            NewMessage?.Invoke(chatMessage);
            if (ChatMessages.Count > SettingsManager.MaxMessages)
            {
                ChatMessages.RemoveAt(ChatMessages.Count - 1);
            }
            NPC(nameof(MessageCount));
        }

        internal void InitWindows()
        {
            SettingsManager.ChatWindowsSettings.ToList().ForEach(s =>
            {
                var w = new ChatWindow(s);
                var m = new ChatViewModel();
                w.DataContext = m;
                ChatWindows.Add(w);
                m.LoadTabs(s.Tabs);
                m.LfgOn = s.LfgOn;
                m.BackgroundOpacity = s.BackgroundOpacity;
                w.Show();
            });
            if (ChatWindows.Count == 0)
            {
                var w = new ChatWindow(
                    new ChatWindowSettings(0, 0, 200, 500, true, ClickThruMode.Never, 1, false, 1, false, true, true)
                    );
                var m = new ChatViewModel();
                w.DataContext = m;
                ChatWindows.Add(w);
                m.LoadTabs();
                w.Show();
            }

            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                if (WindowManager.IsTccVisible)
                {
                    ChatWindows.ToList().ForEach(w => w.RefreshTopmost());
                }
            };

        }

        internal void CloseAllWindows()
        {
            foreach (var w in ChatWindows)
            {
                w.CloseWindowSafe();
            }
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
        private bool TryGetLfg(uint id, string msg, string name, out LFG lfg)
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
                lfg.MembersCount = p.Members.Count;
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


        //public void ScrollToBottom()
        //{
        //    WindowManager.ChatWindow.ScrollToBottom();
        //}

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
            ChatWindows.ToList().ForEach(x =>
            {
                //TODO: make this different per window
                x.VM.NotifyOpacityChange();
            });
        }

        internal void OpenTooltip()
        {
            ChatWindows[0].OpenTooltip();
        }

        internal SynchronizedObservableCollection<HeaderedItemViewModel> FindContainer(HeaderedItemViewModel i)
        {
            foreach (var w in ChatWindows)
            {
                if (w.VM.TabVMs.Contains(i)) return w.VM.TabVMs;
            }
            return null; //should never be the case
        }

        internal ChatWindow FindMyWindow(ChatViewModel chatViewModel)
        {
            foreach (var w in ChatWindows)
            {
                if (w.VM == chatViewModel) return w;
            }
            return null; //should never be the case
        }
    }

    public class ChatViewModel : INotifyPropertyChanged
    {
        private SynchronizedObservableCollection<HeaderedItemViewModel> _tabVMs;
        private bool canUpdate = true;
        private bool paused;
        private bool _lfgOn;
        private double _backgroundOpacity = 0.3;

        public bool Paused
        {
            get { return paused; }
            set
            {
                if (paused == value) return;
                paused = value;
                NPC();
            }
        }
        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
        public SynchronizedObservableCollection<HeaderedItemViewModel> TabVMs
        {
            get => _tabVMs;
            set
            {
                if (_tabVMs == value) return;
                _tabVMs = value;
                NPC(nameof(TabVMs));
            }
        }
        public IInterTabClient InterTabClient { get; } = new ChatTabClient();
        public List<Tab> Tabs
        {
            get
            {
                var ret = new List<Tab>();
                TabVMs.ToList().ForEach(x => ret.Add(x.Content as Tab));
                return ret;
            }
        }
        public SynchronizedObservableCollection<LFG> LFGs => ChatWindowManager.Instance.LFGs;
        public Tab CurrentTab { get; set; }
        public double ChatWindowOpacity => SettingsManager.ChatWindowOpacity;
        public Func<HeaderedItemViewModel> AddNewTabCommand
        {
            get
            {
                return
                    () =>
                    {
                        var t = new HeaderedItemViewModel()
                        {
                            Content = new Tab("NEW TAB", new ChatChannel[] { }, new ChatChannel[] { }, new string[] { }, new string[] { })
                        };
                        t.Header = (t.Content as Tab).TabName;
                        return t;
                    };
            }
        }

        public bool LfgOn
        {
            get => _lfgOn; set
            {
                if (_lfgOn == value) return;
                _lfgOn = value;
                NPC();
            }
        }
        public double BackgroundOpacity
        {
            get => _backgroundOpacity; set
            {
                if (_backgroundOpacity == value) return;
                _backgroundOpacity = value;
                NPC();
            }
        }

        public void NotifyOpacityChange()
        {
            NPC(nameof(ChatWindowOpacity));
        }
        public ChatViewModel()
        {
            TabVMs = new SynchronizedObservableCollection<HeaderedItemViewModel>();
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                NPC("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    //WindowManager.ChatWindow.RefreshTopmost(); //TODO: handle event in ChatWindow.xaml.cs
                }
            };
            ChatWindowManager.Instance.NewMessage += CheckAttention;
            TabVMs.CollectionChanged += TabVMs_CollectionChanged;
            //LoadTabs(SettingsManager.ParseTabsSettings());

        }
        private void TabVMs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if (!canUpdate) return;
            //var w = ChatWindowManager.Instance.FindMyWindow(this);
            //w.UpdateSettings();
        }

        public void LoadTabs(IEnumerable<Tab> tabs = null)
        {
            if (tabs != null)
            {
                canUpdate = false;
                foreach (var chatTabsSetting in tabs)
                {
                    TabVMs.Add(new HeaderedItemViewModel(chatTabsSetting.TabName, chatTabsSetting));
                }
                canUpdate = true;
            }
            if (TabVMs.Count != 0) return;
            canUpdate = false;
            var all = new Tab("ALL", new ChatChannel[] { }, new ChatChannel[] { }, new string[] { }, new string[] { "System" });
            var guild = new Tab("GUILD", new ChatChannel[] { ChatChannel.Guild, ChatChannel.GuildNotice, }, new ChatChannel[] { }, new string[] { }, new string[] { });
            var group = new Tab("GROUP", new ChatChannel[]{ChatChannel.Party, ChatChannel.PartyNotice,
                ChatChannel.RaidLeader, ChatChannel.RaidNotice,
                ChatChannel.Raid, ChatChannel.Ress,ChatChannel.Death,
                ChatChannel.Group, ChatChannel.GroupAlerts  }, new ChatChannel[] { }, new string[] { }, new string[] { });
            var w = new Tab("WHISPERS", new ChatChannel[] { ChatChannel.ReceivedWhisper, ChatChannel.SentWhisper, }, new ChatChannel[] { }, new string[] { }, new string[] { });
            var sys = new Tab("SYSTEM", new ChatChannel[] { }, new ChatChannel[] { }, new string[] { "System" }, new string[] { });
            TabVMs.Add(new HeaderedItemViewModel(all.TabName, all));
            TabVMs.Add(new HeaderedItemViewModel(guild.TabName, guild));
            TabVMs.Add(new HeaderedItemViewModel(group.TabName, group));
            TabVMs.Add(new HeaderedItemViewModel(w.TabName, w));
            TabVMs.Add(new HeaderedItemViewModel(sys.TabName, sys));
            canUpdate = true;
            CurrentTab = TabVMs[0].Content as Tab;
            //ChatWindowManager.Instance.FindMyWindow(this).UpdateSettings();

        }
        public void CheckAttention(ChatMessage chatMessage)
        {
            if (CurrentTab != null && !CurrentTab.Filter(chatMessage))
            {
                chatMessage.Animate = false; //set animate to false if the message is not going in the active tab
                if (chatMessage.ContainsPlayerName || chatMessage.Channel == ChatChannel.ReceivedWhisper)
                {
                    var t = TabVMs.FirstOrDefault(x => (x.Content as Tab).Channels.Contains(chatMessage.Channel));
                    if (t != null)
                    {
                        (t.Content as Tab).Attention = true;
                    }
                    else
                    {
                        t = TabVMs.FirstOrDefault(x => !(x.Content as Tab).ExcludedChannels.Contains(chatMessage.Channel));
                        if (t != null) (t.Content as Tab).Attention = true;
                    }
                }
            }

        }

        private void NPC([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        internal void RemoveTab(Tab dc)
        {
            var t = TabVMs.FirstOrDefault(x => x.Content == dc);
            if (t != null) TabVMs.Remove(t);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal class ChatTabClient : IInterTabClient
    {
        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            var model = new ChatViewModel();
            var view = new ChatWindow(new ChatWindowSettings(0, 0, 200, 500, true, ClickThruMode.Never,
                1, false, 1, false, true, true), model);
            ChatWindowManager.Instance.ChatWindows.Add(view);
            return new NewTabHost<Window>(view, view.tabControl);

        }
        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            ChatWindowManager.Instance.ChatWindows.Remove(window as ChatWindow);
            return TabEmptiedResponse.CloseWindowOrLayoutBranch;
        }
    }
}
