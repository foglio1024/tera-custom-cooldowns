using System.Linq;
using System.Windows;
using System.Windows.Input;
using TCC.Data;
using TCC.ViewModels;

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

        private void WrapPanel_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var contextMenu = (sender as FrameworkElement)?.ContextMenu;
            if (contextMenu != null) contextMenu.IsOpen = true;
        }

        private void PinBtn_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var w in ChatWindowManager.Instance.ChatWindows)
            {
                if (!w.IsMouseOver) continue;
                var currTabVm = w.TabControl.SelectedItem;
                var tabVm = w.VM.TabVMs.FirstOrDefault(x =>
                    ((Tab)x.Content).Messages.Contains(this.DataContext as ChatMessage) && x == currTabVm);
                if (((Tab)tabVm.Content).PinnedMessage == this.DataContext)
                {
                    ((Tab)tabVm?.Content).PinnedMessage = null;
                }
                else ((Tab)tabVm?.Content).PinnedMessage = this.DataContext as ChatMessage;
            }
        }

        private void CopyBtn_OnClick(object sender, RoutedEventArgs e)
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
    }
}
