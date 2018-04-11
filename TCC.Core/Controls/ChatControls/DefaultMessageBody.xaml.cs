using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TCC.Data;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Interaction logic for DefaultMessageBody.xaml
    /// </summary>
    public partial class DefaultMessageBody : UserControl
    {
        public DefaultMessageBody()
        {
            InitializeComponent();
        }

        private void I_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(((ChatMessage)DataContext).ToString());
        }

        private void WrapPanel_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((FrameworkElement)sender).ContextMenu.IsOpen = true;
        }
    }
}
