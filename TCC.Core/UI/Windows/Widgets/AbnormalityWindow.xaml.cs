using Nostrum.WPF.Extensions;
using Nostrum.WPF.Factories;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.ViewModels;
using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets;

public partial class AbnormalityWindow
{
    private readonly DoubleAnimation _opacityUp;
    private readonly DoubleAnimation _opacityDown;

    public AbnormalityWindow(AbnormalityWindowViewModel vm)
    {

        _opacityUp = AnimationFactory.CreateDoubleAnimation(250, 1, easing: true);
        _opacityDown = AnimationFactory.CreateDoubleAnimation(250, 0, easing: true, delay: 1000);


        InitializeComponent();
        DataContext = vm;
        ButtonsRef = Buttons;
        MainContent = WindowContent;
        BoundaryRef = Boundary;

        Init(App.Settings.BuffWindowSettings);
        SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
        App.Settings.BuffWindowSettings.DirectionChanged += OnDirectionChanged;
        OnDirectionChanged();
    }

    private void OnDirectionChanged()
    {
        Dispatcher?.InvokeAsync(() =>
        {

            switch (App.Settings.BuffWindowSettings.Direction)
            {
                case FlowDirection.LeftToRight:
                    Grid.SetColumn(Buffs, 1);
                    Grid.SetColumn(InfBuffs, 1);
                    break;
                case FlowDirection.RightToLeft:
                    Grid.SetColumn(Buffs, 0);
                    Grid.SetColumn(InfBuffs, 0);
                    break;
            }
        });
    }

    private void OnAbnormalityShapeChanged()
    {
        Buffs.RefreshTemplate(R.TemplateSelectors.PlayerAbnormalityTemplateSelector);
        Debuffs.RefreshTemplate(R.TemplateSelectors.PlayerAbnormalityTemplateSelector);
        InfBuffs.RefreshTemplate(R.TemplateSelectors.PlayerAbnormalityTemplateSelector);
    }

    private void OnWindowMouseEnter(object sender, MouseEventArgs e)
    {
        SetAbnormalitiesVisibility(true);
        SettingsButton.BeginAnimation(OpacityProperty, _opacityUp);
    }

    private void OnWindowMouseLeave(object sender, MouseEventArgs e)
    {
        SettingsButton.BeginAnimation(OpacityProperty, _opacityDown);
        Task.Delay(1000).ContinueWith(_ => Dispatcher.InvokeAsync(() =>
        {
            if (IsMouseOver) return;
            SetAbnormalitiesVisibility(false);
        }));
    }

    private void SetAbnormalitiesVisibility(bool visible)
    {
        Game.Me.SetAbnormalitiesVisibility(visible);
    }
}