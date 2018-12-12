using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using Dragablz;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Settings;

namespace TCC.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        private SynchronizedObservableCollection<HeaderedItemViewModel> _tabVMs;
        private bool _paused;
        //private bool _lfgOn;
        //private double _backgroundOpacity = 0.3;
        private DispatcherTimer _hideTimer;
        private ChatWindowSettings _windowSettings;

        public event Action<bool> VisibilityChanged;


        public bool Paused
        {
            get => _paused;
            set
            {
                if (_paused == value) return;
                _paused = value;
                NPC();
            }
        }
        //public bool IsTeraOnTop
        //{
        //    get => WindowManager.IsTccVisible;
        //}
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
        //public double ChatWindowOpacity => Settings.Settings.ChatWindowOpacity;
        public Func<HeaderedItemViewModel> AddNewTabCommand
        {
            get
            {
                return
                    () =>
                    {
                        var t = new HeaderedItemViewModel();
                        var content = new Tab("NEW TAB", new ChatChannel[] { }, new ChatChannel[] { }, new string[] { },
                            new string[] { });
                        content.PropertyChanged += (_, __) =>
                        {
                            if (__.PropertyName == nameof(Tab.TabName)) t.Header = content.TabName;
                        };
                        t.Content = content;
                        return t;
                    };
            }
        }

        //public bool LfgOn
        //{
        //    get => _lfgOn; set
        //    {
        //        if (_lfgOn == value) return;
        //        _lfgOn = value;
        //        NPC();
        //    }
        //}
        //public double BackgroundOpacity
        //{
        //    get => _backgroundOpacity; set
        //    {
        //        if (_backgroundOpacity == value) return;
        //        _backgroundOpacity = value;
        //        NPC();
        //    }
        //}

        public ChatWindowSettings WindowSettings
        {
            get => _windowSettings;
            set
            {
                if (_windowSettings == value) return;
                if (_windowSettings != null)
                {
                    _windowSettings.TimeoutChanged -= ChangeTimerInterval;
                }
                _windowSettings = value;
                if (_windowSettings != null)
                {
                    _hideTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(WindowSettings.HideTimeout) };
                    _hideTimer.Tick += OnHideTimerTick;
                    _windowSettings.TimeoutChanged += ChangeTimerInterval;
                }
            }
        }

        private void ChangeTimerInterval()
        {
            _hideTimer.Interval = TimeSpan.FromSeconds(WindowSettings.HideTimeout);
            _hideTimer.Refresh();
        }

        public void RefreshHideTimer()
        {
            _hideTimer.Refresh();
        }
        public void StopHideTimer()
        {
            _hideTimer.Stop();
            VisibilityChanged?.Invoke(true);// IsChatVisible = true;
        }

        //public void NotifyOpacityChange()
        //{
        //    NPC(nameof(ChatWindowOpacity));
        //}
        public ChatViewModel()
        {
            TabVMs = new SynchronizedObservableCollection<HeaderedItemViewModel>();

            ChatWindowManager.Instance.NewMessage += CheckAttention;
            TabVMs.CollectionChanged += TabVMs_CollectionChanged;

        }
        private void OnHideTimerTick(object sender, EventArgs e)
        {
            VisibilityChanged?.Invoke(false); //IsChatVisible = false;
            _hideTimer.Stop();
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
                foreach (var chatTabsSetting in tabs)
                {
                    TabVMs.Add(new HeaderedItemViewModel(chatTabsSetting.TabName, chatTabsSetting));
                }
            }
            if (TabVMs.Count != 0) return;
            var all = new Tab("ALL", new ChatChannel[] { }, new ChatChannel[] { }, new string[] { }, new[] { "System" });
            var guild = new Tab("GUILD", new[] { ChatChannel.Guild, ChatChannel.GuildNotice, }, new ChatChannel[] { }, new string[] { }, new string[] { });
            var group = new Tab("GROUP", new[]{ChatChannel.Party, ChatChannel.PartyNotice,
                ChatChannel.RaidLeader, ChatChannel.RaidNotice,
                ChatChannel.Raid, ChatChannel.Ress,ChatChannel.Death,
                ChatChannel.Group, ChatChannel.GroupAlerts  }, new ChatChannel[] { }, new string[] { }, new string[] { });
            var w = new Tab("WHISPERS", new[] { ChatChannel.ReceivedWhisper, ChatChannel.SentWhisper, }, new ChatChannel[] { }, new string[] { }, new string[] { });
            var sys = new Tab("SYSTEM", new ChatChannel[] { }, new ChatChannel[] { }, new[] { "System" }, new string[] { });
            TabVMs.Add(new HeaderedItemViewModel(all.TabName, all));
            TabVMs.Add(new HeaderedItemViewModel(guild.TabName, guild));
            TabVMs.Add(new HeaderedItemViewModel(group.TabName, group));
            TabVMs.Add(new HeaderedItemViewModel(w.TabName, w));
            TabVMs.Add(new HeaderedItemViewModel(sys.TabName, sys));
            CurrentTab = TabVMs[0].Content as Tab;
            //ChatWindowManager.Instance.FindMyWindow(this).UpdateSettings();

        }

        private void CheckAttention(ChatMessage chatMessage)
        {
            //chatMessage.Animate = false; //set animate to false if the message is not going in the active tab
            if (chatMessage.ContainsPlayerName || chatMessage.Channel == ChatChannel.ReceivedWhisper)
            {
                //var tabs = TabVMs.Where(x => ((Tab)x.Content).Channels.Contains(chatMessage.Channel)).ToList();
                //tabs.ForEach(tab =>
                //{
                //    ((Tab) tab.Content).Attention = true;
                //});
                //if (tabs.Count != 0) return;

                //tabs = TabVMs.Where(x => !((Tab)x.Content).ExcludedChannels.Contains(chatMessage.Channel)).ToList();
                //tabs.ForEach(tab =>
                //{
                //    ((Tab)tab.Content).Attention = true;
                //});

                TabVMs.Where(x => ((Tab)x.Content).GetDispatcher().Invoke(() => ((Tab)x.Content).Messages.Contains(chatMessage))).ToList().ForEach(t => ((Tab)t.Content).Attention = true);

                //var t = TabVMs.FirstOrDefault(x => ((Tab)x.Content).Channels.Contains(chatMessage.Channel));
                //if (t != null)
                //{
                //    ((Tab)t.Content).Attention = true;
                //}
                //else
                //{
                //    t = TabVMs.FirstOrDefault(x => !((Tab)x.Content).ExcludedChannels.Contains(chatMessage.Channel));
                //    if (t != null) ((Tab)t.Content).Attention = true;
                //}
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

        public void CheckVisibility(IList newItems)
        {
            if (CurrentTab == null)
            {
                CurrentTab = TabVMs[0].Content as Tab;
            }
            if (!CurrentTab.Filter(newItems[0] as ChatMessage)) return;
            RefreshHideTimer();
            VisibilityChanged?.Invoke(true);  // IsChatVisible = true;
        }
    }
}