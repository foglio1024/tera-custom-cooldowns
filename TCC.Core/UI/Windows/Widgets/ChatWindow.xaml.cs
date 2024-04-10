using Dragablz;
using GongSolutions.Wpf.DragDrop.Utilities;
using Nostrum.WPF.Extensions;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCC.Data.Chat;
using TCC.Settings.WindowSettings;
using TCC.ViewModels;
using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets;

public partial class ChatWindow
{
    private bool _bottom = true;
    public ChatViewModel VM { get; }
    public bool IsPaused => VM.Paused;


    public ChatWindow(ChatViewModel vm)
    {
        InitializeComponent();
        MainContent = ChatContent;
        BoundaryRef = Boundary;
        Init(vm.WindowSettings);
        AddHandler(DragablzItem.IsDraggingChangedEvent, new RoutedPropertyChangedEventHandler<bool>(OnIsDraggingChanged));

        VM = vm;
        DataContext = vm;
        if (VM == null) throw new NullReferenceException("Window DataContext is null!");

        VM.RefreshHideTimer();
        VM.ForceSizePosUpdateEvent += OnForceSizePosUpdate;

        if (WindowSettings == null) return;

        ((ChatWindowSettings)WindowSettings).FadeoutChanged += () => VM.RefreshHideTimer();
    }

    private void OnForceSizePosUpdate()
    {
        if (WindowSettings == null) return;

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
        if (sender is not FrameworkElement s) return;
        var p = s.FindVisualParent<DragablzItemsControl>();
        if (p == null) return;
        if (p.ItemsSource.TryGetList().IndexOf(s.DataContext) != 0) return;

        SetTopBorder(s);
    }

    private void OpenTabSettings(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement s) return;
        if (s.DataContext is not HeaderedItemViewModel dc) return;
        var tab = (Tab)dc.Content;

        if (e.RightButton == MouseButtonState.Pressed)
        {
            var sw = ChatSettingsWindow.OpenWindows.FirstOrDefault(x => x.DataContext == tab) 
                ?? new ChatSettingsWindow(tab);
            sw.Show();
            sw.Activate();
        }
        else if (e.MiddleButton == MouseButtonState.Pressed)
        {
            var currSel = TabControl.SelectedIndex;
            VM.RemoveTab(tab);
            UpdateSettings();


            if (VM.TabVMs.Count == 0)
            {
                Close();
                if (WindowSettings == null) return;
                App.Settings.ChatWindowsSettings.Remove((ChatWindowSettings)WindowSettings);
            }
            else
            {
                TabControl.SelectedIndex = currSel == 0 ? 0 : currSel - 1;
            }

        }
        else
        {
            e.Handled = false;
        }
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
        var sw = ((ItemsControl)sender).FindVisualChild<ScrollViewer>();
        if (sw == null) return;

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

    public void ScrollToBottom()
    {
        if (VM.CurrentTab == null)
            TabControl.SelectedIndex = 0;
        TabControl.GetVisualDescendents<ItemsControl>().ToList().ForEach(x =>
        {
            if (!x.IsVisible) return;
            var host = x.FindVisualChild<VirtualizingStackPanel>();
            var sw = host?.FindVisualParent<ScrollViewer>();
            if (sw == null) return;
            sw.ScrollToTop();
        });

    }

    public void ScrollToMessage(Tab tab, ChatMessage msg)
    {
        if (VM.CurrentTab != tab) TabControl.SelectedIndex = VM.Tabs.IndexOf(tab);
        TabControl.GetVisualDescendents<ItemsControl>().ToList().ForEach(x =>
        {
            if (!x.IsVisible) return;

            var host = x.FindVisualChild<VirtualizingStackPanel>();
            if (host == null) return;
            var idx = x.Items.IndexOf(msg);
            if (idx == -1) return;
            host.BringIndexIntoViewPublic(idx);
        });
    }

    private void TabChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0) return;
        var tabVm = (TabViewModel?)e.AddedItems[0];
        var selectedTab = (Tab?)tabVm?.Content;
        if (selectedTab == null) return;

        if (VM.CurrentTab != selectedTab)
        {
            VM.CurrentTab = selectedTab;
        }
        else
        {
            // scroll all tabs to bottom if the same has been clicked
            TabControl.GetVisualDescendents<ItemsControl>().ToList().ForEach(x =>
            {
                var sw = x.FindVisualChild<ScrollViewer>();
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
        if (VM.CurrentTab != (Tab)((TabViewModel)((FrameworkElement)sender).DataContext).Content) return;
        SetTopBorder((FrameworkElement)sender);
    }

    private void SetTopBorder(FrameworkElement s)
    {
        var w = s.ActualWidth;
        var left = s.TransformToAncestor(this).Transform(new Point()).X;
        if (left - 2 >= 0)
        {
            LeftLine.Width = Math.Max(left - 2 - (LeftLine.Margin.Left + LeftLine.Margin.Right), 0);
        }
        if (left + w - 6 >= 0) RightLine.Margin = new Thickness(left + w - 6, 0, 0, 0);

        if (VM.Tabs.IndexOf(VM.CurrentTab!) > 0)
        {
            MainBorder.CornerRadius = MainBorder.CornerRadius with { TopLeft = 5 };
        }
        else
        {
            MainBorder.CornerRadius = MainBorder.CornerRadius with { TopLeft = 0 };
        }
    }
}