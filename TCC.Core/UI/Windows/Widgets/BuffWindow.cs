using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Nostrum.WPF.Extensions;
using Nostrum.WPF.Factories;
using TCC.UI.Controls.Abnormalities;
using TCC.ViewModels;
using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets;

public partial class BuffWindow
{
    readonly DoubleAnimation _opacityUp;
    readonly DoubleAnimation _opacityDown;
    readonly AbnormalityWindowViewModel _vm;

    public BuffWindow(AbnormalityWindowViewModel vm)
    {

        _opacityUp = AnimationFactory.CreateDoubleAnimation(250, 1, easing: true);
        _opacityDown = AnimationFactory.CreateDoubleAnimation(250, 0, easing: true, delay: 1000);


        InitializeComponent();
        _vm = vm;
        DataContext = _vm;
        ButtonsRef = Buttons;
        MainContent = WindowContent;
        BoundaryRef = Boundary;

        Init(App.Settings.BuffWindowSettings);
        SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
        App.Settings.BuffWindowSettings.DirectionChanged += OnDirectionChanged;
        OnDirectionChanged();
    }

    void OnDirectionChanged()
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

    void OnAbnormalityShapeChanged()
    {
        Buffs.RefreshTemplate(R.TemplateSelectors.PlayerAbnormalityTemplateSelector);
        Debuffs.RefreshTemplate(R.TemplateSelectors.PlayerAbnormalityTemplateSelector);
        InfBuffs.RefreshTemplate(R.TemplateSelectors.PlayerAbnormalityTemplateSelector);
    }

    void OnWindowMouseEnter(object sender, MouseEventArgs e)
    {
        SetAbnormalitiesVisibility(true);
        SettingsButton.BeginAnimation(OpacityProperty, _opacityUp);
    }

    void OnWindowMouseLeave(object sender, MouseEventArgs e)
    {
        SettingsButton.BeginAnimation(OpacityProperty, _opacityDown);
        Task.Delay(1000).ContinueWith(_ => Dispatcher.InvokeAsync(() =>
        {
            if (IsMouseOver) return;
            SetAbnormalitiesVisibility(false);
        }));
    }

    void SetAbnormalitiesVisibility(bool visible)
    {
        var normal = _vm.Player.Buffs.ToSyncList()
            .Where(x => App.Settings.BuffWindowSettings.Hidden.Contains(x.Abnormality.Id))
            .ToArray();
        var perma = _vm.Player.InfBuffs.ToSyncList()
            .Where(x => App.Settings.BuffWindowSettings.Hidden.Contains(x.Abnormality.Id))
            .ToArray();
        var debuffs = _vm.Player.Debuffs.ToSyncList()
            .Where(x => App.Settings.BuffWindowSettings.Hidden.Contains(x.Abnormality.Id))
            .ToArray();

        foreach (var abnormality in normal)
        {
            abnormality.IsHidden = !visible;
        }
        foreach (var abnormality in perma)
        {
            abnormality.IsHidden = !visible;
        }
        foreach (var abnormality in debuffs)
        {
            abnormality.IsHidden = !visible;
        }
    }

    void OpenBuffSettings(object sender, RoutedEventArgs e)
    {
        if (TccWindow.Exists(typeof(MyAbnormalConfigWindow))) return;
        new MyAbnormalConfigWindow().Show();
    }
}