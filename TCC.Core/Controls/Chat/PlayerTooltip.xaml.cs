using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TCC.Interop.Proxy;
using TCC.ViewModels;

namespace TCC.Controls.Chat
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
            WindowManager.FloatingButton.ClosePlayerMenu();
        }

        public void AnimateOpening()
        {
            RootBorder.BeginAnimation(OpacityProperty, _expandAnim);

        }

        private void InspectClick(object sender, RoutedEventArgs e)
        {
            ProxyInterface.Instance.Stub.InspectUser(WindowManager.FloatingButton.TooltipInfo.Name); //ProxyOld.Inspect(WindowManager.FloatingButton.TooltipInfo.Name);
            WindowManager.FloatingButton.ClosePlayerMenu();
        }

        private void PartyInviteClick(object sender, RoutedEventArgs e)
        {
            ProxyInterface.Instance.Stub.GroupInviteUser(WindowManager.FloatingButton.TooltipInfo.Name); //ProxyOld.PartyInvite(WindowManager.FloatingButton.TooltipInfo.Name);
            WindowManager.FloatingButton.ClosePlayerMenu();
        }

        private void GuildInviteClick(object sender, RoutedEventArgs e)
        {
            ProxyInterface.Instance.Stub.GuildInviteUser(WindowManager.FloatingButton.TooltipInfo.Name); //ProxyOld.GuildInvite(WindowManager.FloatingButton.TooltipInfo.Name);
            WindowManager.FloatingButton.ClosePlayerMenu();
        }

        private bool _unfriending;

        private void AddFriendClick(object sender, RoutedEventArgs e)
        {
            if (WindowManager.FloatingButton.TooltipInfo.IsFriend)
            {
                if (_unfriending)
                {
                    ProxyInterface.Instance.Stub.UnfriendUser(WindowManager.FloatingButton.TooltipInfo.Name);//ProxyOld.UnfriendUser(WindowManager.FloatingButton.TooltipInfo.Name);
                    WindowManager.FloatingButton.ClosePlayerMenu();
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
            WindowManager.FloatingButton.TooltipInfo.Refresh();
        }
        private void BlockClick(object sender, RoutedEventArgs e)
        {
            if (!WindowManager.FloatingButton.TooltipInfo.IsBlocked)
            {
                if (_blocking)
                {
                    ProxyInterface.Instance.Stub.BlockUser(WindowManager.FloatingButton.TooltipInfo.Name);//ProxyOld.BlockUser(WindowManager.FloatingButton.TooltipInfo.Name);
                    ChatWindowManager.Instance.BlockedUsers.Add(WindowManager.FloatingButton.TooltipInfo.Name);
                    try
                    {
                        var i = ChatWindowManager.Instance.Friends.IndexOf(ChatWindowManager.Instance.Friends.FirstOrDefault(x => x.Name == WindowManager.FloatingButton.TooltipInfo.Name));
                        ChatWindowManager.Instance.Friends.RemoveAt(i);
                    }
                    catch
                    {
                        // ignored
                    }

                    WindowManager.FloatingButton.ClosePlayerMenu();
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
                ProxyInterface.Instance.Stub.UnblockUser(WindowManager.FloatingButton.TooltipInfo.Name);//ProxyOld.UnblockUser(WindowManager.FloatingButton.TooltipInfo.Name);
                ChatWindowManager.Instance.BlockedUsers.Remove(WindowManager.FloatingButton.TooltipInfo.Name);
                WindowManager.FloatingButton.ClosePlayerMenu();

            }
            WindowManager.FloatingButton.TooltipInfo.Refresh();
        }

        private bool _blocking;


        private void WhisperClick(object sender, RoutedEventArgs e)
        {
            WindowManager.FloatingButton.ClosePlayerMenu();
            if (/*Settings.ChatEnabled &&*/ !SessionManager.InGameChatOpen) FocusManager.SendNewLine();

            FocusManager.SendString("/w " + WindowManager.FloatingButton.TooltipInfo.Name + " ");
        }

        private void GrantInviteClick(object sender, RoutedEventArgs e)
        {

            if (WindowManager.ViewModels.Group.TryGetUser(WindowManager.FloatingButton.TooltipInfo.Name, out var u))
            {
                ProxyInterface.Instance.Stub.SetInvitePower(u.ServerId, u.PlayerId, !u.CanInvite); //ProxyOld.SetInvitePower(u.ServerId, u.PlayerId, !u.CanInvite);
                u.CanInvite = !u.CanInvite;
            }
            WindowManager.FloatingButton.ClosePlayerMenu();
        }

        private void DelegateLeaderClick(object sender, RoutedEventArgs e)
        {
            if (WindowManager.ViewModels.Group.TryGetUser(WindowManager.FloatingButton.TooltipInfo.Name, out var u))
            {
                ProxyInterface.Instance.Stub.DelegateLeader(u.ServerId, u.PlayerId);//ProxyOld.DelegateLeader(u.ServerId, u.PlayerId);
            }
            WindowManager.FloatingButton.ClosePlayerMenu();
        }

        private bool _kicking;
        private void KickClick(object sender, RoutedEventArgs e)
        {
            if (_kicking)
            {
                WindowManager.FloatingButton.ClosePlayerMenu();
                KickText.Text = "Kick";
                KickRipple.Opacity = 0;
                _kicking = false;
                if (WindowManager.ViewModels.Group.TryGetUser(WindowManager.FloatingButton.TooltipInfo.Name, out var u))
                {
                    ProxyInterface.Instance.Stub.KickUser(u.ServerId, u.PlayerId); //ProxyOld.KickMember(u.ServerId, u.PlayerId);
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
            var p = MgPopup.Child as MoongourdPopup;
            p?.SetInfo(WindowManager.FloatingButton.TooltipInfo.Name, App.Settings.LastLanguage);
            MgPopup.IsOpen = true;
        }
        private void FpsUtilsClick(object sender, RoutedEventArgs routedEventArgs)
        {
            FpsUtilsPopup.IsOpen = true;
        }

        public void SetMoongourdVisibility()
        {
            Dispatcher.Invoke(() =>
            {
                if (App.Settings.LastLanguage != "NA" &&
                    App.Settings.LastLanguage != "RU" &&
                    !App.Settings.LastLanguage.StartsWith("EU")) MgButton.Visibility = Visibility.Collapsed;
            });

        }

        private void FpsUtilsHideClick(object sender, RoutedEventArgs e)
        {
            if (!ProxyInterface.Instance.IsStubAvailable || !ProxyInterface.Instance.IsFpsUtilsAvailable) return;
            ProxyInterface.Instance.Stub.InvokeCommand($"fps hide {WindowManager.FloatingButton.TooltipInfo.Name}"); //ProxyOld.SendCommand($"fps hide {WindowManager.FloatingButton.TooltipInfo.Name}");
        }

        private void FpsUtilsShowClick(object sender, RoutedEventArgs e)
        {
            if (!ProxyInterface.Instance.IsStubAvailable || !ProxyInterface.Instance.IsFpsUtilsAvailable) return;
            ProxyInterface.Instance.Stub.InvokeCommand($"fps show {WindowManager.FloatingButton.TooltipInfo.Name}"); //ProxyOld.SendCommand($"fps show {WindowManager.FloatingButton.TooltipInfo.Name}");
        }
    }
}
