using System.Windows;
using System.Windows.Input;
using TCC.Data;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Interaction logic for DefaultMessageBody.xaml
    /// </summary>
    public partial class DefaultMessageBody
    {
        public DefaultMessageBody()
        {
            InitializeComponent();
        }

        private void I_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(((ChatMessage)DataContext).ToString());
            }
            catch
            {
                // ignored
            }
        }

        private void WrapPanel_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var contextMenu = (sender as FrameworkElement)?.ContextMenu;
            if (contextMenu != null) contextMenu.IsOpen = true;
        }
    }
}
