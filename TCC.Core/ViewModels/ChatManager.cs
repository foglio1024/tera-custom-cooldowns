//#define BATCH // pepehands

using Nostrum;
using Nostrum.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Analysis;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Processing;
using TCC.R;
using TCC.Settings.WindowSettings;
using TCC.UI;
using TCC.UI.Windows.Widgets;
using TCC.Utilities;
using TCC.Utils;
using TCC.ViewModels.Widgets;
using TeraDataLite;
using TeraPacketParser.Messages;
using TeraPacketParser.TeraCommon.Game;

namespace TCC.ViewModels
{
    [TccModule]
    public class ChatManager : TccWindowViewModel
    {
        private static ChatManager? _instance;

        public static ChatManager Instance => _instance ??= new ChatManager(App.Settings.ChatSettings);

        public ChatMessageFactory Factory { get; }

        private readonly ConcurrentQueue<ChatMessage> _pauseQueue;
        private readonly List<TempPrivateMessage> _privateMessagesCache;
        public readonly PrivateChatChannel[] PrivateChannels = new PrivateChatChannel[8];
        private readonly object _lock = new();

        public event Action<ChatMessage>? NewMessage;
        public event Action<int>? PrivateChannelJoined;

        public LFG? LastClickedLfg { get; set; }

        public int MessageCount => ChatMessages.Count;
        public int QueuedMessagesCount => _pauseQueue.Count;
        public bool IsQueueEmpty => QueuedMessagesCount == 0;
        public TSObservableCollection<ChatWindow> ChatWindows { get; }
#if BATCH
        private readonly ConcurrentQueue<ChatMessage> _mainQueue;
        private readonly DispatcherTimer _flushTimer;
        public TSObservableCollectionBatch<ChatMessage> ChatMessages { get; private set; }
#else
        public TSObservableCollection<ChatMessage> ChatMessages { get; }
#endif
        public TSObservableCollection<LFG> LFGs { get; }

        private ChatManager(WindowSettingsBase settings) : base(settings)
        {
            _pauseQueue = new ConcurrentQueue<ChatMessage>();
            _privateMessagesCache = new List<TempPrivateMessage>();
            ChatWindows = new TSObservableCollection<ChatWindow>(Dispatcher);
#if BATCH
            _mainQueue = new ConcurrentQueue<ChatMessage>();
            _flushTimer =
 new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Background, OnFlushTick, Dispatcher);
            ChatMessages = new TSObservableCollectionBatch<ChatMessage>(Dispatcher);
            _flushTimer.Start();
#else
            ChatMessages = new TSObservableCollection<ChatMessage>(Dispatcher);
#endif
            LFGs = new TSObservableCollection<LFG>(Dispatcher);

            ChatMessages.CollectionChanged += OnChatMessagesCollectionChanged;
            BindingOperations.EnableCollectionSynchronization(ChatMessages, _lock);

            ChatWindows.CollectionChanged += OnChatWindowsCollectionChanged;
            PrivateChannelJoined += OnPrivateChannelJoined;

            KeyboardHook.Instance.RegisterCallback(App.Settings.ForceClickableChatHotkey, ToggleForcedClickThru);

            Factory = new ChatMessageFactory(Dispatcher);

            Log.NewChatMessage += OnLogChatMessage;
        }

        private void OnLogChatMessage(ChatChannel ch, string msg, string auth)
        {
            AddChatMessage(Factory.CreateMessage(ch, auth, msg));
        }
#if BATCH
        private void OnFlushTick(object sender, EventArgs e)
        {
            var list = new List<ChatMessage>();
            while (_mainQueue.TryDequeue(out var m))
            {
                list.Add(m);
            }

            ChatMessages.AddBatch(list);
            if (ChatMessages.Count > App.Settings.MaxMessages)
            {
                var toRemove = new List<ChatMessage>();
                for (int i = 0; i < list.Count; i++)
                {
                    var target = ChatMessages[ChatMessages.Count - 1];
                    target.Dispose();
                    toRemove.Add(target);
                }
                ChatMessages.RemoveBatch(toRemove);
            }
            N(nameof(MessageCount));

        }
#endif

