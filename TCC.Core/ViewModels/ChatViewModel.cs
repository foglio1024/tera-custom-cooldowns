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
    public class ChatViewModel : TSPropertyChanged
    {
        private bool _paused;
        private bool _visible;
        private DispatcherTimer _hideTimer;
        private ChatWindowSettings _windowSettings;

        public bool Paused
        {
            get => _paused;
            set
            {
                if (_paused == value) return;
                _paused = value;
                N();
            }
        }
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible == value) return;
                _visible = value;
                N();
            }
        }
        public Tab CurrentTab { get; set; }
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

        public SynchronizedObservableCollection<HeaderedItemViewModel> TabVMs { get; set; }
        public SynchronizedObservableCollection<LFG> LFGs => ChatWindowManager.Instance.LFGs;
        public IInterTabClient InterTabClient { get; }
        public List<Tab> Tabs
        {
            get
            {
                var ret = new List<Tab>();
                TabVMs.ToList().ForEach(x => ret.Add(x.Content as Tab));
                return ret;
            }
        }
        public Func<HeaderedItemViewModel> AddNewTabCommand
        {
            get
            {
                return () =>
                {
                    var t = new HeaderedItemViewModel();
                    var content = new Tab("New tab", new ChatChannel[] { }, new ChatChannel[] { }, new string[] { }, new string[] { });
                    content.PropertyChanged += (_, ev) =>
                    {
                        if (ev.PropertyName == nameof(Tab.TabName)) t.Header = content.TabName;
                    };
                    t.Content = content;
                    return t;
                };
            }
        }

        public ChatViewModel()
        {
            InterTabClient = new ChatTabClient();
            TabVMs = new SynchronizedObservableCollection<HeaderedItemViewModel>();

            ChatWindowManager.Instance.NewMessage += CheckAttention;
        }

        private void ChangeTimerInterval()
        {
            _hideTimer.Interval = TimeSpan.FromSeconds(WindowSettings.HideTimeout);
            _hideTimer.Refresh();
        }
        private void OnHideTimerTick(object sender, EventArgs e)
        {
            Visible = false;
            _hideTimer.Stop();
        }
        private void CheckAttention(ChatMessage chatMessage)
        {
            if (!chatMessage.ContainsPlayerName && chatMessage.Channel != ChatChannel.ReceivedWhisper) return;
            TabVMs.Where(x =>
                ((Tab)x.Content).GetDispatcher().Invoke(() =>
                    ((Tab)x.Content).Messages.Contains(chatMessage))).ToList().ForEach(t =>
                        ((Tab)t.Content).Attention = true);
        }
        public void HandleIsDraggingChanged(bool isDragging, IEnumerable<DragablzItem> newOrder)
        {
            if (isDragging)
            {
                FocusManager.ForceFocused = true;
                return;
            }

            var old = new HeaderedItemViewModel[TabVMs.Count];
            TabVMs.CopyTo(old, 0);
            var same = true;
            var items = newOrder.ToList();
            for (var i = 0; i < items.Count; i++)
            {
                if (old[i].Header != items.ToList()[i].Content)
                {
                    same = false;
                    break;
                }
            }
            if (same)
            {
                FocusManager.ForceFocused = false;
                return;
            }
            TabVMs.Clear();
            foreach (var tab in items)
            {
                TabVMs.Add(old.FirstOrDefault(x => x.Header == tab.Content));
            }
            FocusManager.ForceFocused = false;
        }
        public void UpdateSettings(double left, double top)
        {
            _windowSettings.Tabs.Clear();
            _windowSettings.Tabs.AddRange(Tabs);
            _windowSettings.X = left / SettingsHolder.ScreenW;
            _windowSettings.Y = top / SettingsHolder.ScreenH;
            var v = SettingsHolder.ChatWindowsSettings;
            var s = v.FirstOrDefault(x => x == _windowSettings);
            if (s == null) v.Add(_windowSettings);
            //if (!Equals(ChatTabClient.LastSource, this) && ChatTabClient.LastSource != null)
            //{
            //    ChatTabClient.LastSource.UpdateSettings();
            //}
        }
        public void RefreshHideTimer()
        {
            _hideTimer.Refresh();
        }
        public void StopHideTimer()
        {
            _hideTimer.Stop();
            Visible = true;
        }
        public void LoadTabs(IEnumerable<Tab> tabs = null)
        {
            if (tabs != null)
            {
                foreach (var chatTabsSetting in tabs)
                {
                    //chatTabsSetting.ApplyFilter();
                    TabVMs.Add(new HeaderedItemViewModel(chatTabsSetting.TabName, chatTabsSetting));
                }
            }
            if (TabVMs.Count != 0) return;
            var all = new Tab("All", new ChatChannel[] { }, new ChatChannel[] { }, new string[] { }, new[] { "System" });
            //all.ApplyFilter();
            var guild = new Tab("Guild", new[] { ChatChannel.Guild, ChatChannel.GuildNotice, }, new ChatChannel[] { }, new string[] { }, new string[] { });
            //guild.ApplyFilter();
            var group = new Tab("Group", new[]{ChatChannel.Party, ChatChannel.PartyNotice,
                ChatChannel.RaidLeader, ChatChannel.RaidNotice,
                ChatChannel.Raid, ChatChannel.Ress,ChatChannel.Death,
                ChatChannel.Group, ChatChannel.GroupAlerts  }, new ChatChannel[] { }, new string[] { }, new string[] { });
            //group.ApplyFilter();
            var w = new Tab("Whisper", new[] { ChatChannel.ReceivedWhisper, ChatChannel.SentWhisper, }, new ChatChannel[] { }, new string[] { }, new string[] { });
            //w.ApplyFilter();
            var sys = new Tab("System", new ChatChannel[] { }, new ChatChannel[] { }, new[] { "System" }, new string[] { });
            //sys.ApplyFilter();

            TabVMs.Add(new HeaderedItemViewModel(all.TabName, all));
            TabVMs.Add(new HeaderedItemViewModel(guild.TabName, guild));
            TabVMs.Add(new HeaderedItemViewModel(group.TabName, group));
            TabVMs.Add(new HeaderedItemViewModel(w.TabName, w));
            TabVMs.Add(new HeaderedItemViewModel(sys.TabName, sys));
            CurrentTab = TabVMs[0].Content as Tab;
            //ChatWindowManager.Instance.FindMyWindow(this).UpdateSettings();

        }
        public void RemoveTab(Tab dc)
        {
            var t = TabVMs.FirstOrDefault(x => x.Content == dc);
            if (t != null) TabVMs.Remove(t);
        }
        public void CheckVisibility(IList newItems)
        {
            if (CurrentTab == null)
            {
                CurrentTab = TabVMs[0].Content as Tab;
            }
            if (!CurrentTab.Filter(newItems[0] as ChatMessage)) return;
            RefreshHideTimer();
            //VisibilityChanged?.Invoke(true);  // IsChatVisible = true;
            Visible = true;

        }
    }
}