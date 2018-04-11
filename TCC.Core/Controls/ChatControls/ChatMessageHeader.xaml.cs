using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using TCC.Data;
using TCC.Parsing;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per ChatMessageHeader.xaml
    /// </summary>
    public partial class ChatMessageHeader : UserControl
    {
        public ChatMessageHeader()
        {
            InitializeComponent();
        }

        private void OutlinedTextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var context = (ChatMessage)DataContext;
            if (context.Author == "System" || context.Channel == ChatChannel.Twitch) return;
            WindowManager.ChatWindow.CurrentSender = sender;
            Proxy.AskInteractive(PacketProcessor.ServerId, context.Author);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowManager.ChatWindow.CloseTooltip();
        }

        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var s = sender as ContentControl;
            var eff = s.Effect as DropShadowEffect;
            eff.Opacity = .7;

        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var s = sender as ContentControl;
            var eff = s.Effect as DropShadowEffect;
            eff.Opacity = 0;

        }
    }
}
