using System.Windows;
using System.Windows.Controls;
using TCC.ViewModels;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Logica di interazione per FriendMessageDialog.xaml
    /// </summary>
    public partial class FriendMessageDialog : Window
    {
        public FriendMessageDialog()
        {
            InitializeComponent();
            TargetName = ChatWindowViewModel.Instance.TooltipInfo.Name;
        }
        string _message = "Friend me?";
        public string TargetName { get; set; }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _message = ((TextBox)sender).Text;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            _message = "Friend me?";
            WindowManager.ChatWindow.LockTooltip(false);
            Close();
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            ProxyInterop.SendFriendRequestMessage(ChatWindowViewModel.Instance.TooltipInfo.Name, _message);
            WindowManager.ChatWindow.LockTooltip(false);
            Close();
            _message = "Friend me?";
            WindowManager.ChatWindow.CloseTooltip();
        }
    }
}