        protected override void InstallHooks()
        {
            PacketAnalyzer.Sniffer.NewConnection += OnConnected;
            PacketAnalyzer.Sniffer.EndConnection += OnDisconnected;

            PacketAnalyzer.Processor.Hook<S_CHAT>(OnChat);
            PacketAnalyzer.Processor.Hook<S_PRIVATE_CHAT>(OnPrivateChat);
            PacketAnalyzer.Processor.Hook<S_WHISPER>(OnWhisper);
            PacketAnalyzer.Processor.Hook<S_JOIN_PRIVATE_CHANNEL>(OnJoinPrivateChannel);
            PacketAnalyzer.Processor.Hook<S_LEAVE_PRIVATE_CHANNEL>(OnLeavePrivateChannel);
            PacketAnalyzer.Processor.Hook<S_TRADE_BROKER_DEAL_SUGGESTED>(OnTradeBrokerDealSuggested);
            PacketAnalyzer.Processor.Hook<S_OTHER_USER_APPLY_PARTY>(OnOtherUserApplyParty);
            PacketAnalyzer.Processor.Hook<S_PARTY_MATCH_LINK>(OnPartyMatchLink);
            PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_INFO>(OnPartyMemberInfo);
            PacketAnalyzer.Processor.Hook<S_CREATURE_CHANGE_HP>(OnCreatureChangeHp);
            PacketAnalyzer.Processor.Hook<S_PLAYER_CHANGE_EXP>(OnPlayerChangeExp);
        }

        protected override void RemoveHooks()
        {
            PacketAnalyzer.Processor.Unhook<S_CHAT>(OnChat);
            PacketAnalyzer.Processor.Unhook<S_PRIVATE_CHAT>(OnPrivateChat);
            PacketAnalyzer.Processor.Unhook<S_WHISPER>(OnWhisper);
            PacketAnalyzer.Processor.Unhook<S_JOIN_PRIVATE_CHANNEL>(OnJoinPrivateChannel);
            PacketAnalyzer.Processor.Unhook<S_LEAVE_PRIVATE_CHANNEL>(OnLeavePrivateChannel);
            PacketAnalyzer.Processor.Unhook<S_TRADE_BROKER_DEAL_SUGGESTED>(OnTradeBrokerDealSuggested);
            PacketAnalyzer.Processor.Unhook<S_OTHER_USER_APPLY_PARTY>(OnOtherUserApplyParty);
            PacketAnalyzer.Processor.Unhook<S_PARTY_MATCH_LINK>(OnPartyMatchLink);
            PacketAnalyzer.Processor.Unhook<S_PARTY_MEMBER_INFO>(OnPartyMemberInfo);
            PacketAnalyzer.Processor.Unhook<S_CREATURE_CHANGE_HP>(OnCreatureChangeHp);
            PacketAnalyzer.Processor.Unhook<S_PLAYER_CHANGE_EXP>(OnPlayerChangeExp);
        }

        private void OnConnected(Server srv)
        {
            AddTccMessage($"Connected to {srv.Name}.");
        }

        private void OnDisconnected()
        {
            AddTccMessage("Disconnected from the server.");
        }

        private void OnPartyMemberInfo(S_PARTY_MEMBER_INFO m)
        {
            UpdateLfgMembers(m.Id, m.Members.Count);
        }

        private void OnPartyMatchLink(S_PARTY_MATCH_LINK m)
        {
            Task.Run(() =>
            {
                if (m.Message.IndexOf("WTB", 0, StringComparison.InvariantCultureIgnoreCase) != -1) return;
                if (m.Message.IndexOf("WTS", 0, StringComparison.InvariantCultureIgnoreCase) != -1) return;
                if (m.Message.IndexOf("WTT", 0, StringComparison.InvariantCultureIgnoreCase) != -1) return;
                AddOrRefreshLfg(m.ListingData);
                AddLfgMessage(m.Id, m.Name, m.Message);
            });
        }

        private void OnOtherUserApplyParty(S_OTHER_USER_APPLY_PARTY m)
        {
            AddChatMessage(new ApplyMessage(m.PlayerId, m.Class, m.Level, m.Name));
        }

