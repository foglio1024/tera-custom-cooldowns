using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Nostrum.WPF.Controls;
using Nostrum.WPF.Factories;
using TCC.Data.Npc;
using TCC.ViewModels;

namespace TCC.UI.Controls.NPCs;

public partial class SmallMobControl
{
    private bool _firstLoad = true;
    private readonly DoubleAnimation _hpAnim;
    private readonly DoubleAnimation _shrinkAnim;

    public SmallMobViewModel? VM { get; private set; }

    public SmallMobControl()
    {
        InitializeComponent();

        DataContextChanged += OnDataContextChanged;
        _hpAnim = AnimationFactory.CreateDoubleAnimation(250, 0, easing: true, framerate: 20);
        _shrinkAnim = AnimationFactory.CreateDoubleAnimation(200, 0, 1, easing: true, framerate: 20);
    }

    private void OnDataContextChanged(object _, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not Npc npc) return;
        VM = new SmallMobViewModel(npc);
    }


    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!_firstLoad) return;
        _firstLoad = false;
        if (VM != null)
        {
            VM.HpFactorChanged += OnHpChanged;
            VM.Disposed += OnDispose;
        }

        SettingsWindowViewModel.AbnormalityShapeChanged += RefreshAbnormalityTemplate;
    }

    private void OnDispose()
    {
        if (VM != null)
        {
            VM.Disposed -= OnDispose;
            VM.HpFactorChanged -= OnHpChanged;
        }
        DataContextChanged -= OnDataContextChanged;
        SettingsWindowViewModel.AbnormalityShapeChanged -= RefreshAbnormalityTemplate;
        RootGrid.LayoutTransform.BeginAnimation(ScaleTransform.ScaleYProperty, _shrinkAnim);
    }

    private void OnHpChanged()
    {
        if (VM == null) return;

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
        if (VM == null) return;
        VM.ShowOverrideBtn = true;
    }

    private void SmallMobControl_OnMouseLeave(object sender, MouseEventArgs e)
    {
        if (VM == null) return;
        VM.ShowOverrideBtn = false;
    }
}