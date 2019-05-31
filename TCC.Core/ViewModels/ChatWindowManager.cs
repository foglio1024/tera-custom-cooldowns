using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Settings;
using FoglioUtils.Extensions;
using TCC.Windows.Widgets;
using TeraDataLite;

namespace TCC.ViewModels
{
    public class ChatWindowManager : TccWindowViewModel
    {
        private static ChatWindowManager _instance;
        public static ChatWindowManager Instance => _instance ?? (_instance = new ChatWindowManager());

        private readonly ConcurrentQueue<ChatMessage> _pauseQueue;
        private readonly List<TempPrivateMessage> _privateMessagesCache;
        public readonly PrivateChatChannel[] PrivateChannels = new PrivateChatChannel[8];
        private readonly object _lock = new object();

        public event Action<ChatMessage> NewMessage;
        public event Action<int> PrivateChannelJoined;

        public List<FriendData> Friends { get; set; }
        public List<string> BlockedUsers { get; set; }
        public LFG LastClickedLfg { get; set; }

        public int MessageCount => ChatMessages.Count;
        public bool IsQueueEmpty => _pauseQueue.Count == 0;
        public SynchronizedObservableCollection<ChatWindow> ChatWindows { get; private set; }
        public SynchronizedObservableCollection<ChatMessage> ChatMessages { get; private set; }
        public SynchronizedObservableCollection<LFG> LFGs { get; private set; }

        private ChatWindowManager()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;

            _pauseQueue = new ConcurrentQueue<ChatMessage>();
            _privateMessagesCache = new List<TempPrivateMessage>();

            BlockedUsers = new List<string>();
            Friends = new List<FriendData>();
            ChatWindows = new SynchronizedObservableCollection<ChatWindow>(Dispatcher);
            ChatMessages = new SynchronizedObservableCollection<ChatMessage>(Dispatcher);
            LFGs = new SynchronizedObservableCollection<LFG>(Dispatcher);

            ChatMessages.CollectionChanged += OnChatMessagesCollectionChanged;
            BindingOperations.EnableCollectionSynchronization(ChatMessages, _lock);

            ChatWindows.CollectionChanged += OnChatWindowsCollectionChanged;
            PrivateChannelJoined += OnPrivateChannelJoined;


        }


