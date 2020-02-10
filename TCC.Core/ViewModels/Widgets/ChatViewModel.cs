using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Dragablz;
using Nostrum;
using Nostrum.Extensions;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Settings;
using TCC.Settings.WindowSettings;
using TCC.Windows;

namespace TCC.ViewModels.Widgets
{


    public class ImportantRemovedArgs : EventArgs
    {
        public ActionType Action { get; }
        public ChatMessage Item { get; }
        public enum ActionType
        {
            Remove,
            Clear
        }

        public ImportantRemovedArgs(ActionType action, ChatMessage item = null)
        {
            Action = action;
            Item = item;
        }

    }
    public class ChatViewModel : TSPropertyChanged
    {
        public event Action ForceSizePosUpdateEvent;

        private bool _paused;
        private bool _visible = true;
        private DispatcherTimer _hideTimer;
        private ChatWindowSettings _windowSettings;
        private bool _collapsed;
        private bool _mouseOver;
        private Tab _currentTab;

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
        public bool Collapsed
        {
            get => WindowSettings.CanCollapse && _collapsed;
            set
            {
                if (_collapsed == value) return;
                _collapsed = value;
                if (_collapsed && WindowSettings.StaysCollapsed)
                {
                    WindowSettings.H = 84;
                    WindowSettings.Y += 84;
                    ForceSizePosUpdateEvent?.Invoke();
                }
                N();
            }
        }
        public bool MouseOver
        {
            get => _mouseOver;
            set
            {
                if (_mouseOver == value) return;
                _mouseOver = value;
                N();
            }
        }
        public Tab CurrentTab
        {
            get => _currentTab;
            set
            {
                if (_currentTab == value) return;
                _currentTab = value;
                N();
            }
        }
        public ICommand MakeGlobalCommand { get; }
        public ICommand OpenSysMsgSettingsCommand { get; }
        public ICommand UnpinMessageCommand { get; }


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
                    _hideTimer.Start();
                    _windowSettings.TimeoutChanged += ChangeTimerInterval;
                }
            }
        }

        public TSObservableCollection<TabViewModel> TabVMs { get; set; }
        public TSObservableCollection<LFG> LFGs => ChatManager.Instance.LFGs;
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
        public Func<TabViewModel> AddNewTabCommand
        {
            get
            {
                return () =>
                {
                    // invoked on main thread because TabData is created there by JSON settings deserialization
                    //return App.BaseDispatcher.Invoke(() =>
                    //{
                    var t = new TabViewModel();
                    var content = new Tab(new TabInfo("New tab"));
                    content.PropertyChanged += (_, ev) =>
                    {
                        if (ev.PropertyName == nameof(TabInfoVM.TabName)) t.Header = content.TabInfoVM.TabName;
                    };
                    t.Content = content;
                    return t;
                    //});
                };
            }
        }

        public ChatViewModel(ChatWindowSettings s)
        {
            WindowSettings = s;
            InterTabClient = new ChatTabClient();
            TabVMs = new TSObservableCollection<TabViewModel>();

            MakeGlobalCommand = new RelayCommand(_ => WindowSettings.MakePositionsGlobal());
            OpenSysMsgSettingsCommand = new RelayCommand(_ => new SystemMessagesConfigWindow { ShowActivated = true, Topmost = true }.Show());

            ChatManager.Instance.NewMessage += CheckAttention;
            Game.GameUiModeChanged += CheckCollapsed;
            Game.ChatModeChanged += CheckCollapsed;
            WindowSettings.CanCollapseChanged += () => N(nameof(Collapsed));
            WindowSettings.StaysCollapsedChanged += () => N(nameof(Collapsed));
            if (WindowSettings.StaysCollapsed) _collapsed = true;
        }

        private void CheckCollapsed()
        {
            Collapsed = !(Game.InGameUiOn || Game.InGameChatOpen) || WindowSettings.StaysCollapsed;
        }

        private void ChangeTimerInterval()
        {
            _hideTimer.Interval = TimeSpan.FromSeconds(WindowSettings.HideTimeout);
            _hideTimer.Refresh();
        }
        private void OnHideTimerTick(object sender, EventArgs e)
        {
            if (!_windowSettings.FadeOut)
            {
                if (!Visible) Visible = true;
                return;
            }

            if (MouseOver) return;
            Visible = false;
            _hideTimer.Stop();
        }
        private void CheckAttention(ChatMessage chatMessage)
        {
            if (!WindowSettings.ShowImportant) return;
            if (!chatMessage.ContainsPlayerName && chatMessage.Channel != ChatChannel.ReceivedWhisper) return;
            TabVMs.Where(x => ((Tab)x.Content).GetDispatcher()
                                .Invoke(() => ((Tab)x.Content).Messages.Contains(chatMessage)))
                        .ToList()
                        .ForEach(t =>
                        {
                            ((Tab)t.Content).AddImportantMessage(chatMessage);
                            //((Tab) t.Content).Attention = true;
                        });
        }
        public void HandleIsDraggingChanged(bool isDragging, IEnumerable<DragablzItem> newOrder)
        {
            if (isDragging)
            {
                FocusManager.ForceFocused = true;
                return;
            }

            var old = new TabViewModel[TabVMs.Count];
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
            //_windowSettings.Tabs.AddRange(Tabs);
            Tabs.ForEach(t => _windowSettings.Tabs.Add(t.TabInfo));
            _windowSettings.X = (left + FocusManager.TeraScreen.Bounds.Left) / WindowManager.ScreenSize.Width;
            _windowSettings.Y = (top + FocusManager.TeraScreen.Bounds.Top) / WindowManager.ScreenSize.Height;
            Task.Run(() =>
            {
                var v = App.Settings.ChatWindowsSettings;
                var s = v.FirstOrDefault(x => x == _windowSettings);
                if (s == null) v.Add(_windowSettings);
            });
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
        public void LoadTabs(IEnumerable<TabInfo> tabs = null)
        {
            if (tabs != null)
            {
                foreach (var tabInfo in tabs)
                {
                    //chatTabsSetting.ApplyFilter();
                    if (tabInfo.Name == null) continue;
                    TabVMs.Add(new TabViewModel(tabInfo.Name, new Tab(tabInfo)));
                }
            }
            if (TabVMs.Count != 0) return;
            var allTabData = new TabInfo("All");
            allTabData.HiddenAuthors.Add("System");
            var all = new Tab(allTabData);

            var guildTabData = new TabInfo("Guild");
            guildTabData.ShowedChannels.Add(ChatChannel.Guild);
            guildTabData.ShowedChannels.Add(ChatChannel.GuildNotice);
            var guild = new Tab(guildTabData);

            var groupTabData = new TabInfo("Group");
            groupTabData.ShowedChannels.Add(ChatChannel.Party);
            groupTabData.ShowedChannels.Add(ChatChannel.PartyNotice);
            groupTabData.ShowedChannels.Add(ChatChannel.Raid);
            groupTabData.ShowedChannels.Add(ChatChannel.RaidLeader);
            groupTabData.ShowedChannels.Add(ChatChannel.Ress);
            groupTabData.ShowedChannels.Add(ChatChannel.Death);
            groupTabData.ShowedChannels.Add(ChatChannel.Group);
            groupTabData.ShowedChannels.Add(ChatChannel.GroupAlerts);
            var group = new Tab(groupTabData);

            var whisperData = new TabInfo("Whisper");
            whisperData.ShowedChannels.Add(ChatChannel.ReceivedWhisper);
            whisperData.ShowedChannels.Add(ChatChannel.SentWhisper);
            var w = new Tab(whisperData);

            var sysData = new TabInfo("System");
            sysData.ShowedAuthors.Add("System");

            var sys = new Tab(sysData);

            TabVMs.Add(new TabViewModel(all.TabInfoVM.TabName, all));
            TabVMs.Add(new TabViewModel(guild.TabInfoVM.TabName, guild));
            TabVMs.Add(new TabViewModel(group.TabInfoVM.TabName, group));
            TabVMs.Add(new TabViewModel(w.TabInfoVM.TabName, w));
            TabVMs.Add(new TabViewModel(sys.TabInfoVM.TabName, sys));
            CurrentTab = TabVMs[0].Content as Tab;
            Tabs.ForEach(t => WindowSettings.Tabs.Add(t.TabInfo));
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
            if (CurrentTab != null && !CurrentTab.Filter(newItems[0] as ChatMessage)) return;
            RefreshHideTimer();
            //VisibilityChanged?.Invoke(true);  // IsChatVisible = true;
            Visible = true;

        }

        public IEnumerable<ClickThruMode> ClickThruModes => EnumUtils.ListFromEnum<ClickThruMode>();

    }
}