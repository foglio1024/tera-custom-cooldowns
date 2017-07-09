using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TCC.Controls;
using TCC.Controls.ChatControls;
using TCC.Data;
using TCC.Parsing;
using TCC.ViewModels;

namespace TCC.Windows
{

    public partial class ChatWindow : TccWindow
    {
        DispatcherTimer testTimer;
        bool _bottom = true;
        int _testCounter = 0;

        public object CurrentSender { get; internal set; }

        private void TccWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //InitWindow(SettingsManager.ChatWindowSettings);

            ChatWindowViewModel.Instance.PropertyChanged += Instance_PropertyChanged;
            //_scroller = (ScrollViewer)VisualTreeHelper.GetChild(itemsControl, 0);
            //_scroller.ScrollChanged += Scroller_ScrollChanged;
            testTimer = new DispatcherTimer();
            testTimer.Interval = TimeSpan.FromMilliseconds(1000);
            testTimer.Tick += (s, ev) =>
            {
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Say, "Author", "<FONT>SayMessage " + _testCounter + "</FONT>"));
                if(_testCounter % 5 == 0) ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Say, "Author", "<FONT>Testtttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt Messageeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee </FONT>"));

                _testCounter++;
            };
            _currentContent = itemsControl;
            //ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Say, "Author", "<FONT>Test Message </FONT>"));
            //ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Say, "Author", "<FONT>Testtttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt Messageeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee </FONT>"));
            //ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Say, "Author", "<FONT>Testtttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt Messageeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee Testtttttttttttttttttttttttttttttttttttttttt  </FONT>"));
            //ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Say, "Author", "<FONT>Testtttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt Messageeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee Testtttttttttttttttttttttttttttttttttttttttt Messageeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee </FONT>"));

            //testTimer.Start();
            //for (int i = 0; i < 20; i++)
            //{
            //    ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Guild, "Author" + i, "<FONT>GuildMessage " + i + "</FONT>"));

            //}
            //ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Party, "Author", "<FONT>PartyMessage eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee" + "</FONT>"));
            //for (int i = 0; i < 10; i++)
            //{
            //    ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.System, "System", "<FONT>SysMessage " + i + "</FONT>"));

            //}

            //ChatWindowViewModel.Instance.LFGs.Add(new LFG(1, "asd", "Aasdasd", false));
        }


        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "NewItem")
            {
                if (_bottom)
                {
                    var t = tabControl.SelectedContent as ItemsControl;
                    var b = (Border)VisualTreeHelper.GetChild(t, 0);
                    var s = (ScrollViewer)VisualTreeHelper.GetChild(b, 0);

                    s.ScrollToBottom();
                }
            }
        }
        bool _locked;
        internal void LockTooltip(bool locked)
        {
            _locked = locked;
        }

        private void TccWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();

            }
            catch (Exception)
            {

            }
        }
        private void SWPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer s = (ScrollViewer)sender;
            if (s.VerticalOffset == s.ScrollableHeight)
            {
                _bottom = true;
            }
            else
            {
                _bottom = false;

            }

        }
        public void OpenTooltip()
        {
            Dispatcher.Invoke(() =>
            {
                if (playerInfo.IsOpen) CloseTooltip();
                ChatWindowViewModel.Instance.TooltipInfo.Refresh();
                playerInfo.IsOpen = true;
                ((PlayerTooltip)playerInfo.Child).AnimateOpening();
            });
        }
        public void CloseTooltip()
        {
            Dispatcher.Invoke(() =>
            {
                if (_locked) return;
                playerInfo.IsOpen = false;
            });
        }
        public ChatWindow()
        {
            InitializeComponent();

        }

        private void playerInfo_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseTooltip();
        }

        private void TabClicked(object sender, MouseButtonEventArgs e)
        {

            var s = sender as FrameworkElement;
            AnimateTabRect(s);
            _currentContent = tabControl.SelectedContent as ItemsControl;
            var b = (Border)VisualTreeHelper.GetChild(_currentContent, 0);
            var sw = (ScrollViewer)VisualTreeHelper.GetChild(b, 0);

            sw.ScrollToBottom();
        }
        private void AnimateTabRect(FrameworkElement s)
        {
            var w = s.ActualWidth;
            Point r = s.TranslatePoint(new Point(0, 0), (UIElement)s.Parent);
            var sizeAn = new DoubleAnimation(w, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() };
            var posAn = new DoubleAnimation(r.X, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() };

            selectionRect.BeginAnimation(WidthProperty, sizeAn);
            selectionRect.RenderTransform.BeginAnimation(TranslateTransform.XProperty, posAn);

        }


        ItemsControl _currentContent;
        VirtualizingStackPanel _currentPanel;
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //if (!_bottom) return;
            _currentContent = tabControl.SelectedContent as ItemsControl;
            _currentPanel = GetInnerStackPanel(_currentContent);
            var sw = (ScrollViewer)sender;
            Rect svBounds = LayoutInformation.GetLayoutSlot(sw);
            var testRect = new Rect(svBounds.Top-5, svBounds.Left, svBounds.Width, svBounds.Height+10);
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
                    Debug.WriteLine("Rows = {0} - LastItemRows = {1}", dc.Rows, lastItemRows);
                    if(dc.Rows - lastItemRows >= 1)
                    {
                        dc.Rows = dc.Rows - lastItemRows;
                        dc.IsContracted = true;
                    }
                }
                else
                {

                }
            }
        }
        private List<FrameworkElement> GetVisibleItems(Rect svViewportBounds)
        {
            List<FrameworkElement> result = new List<FrameworkElement>();
            if (_currentPanel.Children.Count > 2)
            {
                for (int i = 0; i < _currentPanel.Children.Count; i++)
                {
                    var container = (_currentPanel.Children[i] as FrameworkElement);

                    if (container != null)
                    {
                        var offset = VisualTreeHelper.GetOffset(container);
                        var bounds = new Rect(offset.X, offset.Y, container.ActualWidth, container.ActualHeight);

                        if (svViewportBounds.Contains(bounds) || svViewportBounds.IntersectsWith(bounds))
                        {
                            result.Add(container);
                        }
                    }
                }
            }
            return result;
        }
        public int GetMessageRows(double height)
        {
            Debug.WriteLine("Checking rows:{0}", height);
            if (height < 28) return 1;
            else if (height < 46) return 2;
            else if (height < 64) return 3;
            else if (height < 82) return 4;
            else if (height < 100) return 5;
            else return 6;
        }
        private VirtualizingStackPanel GetInnerStackPanel(FrameworkElement element)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child == null) continue;


                if (child is VirtualizingStackPanel) return child as VirtualizingStackPanel;

                var panel = GetInnerStackPanel(child);

                if (panel != null)
                    return panel;
            }

            return null;

        }
        private void TccWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PacketProcessor.SendFakeBamMessage();
            //ChatWindowViewModel.Instance.LFGs.Clear();
        }
    }

}