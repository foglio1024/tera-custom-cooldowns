using System;
using System.Windows.Controls;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per ChatMessageControl.xaml
    /// </summary>
    public partial class ChatMessageControl : UserControl
    {
        public ChatMessageControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (((ChatMessage)DataContext).IsContracted)
            {
                popup.IsOpen = true;
            }
        }

        private void popup_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            popup.IsOpen = false;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ((ChatMessage)DataContext).Rows = WindowManager.ChatWindow.GetMessageRows(this.ActualHeight);
            }), DispatcherPriority.Loaded);
        }
    }
}
