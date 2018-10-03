using System.Windows;
using System.Windows.Controls;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Logica di interazione per FriendMessageDialog.xaml
    /// </summary>
    public partial class FriendMessageDialog
    {
        public FriendMessageDialog()
        {
            InitializeComponent();
            TargetName = WindowManager.FloatingButton.TooltipInfo.Name;
        }

        private string _message = "Friend me?";
        public string TargetName { get; set; }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _message = ((TextBox)sender).Text;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            _message = "Friend me?";
            //ChatWindowManager.Instance.LockTooltip();
            Close();
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            Proxy.FriendRequest(WindowManager.FloatingButton.TooltipInfo.Name, _message);
            //ChatWindowManager.Instance.LockTooltip();
            Close();
            _message = "Friend me?";
            WindowManager.FloatingButton.ClosePlayerMenu();
        }
    }
}