        private void OnTradeBrokerDealSuggested(S_TRADE_BROKER_DEAL_SUGGESTED m)
        {
            AddChatMessage(new BrokerChatMessage(m.PlayerId, m.Listing, m.Item, m.Amount, m.SellerPrice, m.OfferedPrice, m.Name));
        }

        private void OnWhisper(S_WHISPER m)
        {
            var isMe = m.Author == Game.Me.Name;
            var author = isMe ? m.Recipient : m.Author;
            var channel = isMe ? ChatChannel.SentWhisper : ChatChannel.ReceivedWhisper;
            AddChatMessage(Factory.CreateMessage(channel, author, m.Message, m.IsGm, m.GameId));
        }

        private void OnLeavePrivateChannel(S_LEAVE_PRIVATE_CHANNEL m)
        {
            var i = PrivateChannels.FirstOrDefault(c => c.Id == m.Id).Index;
            PrivateChannels[i].Joined = false;
        }

        private void OnJoinPrivateChannel(S_JOIN_PRIVATE_CHANNEL m)
        {
            JoinPrivateChannel(m.Id, m.Index, m.Name);
        }

        private void OnPrivateChat(S_PRIVATE_CHAT m)
        {
            // ignore these since they're handled differently
            //if (m.Message.Contains(":tcc-chatMode:") || m.Message.Contains(":tcc-uiMode:")) return;
            var i = PrivateChannels.FirstOrDefault(y => y.Id == m.Channel).Index;
            var ch = (ChatChannel) (PrivateChannels[i].Index + 11);
            if (ch == ChatChannel.Private8) return; // already sent by stub

            AddChatMessage(Factory.CreateMessage(ch, m.AuthorName, m.Message, false, m.AuthorId));
        }

        private void OnChat(S_CHAT m)
        {
            Task.Run(() =>
            {
                AddChatMessage(Factory.CreateMessage(m.Channel == 212 ? (ChatChannel)26 : (ChatChannel)m.Channel, m.AuthorName, m.Message, m.IsGm, m.AuthorId));
            });
        }

        private void OnPlayerChangeExp(S_PLAYER_CHANGE_EXP m)
        {
            if (Game.Me.Level == 70) return;

            var msg = ChatUtils.Font("You gained ")
                    + ChatUtils.Font($"{m.GainedTotalExp - m.GainedRestedExp:N0}", Colors.GoldColor.ToHex());

            if (m.GainedRestedExp > 0)
                msg += ChatUtils.Font(" + ") +
                       ChatUtils.Font($"{m.GainedRestedExp:N0}", Colors.ChatMegaphoneColor.ToHex());

            msg += ChatUtils.Font($" (");
            msg += ChatUtils.Font($"{m.GainedTotalExp / (double)m.NextLevelExp:P3}", Colors.GoldColor.ToHex());
            msg += ChatUtils.Font($") XP. Total: ");
            msg += ChatUtils.Font($"{m.LevelExp / (double)m.NextLevelExp:P3}", Colors.GoldColor.ToHex());
            msg += ChatUtils.Font($".");

            AddChatMessage(Factory.CreateMessage(ChatChannel.Exp, "System", msg));
        }

        private void OnCreatureChangeHp(S_CREATURE_CHANGE_HP m)
        {
            Task.Run(() => AddDamageReceivedMessage(m.Source, m.Target, m.Diff, m.MaxHP, m.Crit));
        }

        private void InitWindows()
        {
            ChatWindows.Clear();
            App.Settings.ChatWindowsSettings.ToList().ForEach(s =>
            {
                if (s.Tabs.Count == 0) return;
                var m = new ChatViewModel(s);
                var w = new ChatWindow(m);
                ChatWindows.Add(w);
                m.LoadTabs(s.Tabs);
            });

            if (ChatWindows.Count != 0) return;
            {
                Log.CW("No chat windows found, initializing default one.");
                var ws = new ChatWindowSettings(0, 1, 200, 500, true, ClickThruMode.Never, 1, false, 1, false, true,
                    false)
                {HideTimeout = 10, FadeOut = true, LfgOn = false};
                var m = new ChatViewModel(ws);
                var w = new ChatWindow(m);
                App.BaseDispatcher.InvokeAsync(() =>
                {
                    if (w.WindowSettings == null) return;
                    App.Settings.ChatWindowsSettings.Add((ChatWindowSettings) w.WindowSettings);
                });
                ChatWindows.Add(w);
                m.LoadTabs();
                if (App.Settings.ChatEnabled) w.Show();
            }
        }

