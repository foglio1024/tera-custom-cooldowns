using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Data.NPCs;
using TCC.ViewModels;

namespace TCC.Controls.NPCs
{
    public partial class BossGage
    {
        private bool _firstLoad = true;

        public BossViewModel VM { get; set; }

        private readonly DoubleAnimation _enrageArcAnimation;
        private readonly DoubleAnimation _hpAnim;
        private readonly DoubleAnimation _flash;
        private readonly DoubleAnimation _slideAnim;
        private readonly DoubleAnimation _timerAnim;
        private readonly DoubleAnimation _fadeAnim;

        public BossGage()
        {

            InitializeComponent();

            DataContextChanged += (_, e) =>
            {
                if (e.NewValue is NPC npc) VM = new BossViewModel(npc);
            };

            _slideAnim = new DoubleAnimation
            {
                EasingFunction = R.MiscResources.QuadraticEase,
                Duration = TimeSpan.FromMilliseconds(250)
            };
            Timeline.SetDesiredFrameRate(_slideAnim, 60);

            _hpAnim = new DoubleAnimation(1, TimeSpan.FromMilliseconds(150))
            {
                EasingFunction = R.MiscResources.QuadraticEase,
            };
            Timeline.SetDesiredFrameRate(_hpAnim, 60);

            _flash = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(1000))
            {
                EasingFunction = R.MiscResources.QuadraticEase,
            };
            Timeline.SetDesiredFrameRate(_flash, 30);

            _timerAnim = new DoubleAnimation { To = 0 };
            Timeline.SetDesiredFrameRate(_timerAnim, 20);

            _enrageArcAnimation = new DoubleAnimation { From = 1, To = 0 };
            _enrageArcAnimation.Completed += _enrageArcAnimation_Completed;
            Timeline.SetDesiredFrameRate(_enrageArcAnimation, 30);

            _fadeAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(250));
            Timeline.SetDesiredFrameRate(_fadeAnim, 30);

            SettingsWindowViewModel.AbnormalityShapeChanged += RefreshAbnormalityTemplate;


        }

        private void RefreshAbnormalityTemplate()
        {
            Abnormalities.ItemTemplateSelector = null;
            Abnormalities.ItemTemplateSelector = R.TemplateSelectors.BossAbnormalityTemplateSelector;
        }

        private void AnimateTimer()
        {
            Dispatcher.Invoke(() =>
            {
                _timerAnim.From = VM.NPC.TimerPattern is HpTriggeredTimerPattern hptp ? hptp.StartAt : 1;
                _timerAnim.Duration = TimeSpan.FromSeconds(VM.NPC.TimerPattern.Duration);
                TimerDotPusher.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _timerAnim);
                TimerBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _timerAnim);
            });
        }
        private void OnHpChanged()
        {
            //_doubleAnim.To = DC.Boss.HPFactor;
            AnimateHp();
            if (VM.NPC.Enraged) SlideEnrageIndicator(VM.CurrentPercentage);
        }
        private void OnEnragedChanged()
        {
            if (VM.NPC.Enraged)
            {
                SlideEnrageIndicator(VM.CurrentPercentage);
                //EnrageBorder.BeginAnimation(OpacityProperty, _flash);
                if (!VM.NPC.EnragePattern.StaysEnraged)
               {
                    _enrageArcAnimation.Duration = TimeSpan.FromSeconds(VM.NPC.EnragePattern.Duration);
                    EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _enrageArcAnimation);
                }
            }
            else
            {
                SlideEnrageIndicator(VM.NextEnragePercentage);
                EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                ((ScaleTransform)EnrageBar.RenderTransform).ScaleX = 0;

            }

        }

        private void AnimateHp()
        {
            if (VM.NPC == null) return; //weird but could happen 
            _hpAnim.To = VM.NPC.HPFactor; //still crashing here ffs
            DotPusher.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hpAnim);
            HpBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hpAnim);
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!_firstLoad) return;
            _firstLoad = false;
            VM.HpFactorChanged += OnHpChanged;
            VM.EnragedChanged += OnEnragedChanged;
            VM.Disposed += OnDispose;
            if (VM.NPC.TimerPattern != null) VM.NPC.TimerPattern.Started += AnimateTimer;

            NextEnrage.RenderTransform = new TranslateTransform(HpBarGrid.Width, 0);

            SlideEnrageIndicator(VM.NextEnragePercentage);


        }

        private void OnDispose()
        {
            SettingsWindowViewModel.AbnormalityShapeChanged -= RefreshAbnormalityTemplate;
            VM.HpFactorChanged -= OnHpChanged;
            VM.EnragedChanged -= OnEnragedChanged;

            AnimateFadeOut();
        }

        private void AnimateFadeOut()
        {
            _fadeAnim.Duration = TimeSpan.FromMilliseconds(500);
            //LayoutTransform = new ScaleTransform { ScaleY = 1 };
            //LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _fadeAnim);
            //this.BeginAnimation(OpacityProperty, _fadeAnim);            
            //BossNameGrid.BeginAnimation(OpacityProperty, _fadeAnim);
            //HpBarGrid.BeginAnimation(OpacityProperty, _fadeAnim);
            //TopInfoGrid.BeginAnimation(OpacityProperty, _fadeAnim);
            BeginAnimation(OpacityProperty, _fadeAnim);
        }

        private void _enrageArcAnimation_Completed(object sender, EventArgs e)
        {
            EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            try
            {
                ((ScaleTransform)EnrageBar.RenderTransform).ScaleX = VM.NPC.Enraged ? 1 : 0;
            }
            catch
            {
                // ignored
            }
        }

        private void SlideEnrageIndicator(double val)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _slideAnim.To = val < 0 ? 0 : HpBarGrid.ActualWidth * (val / 100);
                NextEnrage.RenderTransform.BeginAnimation(TranslateTransform.XProperty, _slideAnim);
            }));
        }

    }
}