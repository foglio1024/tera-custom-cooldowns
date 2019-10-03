using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Controls;
using TCC.Data.Pc;
using TCC.ViewModels.Widgets;

namespace TCC.Windows.Widgets
{
    //TODO: refactor more?
    public partial class CharacterWindow
    {
        private DoubleAnimation _hpAnim;
        private DoubleAnimation _mpAnim;
        private DoubleAnimation _stAnim;
        private DoubleAnimation _shAnim;

        private CharacterWindowViewModel _vm { get; }

        public CharacterWindow(CharacterWindowViewModel vm)
        {
            DataContext = vm;
            _vm = DataContext as CharacterWindowViewModel;

            InitializeComponent();
            ButtonsRef = Buttons;
            BoundaryRef = Boundary;
            MainContent = WindowContent;
            Init(App.Settings.CharacterWindowSettings); //TODO: us vm.Settings

            InitAnimations();

            _vm.Player.PropertyChanged += OnPropertyChanged;
        }

        private void InitAnimations()
        {
            var duration = TimeSpan.FromMilliseconds(200);
            var ease = R.MiscResources.QuadraticEase;
            _hpAnim = new DoubleAnimation
            {
                Duration = duration,
                EasingFunction = ease
            };
            _mpAnim = new DoubleAnimation
            {
                Duration = duration,
                EasingFunction = ease
            };
            _stAnim = new DoubleAnimation
            {
                Duration = duration,
                EasingFunction = ease
            };
            _shAnim = new DoubleAnimation
            {
                Duration = duration,
                EasingFunction = ease
            };

            Timeline.SetDesiredFrameRate(_hpAnim, 30);
            Timeline.SetDesiredFrameRate(_mpAnim, 30);
            Timeline.SetDesiredFrameRate(_stAnim, 30);
            Timeline.SetDesiredFrameRate(_shAnim, 30);


        }
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Action action;
            switch (e.PropertyName)
            {
                case nameof(Player.HpFactor): action = ChangeHP; break;
                case nameof(Player.MpFactor): action = ChangeMP; break;
                case nameof(Player.StFactor): action = ChangeStamina; break;
                case nameof(Player.ShieldFactor): action = ChangeShield; break;
                default: return;
            }
            Dispatcher.BeginInvoke(action, DispatcherPriority.DataBind);
        }

        private void ChangeHP()
        {
            _hpAnim.From = (HpGovernor.LayoutTransform as ScaleTransform)?.ScaleX;
            _hpAnim.To = _vm.Player.HpFactor;

            if (_hpAnim.From > _hpAnim.To)
            {
                //taking damage
                HpGovernor.LayoutTransform = new ScaleTransform(_vm.Player.HpFactor, 1);
                HpGovernorWhite.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hpAnim);
            }
            else
            {
                //healing
                HpGovernorWhite.LayoutTransform = new ScaleTransform(_vm.Player.HpFactor, 1);
                HpGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hpAnim);
            }
        }
        private void ChangeMP()
        {
            _mpAnim.From = ((ScaleTransform)MpGovernor.LayoutTransform).ScaleX;
            _mpAnim.To = _vm.Player.MpFactor;
            if (_mpAnim.From > _mpAnim.To)
            {
                //taking damage
                MpGovernor.LayoutTransform = new ScaleTransform(_vm.Player.MpFactor, 1);
                MpGovernorWhite.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _mpAnim);
            }
            else
            {
                //healing
                MpGovernorWhite.LayoutTransform = new ScaleTransform(_vm.Player.MpFactor, 1);
                MpGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _mpAnim);
            }
        }
        private void ChangeStamina()
        {
            _stAnim.From = ((ScaleTransform)StGovernor.LayoutTransform).ScaleX;
            _stAnim.To = _vm.Player.StFactor;
            if (_stAnim.From > _stAnim.To)
            {
                //taking damage
                StGovernor.LayoutTransform = new ScaleTransform(_vm.Player.StFactor, 1);
                StGovernorWhite.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _stAnim);
            }
            else
            {
                //healing
                StGovernorWhite.LayoutTransform = new ScaleTransform(_vm.Player.StFactor, 1);
                StGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _stAnim);
            }
            ReArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(_vm.Player.StFactor * (360 - 2 * ReArc.StartAngle) + ReArc.StartAngle, _stAnim.Duration));

        }
        private void ChangeShield()
        {
            _shAnim.From = (ShGovernor.LayoutTransform as ScaleTransform)?.ScaleX;
            _shAnim.To = _vm.Player.ShieldFactor;
            ShGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _shAnim);
        }
    }
}