using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;
using TCC.Data;
using TCC.Parsing;
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
        private ICollectionView _allMessages;
        private ICollectionView _guildMessages;
        private ICollectionView _groupMessages;
        private ICollectionView _systemMessages;
        private ICollectionView _whisperMessages;
        public List<string> Friends;
        public List<string> BlockedUsers;
        public LFG LastClickedLfg;
        private SynchronizedObservableCollection<LFG> _lfgs;
        public PrivateChatChannel[] PrivateChannels = new PrivateChatChannel[8];
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
        public ICollectionView WhisperMessages
        {
            get => _whisperMessages;
        }
        public List<ChatChannelOnOff> VisibleChannels { get => SettingsManager.EnabledChatChannels; }

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


            //get bool from settings
            //
            //
            BlockedUsers = new List<string>();
            Friends = new List<string>();
            TooltipInfo = new TooltipInfo("", "", 1);

            _allMessages = new CollectionViewSource { Source = _chatMessages }.View;
            _guildMessages = new CollectionViewSource { Source = _chatMessages }.View;
            _groupMessages = new CollectionViewSource { Source = _chatMessages }.View;
            _systemMessages = new CollectionViewSource { Source = _chatMessages }.View;
            _whisperMessages = new CollectionViewSource { Source = _chatMessages }.View;

            _allMessages.Filter = null;
            _guildMessages.Filter = p => ((ChatMessage)p).Channel == ChatChannel.Guild ||
                                        ((ChatMessage)p).Channel == ChatChannel.GuildNotice;
            _groupMessages.Filter = p => ((ChatMessage)p).Channel == ChatChannel.Party ||
                                        ((ChatMessage)p).Channel == ChatChannel.PartyNotice ||
                                        ((ChatMessage)p).Channel == ChatChannel.Raid ||
                                        ((ChatMessage)p).Channel == ChatChannel.RaidLeader ||
                                        ((ChatMessage)p).Channel == ChatChannel.Group ||
                                        ((ChatMessage)p).Channel == ChatChannel.GroupAlerts ||
                                        ((ChatMessage)p).Channel == ChatChannel.RaidNotice;
            _systemMessages.Filter = p => ((ChatMessage)p).Author == "System" ||
                                           ((ChatMessage)p).Channel == ChatChannel.TCC;
            _whisperMessages.Filter = p => ((ChatMessage)p).Channel == ChatChannel.ReceivedWhisper ||
                                            ((ChatMessage)p).Channel == ChatChannel.SentWhisper;
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
                        m.Channel != ChatChannel.Loot) return;
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
    }
}
