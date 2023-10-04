using System.Windows;
using System.Windows.Controls;
using TCC.Data.Npc;

namespace TCC.UI.TemplateSelectors;

public class EnemyTemplateSelector : DataTemplateSelector
{
    public DataTemplate? BossDataTemplate { get; set; }
    public DataTemplate? MobDataTemplate { get; set; }
    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (item == null) return null;

        return ((NPC)item).IsBoss ? BossDataTemplate : MobDataTemplate;
    }
}