        public void CloseAllWindows()
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var w in ChatWindows) w.CloseWindowSafe();
            });
        }

        public void SetPaused(bool v)
        {
            ChatWindows.ToList().ForEach(w => w.VM.Paused = v);
        }

        public void SetPaused(bool v, ChatMessage dc)
        {
            ChatWindows.ToList().ForEach(w =>
            {
                if (w.VM.CurrentTab?.Messages == null) return;
                if (w.VM.CurrentTab.Messages.Contains(dc)) w.VM.Paused = v;
            });
        }

        public void ScrollToBottom()
        {
            //TODO?
            foreach (var win in ChatWindows)
            {
                win.ScrollToBottom();
            }

            AddFromQueue();

            SetPaused(false);

        }

        private bool Filtered(ChatMessage message)
        {
            if (!App.Settings.ChatEnabled)
            {
                message.Dispose();
                return true;
            }

            if (Game.BlockList.Contains(message.Author) && !(message is LfgMessage))
            {
                message.Dispose();
                return true;
            }

            var pausedCount = _pauseQueue.Count;
            for (var i = 0; i < App.Settings.SpamThreshold; i++)
            {
                if (i >= pausedCount + ChatMessages.Count) continue;
                if (Pass(message, i <= pausedCount - 1
                    ? _pauseQueue.ElementAt(i)
                    : ChatMessages[i - pausedCount])) continue;
                message.Dispose();
                return true;
            }

            //if (ChatMessages.Count < App.Settings.SpamThreshold)
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
            //    for (var i = 0; i < App.Settings.SpamThreshold; i++)
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

        public void AddSystemMessage(string template, SystemMessageData sysMsg, string authorOverride = "System")
        {
            if (!App.Settings.ChatEnabled) return;
            Task.Run(() => AddChatMessage(Factory.CreateSystemMessage(template, sysMsg, (ChatChannel) sysMsg.ChatChannel, authorOverride)));
        }

        public void AddSystemMessage(string template, SystemMessageData sysMsg, ChatChannel channelOverride, string authorOverride = "System")
        {
            if (!App.Settings.ChatEnabled) return;
            Task.Run(() => AddChatMessage(Factory.CreateSystemMessage(template, sysMsg, channelOverride, authorOverride)));
        }

        private void AddLfgMessage(uint id, string name, string msg)
        {
            if (!App.Settings.ChatEnabled) return;
            Task.Run(() => AddChatMessage(Factory.CreateLfgMessage(id, name, msg)));
        }

        public void AddChatMessage(ChatMessage chatMessage)
        {
            if (!App.Settings.ChatEnabled) return;

            Dispatcher.InvokeAsync(() =>
            {
                if (Filtered(chatMessage)) return;

                if (chatMessage is LfgMessage lm && !App.Settings.DisableLfgChatMessages) lm.LinkListing();

                chatMessage.SplitSimplePieces();

                if (ChatWindows.All(x => !x.IsPaused))
                {
#if BATCH
                    _mainQueue.Enqueue(chatMessage);
#else
                    ChatMessages.Insert(0, chatMessage);
#endif
                }
                else
                {
                    _pauseQueue.Enqueue(chatMessage);
                    N(nameof(QueuedMessagesCount));

                }

                NewMessage?.Invoke(chatMessage);
#if BATCH
#else
                if (ChatMessages.Count > App.Settings.MaxMessages)
                {
                    var toRemove = ChatMessages[ChatMessages.Count - 1];
                    toRemove.Dispose();
                    ChatMessages.RemoveAt(ChatMessages.Count - 1);
                }

                N(nameof(MessageCount));
#endif
            }, DispatcherPriority.DataBind);
        }

        public void AddTccMessage(string message)
        {
            if (!App.Settings.ChatEnabled) return;

            var msg = Factory.CreateMessage(ChatChannel.TCC, "System", ChatUtils.Font(message));
            AddChatMessage(msg);
        }

        private void AddDamageReceivedMessage(ulong source, ulong target, long diff, long maxHP, bool crit)
        {
            if (!App.Settings.ChatEnabled) return;

            if (!Game.IsMe(target) || diff > 0 || target == source || source == 0 || !TccUtils.IsEntitySpawned(source)) return;
            var srcName = TccUtils.GetEntityName(source);
            var parameters = $"@0\vAmount\v{-diff}\vPerc\v{-diff / (double) maxHP:P}{(srcName != "" ? $"\vSource\v{srcName}" : "")}";

            SystemMessagesProcessor.AnalyzeMessage(parameters, srcName == ""
                ? !crit ? "TCC_DAMAGE_RECEIVED_UNKNOWN" : "TCC_DAMAGE_RECEIVED_UNKNOWN_CRIT"
                : !crit ? "TCC_DAMAGE_RECEIVED" : "TCC_DAMAGE_RECEIVED_CRIT");
        }

        public void AddFromQueue(int itemsToAdd = 0)
        {
            if (itemsToAdd == 0) itemsToAdd = _pauseQueue.Count;
            for (var i = 0; i < itemsToAdd; i++)
                if (_pauseQueue.TryDequeue(out var msg))
                {
                    N(nameof(QueuedMessagesCount));
#if BATCH
                    _mainQueue.Enqueue(msg);
#else
                    ChatMessages.Insert(0, msg);
#endif
                    if (ChatMessages.Count > App.Settings.MaxMessages) 
                        ChatMessages.RemoveAt(ChatMessages.Count - 1);
                }
        }

        public void ClearMessages()
        {
            Task.Factory.StartNew(() =>
            {
                var list = ChatMessages.ToSyncList();
                for (var i = list.Count - 1; i >= 0; i--)
                {
                    var idx = i;
                    Dispatcher.InvokeAsync(() =>
                    {
                        ChatMessages.RemoveAt(idx);
                        list[idx].Dispose();
                    });
                    Thread.Sleep(1);
                }
            });
            //ChatMessages.Clear();
        }

        private static bool Pass(ChatMessage current, ChatMessage old)
        {
            if (current.Author == "System") return true;
            if (current.Author == Game.Me.Name) return true;
            if (current.RawMessage != old.RawMessage) return true;

            if (old.Author != current.Author) return true;
            return current.Channel switch
            {
                ChatChannel.Exp => true,
                ChatChannel.Group => true,
                ChatChannel.Party => true,
                ChatChannel.PartyNotice => true,
                ChatChannel.Raid => true,
                ChatChannel.RaidLeader => true,
                ChatChannel.RaidNotice => true,
                ChatChannel.GroupAlerts => true,
                ChatChannel.Money => true,
                ChatChannel.Loot => true,
                ChatChannel.Bargain => true,
                ChatChannel.Damage => true,
                ChatChannel.Private7 => true,
                ChatChannel.Private8 => true,
                ChatChannel.TCC => true,
                ChatChannel.Guardian => true,
                ChatChannel.Apply => true,
                ChatChannel.Death => true,
                ChatChannel.Ress => true,
                ChatChannel.WorldBoss => true,
                ChatChannel.Enchant => true,
                ChatChannel.Friend => true,
                ChatChannel.Laurel => true,
                ChatChannel.GuildNotice => true,
                ChatChannel.SentWhisper => true,
                ChatChannel.ReceivedWhisper when old.Channel == ChatChannel.SentWhisper => true,
                _ => false
            };
        }

        private void JoinPrivateChannel(uint id, int index, string name)
        {
            Instance.PrivateChannels[index] = new PrivateChatChannel(id, name, index);
            PrivateChannelJoined?.Invoke(index);
        }

        private void OnPrivateChannelJoined(int index)
        {
            var messagesToAdd = _privateMessagesCache.Where(x => x.Channel == PrivateChannels[index].Id).ToList();
            messagesToAdd.ForEach(x =>
            {
                AddChatMessage(Factory.CreateMessage(
                    (ChatChannel)index + 11,
                    x.Author == "undefined" ? "System" : x.Author,
                    x.Message));
            });
            messagesToAdd.ForEach(x => _privateMessagesCache.Remove(x));
        }

        public void CachePrivateMessage(uint channel, string author, string message)
        {
            _privateMessagesCache.Add(new TempPrivateMessage { Channel = channel, Author = author, Message = message });
        }

        private static void OnChatWindowsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Remove) return;
            if (e.OldItems?.Count == 0) return;
            var ws = (ChatWindowSettings?) ((ChatWindow?) e.OldItems?[0])?.WindowSettings;
            if(ws == null) return;
            App.Settings.ChatWindowsSettings.Remove(ws);
        }

        private void OnChatMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;
            //TODO
            foreach (var chatWindow in ChatWindows)
            {
                if (e.NewItems != null) chatWindow.VM.CheckVisibility(e.NewItems);
            }
        }

        private void AddOrRefreshLfg(ListingData x)
        {
            if (TryGetLfg(x.LeaderId, x.Message, x.LeaderName, out var lfg))
            {
                lfg!.Message = x.Message;
                lfg!.Refresh();
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
            if (LastClickedLfg == null) return;
            RemoveLfg(LastClickedLfg);
            LastClickedLfg = null;
        }

        private bool TryGetLfg(uint id, string msg, string name, out LFG? lfg)
        {
            lfg = LFGs.ToSyncList().FirstOrDefault(x => x.Id == id);
            if (lfg != null) return true;
            lfg = LFGs.ToSyncList().FirstOrDefault(x => x.Name == name);
            if (lfg != null) return false;
            lfg = LFGs.ToSyncList().FirstOrDefault(x => x.Message == msg);
            return lfg != null;
        }

        private void UpdateLfgMembers(uint id, int count)
        {
            if (!TryGetLfg(id, "", "", out var lfg)) return;
            lfg!.MembersCount = count;
        }

        public void ForceHideTimerRefresh()
        {
            foreach (var chatWindow in ChatWindows) chatWindow.VM.RefreshHideTimer();
        }

        public void ScrollToMessage(Tab tab, ChatMessage msg)
        {
            var win = ChatWindows.FirstOrDefault(x => x.VM.Tabs.Contains(tab));
            if (win == null) return;

            win.ScrollToMessage(tab, msg);
        }

        private void ToggleForcedClickThru()
        {
            App.Settings.ChatWindowsSettings.ToSyncList().ForEach(s => { s.ForcedClickable = !s.ForcedClickable; });
            if (App.Settings.ChatWindowsSettings.Count == 0) return;
            var msg =
                $"Forcing chat clickable turned {(App.Settings.ChatWindowsSettings[0].ForcedClickable ? "on" : "off")}";
            Log.N("TCC", msg, NotificationType.None, 2000);
            AddTccMessage(msg);
        }

        public void NotifyEnabledChanged(bool value)
        {
            OnEnabledChanged(value);
        }

        public void RemoveEmptyChatWindows()
        {
            App.BaseDispatcher.InvokeAsync(() =>
            {
                foreach (var window in Application.Current.Windows.ToList()
                    .Where(x => x is ChatWindow c && c.VM.TabVMs.Count == 0))
                {
                    var w = (ChatWindow) window;
                    ChatWindows.Remove(w);
                    w.Close();
                }

                if (FocusManager.ForceFocused) FocusManager.ForceFocused = false;
            }, DispatcherPriority.Background);
        }

        public static void Start()
        {
            var t = new Thread(() =>
            {
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                Instance.InitWindows();
                App.AddDispatcher(Thread.CurrentThread.ManagedThreadId, Dispatcher.CurrentDispatcher);
                Dispatcher.Run();
                Log.CW("[ChatWindow] Dispatcher stopped.");
                App.RemoveDispatcher(Thread.CurrentThread.ManagedThreadId);
            })
            {
                Name = "ChatThread"
            };
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
    }
}