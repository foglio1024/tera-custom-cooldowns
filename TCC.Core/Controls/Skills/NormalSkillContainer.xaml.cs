using System.Windows;
using System.Windows.Controls;
using TCC.ViewModels;

namespace TCC.Controls.Skills
{
    /// <summary>
    /// Logica di interazione per NormalSkillContainer.xaml
    /// </summary>
    public partial class NormalSkillContainer
    {
        public NormalSkillContainer()
        {
            InitializeComponent();
            Loaded += (_, __) => { SettingsWindowViewModel.SkillShapeChanged += OnSkillShapeChanged; };
            Unloaded += (_, __) => { SettingsWindowViewModel.SkillShapeChanged -= OnSkillShapeChanged; };
        }

        private void OnSkillShapeChanged()
        {
            NormalSkillsPanel.RefreshTemplate("NormalSkillTemplateSelector");
            LongSkillsPanel.RefreshTemplate("NormalSkillTemplateSelector");
            ItemSkillsPanel.RefreshTemplate("NormalSkillTemplateSelector");
        }
    }
}
