using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Nostrum.Controls;
using Nostrum.Factories;
using TCC.Data.NPCs;
using TCC.ViewModels;

namespace TCC.Controls.NPCs
{
    public partial class SmallMobControl
    {
        private bool _firstLoad = true;
        private readonly DoubleAnimation _hpAnim;
        private readonly DoubleAnimation _shrinkAnim;

        public SmallMobViewModel VM { get; set; }

        public SmallMobControl()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
            _hpAnim = AnimationFactory.CreateDoubleAnimation(250, 0, easing: true, framerate: 20);
            _shrinkAnim = AnimationFactory.CreateDoubleAnimation(200, 0, 1, easing: true, framerate: 20);
        }

        private void OnDataContextChanged(object _, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is NPC npc) VM = new SmallMobViewModel(npc);
        }


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!_firstLoad) return;
            _firstLoad = false;
            VM.HpFactorChanged += OnHpChanged;
            VM.Disposed += OnDispose;

            SettingsWindowViewModel.AbnormalityShapeChanged += RefreshAbnormalityTemplate;

        }

        private void OnDispose()
        {
            VM.Disposed -= OnDispose;
            VM.HpFactorChanged -= OnHpChanged;
            DataContextChanged -= OnDataContextChanged;
            SettingsWindowViewModel.AbnormalityShapeChanged -= RefreshAbnormalityTemplate;
            RootGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty,_shrinkAnim);
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
