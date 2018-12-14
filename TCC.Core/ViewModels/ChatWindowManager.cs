using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Data.Pc;
using TCC.Parsing.Messages;
using TCC.Settings;
using TCC.Windows.Widgets;

namespace TCC.ViewModels
{
    public class ChatWindowManager : TccWindowViewModel
    {
        private static ChatWindowManager _instance;
        public static ChatWindowManager Instance => _instance ?? (_instance = new ChatWindowManager());

        private readonly ConcurrentQueue<ChatMessage> _queue;
        private readonly List<TempPrivateMessage> _privateMessagesCache;
        public readonly PrivateChatChannel[] PrivateChannels = new PrivateChatChannel[8];
        private readonly object _lock = new object();

        public event Action<ChatMessage> NewMessage;
        public event Action<int> PrivateChannelJoined;

        public List<SimpleUser> Friends { get; set; }
        public List<string> BlockedUsers { get; set; }
        public LFG LastClickedLfg { get; set; }

        public int MessageCount => ChatMessages.Count;
        public bool IsQueueEmpty => _queue.Count == 0;
        public SynchronizedObservableCollection<ChatMessage> ChatMessages { get; }
        public SynchronizedObservableCollection<LFG> LFGs { get; }
        public SynchronizedObservableCollection<ChatWindow> ChatWindows { get; }

        private ChatWindowManager()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            //_scale = Settings.ChatWindowSettings.Scale; TODO
            ChatMessages = new SynchronizedObservableCollection<ChatMessage>(Dispatcher);
            _queue = new ConcurrentQueue<ChatMessage>();
            _privateMessagesCache = new List<TempPrivateMessage>();
            LFGs = new SynchronizedObservableCollection<LFG>(Dispatcher);
            ChatWindows = new SynchronizedObservableCollection<ChatWindow>();

            ChatMessages.CollectionChanged += OnChatMessagesCollectionChanged;
            BindingOperations.EnableCollectionSynchronization(ChatMessages, _lock);

            BlockedUsers = new List<string>();
            Friends = new List<SimpleUser>();
            ChatWindows.CollectionChanged += OnChatWindowsCollectionChanged;
            //TODO: create windows based on settings
            PrivateChannelJoined += OnPrivateChannelJoined;
        }
        public void InitWindows()
        {
            ChatWindows.Clear();
            SettingsStorage.ChatWindowsSettings.ToList().ForEach(s =>
            {
                if (s.Tabs.Count == 0) return;
                var m = new ChatViewModel();
                var w = new ChatWindow(s, m);
                //w.DataContext = m;
                ChatWindows.Add(w);
                m.LoadTabs(s.Tabs);
                //m.LfgOn = s.LfgOn;
                //m.BackgroundOpacity = s.BackgroundOpacity;
            });
            if (ChatWindows.Count == 0)
            {
                var ws = new ChatWindowSettings(0, 1, 200, 500, true, ClickThruMode.Never, 1, false, 1, false, true,
                    false);
                var m = new ChatViewModel();
                var w = new ChatWindow(ws, m);
                SettingsStorage.ChatWindowsSettings.Add(w.WindowSettings as ChatWindowSettings);
                ChatWindows.Add(w);
                m.LoadTabs();
                //m.LfgOn = ws.LfgOn;
                //m.BackgroundOpacity = ws.BackgroundOpacity;
                if (SettingsStorage.ChatEnabled) w.Show();
            }
        }
        public void CloseAllWindows()
        {
            foreach (var w in ChatWindows)
            {
                w.CloseWindowSafe();
            }
        }

        public void SetPaused(bool v)
        {
            ChatWindows.ToList().ForEach(w =>
            {
                if (w.VM != null) w.VM.Paused = v;
            });
        }
        public void SetPaused(bool v, ChatMessage dc)
        {
            ChatWindows.ToList().ForEach(w =>
            {
                if (w.VM?.CurrentTab?.Messages == null) return;
                if (w.VM.CurrentTab.Messages.Contains(dc)) w.VM.Paused = v;
            });
        }
        public void ScrollToBottom()
        {
            //TODO?
        }
        //public void NotifyOpacityChange()
        //{
        //    ChatWindows.ToList().ForEach(x =>
        //    {
        //        //TODO: make this different per window
        //        x.VM.NotifyOpacityChange();
        //    });
        //}

