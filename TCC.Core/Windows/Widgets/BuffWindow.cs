using System;
using System.Windows;
using System.Windows.Controls;
using FoglioUtils.Extensions;
using TCC.ViewModels;
using TCC.ViewModels.Widgets;

namespace TCC.Windows.Widgets
{
    public partial class BuffWindow
    {
        public BuffWindow(AbnormalityWindowViewModel vm)
        {
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
    }
}
