using System.Windows;
using System.Windows.Controls;
using TCC.Data.NPCs;

namespace TCC.TemplateSelectors
{
    public class EnemyTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BossDataTemplate { get; set; }
        public DataTemplate MobDataTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            if (((NPC)item).IsBoss) return BossDataTemplate;
            else return MobDataTemplate;
        }
    }
}
