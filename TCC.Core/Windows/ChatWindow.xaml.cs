using System;
using System.Collections.Generic;
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
            testTimer.Interval = TimeSpan.FromMilliseconds(100);
            testTimer.Tick += (s, ev) =>
            {
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Say, "Author", "<FONT>SayMessage " + _testCounter + "</FONT>"));
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Party, "Author2", "<FONT>SayMessage " + _testCounter + "</FONT>"));
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Guild, "Author3", "<FONT>SayMessage " + _testCounter + "</FONT>"));
                _testCounter++;
            };
            //testTimer.Start();
            //for (int i = 0; i < 100; i++)
            //{
            //    ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Guild, "Author" + i, "<FONT>GuildMessage " + i + "</FONT>"));

            //}
            //ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Party, "Author", "<FONT>PartyMessage eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee" + "</FONT>"));
            //for (int i = 0; i < 10; i++)
            //{
            //    ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.System, "System", "<FONT>SysMessage " + i + "</FONT>"));

            //}
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
            DragMove();
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
            var w = s.ActualWidth;
            Point r = s.TranslatePoint(new Point(0, 0), (UIElement)s.Parent);
            var sizeAn = new DoubleAnimation(w, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() };
            var posAn = new DoubleAnimation(r.X, TimeSpan.FromMilliseconds(250)) { EasingFunction = new QuadraticEase() };

            selectionRect.BeginAnimation(WidthProperty, sizeAn);
            selectionRect.RenderTransform.BeginAnimation(TranslateTransform.XProperty, posAn);

            var t = tabControl.SelectedContent as ItemsControl;
            var b = (Border)VisualTreeHelper.GetChild(t, 0);
            var sw = (ScrollViewer)VisualTreeHelper.GetChild(b, 0);

            sw.ScrollToBottom();


        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var t = tabControl.SelectedContent as ItemsControl;
            var sp = GetInnerStackPanel(t);
            if (sp.Children.Count > 2)
            {
                for (int i = 0; i < sp.Children.Count; i++)
                {
                    var el = (sp.Children[i] as FrameworkElement);
                    if(i <= 2)
                    {
                        if (el != null && el.ActualHeight > 22)
                        {
                            // set some property in (ChatMessage)DataContext to indicate that the message is collapsed
                            //el.Height = 22;
                        }
                    }
                    else
                    {
                        el.Height = Double.NaN;
                    }
                }

            }
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
    }

}
namespace TCC
{
    public static class ChatWindowBrushes
    {
        public static SolidColorBrush Say = new SolidColorBrush(Colors.White);
        public static SolidColorBrush Party = new SolidColorBrush(Color.FromRgb(0, 113, 187));
        public static SolidColorBrush Guild = new SolidColorBrush(Color.FromRgb(64, 251, 64));
        public static SolidColorBrush Area = new SolidColorBrush(Color.FromRgb(186, 130, 242));
        public static SolidColorBrush Trade = new SolidColorBrush(Color.FromRgb(192, 122, 129));
        public static SolidColorBrush Greet = new SolidColorBrush(Color.FromRgb(220, 220, 220));
        public static SolidColorBrush PartyNotice = new SolidColorBrush(Color.FromRgb(142, 255, 255));
        public static SolidColorBrush RaidNotice = new SolidColorBrush(Color.FromRgb(242, 135, 48));
        public static SolidColorBrush Emote = new SolidColorBrush(Color.FromRgb(255, 185, 185));
        public static SolidColorBrush Global = new SolidColorBrush(Color.FromRgb(240, 255, 137));
        public static SolidColorBrush Raid = new SolidColorBrush(Color.FromRgb(255, 255, 0));
        public static SolidColorBrush Megaphone = new SolidColorBrush(Color.FromRgb(0, 216, 255));
        public static SolidColorBrush GuildAd = new SolidColorBrush(Color.FromRgb(112, 196, 1));
        public static SolidColorBrush Private = new SolidColorBrush(Color.FromRgb(255, 95, 56));
        public static SolidColorBrush Whisper = new SolidColorBrush(Color.FromRgb(244, 121, 244));
        public static SolidColorBrush SystemGeneric = new SolidColorBrush(Color.FromRgb(0xc8, 0xc8, 0xb6));
        public static SolidColorBrush SystemNotify = new SolidColorBrush(Color.FromRgb(0xc8, 0xc8, 0xb6));
        public static SolidColorBrush SystemEvent = new SolidColorBrush(Color.FromRgb(0xc8, 0xc8, 0xb6));
        public static SolidColorBrush SystemError = new SolidColorBrush(Colors.Red);
        public static SolidColorBrush SystemGroup = new SolidColorBrush(Color.FromRgb(0xd9, 0xd9, 0xd9));
        public static SolidColorBrush Deathmatch = new SolidColorBrush(Color.FromRgb(0xd9, 0xd9, 0xd9));
        public static SolidColorBrush ContractAlert = new SolidColorBrush(Color.FromRgb(0xd9, 0xd9, 0xd9));
        public static SolidColorBrush GroupAlert = new SolidColorBrush(Color.FromRgb(0xd9, 0xd9, 0xd9));
        public static SolidColorBrush Loot = new SolidColorBrush(Color.FromRgb(0xd9, 0xd9, 0xd9));
        public static SolidColorBrush Exp = new SolidColorBrush(Color.FromRgb(0xd9, 0xd9, 0xd9));
        public static SolidColorBrush Money = new SolidColorBrush(Color.FromRgb(0xd9, 0xd9, 0xd9));
    }
}