        public void AddChatMessage(ChatMessage chatMessage)
        {
            //return;
            if (!SettingsStorage.ChatEnabled)
            {
                chatMessage.Dispose();
                return;
            }

            if (BlockedUsers.Contains(chatMessage.Author))
            {
                chatMessage.Dispose();
                return;
            }
            if (ChatMessages.Count < SettingsStorage.SpamThreshold)
            {
                for (var i = 0; i < ChatMessages.Count - 1; i++)
                {
                    var m = ChatMessages[i];
                    if (!Pass(chatMessage, m))
                    {
                        chatMessage.Dispose();
                        return;
                    }
                }
            }
            else
            {
                for (var i = 0; i < SettingsStorage.SpamThreshold; i++)
                {
                    if (i > ChatMessages.Count - 1) continue;

                    var m = ChatMessages[i];
                    if (!Pass(chatMessage, m))
                    {
                        chatMessage.Dispose();

                        return;
                    }
                }
            }

            chatMessage.SplitSimplePieces();

            if (ChatWindows.All(x => !x.IsPaused))
            {
                ChatMessages.Insert(0, chatMessage);
                //_cache.Add(chatMessage);
            }
            else _queue.Enqueue(chatMessage);

            NewMessage?.Invoke(chatMessage);
            if (ChatMessages.Count > SettingsStorage.MaxMessages)
            {
                var toRemove = ChatMessages[ChatMessages.Count - 1];
                toRemove.Dispose();
                ChatMessages.RemoveAt(ChatMessages.Count - 1);
            }
            N(nameof(MessageCount));
        }
        public void AddTccMessage(string message)
        {
            var msg = new ChatMessage(ChatChannel.TCC, "System", "<FONT>" + message + "</FONT>");
            AddChatMessage(msg);
        }
        public void AddDamageReceivedMessage(ulong source, ulong target, long diff, long maxHP)
        {
            if (!target.IsMe() || diff > 0 || target == source || source == 0 ||
                !EntityManager.IsEntitySpawned(source)) return;
            var srcName = EntityManager.GetEntityName(source);
            srcName = srcName != ""
                ? $"<font color=\"#cccccc\"> from </font><font>{srcName}</font><font color=\"#cccccc\">.</font>"
                : "<font color=\"#cccccc\">.</font>";
            AddChatMessage(new ChatMessage(ChatChannel.Damage, "System",
                $"<font color=\"#cccccc\">Received </font> <font>{-diff}</font> <font color=\"#cccccc\"> (</font><font>{-diff / (double)maxHP:P}</font><font color=\"#cccccc\">)</font> <font color=\"#cccccc\"> damage</font>{srcName}"));
        }
        public void AddFromQueue(int itemsToAdd)
        {
            if (itemsToAdd == 0) itemsToAdd = _queue.Count;
            for (var i = 0; i < itemsToAdd; i++)
            {
                if (_queue.TryDequeue(out var msg))
                {
                    ChatMessages.Insert(0, msg);
                    if (ChatMessages.Count > SettingsStorage.MaxMessages)
                    {
                        ChatMessages.RemoveAt(ChatMessages.Count - 1);
                    }
                }
            }
        }

        private static bool Pass(ChatMessage current, ChatMessage old)
        {
            if (current.Author == SessionManager.CurrentPlayer.Name ||
                old.Author == SessionManager.CurrentPlayer.Name) return true;
            if (old.RawMessage != current.RawMessage) return true;
            if (old.Author != current.Author) return true;
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
                case ChatChannel.TCC:
                case ChatChannel.Guardian:
                case ChatChannel.Apply:
                case ChatChannel.Death:
                case ChatChannel.Ress:
                case ChatChannel.WorldBoss:
                case ChatChannel.Enchant:
                case ChatChannel.Friend:
                case ChatChannel.Laurel:
                case ChatChannel.GuildNotice:
                case ChatChannel.SentWhisper:
                case ChatChannel.ReceivedWhisper when old.Channel == ChatChannel.SentWhisper:
                    return true;
            }
            return false;
        }

        public void JoinPrivateChannel(uint id, int index, string name)
        {
            Instance.PrivateChannels[index] = new PrivateChatChannel(id, name, index);
            PrivateChannelJoined?.Invoke(index);
        }
        private void OnPrivateChannelJoined(int index)
        {
            var messagesToAdd = _privateMessagesCache.Where(x => x.Channel == PrivateChannels[index].Id).ToList();
            messagesToAdd.ForEach(x =>
            {
                AddChatMessage(new ChatMessage(
                    (ChatChannel)index + 11,
                    x.Author == "undefined" ? "System" : x.Author,
                    x.Message));
            });
            messagesToAdd.ForEach(x => _privateMessagesCache.Remove(x));
        }
        public void CachePrivateMessage(uint channel, string author, string message)
        {
            _privateMessagesCache.Add(new TempPrivateMessage() { Channel = channel, Author = author, Message = message });
        }

        private static void OnChatWindowsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Remove) return;
            if (e.OldItems.Count == 0) return;
            SettingsStorage.ChatWindowsSettings.Remove((e.OldItems[0] as ChatWindow)?.WindowSettings as ChatWindowSettings);
        }

        private void OnChatMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;
            //TODO
            foreach (var chatWindow in ChatWindows)
            {
                chatWindow.VM.CheckVisibility(e.NewItems);
            }
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
        public void RemoveDeadLfg()
        {
            if (LastClickedLfg != null)
            {
                RemoveLfg(LastClickedLfg);
                LastClickedLfg = null;
            }
        }
        private bool TryGetLfg(uint id, string msg, string name, out LFG lfg)
        {
            lfg = LFGs.ToSyncArray().FirstOrDefault(x => x.Id == id);
            if (lfg != null) return true;
            lfg = LFGs.ToSyncArray().FirstOrDefault(x => x.Name == name);
            if (lfg != null) return false;
            lfg = LFGs.ToSyncArray().FirstOrDefault(x => x.Message == msg);
            return lfg != null;
        }
        public void UpdateLfgMembers(S_PARTY_MEMBER_INFO p)
        {
            if (TryGetLfg(p.Id, "", "", out var lfg))
            {
                lfg.MembersCount = p.Members.Count;
            }
        }

        public void ForceHideTimerRefresh()
        {
            foreach (var chatWindow in ChatWindows)
            {
                chatWindow.VM.RefreshHideTimer();
            }
        }
    }
}