using FoglioUtils.Extensions;
using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    /// <summary>
    /// Logica di interazione per AbnormalitiesWindow.xaml
    /// </summary>
    public partial class BuffWindow
    {
        public BuffBarWindowViewModel VM { get; }
        public BuffWindow()
        {
            InitializeComponent();
            DataContext = new BuffBarWindowViewModel();
            VM = DataContext as BuffBarWindowViewModel;
            ButtonsRef = Buttons;
            MainContent = WindowContent;
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
