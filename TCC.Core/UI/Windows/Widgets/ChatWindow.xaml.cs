using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Dragablz;
using GongSolutions.Wpf.DragDrop.Utilities;
using Nostrum.Extensions;
using TCC.Data.Chat;
using TCC.Settings.WindowSettings;
using TCC.ViewModels;
using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets
{
    public partial class ChatWindow
    {
        private bool _bottom = true;
        public ChatViewModel VM { get; }
        public bool IsPaused => VM.Paused;

        private ChatWindow(ChatWindowSettings ws)
        {
            InitializeComponent();
            MainContent = ChatContent;
            Init(ws);
            AddHandler(DragablzItem.IsDraggingChangedEvent, new RoutedPropertyChangedEventHandler<bool>(OnIsDraggingChanged));
        }
        public ChatWindow(ChatWindowSettings ws, ChatViewModel vm) : this(ws)
        {
            DataContext = vm;
            VM = DataContext as ChatViewModel;
            if (VM == null) return;
            VM.WindowSettings = ws;
            ((ChatWindowSettings)WindowSettings).FadeoutChanged += () => VM.RefreshHideTimer();
            VM.RefreshHideTimer();
            VM.ForceSizePosUpdateEvent += OnForceSizePosUpdate;
        }

        private void OnForceSizePosUpdate()
        {
            Dispatcher?.InvokeAsync(() =>
            {
                ReloadPosition();
                Height = WindowSettings.H;
                Width = WindowSettings.W;
            });
        }

        public void UpdateSettings()
        {
            Dispatcher?.InvokeAsync(() => { VM.UpdateSettings(Left, Top); });
        }

        private void TabLoaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement s)) return;
            var p = s.FindVisualParent<DragablzItemsControl>();
            if (p.ItemsSource.TryGetList().IndexOf(s.DataContext) != 0) return;

            SetTopBorder(s);
        }
        private void OpenTabSettings(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement s)) return;
            if (!(s.DataContext is HeaderedItemViewModel dc)) return;

            var sw = new ChatSettingsWindow((Tab) dc.Content);
            sw.Show();
            sw.Activate();
        }
        private void OnIsDraggingChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            VM.HandleIsDraggingChanged(e.NewValue, TabControl.GetOrderedHeaders());
        }
        private void OnWindowMouseLeave(object sender, MouseEventArgs e)
        {
            VM.MouseOver = false;
            VM.RefreshHideTimer();
            if (e.LeftButton == MouseButtonState.Pressed)
                UpdateSettings();
        }
        private void OnWindowMouseEnter(object sender, MouseEventArgs e)
        {
            VM.MouseOver = true;
            VM.StopHideTimer();
        }
        private void OnSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            SettingsPopup.IsOpen = !SettingsPopup.IsOpen;
        }

        private void OnWindowPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ChatManager.Instance.RemoveEmptyChatWindows();
            UpdateSettings();
        }
        private new void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);
            TabControl.SelectedIndex = 0;
        }

        private void ItemsControl_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var sw = ((ItemsControl)sender).GetChild<ScrollViewer>();
            var lines = sw.VerticalOffset + e.Delta >= sw.ScrollableHeight ? 1 : App.Settings.ChatScrollAmount;
            sw.ScrollToVerticalOffset(sw.VerticalOffset + (e.Delta > 0 ? lines : -lines));

            e.Handled = true;

            if (sw.VerticalOffset == 0)
            {
                _bottom = true;
                ChatManager.Instance.AddFromQueue(App.Settings.ChatScrollAmount);
            }
            else
            {
                _bottom = false;
            }
            ChatManager.Instance.SetPaused(!_bottom || !ChatManager.Instance.IsQueueEmpty);

        }

        public void ScrollToMessage(Tab tab, ChatMessage msg)
        {
            if (VM.CurrentTab != tab) TabControl.SelectedIndex = VM.Tabs.IndexOf(tab);
            TabControl.GetVisualDescendents<ItemsControl>().ToList().ForEach(x =>
            {
                if (!x.IsVisible) return;

                var host = x.GetChild<VirtualizingStackPanel>();
                if (host == null) return;
                var idx = x.Items.IndexOf(msg);
                if (idx == -1) return;
                host.BringIndexIntoViewPublic(idx);
            });
        }

        private void TabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            var tabVm = (TabViewModel) e.AddedItems[0];
            var selectedTab = (Tab)tabVm.Content;
            //clickedTab.ClearImportant();

            if (VM.CurrentTab != selectedTab) VM.CurrentTab = selectedTab;
            else
            {
                // scroll all tabs to bottom if the same has been clicked
                TabControl.GetVisualDescendents<ItemsControl>().ToList().ForEach(x =>
                {
                    var sw = x.GetChild<ScrollViewer>();
                    sw?.ScrollToVerticalOffset(0);
                });
                _bottom = true;
                ChatManager.Instance.AddFromQueue(2);
                if (ChatManager.Instance.IsQueueEmpty) ChatManager.Instance.SetPaused(false);
                ChatManager.Instance.SetPaused(!_bottom);
            }
        }

        private void SetLines(object sender, MouseButtonEventArgs e)
        {
            if (VM.CurrentTab != ((sender as FrameworkElement)?.DataContext as TabViewModel)?.Content as Tab) return;
            SetTopBorder(sender as FrameworkElement);
        }
        private void SetTopBorder(FrameworkElement s)
        {
            var w = s.ActualWidth;
            var left = s.TransformToAncestor(this).Transform(new Point()).X;
            if (left - 2 >= 0) LeftLine.Width = left - 2;
            if (left + w - 6 >= 0) RightLine.Margin = new Thickness(left + w - 6, 0, 0, 0);
        }

    }
}