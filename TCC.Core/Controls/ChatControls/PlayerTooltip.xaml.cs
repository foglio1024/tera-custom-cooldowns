using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TCC.Converters;
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
            KickRipple.Opacity = 0;
            UnfriendRipple.Opacity = 0;
            BlockRipple.Opacity = 0;
            _kicking = false;
            _blocking = false;
            _unfriending = false;
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

        private bool _unfriending;

        private void AddFriendClick(object sender, RoutedEventArgs e)
        {
            if (ChatWindowManager.Instance.TooltipInfo.IsFriend)
            {
                if (_unfriending)
                {
                    Proxy.UnfriendUser(ChatWindowManager.Instance.TooltipInfo.Name);
                    ChatWindowManager.Instance.CloseTooltip();
                    UnfriendRipple.Opacity = 0;
                    _unfriending = false;
                }
                else
                {
                    ExpandRedRipple(UnfriendRipple, FriendGrid);
                    _unfriending = true;
                }
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
                if (_blocking)
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

                    ChatWindowManager.Instance.CloseTooltip();
                    BlockRipple.Opacity = 0;
                    _blocking = false;
                }
                else
                {

                    ExpandRedRipple(BlockRipple, BlockGrid);
                    _blocking = true;
                }
            }
            else
            {
                Proxy.UnblockUser(ChatWindowManager.Instance.TooltipInfo.Name);
                ChatWindowManager.Instance.BlockedUsers.Remove(ChatWindowManager.Instance.TooltipInfo.Name);
                ChatWindowManager.Instance.CloseTooltip();

            }
            ChatWindowManager.Instance.TooltipInfo.Refresh();
        }

        private bool _blocking;


        private void WhisperClick(object sender, RoutedEventArgs e)
        {
            ChatWindowManager.Instance.CloseTooltip();
            if (SettingsManager.ChatEnabled && !SessionManager.InGameChatOpen) FocusManager.SendNewLine();

            FocusManager.SendString("/w " + ChatWindowManager.Instance.TooltipInfo.Name + " ");
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
                KickRipple.Opacity = 0;
                _kicking = false;
                if (GroupWindowViewModel.Instance.TryGetUser(ChatWindowManager.Instance.TooltipInfo.Name, out var u))
                {
                    Proxy.KickMember(u.ServerId, u.PlayerId);
                }
            }
            else
            {
                KickText.Text = "Are you sure?";
                ExpandRedRipple(KickRipple, KickGrid);
                _kicking = true;
            }
        }

        private void ExpandRedRipple(Ellipse ripple, Grid container)
        {
            var scaleTrans = (ripple.RenderTransform as TransformGroup)?.Children[0];
            ((TransformGroup)ripple.RenderTransform).Children[1] = new TranslateTransform(Mouse.GetPosition(container).X - ripple.Width / 2,
                Mouse.GetPosition(container).Y - ripple.Height / 2);
            ripple.Opacity = 1;
            scaleTrans?.BeginAnimation(ScaleTransform.ScaleYProperty, _rippleScale);
            scaleTrans?.BeginAnimation(ScaleTransform.ScaleXProperty, _rippleScale);

        }
        private void MoongourdClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var p = (MgPopup.Child as MoongourdPopup);
            p.SetInfo(ChatWindowManager.Instance.TooltipInfo.Name, SettingsManager.LastRegion);
            MgPopup.IsOpen = true;
        }

        public void SetMoongourdVisibility()
        {
            Dispatcher.Invoke(() =>
            {
                if (SettingsManager.LastRegion != "NA" &&
                    SettingsManager.LastRegion != "RU" &&
                    !SettingsManager.LastRegion.StartsWith("EU")) MgButton.Visibility = Visibility.Collapsed;
            });

        }
    }
}
