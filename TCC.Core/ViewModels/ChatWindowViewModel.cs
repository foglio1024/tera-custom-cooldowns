using Dragablz;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Controls.ChatControls;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.Windows;

namespace TCC.ViewModels
{
    public struct TempPrivateMessage
    {
        public uint Channel { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
    }
    public class ChatWindowManager : TccWindowViewModel
    {
        private readonly DispatcherTimer _hideTimer;
        private static ChatWindowManager _instance;
        private bool _isChatVisible;
        private readonly ConcurrentQueue<ChatMessage> _queue;
        private readonly List<TempPrivateMessage> _privateMessagesCache;
        public event Action<ChatMessage> NewMessage;
        public event Action<int> PrivateChannelJoined;

        //internal void CloseTooltip()
        //{
        //    ChatWindows[0].Dispatcher.Invoke(() =>
        //    {
        //        if ((ChatWindows[0].PlayerInfo.Child as PlayerTooltip).MgPopup.IsMouseOver) return;
        //        ChatWindows[0].CloseTooltip();
        //    });
        //}

        public List<SimpleUser> Friends;
        public List<string> BlockedUsers;
        public LFG LastClickedLfg;
        public readonly PrivateChatChannel[] PrivateChannels = new PrivateChatChannel[8];

        public static ChatWindowManager Instance => _instance ?? (_instance = new ChatWindowManager());

        internal void SetPaused(bool v, ChatMessage dc)
        {
            ChatWindows.ToList().ForEach(w =>
            {
                if (w.VM == null) return;
                if (w.VM.CurrentTab == null) return;
                if (w.VM.CurrentTab.Messages == null) return; //whatever
                if (w.VM.CurrentTab.Messages.Contains(dc)) w.VM.Paused = v;
            });
        }

        public bool IsChatVisible
        {
            get => _isChatVisible;
            private set
            {
                if (_isChatVisible == value) return;
                _isChatVisible = value;
                NPC(nameof(IsChatVisible));
            }
        }

        internal void SetPaused(bool v)
        {
            ChatWindows.ToList().ForEach(w =>
            {
                if (w.VM != null) w.VM.Paused = v;
            });
        }

        internal void ScrollToBottom()
        {
            //throw new NotImplementedException();
        }

        public int MessageCount => ChatMessages.Count;

        public bool IsQueueEmpty => _queue.Count == 0;
        public SynchronizedObservableCollection<ChatMessage> ChatMessages { get; }
        public SynchronizedObservableCollection<LFG> LFGs { get; }
        public SynchronizedObservableCollection<ChatWindow> ChatWindows { get; }


        //private List<ChatChannelOnOff> VisibleChannels => SettingsManager.EnabledChatChannels;

        private readonly object _lock = new object();

        private ChatWindowManager()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            //_scale = SettingsManager.ChatWindowSettings.Scale; TODO
            ChatMessages = new SynchronizedObservableCollection<ChatMessage>(_dispatcher);
            _queue = new ConcurrentQueue<ChatMessage>();
            _privateMessagesCache = new List<TempPrivateMessage>();
            LFGs = new SynchronizedObservableCollection<LFG>(_dispatcher);
            ChatWindows = new SynchronizedObservableCollection<ChatWindow>();
            _hideTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(15) };
            _hideTimer.Tick += HideTimer_Tick;

            ChatMessages.CollectionChanged += ChatMessages_CollectionChanged;
            BindingOperations.EnableCollectionSynchronization(ChatMessages, _lock);

            //get bool from settings
            //
            //
            BlockedUsers = new List<string>();
            Friends = new List<SimpleUser>();
            //PrivateChannels[6] = new PrivateChatChannel(uint.MaxValue - 3, "Astral", 6);
            //PrivateChannels[7] = new PrivateChatChannel(uint.MaxValue - 1, "Proxy", 7);
            ChatWindows.CollectionChanged += ChatWindows_CollectionChanged;
            //TODO: create windows based on settings
            PrivateChannelJoined += OnPrivateChannelJoined;

        }

        private void OnPrivateChannelJoined(int index)
        {
            //Console.WriteLine($"Joined channel {PrivateChannels[index].Name}");
            var messagesToAdd = _privateMessagesCache.Where(x => x.Channel == PrivateChannels[index].Id).ToList();
            messagesToAdd.ForEach(x =>
            {
                //Console.WriteLine($"Flushing {x.Channel}|{x.Message} to main list");
                AddChatMessage(new ChatMessage(
                    (ChatChannel)index + 11, 
                    x.Author == "undefined" ? "System" : x.Author,
                    x.Message));
            });
            messagesToAdd.ForEach(x => _privateMessagesCache.Remove(x));
        }

