using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Dragablz;
using GongSolutions.Wpf.DragDrop.Utilities;
using TCC.Settings;
using TCC.ViewModels;

namespace TCC.Windows.Widgets
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
            Init(ws, false, true, false);
            AddHandler(DragablzItem.IsDraggingChangedEvent, new RoutedPropertyChangedEventHandler<bool>(OnIsDraggingChanged));
        }
        public ChatWindow(ChatWindowSettings ws, ChatViewModel vm) : this(ws)
        {
            DataContext = vm;
            VM = DataContext as ChatViewModel;
            if (VM == null) return;
            VM.WindowSettings = ws;
            ((ChatWindowSettings) WindowSettings).FadeoutChanged += () => VM.RefreshHideTimer();
            VM.RefreshHideTimer();
        }

        public void UpdateSettings()
        {
            VM.UpdateSettings(Left, Top);
        }

        private void TabClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement s) || !(s.DataContext is HeaderedItemViewModel hivm)) return;
            var clickedTab = (Tab)hivm.Content;
            clickedTab.Attention = false;

            if (VM.CurrentTab != clickedTab) VM.CurrentTab = clickedTab;
            else
            {
                // scroll all tabs to bottom if the same has been clicked
                TabControl.GetVisualDescendents<ItemsControl>().ToList().ForEach(x =>
                {
                    var sw = Utils.GetChild<ScrollViewer>(x);
                    sw?.ScrollToVerticalOffset(0);
                });
                _bottom = true;
                ChatWindowManager.Instance.AddFromQueue(2);
                if (ChatWindowManager.Instance.IsQueueEmpty) ChatWindowManager.Instance.SetPaused(false);
                ChatWindowManager.Instance.SetPaused(!_bottom);
            }
            SetTopBorder(s);
        }
        private void TabLoaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement s)) return;
            var p = Utils.FindVisualParent<DragablzItemsControl>(s);
            if (p.ItemsSource.TryGetList().IndexOf(s.DataContext) != 0) return;

            SetTopBorder(s);
        }
        private void SetTopBorder(FrameworkElement s)
        {
            var w = s.ActualWidth;
            var left = s.TransformToAncestor(this).Transform(new Point()).X;
            if (left - 3 >= 0) LeftLine.Width = left - 3;
            if (left + w - 3 >= 0) RightLine.Margin = new Thickness(left + w - 3, 0, 0, 0);
        }
        private void OpenTabSettings(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement s)) return;
            if (!(s.DataContext is HeaderedItemViewModel dc)) return;
            var sw = new ChatSettingsWindow((Tab)dc.Content);
            sw.Show();
            sw.Activate();
        }
        private void OnIsDraggingChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            VM.HandleIsDraggingChanged(e.NewValue, TabControl.GetOrderedHeaders());
        }
        private void OnWindowMouseLeave(object sender, MouseEventArgs e)
        {
            VM.RefreshHideTimer();
            if (e.LeftButton == MouseButtonState.Pressed)
                UpdateSettings();
        }
        private void OnWindowMouseEnter(object sender, MouseEventArgs e)
        {
            VM.StopHideTimer();
        }
        private void OnSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            SettingsPopup.DataContext = DataContext;
            SettingsPopup.IsOpen = !SettingsPopup.IsOpen;
        }
        private void OnSettingsPopupMouseLeave(object sender, MouseEventArgs e)
        {
            SettingsPopup.IsOpen = false;
        }
        private void OnWindowPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            WindowManager.RemoveEmptyChatWindows();
            UpdateSettings();
        }
        private new void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);
            TabControl.SelectedIndex = 0;
        }
        private void UnpinMessage(object sender, RoutedEventArgs e)
        {
            var currTabVm = TabControl.SelectedItem as HeaderedItemViewModel;
            if (currTabVm?.Content != null) ((Tab)currTabVm.Content).PinnedMessage = null;
        }
        private void MakeGlobal(object sender, RoutedEventArgs e)
        {
            WindowSettings.MakePositionsGlobal();
        }
        private void ItemsControl_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var sw = Utils.GetChild<ScrollViewer>((ItemsControl)sender);
            var lines = sw.VerticalOffset + e.Delta >= sw.ScrollableHeight ? 1 : SettingsHolder.ChatScrollAmount;
            sw.ScrollToVerticalOffset(sw.VerticalOffset + (e.Delta > 0 ? lines : -lines));

            e.Handled = true;

            if (sw.VerticalOffset == 0)
            {
                _bottom = true;
                ChatWindowManager.Instance.AddFromQueue(0);
                if (ChatWindowManager.Instance.IsQueueEmpty) ChatWindowManager.Instance.SetPaused(false);
            }
            else
            {

                _bottom = false;
            }
            ChatWindowManager.Instance.SetPaused(!_bottom);

        }

        private void OpenSysMsgSettings(object sender, RoutedEventArgs e)
        {
            new SystemMessagesConfigWindow { ShowActivated = true, Topmost = true }.Show();
        }
    }
}