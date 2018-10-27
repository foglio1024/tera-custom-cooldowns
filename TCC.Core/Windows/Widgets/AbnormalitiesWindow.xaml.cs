using System.Windows.Controls;
using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    /// <summary>
    /// Logica di interazione per AbnormalitiesWindow.xaml
    /// </summary>
    public partial class BuffWindow
    {
        public BuffWindow()
        {
            InitializeComponent();
            ButtonsRef = Buttons;
            MainContent = WindowContent;
            Init(Settings.Settings.BuffWindowSettings);
            SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
        }

        private void OnAbnormalityShapeChanged()
        {
            Buffs.ItemTemplateSelector = null;
            Buffs.ItemTemplateSelector = System.Windows.Application.Current.FindResource("PlayerAbnormalityTemplateSelector") as DataTemplateSelector;
            Debuffs.ItemTemplateSelector = null;
            Debuffs.ItemTemplateSelector = System.Windows.Application.Current.FindResource("PlayerAbnormalityTemplateSelector") as DataTemplateSelector;
            InfBuffs.ItemTemplateSelector = null;
            InfBuffs.ItemTemplateSelector = System.Windows.Application.Current.FindResource("PlayerAbnormalityTemplateSelector") as DataTemplateSelector;

        }
    }
}
