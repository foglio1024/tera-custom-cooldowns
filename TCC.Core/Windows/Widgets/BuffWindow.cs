using FoglioUtils.Extensions;
using TCC.ViewModels;
using TCC.ViewModels.Widgets;

namespace TCC.Windows.Widgets
{
    public partial class BuffWindow
    {
        public BuffWindow(BuffBarWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            BoundaryRef = Boundary;

            Init(App.Settings.BuffWindowSettings);
            SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
        }

        private void OnAbnormalityShapeChanged()
        {
            Buffs.RefreshTemplate(R.TemplateSelectors.PlayerAbnormalityTemplateSelector);
            Debuffs.RefreshTemplate(R.TemplateSelectors.PlayerAbnormalityTemplateSelector);
            InfBuffs.RefreshTemplate(R.TemplateSelectors.PlayerAbnormalityTemplateSelector);
        }
    }
}
