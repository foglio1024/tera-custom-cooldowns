using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Data.NPCs;
using TCC.ViewModels;

namespace TCC.Controls.NPCs
{
    public partial class SmallMobControl
    {
        bool _firstLoad = true;
        private readonly DoubleAnimation _hpAnim;

        public SmallMobViewModel VM { get; set; }

        public SmallMobControl()
        {
            InitializeComponent();

            DataContextChanged += (_, e) =>
            {
                if (e.NewValue is NPC npc) VM = new SmallMobViewModel(npc);
            };
            _hpAnim = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = R.MiscResources.QuadraticEase
            };
            Timeline.SetDesiredFrameRate(_hpAnim, 20);

        }


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!_firstLoad) return;
            _firstLoad = false;
            VM.HpFactorChanged += OnHpChanged;
            VM.Disposed += OnDispose;
            //RootGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));
            SettingsWindowViewModel.AbnormalityShapeChanged += RefreshAbnormalityTemplate;

        }

        private void OnDispose()
        {
            VM.HpFactorChanged -= OnHpChanged;
            SettingsWindowViewModel.AbnormalityShapeChanged -= RefreshAbnormalityTemplate;
            RootGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty,
                    new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200)));
        }

        private void OnHpChanged()
        {
            if (VM.Compact)
            {
                _hpAnim.To = VM.NPC.HPFactor * 359.9;
                ExternalArc.BeginAnimation(Arc.EndAngleProperty, _hpAnim);
            }
            else
            {
                _hpAnim.To = VM.NPC.HPFactor;
                HpBarGovernor.LayoutTransform.BeginAnimation(ScaleTransform.ScaleXProperty, _hpAnim);
            }
        }

        private void RefreshAbnormalityTemplate()
        {
            Abnormalities.ItemTemplateSelector = null;
            Abnormalities.ItemTemplateSelector = R.TemplateSelectors.RaidAbnormalityTemplateSelector; 
        }

        private void SmallMobControl_OnMouseEnter(object sender, MouseEventArgs e)
        {
            VM.ShowOverrideBtn = true;
        }

        private void SmallMobControl_OnMouseLeave(object sender, MouseEventArgs e)
        {
            VM.ShowOverrideBtn = false;
        }
    }
}
