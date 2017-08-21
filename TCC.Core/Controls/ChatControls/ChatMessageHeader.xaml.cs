using System.Windows.Controls;
using System.Windows.Input;
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
            if (context.Author == "System") return;
            WindowManager.ChatWindow.CurrentSender = sender;
            ProxyInterop.SendAskInteractiveMessage(PacketProcessor.ServerId, context.Author);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowManager.ChatWindow.CloseTooltip();
        }
    }
}
