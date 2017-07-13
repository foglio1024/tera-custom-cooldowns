using System;
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
        static ChatWindowViewModel _instance;
        public static ChatWindowViewModel Instance => _instance ?? (_instance = new ChatWindowViewModel());
        const int MESSAGE_CAP = 100000;
        const int SPAM_THRESHOLD = 1;
        DispatcherTimer hideTimer;
        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
        private bool isChatVisible;
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
        public ICollectionView AllMessages
        {
            get => _allMessages;
        }
        private ICollectionView _guildMessages;
        public ICollectionView GuildMessages
        {
            get => _guildMessages;
        }
        private ICollectionView _groupMessages;
        public ICollectionView GroupMessages
        {
            get => _groupMessages;
        }
        private ICollectionView _systemMessages;
        public ICollectionView SystemMessages
        {
            get => _systemMessages;
        }
        private ICollectionView _whisperMessages;
        public ICollectionView WhisperMessages
        {
            get => _whisperMessages;
        }


        public List<ChatChannel> VisibleChannels;
        public List<string> BlockedUsers;

        public LFG LastClickedLfg;

        internal void RemoveDeadLfg()
        {
            if(LastClickedLfg != null)
            {
                RemoveLfg(LastClickedLfg);
                LastClickedLfg = null;
            }
        }

        public List<string> Friends;
        private SynchronizedObservableCollection<LFG> _lfgs;
        public SynchronizedObservableCollection<LFG> LFGs
        {
            get => _lfgs;
            set
            {
                if (_lfgs == value) return;
                _lfgs = value;
            }
        }
        public PrivateChatChannel[] PrivateChannels = new PrivateChatChannel[8];

        public ChatWindowViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _chatMessages = new SynchronizedObservableCollection<ChatMessage>(_dispatcher);
            _lfgs = new SynchronizedObservableCollection<LFG>(_dispatcher);
            hideTimer = new DispatcherTimer();
            hideTimer.Interval = TimeSpan.FromSeconds(15);
            hideTimer.Tick += HideTimer_Tick;
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
                ChatChannel.RaidLeader,
                ChatChannel.PartyNotice,
                ChatChannel.Say
            };
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
            if (BlockedUsers.Contains(chatMessage.Author)) return;
            if (ChatMessages.Count < SettingsManager.SpamThreshold)
            {
                for (int i = 0; i < ChatMessages.Count - 1; i++)
                {
                    var m = ChatMessages[i];
                    if (m.RawMessage == chatMessage.RawMessage &&
                        m.Channel == chatMessage.Channel &&
                        m.Author == chatMessage.Author &&
                        !VisibleChannels.Contains(chatMessage.Channel)) return;
                }
            }
            else
            {

                int offset = 0;
                for (int i = 0; i < SettingsManager.SpamThreshold; i++)
                {
                    if (1 + i + offset > ChatMessages.Count) continue;

                    var m = ChatMessages[ChatMessages.Count - 1 - i - offset];
                    //if (!VisibleChannels.Contains(m.Channel))
                    //{
                    //    i--;
                    //    offset++;
                    //    continue;
                    //}
                    if (m.RawMessage == chatMessage.RawMessage && 
                        m.Author == chatMessage.Author && 
                        m.Channel != ChatChannel.Money &&
                        m.Channel != ChatChannel.Loot) return;
                }
            }

            ChatMessage.SplitSimplePieces(chatMessage);
            ChatMessages.Add(chatMessage);

            if (ChatMessages.Count > SettingsManager.MaxMessages)
            {
                ChatMessages.RemoveAt(0);
            }
            NotifyPropertyChanged(nameof(MessageCount));
        }

        internal void AddOrRefreshLfg(S_PARTY_MATCH_LINK x)
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
        internal void RemoveLfg(LFG lfg)
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
                    if(lfg == null)
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

        internal void UpdateLfgMembers(S_PARTY_MEMBER_INFO p)
        {
            LFG lfg;
            if(TryGetLfg(p.Id, "", "", out lfg))
            {
                lfg.MembersCount = p.MembersCount;
            }
        }

        internal void RefreshHideTimer()
        {
            hideTimer.Stop();
            hideTimer.Start();
        }
        internal void StopHideTimer()
        {
            hideTimer.Stop();
            IsChatVisible = true;

        }
    }
}
