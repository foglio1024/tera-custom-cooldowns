using System;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Controls;
using TCC.Data.Pc;

namespace TCC.Windows.Widgets
{
    /// <summary>
    /// Logica di interazione per HPbar.xaml
    /// </summary>
    public partial class CharacterWindow
    {
        private DoubleAnimation _hp;
        private DoubleAnimation _mp;
        private DoubleAnimation _st;
        private TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(200);
        private QuadraticEase DefaultEasing = new QuadraticEase();
        public CharacterWindow()
        {
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(Settings.SettingsHolder.CharacterWindowSettings, ignoreSize: true, undimOnFlyingGuardian: false);
            _hp = new DoubleAnimation()
            {
                Duration = DefaultDuration,
                EasingFunction = DefaultEasing
            };
            _mp = new DoubleAnimation()
            {
                Duration = DefaultDuration,
                EasingFunction = DefaultEasing
            };
            _st = new DoubleAnimation()
            {
                Duration = DefaultDuration,
                EasingFunction = DefaultEasing
            };
            Timeline.SetDesiredFrameRate(_hp, 30);
            Timeline.SetDesiredFrameRate(_mp, 30);
            Timeline.SetDesiredFrameRate(_st, 30);
            //(DataContext as CharacterWindowViewModel).Player.PropertyChanged += Player_PropertyChanged;
            SessionManager.CurrentPlayer.PropertyChanged += Player_PropertyChanged;
        }
        //TODO: refactor
        private void Player_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName == nameof(Player.HpFactor))
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _hp.From = (HpGovernor.LayoutTransform as ScaleTransform)?.ScaleX;
                    _hp.To = (sender as Player)?.HpFactor;

                    if (_hp.From > _hp.To)
                    {
                        //taking damage

                        HpGovernor.LayoutTransform = new ScaleTransform(((Player)sender).HpFactor, 1);
                        HpGovernorWhite.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hp);
                    }
                    else
                    {
                        //healing
                        HpGovernorWhite.LayoutTransform = new ScaleTransform(((Player)sender).HpFactor, 1);
                        HpGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hp);
                    }
                }), DispatcherPriority.DataBind);
            }
            else if (e.PropertyName == nameof(Player.MpFactor))
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _mp.From = ((ScaleTransform)MpGovernor.LayoutTransform).ScaleX;
                    _mp.To = ((Player)sender).MpFactor;
                    if (_mp.From > _mp.To)
                    {
                        //taking damage
                        MpGovernor.LayoutTransform = new ScaleTransform(((Player)sender).MpFactor, 1);
                        MpGovernorWhite.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _mp);
                    }
                    else
                    {
                        //healing
                        MpGovernorWhite.LayoutTransform = new ScaleTransform(((Player)sender).MpFactor, 1);
                        MpGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _mp);
                    }
                }), DispatcherPriority.DataBind);
            }
            else if (e.PropertyName == nameof(Player.StFactor))
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _st.From = ((ScaleTransform)StGovernor.LayoutTransform).ScaleX;
                    _st.To = (sender as Player)?.StFactor;
                    if (_st.From > _st.To)
                    {
                        //taking damage
                        StGovernor.LayoutTransform = new ScaleTransform(((Player)sender).StFactor, 1);
                        StGovernorWhite.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _st);
                    }
                    else
                    {
                        //healing
                        StGovernorWhite.LayoutTransform = new ScaleTransform(((Player)sender).StFactor, 1);
                        StGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _st);
                    }
                    ReArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation((((Player)sender).StFactor) * (360 - 2 * ReArc.StartAngle) + ReArc.StartAngle, _st.Duration));
                }), DispatcherPriority.DataBind);
            }
            else if (e.PropertyName == nameof(Player.ShieldFactor))
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var sh = new DoubleAnimation
                    {
                        Duration = DefaultDuration,
                        EasingFunction = DefaultEasing,
                        From = (ShGovernor.LayoutTransform as ScaleTransform)?.ScaleX,
                        To = (sender as Player)?.ShieldFactor
                    };
                    ShGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, sh);
                }), DispatcherPriority.DataBind);
            }
        }
    }
}

