using System.Windows.Controls;
using TCC.ViewModels;

namespace TCC.Windows
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
            MainContent = content;
            Init(Settings.BuffWindowSettings, ignoreSize: true);
            SettingsWindowViewModel.AbnormalityShapeChanged += OnAbnormalityShapeChanged;
        }

        private void OnAbnormalityShapeChanged()
        {
            Buffs.ItemTemplateSelector = null;
            Buffs.ItemTemplateSelector = App.Current.FindResource("PlayerAbnormalityTemplateSelector") as DataTemplateSelector;
            Debuffs.ItemTemplateSelector = null;
            Debuffs.ItemTemplateSelector = App.Current.FindResource("PlayerAbnormalityTemplateSelector") as DataTemplateSelector;
            InfBuffs.ItemTemplateSelector = null;
            InfBuffs.ItemTemplateSelector = App.Current.FindResource("PlayerAbnormalityTemplateSelector") as DataTemplateSelector;

        }
    }
}
