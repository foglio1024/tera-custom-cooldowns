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
        DoubleAnimation opacityUp;
        DoubleAnimation opacityDown;
        bool _bottom = true;
        int _testCounter = 0;

        public object CurrentSender { get; internal set; }

        private void TccWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitWindow(SettingsManager.ChatWindowSettings, false, true);
            opacityUp = new DoubleAnimation(0.01, 1, TimeSpan.FromMilliseconds(300));
            opacityDown = new DoubleAnimation(1, 0.01, TimeSpan.FromMilliseconds(300));
            ChatWindowViewModel.Instance.PropertyChanged += Instance_PropertyChanged;
            _currentContent = itemsControl;
        }

        internal void CloseMap()
        {
            if (mapPopup.IsOpen) mapPopup.IsOpen = false;
            map.StopAnimation();

        }

        internal void OpenMap(MessagePiece context)
        {
            map.DataContext = context;
            map.OnDataChanged();
            mapPopup.IsOpen = true;
            map.StartAnimation();
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
            else if(e.PropertyName == nameof(ChatWindowViewModel.Instance.IsChatVisible))
            {
                AnimateChatVisibility(ChatWindowViewModel.Instance.IsChatVisible);
            }
        }

        private void AnimateChatVisibility(bool isChatVisible)
        {
            Dispatcher.Invoke(() =>
            {
                if (isChatVisible)
                {
                    root.BeginAnimation(OpacityProperty, opacityUp);
                }
                else
                {
                    root.BeginAnimation(OpacityProperty, opacityDown);
                }
            });
        }

        bool _locked;
        internal void LockTooltip(bool locked)
        {
            _locked = locked;
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
                    //Debug.WriteLine("Rows = {0} - LastItemRows = {1}", dc.Rows, lastItemRows);
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
            //Debug.WriteLine("Checking rows:{0}", height);
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
        }

        private void TccWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseMap();
            ChatWindowViewModel.Instance.RefreshHideTimer();
        }

        private void TccWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            ChatWindowViewModel.Instance.StopHideTimer();

        }
    }

}