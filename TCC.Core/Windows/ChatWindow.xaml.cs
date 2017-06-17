using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : TccWindow
    {
        public ChatWindow()
        {
            InitializeComponent();
        }
        ScrollViewer _scroller;
        bool _bottom = true;
        DispatcherTimer testTimer;
        private void TccWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //InitWindow(SettingsManager.ChatWindowSettings);
            itemsControl.ItemsSource = ChatWindowViewModel.Instance.ChatMessages;
            ChatWindowViewModel.Instance.PropertyChanged += Instance_PropertyChanged;
            _scroller = (ScrollViewer)VisualTreeHelper.GetChild(itemsControl, 0);
            _scroller.ScrollChanged += Scroller_ScrollChanged;
            testTimer = new DispatcherTimer();
            testTimer.Interval = TimeSpan.FromMilliseconds(1000);
            testTimer.Tick += (s, ev) =>
            {
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Say, "Author", "<FONT>SayMessage " + i + "</FONT>"));
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.ReceivedWhisper, "Author", "<FONT>ReceivedWhisperLooooooooooooooooooo ooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo oooooooooooongMessage " + i + "</FONT>"));
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(ChatChannel.Megaphone, "Author", "<FONT>SentWhisperMessage " + i + "</FONT>"));
                i++;
            };
            //testTimer.Start();

        }
        int i = 0;
        private void Scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if(_scroller.VerticalOffset == _scroller.ScrollableHeight)
            {
                _bottom = true;
            }
            else
            {
                _bottom = false;
            }
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "NewItem")
            {
                if(_bottom) _scroller.ScrollToBottom();
            }
        }

        private void TccWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void SWPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer s = (ScrollViewer)sender;
            s.ScrollToVerticalOffset(s.VerticalOffset - e.Delta/2);
        }
    }
    public class ChannelLabelDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalChannelDataTemplate { get; set; }
        public DataTemplate WhisperChannelDataTemplate { get; set; }
        public DataTemplate MegaphoneChannelDataTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ChatMessage m = item as ChatMessage;
            switch (m.Channel)
            {
                case ChatChannel.SentWhisper:
                    return WhisperChannelDataTemplate;
                case ChatChannel.ReceivedWhisper:
                    return WhisperChannelDataTemplate;
                case ChatChannel.Megaphone:
                    return MegaphoneChannelDataTemplate;
                default:
                    return NormalChannelDataTemplate;
            }
        }
    }

    #region Converters

    public class ChatColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ch = (ChatChannel)value;
            Color col;
            switch (ch)
            {
                case ChatChannel.Say:
                    col = Colors.White;
                    break;
                case ChatChannel.Party:
                    col = Color.FromRgb(0, 113, 187);
                    break;
                case ChatChannel.Guild:
                    col = Color.FromRgb(64, 251, 64);
                    break;
                case ChatChannel.Area:
                    col = Color.FromRgb(186, 130, 242);
                    break;
                case ChatChannel.Trade:
                    col = Color.FromRgb(192, 122, 129);
                    break;
                case ChatChannel.Greet:
                    col = Color.FromRgb(220, 220, 220);
                    break;
                case ChatChannel.PartyNotice:
                    col = Color.FromRgb(142, 255, 255);
                    break;
                case ChatChannel.RaidNotice:
                    col = Color.FromRgb(242, 135, 48);
                    break;
                case ChatChannel.Emote:
                    col = Color.FromRgb(255, 185, 185);
                    break;
                case ChatChannel.Global:
                    col = Color.FromRgb(240, 255, 137);
                    break;
                case ChatChannel.Raid:
                    col = Color.FromRgb(255, 255, 0);
                    break;
                case ChatChannel.Megaphone:
                    col = Color.FromRgb(0, 216, 255);
                    break;
                case ChatChannel.GuildAdvertising:
                    col = Color.FromRgb(112, 196, 1);
                    break;
                case ChatChannel.Private1:
                    col = Color.FromRgb(255, 95, 56);
                    break;
                case ChatChannel.Private2:
                    col = Color.FromRgb(255, 95, 56);
                    break;
                case ChatChannel.Private3:
                    col = Color.FromRgb(255, 95, 56);
                    break;
                case ChatChannel.Private4:
                    col = Color.FromRgb(255, 95, 56);
                    break;
                case ChatChannel.Private5:
                    col = Color.FromRgb(255, 95, 56);
                    break;
                case ChatChannel.Private6:
                    col = Color.FromRgb(255, 95, 56);
                    break;
                case ChatChannel.Private7:
                    col = Color.FromRgb(255, 95, 56);
                    break;
                case ChatChannel.Private8:
                    col = Color.FromRgb(255, 95, 56);
                    break;
                case ChatChannel.SentWhisper:
                    col = Color.FromRgb(244, 121, 244);
                    break;
                case ChatChannel.ReceivedWhisper:
                    col = Color.FromRgb(244, 121, 244);
                    break;
                default:
                    col = Color.FromRgb(220, 220, 220);
                    break;
            }
            return new SolidColorBrush(col);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ChatChannelNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ch = (ChatChannel)value;
            switch (ch)
            {
                case ChatChannel.PartyNotice:
                    return "Notice";
                case ChatChannel.RaidNotice:
                    return "Notice";
                case ChatChannel.GuildAdvertising:
                    return "G. Ad";
                case ChatChannel.Megaphone:
                    return "Shout";
                case ChatChannel.Private1:
                    return ChatMessage.PrivateChannels[0].Name;
                case ChatChannel.Private2:
                    return ChatMessage.PrivateChannels[1].Name;
                case ChatChannel.Private3:
                    return ChatMessage.PrivateChannels[2].Name;
                case ChatChannel.Private4:
                    return ChatMessage.PrivateChannels[3].Name;
                case ChatChannel.Private5:
                    return ChatMessage.PrivateChannels[4].Name;
                case ChatChannel.Private6:
                    return ChatMessage.PrivateChannels[5].Name;
                case ChatChannel.Private7:
                    return ChatMessage.PrivateChannels[6].Name;
                case ChatChannel.Private8:
                    return ChatMessage.PrivateChannels[7].Name;
                case ChatChannel.ReceivedWhisper:
                    return "W <";
                case ChatChannel.SentWhisper:
                    return "W >";
                default:
                    return ch.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MentionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Brushes.Orange; 
            }
            else
            {
                return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class SentReceivedToAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (ChatChannel)value;

            switch (c)
            {
                case ChatChannel.SentWhisper:
                    return 1;
                case ChatChannel.ReceivedWhisper:
                    return -1;
                default:
                    return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
