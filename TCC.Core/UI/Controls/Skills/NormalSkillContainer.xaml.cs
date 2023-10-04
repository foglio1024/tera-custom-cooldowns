using System.Windows;
using Nostrum.WPF.Extensions;
using TCC.ViewModels;

namespace TCC.UI.Controls.Skills;

public partial class NormalSkillContainer
{
    public NormalSkillContainer()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    void OnUnloaded(object _, RoutedEventArgs __)
    {
        SettingsWindowViewModel.SkillShapeChanged -= OnSkillShapeChanged;
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;

    }

    void OnLoaded(object _, RoutedEventArgs __)
    {
        SettingsWindowViewModel.SkillShapeChanged += OnSkillShapeChanged;
    }

    void OnSkillShapeChanged()
    {
        NormalSkillsPanel.RefreshTemplate("NormalSkillTemplateSelector");
        LongSkillsPanel.RefreshTemplate("NormalSkillTemplateSelector");
        ItemSkillsPanel.RefreshTemplate("NormalSkillTemplateSelector");
    }
}