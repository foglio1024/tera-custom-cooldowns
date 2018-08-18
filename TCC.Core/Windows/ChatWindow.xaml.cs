using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shell;
using Dragablz;
using GongSolutions.Wpf.DragDrop.Utilities;
using TCC.Annotations;
using TCC.Controls.ChatControls;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Windows
{

    public partial class ChatWindow
    {
        private readonly DoubleAnimation _opacityUp;
        private readonly DoubleAnimation _opacityDown;
        private bool _bottom = true;
        private bool _isChatEnabled;
        public ChatViewModel VM => Dispatcher.Invoke(() => DataContext as ChatViewModel);
        public bool IsPaused => Dispatcher.Invoke(() => VM.Paused);
        public ChatWindow(ChatWindowSettings ws)
        {
            InitializeComponent();
            //ButtonsRef = buttons;
            MainContent = content;
            Init(ws, false);
            _opacityUp = new DoubleAnimation(0.01, 1, TimeSpan.FromMilliseconds(300));
            _opacityDown = new DoubleAnimation(1, 0.01, TimeSpan.FromMilliseconds(300));
            ChatWindowManager.Instance.PropertyChanged += Instance_PropertyChanged; //TODO: use DataContext as ChatWindowVM?
            AddHandler(DragablzItem.IsDraggingChangedEvent, new RoutedPropertyChangedEventHandler<bool>(OnDragCompleted));
        }
        public ChatWindow(ChatWindowSettings ws, ChatViewModel vm) : this(ws)
        {
            DataContext = vm;
            UpdateSettings();
        }



        private void OnDragCompleted(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (e.NewValue)
            {
                FocusManager.FocusTimer.Enabled = false;
                return;
            }

            var newOrder = TabControl.GetOrderedHeaders();
            var old = new HeaderedItemViewModel[VM.TabVMs.Count];
            VM.TabVMs.CopyTo(old, 0);
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
            if (same) return;
            VM.TabVMs.Clear();
            foreach (var tab in items)
            {
                VM.TabVMs.Add(old.FirstOrDefault(x => x.Header == tab.Content));
            }
        }

        public void UpdateSettings()
        {
            Dispatcher.Invoke(() =>
            {

                if (VM.TabVMs.Count == 0)
                {
                    foreach (var tab in TabControl.ItemsSource)
                    {
                        VM.TabVMs.Add(tab as HeaderedItemViewModel);
                    }
                }

            ((ChatWindowSettings)WindowSettings).Tabs.Clear();
                ((ChatWindowSettings)WindowSettings).Tabs.AddRange(VM.Tabs);
                ((ChatWindowSettings)WindowSettings).LfgOn = VM.LfgOn;
                ((ChatWindowSettings)WindowSettings).BackgroundOpacity = VM.BackgroundOpacity;
                ((ChatWindowSettings)WindowSettings).X = Left / SettingsManager.ScreenW;
                ((ChatWindowSettings)WindowSettings).Y = Top / SettingsManager.ScreenH;
                var v = SettingsManager.ChatWindowsSettings;
                var s = v.FirstOrDefault(x => x == WindowSettings);
                if (s == null) v.Add(WindowSettings as ChatWindowSettings);
                else s = WindowSettings as ChatWindowSettings;

                if (ChatTabClient.LastSource != this && ChatTabClient.LastSource != null)
                {
                    ChatTabClient.LastSource.UpdateSettings();
                }
            });
        }
        private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChatWindowManager.Instance.IsChatVisible))
            {
                AnimateChatVisibility(ChatWindowManager.Instance.IsChatVisible);
            }
        }

        private void AnimateChatVisibility(bool isChatVisible)
        {
            Dispatcher.Invoke(() => { Root.BeginAnimation(OpacityProperty, isChatVisible ? _opacityUp : _opacityDown); });
        }

        private void SwPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var s = (ScrollViewer)sender;
            var offset = e.Delta > 0 ? -2 : 2;
            s.ScrollToVerticalOffset(s.VerticalOffset - offset);
            e.Handled = true;
            if (s.VerticalOffset == 0)
            {
                _bottom = true;
                ChatWindowManager.Instance.AddFromQueue(2);
                if (ChatWindowManager.Instance.IsQueueEmpty) ChatWindowManager.Instance.SetPaused(false);

            }
            else
            {

                _bottom = false;
            }

            ChatWindowManager.Instance.SetPaused(!_bottom);

        }

        //public void OpenTooltip()
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        FocusManager.FocusTimer.Enabled = false;
        //        if (PlayerInfo.IsOpen) CloseTooltip();
        //        WindowManager.FloatingButton.TooltipInfo.Refresh();
        //        PlayerInfo.IsOpen = true;
        //        ((PlayerTooltip)PlayerInfo.Child).AnimateOpening();
        //    });
        //}
        //public void CloseTooltip()
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        FocusManager.FocusTimer.Enabled = true;
        //        PlayerInfo.IsOpen = false;
        //    });
        //}

        private void TabClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement s) || !(s.DataContext is HeaderedItemViewModel t)) return;
            ((Tab)t.Content).Attention = false;
            ((ChatViewModel)DataContext).CurrentTab = (Tab)t.Content;

            var w = s.ActualWidth;
            var left = s.TransformToAncestor(this).Transform(new Point()).X;
            if (left - 3 > 0)
            {
                LeftLine.Width = left - 3;
                RightLine.Margin = new Thickness(left + w - 3, 0, 0, 0);
            }
        }

        private void TabLoaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement s)) return;
            var p = VisualTreeHelper.GetParent(s);
            p = VisualTreeHelper.GetParent(p);
            p = VisualTreeHelper.GetParent(p);
            p = VisualTreeHelper.GetParent(p);
            p = VisualTreeHelper.GetParent(p);
            p = VisualTreeHelper.GetParent(p);
            p = VisualTreeHelper.GetParent(p); //TODO: REFACTOR THIS
            if ((p as ItemsControl).ItemsSource.TryGetList().IndexOf(s.DataContext) != 0) return;
            var w = s.ActualWidth;
            var left = s.TransformToAncestor(this).Transform(new Point()).X;
            LeftLine.Width = left - 3;
            RightLine.Margin = new Thickness(left + w - 3, 0, 0, 0);

        }


        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //if (!_bottom) return;
            /*
                        _currentContent = tabControl.SelectedContent as ItemsControl;
                        _currentPanel = GetInnerStackPanel(_currentContent);
                        var sw = (ScrollViewer)sender;
                        Rect svBounds = LayoutInformation.GetLayoutSlot(sw);
                        var testRect = new Rect(svBounds.Top - 5, svBounds.Left, svBounds.Width, svBounds.Height + 10);
                        List<FrameworkElement> visibleItems = GetVisibleItems(testRect);

                        foreach (var item in visibleItems)
                        {
                            var i = visibleItems.IndexOf(item);
                            if (i > 4)
                            {
                                continue;
                            }
                            else
                            {

                            }

                            var dc = ((ChatMessage)item.DataContext);
                            if (dc.Channel == ChatChannel.Bargain) return;
                            if (GetMessageRows(item.ActualHeight) > 1)
                            {
                                var lastItemRows = GetMessageRows(visibleItems.Last().ActualHeight);
                                //Debug.WriteLine("Rows = {0} - LastItemRows = {1}", dc.Rows, lastItemRows);
                                if (dc.Rows - lastItemRows >= 1)
                                {
                                    dc.Rows = dc.Rows - lastItemRows;
                                    dc.IsContracted = true;
                                }
                            }
                            else
                            {

                            }
                        }
            */
        }

        private void TccWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //var res = TccMessageBox.Show("TCC", $"There was an error while reading events-EU.xml. Manually correct the error and and press Ok to try again, else press Cancel to build a default config file.", MessageBoxButton.OKCancel);
            //Proxy.ChatTest("test");
            //ChatWindowManager.Instance.AddChatMessage(new ChatMessage(ChatChannel.Global, "Moonfury", "<font size=\"50\" color=\"#e87d7d\"> M</font>" + "<font size=\"70\" color=\"#e8b07d\">E</font>" + "<font size=\"70\" color=\"#e8d77d\">M</font>" + "<font size=\"70\" color=\"#c6e87d\">E</font>" + "<font size=\"50\" color=\"#92e87d\">S</font>" + "<font size=\"50\" color=\"#7de89b\">L</font>" + "<font size=\"50\" color=\"#7de8ce\">A</font>" + "<font size=\"50\" color=\"#7dcee8\">S</font>" + "<font size=\"50\" color=\"#7d8ee8\">H</font>"));
            //ChatWindowManager.Instance.AddChatMessage(new ChatMessage(ChatChannel.Global, "PODEM CONFIAR", "<font size=\"25\" color=\"#ff0000\"><a href=\"asfunction:chatLinkAction\">I am a retard that runs random code from the internet, so my character has been sent to the DOOMZONE.</a></font>"));
            //ChatWindowManager.Instance.AddChatMessage(new ChatMessage(ChatChannel.Global, "PODEM CONFIAR", "<font size=\"32\" color=\"#ff0000\"><a href=\"asfunction:chatLinkAction\">Goodbye everyone!</a></font>"));

            //GroupWindowViewModel.Instance.ClearAll();
            //for (int i = 0; i < 23; i++)
            //{
            //    var name = "D" + i;
            //    GroupWindowViewModel.Instance.AddOrUpdateMember(new User(GroupWindowViewModel.Instance.GetDispatcher()) { Name = name, UserClass = Class.Warrior, ServerId = (uint)i + 100 });
            //}
            //for (int i = 0; i < 2; i++)
            //{
            //    var name = "T" + i;

            //    GroupWindowViewModel.Instance.AddOrUpdateMember(new User(GroupWindowViewModel.Instance.GetDispatcher()) { Name = name, UserClass = Class.Lancer, ServerId = (uint)i + 200 });
            //}
            //for (int i = 0; i < 5; i++)
            //{
            //    var name = "H" + i;

            //    GroupWindowViewModel.Instance.AddOrUpdateMember(new User(GroupWindowViewModel.Instance.GetDispatcher()) { Name = name, UserClass = Class.Mystic, ServerId = (uint)i + 300 });
            //}
        }

        private void TccWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            ChatWindowManager.Instance.RefreshTimer();
            Settings.BeginAnimation(OpacityProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)));
            if (e.LeftButton == MouseButtonState.Pressed) UpdateSettings();
        }

        private void TccWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            ChatWindowManager.Instance.StopHideTimer();
            Settings.BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)));
        }

        private void OpenTabSettings(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is FrameworkElement s)) return;
            if (!(s.DataContext is HeaderedItemViewModel dc)) return;
            var t = (Tab)dc.Content;
            var sw = new ChatSettingsWindow(t);
            sw.Show();
            sw.Activate();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsPopup.IsOpen = !SettingsPopup.IsOpen;
        }

        private void SettingsPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            if (SettingsPopup.IsMouseCaptured) return;
            SettingsPopup.IsOpen = false;
        }

        private void SettingsPopup_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (sender as FrameworkElement)?.CaptureMouse();
        }

        private void SettingsPopup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as FrameworkElement)?.ReleaseMouseCapture();

        }

        private void ChatWindow_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var ws = App.Current.Windows;

            foreach (Window w in ws)
            {
                if (w is ChatWindow cw)
                {
                    if (cw.VM.TabVMs.Count == 0)
                    {
                        ChatWindowManager.Instance.ChatWindows.Remove(cw);
                        cw.Close();
                    }
                }
            }
            UpdateSettings();
            if (!FocusManager.FocusTimer.Enabled) FocusManager.FocusTimer.Enabled = true;
        }

        private void ChatWindow_OnDragLeave(object sender, DragEventArgs e)
        {

        }



        private new void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);
            TabControl.SelectedIndex = 0;

        }

        //private new void Drag(object sender, MouseButtonEventArgs e)
        //{
        //    //needed to keep windowchrome, no idea why
        //    return;
        //}
        private void TabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void UnpinMessage(object sender, RoutedEventArgs e)
        {
            var currTabVm = TabControl.SelectedItem as HeaderedItemViewModel;
            //var msg = (sender as FrameworkElement).DataContext as ChatMessage;
            //var tabVm = VM.TabVMs.FirstOrDefault(x =>
            //    ((Tab)x.Content).Messages.Contains(msg) && x == currTabVm);
            if (currTabVm?.Content != null) ((Tab)currTabVm?.Content).PinnedMessage = null;

            //var tab = VM.Tabs.FirstOrDefault(x => 
            //x.PinnedMessage == (((sender as FrameworkElement)?.DataContext as HeaderedItemViewModel)?.Content as Tab)?.PinnedMessage
            //);
            //if (tab != null) tab.PinnedMessage = null;
        }

        private void PinnedMessageOnContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            FocusManager.FocusTimer.Enabled = true;
        }

        private void PinnedMessageOnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            FocusManager.FocusTimer.Enabled = false;
        }
    }

}