        public void InitWindows()
        {
            ChatWindows.Clear();
            SettingsHolder.ChatWindowsSettings.ToList().ForEach(s =>
            {
                if (s.Tabs.Count == 0) return;
                var m = new ChatViewModel();
                var w = new ChatWindow(s, m);
                ChatWindows.Add(w);
                m.LoadTabs(s.Tabs);
            });

            if (ChatWindows.Count != 0) return;
            {
                Log.CW("No chat windows found, initializing default one.");
                var ws = new ChatWindowSettings(0, 1, 200, 500, true, ClickThruMode.Never, 1, false, 1, false, true, false) { HideTimeout = 10, FadeOut = true, LfgOn = false };
                var m = new ChatViewModel();
                var w = new ChatWindow(ws, m);
                SettingsHolder.ChatWindowsSettings.Add(w.WindowSettings as ChatWindowSettings);
                ChatWindows.Add(w);
                m.LoadTabs();
                if (SettingsHolder.ChatEnabled) w.Show();
            }
        }
        public void CloseAllWindows()
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var w in ChatWindows)
                {
                    w.CloseWindowSafe();
                }
            });
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

        private bool Filtered(ChatMessage message)
        {
            if (!SettingsHolder.ChatEnabled)
            {
                message.Dispose();
                return true;
            }
            if (BlockedUsers.Contains(message.Author) && !(message is LfgMessage))
            {
                message.Dispose();
                return true;
            }

            var pausedCount = _pauseQueue.Count;
            for (var i = 0; i < SettingsHolder.SpamThreshold; i++)
            {
                if (i >= pausedCount + ChatMessages.Count) continue;
                if (Pass(message, i <= pausedCount - 1
                                                 ? _pauseQueue.ElementAt(i)
                                                 : ChatMessages[i - pausedCount])) continue;
                message.Dispose();
                return true;
            }
            //if (ChatMessages.Count < SettingsHolder.SpamThreshold)
            //{
            //    for (var i = 0; i < ChatMessages.Count - 1; i++)
            //    {
            //        var m = ChatMessages[i];
            //        if (Pass(message, m)) continue;
            //        message.Dispose();
            //        return false;
            //    }
            //}
            //else
            //{
            //    for (var i = 0; i < SettingsHolder.SpamThreshold; i++)
            //    {
            //        if (i > ChatMessages.Count - 1) continue;
            //        var m = ChatMessages[i];
            //        if (Pass(message, m)) continue;
            //        message.Dispose();
            //        return false;
            //    }
            //}
            return false;
        }
        public void AddSystemMessage(string srvMsg, SystemMessage sysMsg)
        {
            AddChatMessage(new ChatMessage(srvMsg, sysMsg, (ChatChannel)sysMsg.ChatChannel));
        }
        public void AddSystemMessage(string srvMsg, SystemMessage sysMsg, ChatChannel channelOverride)
        {
            AddChatMessage(new ChatMessage(srvMsg, sysMsg, channelOverride));
        }
        public void AddChatMessage(ChatMessage chatMessage)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (Filtered(chatMessage)) return;

                if (chatMessage is LfgMessage lm && !SettingsHolder.DisableLfgChatMessages) lm.LinkLfg();

                chatMessage.SplitSimplePieces();

                if (ChatWindows.All(x => !x.IsPaused))
                {
                    ChatMessages.Insert(0, chatMessage);
                }
                else
                {
                    _pauseQueue.Enqueue(chatMessage);
                }

                NewMessage?.Invoke(chatMessage);
                if (ChatMessages.Count > SettingsHolder.MaxMessages)
                {
                    var toRemove = ChatMessages[ChatMessages.Count - 1];
                    toRemove.Dispose();
                    ChatMessages.RemoveAt(ChatMessages.Count - 1);
                }
                N(nameof(MessageCount));
            }), DispatcherPriority.DataBind);
        }
        public void AddTccMessage(string message)
        {
            var msg = new ChatMessage(ChatChannel.TCC, "System", $"<FONT>{message}</FONT>");
            AddChatMessage(msg);
        }
        public void AddDamageReceivedMessage(ulong source, ulong target, long diff, long maxHP)
        {
            if (!SessionManager.IsMe(target) || diff > 0 || target == source || source == 0 || !EntityManager.IsEntitySpawned(source)) return;
            var srcName = EntityManager.GetEntityName(source);
            srcName = srcName != ""
                ? $"<font color=\"#cccccc\"> from </font><font>{srcName}</font><font color=\"#cccccc\">.</font>"
                : "<font color=\"#cccccc\">.</font>";
            AddChatMessage(new ChatMessage(ChatChannel.Damage, "System",
                $"<font color=\"#cccccc\">Received </font> <font>{-diff}</font> <font color=\"#cccccc\"> (</font><font>{-diff / (double)maxHP:P}</font><font color=\"#cccccc\">)</font> <font color=\"#cccccc\"> damage</font>{srcName}"));
        }
        public void AddFromQueue(int itemsToAdd)
        {
            if (itemsToAdd == 0) itemsToAdd = _pauseQueue.Count;
            for (var i = 0; i < itemsToAdd; i++)
            {
                if (_pauseQueue.TryDequeue(out var msg))
                {
                    ChatMessages.Insert(0, msg);
                    if (ChatMessages.Count > SettingsHolder.MaxMessages)
                    {
                        ChatMessages.RemoveAt(ChatMessages.Count - 1);
                    }
                }
            }
        }

        private static bool Pass(ChatMessage current, ChatMessage old)
        {
            if (current.Author == "System") return true;
            if (current.Author == SessionManager.CurrentPlayer.Name) return true;
            if (old.RawMessage != current.RawMessage) return true;

            if (old.Author != current.Author) return true;
            switch (current.Channel)
            {
                case ChatChannel.Exp:
                case ChatChannel.Group:
                case ChatChannel.Party:
                case ChatChannel.PartyNotice:
                case ChatChannel.Raid:
                case ChatChannel.RaidLeader:
                case ChatChannel.RaidNotice:
                case ChatChannel.GroupAlerts:
                case ChatChannel.Money:
                case ChatChannel.Loot:
                case ChatChannel.Bargain:
                case ChatChannel.Damage:
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
            SettingsHolder.ChatWindowsSettings.Remove((e.OldItems[0] as ChatWindow)?.WindowSettings as ChatWindowSettings);
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

        public void AddOrRefreshLfg(ListingData x)
        {
            if (TryGetLfg(x.LeaderId, x.Message, x.LeaderName, out var lfg))
            {
                lfg.Message = x.Message;
                lfg.Refresh();
            }
            else
            {
                LFGs.Add(new LFG(x.LeaderId, x.LeaderName, x.Message, x.IsRaid));
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
            lfg = LFGs.ToSyncList().FirstOrDefault(x => x.Id == id);
            if (lfg != null) return true;
            lfg = LFGs.ToSyncList().FirstOrDefault(x => x.Name == name);
            if (lfg != null) return false;
            lfg = LFGs.ToSyncList().FirstOrDefault(x => x.Message == msg);
            return lfg != null;
        }
        public void UpdateLfgMembers(uint id, int count)
        {
            if (TryGetLfg(id, "", "", out var lfg))
            {
                lfg.MembersCount = count;
            }
        }

        public void ForceHideTimerRefresh()
        {
            foreach (var chatWindow in ChatWindows)
            {
                chatWindow.VM.RefreshHideTimer();
            }
        }

        public void ScrollToMessage(Tab tab, ChatMessage msg)
        {
            var win = ChatWindows.FirstOrDefault(x => x.VM.Tabs.Contains(tab));
            if (win == null) return;

            win.ScrollToMessage(tab, msg);
        }

        public void ToggleForcedClickThru()
        {
            SettingsHolder.ChatWindowsSettings.ToSyncList().ForEach(s => { s.ForceToggleClickThru(); });
            if (SettingsHolder.ChatWindowsSettings.Count == 0) return;
            var msg = $"Forcing chat clickable turned {(SettingsHolder.ChatWindowsSettings[0].ForcedClickable ? "on" : "off")}";
            WindowManager.FloatingButton.NotifyExtended("TCC", msg, NotificationType.Normal, 2000);
            AddTccMessage(msg);
        }
    }
}