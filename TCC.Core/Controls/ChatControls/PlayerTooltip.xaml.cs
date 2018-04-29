using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Logica di interazione per PlayerTooltip.xaml
    /// </summary>
    public partial class PlayerTooltip
    {
        private readonly DoubleAnimation _expandAnim;
        private readonly DoubleAnimation _rippleScale;

        public PlayerTooltip()
        {
            InitializeComponent();
            _expandAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() };
            _rippleScale = new DoubleAnimation(1, 50, TimeSpan.FromMilliseconds(650)) { EasingFunction = new QuadraticEase() };
        }
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            KickText.Text = "Kick";
            Ripple.Opacity = 0;
            _kicking = false;
            ChatWindowManager.Instance.CloseTooltip();
        }

        public void AnimateOpening()
        {
            RootBorder.BeginAnimation(OpacityProperty, _expandAnim);
        }

        private void InspectClick(object sender, RoutedEventArgs e)
        {
            Proxy.Inspect(ChatWindowManager.Instance.TooltipInfo.Name);
            ChatWindowManager.Instance.CloseTooltip();
        }

        private void PartyInviteClick(object sender, RoutedEventArgs e)
        {
            Proxy.PartyInvite(ChatWindowManager.Instance.TooltipInfo.Name);
            ChatWindowManager.Instance.CloseTooltip();
        }

        private void GuildInviteClick(object sender, RoutedEventArgs e)
        {
            Proxy.GuildInvite(ChatWindowManager.Instance.TooltipInfo.Name);
            ChatWindowManager.Instance.CloseTooltip();
        }

        private void AddFriendClick(object sender, RoutedEventArgs e)
        {
            if (ChatWindowManager.Instance.TooltipInfo.IsFriend)
            {
                Proxy.UnfriendUser(ChatWindowManager.Instance.TooltipInfo.Name);
                ChatWindowManager.Instance.CloseTooltip();
            }
            else
            {
                var friendDg = new FriendMessageDialog();
                friendDg.Show();
            }
            ChatWindowManager.Instance.TooltipInfo.Refresh();
        }
        private void BlockClick(object sender, RoutedEventArgs e)
        {
            if (!ChatWindowManager.Instance.TooltipInfo.IsBlocked)
            {
                Proxy.BlockUser(ChatWindowManager.Instance.TooltipInfo.Name);
                ChatWindowManager.Instance.BlockedUsers.Add(ChatWindowManager.Instance.TooltipInfo.Name);
                try
                {
                    var i = ChatWindowManager.Instance.Friends.IndexOf(ChatWindowManager.Instance.Friends.FirstOrDefault(x => x.Name == ChatWindowManager.Instance.TooltipInfo.Name));
                    ChatWindowManager.Instance.Friends.RemoveAt(i);
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                Proxy.UnblockUser(ChatWindowManager.Instance.TooltipInfo.Name);
                ChatWindowManager.Instance.BlockedUsers.Remove(ChatWindowManager.Instance.TooltipInfo.Name);

            }
            ChatWindowManager.Instance.TooltipInfo.Refresh();
            ChatWindowManager.Instance.CloseTooltip();

        }

        private void SendString(string s)
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
            ChatWindowManager.Instance.CloseTooltip();
            SendString("/w " + ChatWindowManager.Instance.TooltipInfo.Name + " ");
        }

        private void GrantInviteClick(object sender, RoutedEventArgs e)
        {

            if (GroupWindowViewModel.Instance.TryGetUser(ChatWindowManager.Instance.TooltipInfo.Name, out var u))
            {
                Proxy.SetInvitePower(u.ServerId, u.PlayerId, !u.CanInvite);
                u.CanInvite = !u.CanInvite;
            }
            ChatWindowManager.Instance.CloseTooltip();
        }

        private void DelegateLeaderClick(object sender, RoutedEventArgs e)
        {
            if (GroupWindowViewModel.Instance.TryGetUser(ChatWindowManager.Instance.TooltipInfo.Name, out var u))
            {
                Proxy.DelegateLeader(u.ServerId, u.PlayerId);
            }
            ChatWindowManager.Instance.CloseTooltip();
        }

        private bool _kicking;
        private void KickClick(object sender, RoutedEventArgs e)
        {
            if (_kicking)
            {
                ChatWindowManager.Instance.CloseTooltip();
                KickText.Text = "Kick";
                Ripple.Opacity = 0;
                _kicking = false;
                if (GroupWindowViewModel.Instance.TryGetUser(ChatWindowManager.Instance.TooltipInfo.Name, out var u))
                {
                    Proxy.KickMember(u.ServerId, u.PlayerId);
                }
            }
            else
            {
                KickText.Text = "Are you sure?";
                var scaleTrans = (Ripple.RenderTransform as TransformGroup)?.Children[0];
                ((TransformGroup) Ripple.RenderTransform).Children[1] = new TranslateTransform(Mouse.GetPosition(KickGrid).X - Ripple.Width / 2,
                    Mouse.GetPosition(KickGrid).Y - Ripple.Height / 2);
                Ripple.Opacity = 1;
                scaleTrans?.BeginAnimation(ScaleTransform.ScaleYProperty, _rippleScale);
                scaleTrans?.BeginAnimation(ScaleTransform.ScaleXProperty, _rippleScale);
                _kicking = true;
            }
        }
    }
}
