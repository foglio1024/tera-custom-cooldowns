using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FoglioUtils;
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
        //private readonly DoubleAnimation _flash;
        private readonly DoubleAnimation _slideAnim;
        private readonly DoubleAnimation _timerAnim;
        private readonly DoubleAnimation _fadeAnim;

        public BossGage()
        {

            InitializeComponent();

            DataContextChanged += OnDataContextChanged;

            _slideAnim= AnimationFactory.CreateDoubleAnimation(250, 1, easing: true);
            _hpAnim = AnimationFactory.CreateDoubleAnimation(150, 1, easing: true);
            _timerAnim = AnimationFactory.CreateDoubleAnimation(1, 0, framerate: 20);
            _enrageArcAnimation = AnimationFactory.CreateDoubleAnimation(1, 0, 1, framerate: 30, completed: _enrageArcAnimation_Completed);
            _fadeAnim = AnimationFactory.CreateDoubleAnimation(250, 0, 1, framerate: 30);

            SettingsWindowViewModel.AbnormalityShapeChanged += RefreshAbnormalityTemplate;
        }

        private void OnDataContextChanged(object _, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is NPC npc) VM = new BossViewModel(npc);
        }

        private void RefreshAbnormalityTemplate()
        {
            Abnormalities.ItemTemplateSelector = null;
            Abnormalities.ItemTemplateSelector = R.TemplateSelectors.BossAbnormalityTemplateSelector;
        }

        private void AnimateTimer()
        {
            Dispatcher?.Invoke(() =>
            {
                _timerAnim.From = VM.NPC.TimerPattern is HpTriggeredTimerPattern hptp ? hptp.StartAt : 1;
                _timerAnim.Duration = TimeSpan.FromSeconds(VM.NPC.TimerPattern.Duration);
                TimerDotPusher.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _timerAnim);
                TimerBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _timerAnim);
            });
        }
        private void OnHpChanged()
        {
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
            VM.ReEnraged += OnReEnraged;
            VM.Disposed += OnDispose;
            if (VM.NPC.TimerPattern != null) VM.NPC.TimerPattern.Started += AnimateTimer;

            NextEnrage.RenderTransform = new TranslateTransform(HpBarGrid.Width, 0);

            SlideEnrageIndicator(VM.NextEnragePercentage);


        }

        private void OnReEnraged()
        {
            _enrageArcAnimation.Duration = TimeSpan.FromSeconds(VM.NPC.EnragePattern.Duration);
            EnrageBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _enrageArcAnimation);
        }

        private void OnDispose()
        {
            DataContextChanged -= OnDataContextChanged;
            SettingsWindowViewModel.AbnormalityShapeChanged -= RefreshAbnormalityTemplate;
            VM.HpFactorChanged -= OnHpChanged;
            VM.EnragedChanged -= OnEnragedChanged;
            VM.ReEnraged -= OnReEnraged;
            VM.Disposed -= OnDispose;
            if (VM.NPC.TimerPattern != null) VM.NPC.TimerPattern.Started -= AnimateTimer;
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
            Dispatcher?.InvokeAsync(() =>
            {
                _slideAnim.To = val < 0 ? 0 : HpBarGrid.ActualWidth * (val / 100);
                NextEnrage.RenderTransform.BeginAnimation(TranslateTransform.XProperty, _slideAnim);
            });
        }

        private void BossGage_OnMouseEnter(object sender, MouseEventArgs e)
        {
            VM.ShowOverrideBtn = true;
        }

        private void BossGage_OnMouseLeave(object sender, MouseEventArgs e)
        {
            VM.ShowOverrideBtn = false;
        }
    }
}