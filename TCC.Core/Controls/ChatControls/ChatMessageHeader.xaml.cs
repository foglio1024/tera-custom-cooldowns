using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            WindowManager.ChatWindow.CurrentSender = sender;
            ProxyInterop.SendAskInteractiveMessage(PacketProcessor.ServerId, context.Author);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowManager.ChatWindow.CloseTooltip();
        }
    }
}
