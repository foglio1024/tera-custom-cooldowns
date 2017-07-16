using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCC.Data;
using TCC.ViewModels;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Logica di interazione per PlayerTooltip.xaml
    /// </summary>
    public partial class PlayerTooltip : UserControl
    {
        public PlayerTooltip()
        {
            InitializeComponent();
            expandAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() };
        }
        DoubleAnimation expandAnim;
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            WindowManager.ChatWindow.CloseTooltip();
        }

        public void AnimateOpening()
        {
            rootBorder.BeginAnimation(OpacityProperty, expandAnim);
        }

        private void InspectClick(object sender, RoutedEventArgs e)
        {
            ProxyInterop.SendInspect(ChatWindowViewModel.Instance.TooltipInfo.Name);
            WindowManager.ChatWindow.CloseTooltip();
        }

        private void PartyInviteClick(object sender, RoutedEventArgs e)
        {
            ProxyInterop.SendPartyInvite(ChatWindowViewModel.Instance.TooltipInfo.Name);
            WindowManager.ChatWindow.CloseTooltip();
        }

        private void GuildInviteClick(object sender, RoutedEventArgs e)
        {
            ProxyInterop.SendGuildInvite(ChatWindowViewModel.Instance.TooltipInfo.Name);
            WindowManager.ChatWindow.CloseTooltip();
        }

        private void AddFriendClick(object sender, RoutedEventArgs e)
        {
            if (ChatWindowViewModel.Instance.TooltipInfo.IsFriend)
            {
                ProxyInterop.SendUnfriendUserMessage(ChatWindowViewModel.Instance.TooltipInfo.Name);
                WindowManager.ChatWindow.CloseTooltip();
            }
            else
            {
                var friendDg = new FriendMessageDialog();
                friendDg.Show();
            }
            ChatWindowViewModel.Instance.TooltipInfo.Refresh();
        }
        private void BlockClick(object sender, RoutedEventArgs e)
        {
            if (!ChatWindowViewModel.Instance.TooltipInfo.IsBlocked)
            {
                ProxyInterop.SendBlockUserMessage(ChatWindowViewModel.Instance.TooltipInfo.Name);
                ChatWindowViewModel.Instance.BlockedUsers.Add(ChatWindowViewModel.Instance.TooltipInfo.Name);
                try
                {
                    ChatWindowViewModel.Instance.Friends.Remove(ChatWindowViewModel.Instance.TooltipInfo.Name);
                } catch (Exception) { }
            }
            else
            {
                ProxyInterop.SendUnblockUserMessage(ChatWindowViewModel.Instance.TooltipInfo.Name);
                ChatWindowViewModel.Instance.BlockedUsers.Remove(ChatWindowViewModel.Instance.TooltipInfo.Name);

            }
            ChatWindowViewModel.Instance.TooltipInfo.Refresh();
            WindowManager.ChatWindow.CloseTooltip();

        }
        void SendString(string s)
        {
            var teraWindow = FocusManager.FindTeraWindow();
            if (teraWindow == IntPtr.Zero) { return; }

            PasteString(teraWindow, s);

        }
        private static void PasteString(IntPtr hWnd, string s)
        {
            Thread.Sleep(100);
            foreach (var character in s)
            {
                if (!FocusManager.PostMessage(hWnd, FocusManager.WM_CHAR, character, 0)) { throw new Win32Exception(); }
                Thread.Sleep(1);
            }
        }

        private void WhisperClick(object sender, RoutedEventArgs e)
        {
            WindowManager.ChatWindow.CloseTooltip();
            SendString("/w " + ChatWindowViewModel.Instance.TooltipInfo.Name + " ");
        }

        private void GrantInviteClick(object sender, RoutedEventArgs e)
        {
            
            GroupWindowViewModel.Instance.GetUser(ChatWindowViewModel.Instance.TooltipInfo.Name, out User u);
            if (u != null) { ProxyInterop.SendGrantRevokeInvite(u.ServerId, u.PlayerId, !u.CanInvite); u.CanInvite = !u.CanInvite; }
            WindowManager.ChatWindow.CloseTooltip();
        }

        private void DelegateLeaderClick(object sender, RoutedEventArgs e)
        {
            GroupWindowViewModel.Instance.GetUser(ChatWindowViewModel.Instance.TooltipInfo.Name, out User u);
            ProxyInterop.SendDelegateLeader(u.ServerId, u.PlayerId);
            WindowManager.ChatWindow.CloseTooltip();
        }
    }
}
