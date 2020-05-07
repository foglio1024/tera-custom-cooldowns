using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Nostrum.Extensions;
using Nostrum.Factories;
using TCC.UI.Controls.Abnormalities;
using TCC.ViewModels;
using TCC.ViewModels.Widgets;

namespace TCC.UI.Windows.Widgets
{
    public partial class BuffWindow
    {
        private readonly DoubleAnimation _opacityUp;
        private readonly DoubleAnimation _opacityDown;

        public BuffWindow(AbnormalityWindowViewModel vm)
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
                        Grid.SetColumn(SpecBuffs, 0);
                        Grid.SetColumn(SpecInfBuffs, 0);
                        break;
                    case FlowDirection.RightToLeft:
                        Grid.SetColumn(Buffs, 0);
                        Grid.SetColumn(InfBuffs, 0);
                        Grid.SetColumn(SpecBuffs, 1);
                        Grid.SetColumn(SpecInfBuffs, 1);
                        break;
                }
            });
        }

        private void OnAbnormalityShapeChanged()
        {
            Buffs.RefreshTemplate(R.TemplateSelectors.PlayerAbnormalityTemplateSelector);
            SpecBuffs.RefreshTemplate(R.TemplateSelectors.PlayerAbnormalityTemplateSelector);
            Debuffs.RefreshTemplate(R.TemplateSelectors.PlayerAbnormalityTemplateSelector);
            InfBuffs.RefreshTemplate(R.TemplateSelectors.PlayerAbnormalityTemplateSelector);
            SpecInfBuffs.RefreshTemplate(R.TemplateSelectors.PlayerAbnormalityTemplateSelector);
        }

        private void OnWindowMouseEnter(object sender, MouseEventArgs e)
        {
            AbnormalityIndicatorBase.InvokeVisibilityChanged(this, true);
            SettingsButton.BeginAnimation(OpacityProperty, _opacityUp);
        }

        private void OnWindowMouseLeave(object sender, MouseEventArgs e)
        {
            SettingsButton.BeginAnimation(OpacityProperty, _opacityDown);
            Task.Delay(1000).ContinueWith(t => Dispatcher.InvokeAsync(() =>
            {
                if (IsMouseOver) return;
                AbnormalityIndicatorBase.InvokeVisibilityChanged(this, false);
            }));
        }

        private void OpenBuffSettings(object sender, RoutedEventArgs e)
        {
            if (TccWindow.IsCreated(typeof(MyAbnormalConfigWindow))) return;
            new MyAbnormalConfigWindow().Show();
        }
    }
}