        private void ChatWindows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems.Count == 0) return;
                SettingsManager.ChatWindowsSettings.Remove((e.OldItems[0] as ChatWindow).WindowSettings as ChatWindowSettings);
            }
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
            for (var i = 0; i < itemsToAdd; i++)
            {
                if (_queue.TryDequeue(out var msg))
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
            _hideTimer.Stop();
        }
        private void ChatMessages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshTimer();
            NPC($"NewItem");
            IsChatVisible = true;
        }
        public void AddChatMessage(ChatMessage chatMessage)
        {
            if (!SettingsManager.ChatEnabled) return;
            if (BlockedUsers.Contains(chatMessage.Author)) return;
            if (ChatMessages.Count < SettingsManager.SpamThreshold)
            {
                for (var i = 0; i < ChatMessages.Count - 1; i++)
                {
                    var m = ChatMessages[i];
                    if (!Pass(chatMessage, m)) return;
                }
            }
            else
            {
                for (var i = 0; i < SettingsManager.SpamThreshold; i++)
                {
                    if (i > ChatMessages.Count - 1) continue;

                    var m = ChatMessages[i];
                    if (!Pass(chatMessage, m)) return;
                }
            }

            ChatMessage.SplitSimplePieces(chatMessage);

            if (ChatWindows.All(x => !x.IsPaused))
            {

                    //Console.WriteLine($"Adding {chatMessage.Channel}|{chatMessage.RawMessage} to main list");
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
            ChatWindows.Clear();
            SettingsManager.ChatWindowsSettings.ToList().ForEach(s =>
            {
                if (s.Tabs.Count == 0) return;
                var w = new ChatWindow(s);
                var m = new ChatViewModel();
                w.DataContext = m;
                ChatWindows.Add(w);
                m.LoadTabs(s.Tabs);
                m.LfgOn = s.LfgOn;
                m.BackgroundOpacity = s.BackgroundOpacity;
            });
            if (ChatWindows.Count == 0)
            {
                var w = new ChatWindow(
                    new ChatWindowSettings(0, 1, 200, 500, true, ClickThruMode.Never, 1, false, 1, false, true, false)
                    );
                SettingsManager.ChatWindowsSettings.Add(w.WindowSettings as ChatWindowSettings);
                var m = new ChatViewModel();
                w.DataContext = m;
                ChatWindows.Add(w);
                m.LoadTabs();
                if (SettingsManager.ChatEnabled) w.Show();
            }

            //WindowManager.TccVisibilityChanged += (s, ev) =>
            //{
            //    if (WindowManager.IsTccVisible)
            //    {
            //        ChatWindows.ToList().ForEach(w => w.RefreshTopmost());
            //    }
            //};

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
            if (TryGetLfg(x.Id, x.Message, x.Name, out var lfg))
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
                return false;
            }
            else
            {
                return true;
            }
        }
        public void UpdateLfgMembers(S_PARTY_MEMBER_INFO p)
        {
            if (TryGetLfg(p.Id, "", "", out var lfg))
            {
                lfg.MembersCount = p.Members.Count;
            }
        }
        public void RefreshTimer()
        {
            _hideTimer.Stop();
            _hideTimer.Start();
        }
        public void StopHideTimer()
        {
            _hideTimer.Stop();
            IsChatVisible = true;

        }

        public void JoinPrivateChannel(uint id, int index, string name)
        {
            ChatWindowManager.Instance.PrivateChannels[index] = new PrivateChatChannel(id, name, index);
            PrivateChannelJoined?.Invoke(index);
        }
        private bool Pass(ChatMessage current, ChatMessage old)
        {
            if (current.Author == SessionManager.CurrentPlayer.Name) return true;
            if (old.RawMessage == current.RawMessage)
            {
                if (old.Author == current.Author)
                {
                    switch (current.Channel)
                    {
                        case ChatChannel.Money:
                        case ChatChannel.Loot:
                        case ChatChannel.Bargain:
                        case ChatChannel.Damage:
                        case ChatChannel.Private1:
                        case ChatChannel.Private2:
                        case ChatChannel.Private3:
                        case ChatChannel.Private4:
                        case ChatChannel.Private5:
                        case ChatChannel.Private6:
                        case ChatChannel.Private7:
                        case ChatChannel.Private8:
                            return true;
                    }

                    switch (old.Channel)
                    {
                        case ChatChannel.SentWhisper when current.Channel == ChatChannel.ReceivedWhisper:
                        case ChatChannel.ReceivedWhisper when current.Channel == ChatChannel.SentWhisper:
                            return true;
                    }

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

        public void TempShow()
        {
            foreach (var chatWindow in ChatWindows)
            {
                //chatWindow.TempShow();
            }
        }

        internal void CachePrivateMessage(uint channel, string author, string message)
        {
            _privateMessagesCache.Add(new TempPrivateMessage(){Channel = channel, Author = author, Message = message});
        }

    }

    public class ChatTabClient : IInterTabClient
    {
        public static ChatWindow LastSource;
        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            LastSource = Window.GetWindow(source) as ChatWindow;
            var model = new ChatViewModel();
            var view = new ChatWindow(new ChatWindowSettings(0, 0, 200, 500, true, ClickThruMode.Never,
                1, false, 1, false, true, false), model);
            ChatWindowManager.Instance.ChatWindows.Add(view);
            return new NewTabHost<Window>(view, view.TabControl);

        }
        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            ChatWindowManager.Instance.ChatWindows.Remove(window as ChatWindow);
            window.Close();
            return TabEmptiedResponse.DoNothing;
        }
    }
}
