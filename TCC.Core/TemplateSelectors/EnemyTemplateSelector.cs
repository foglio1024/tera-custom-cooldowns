using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TCC.Data;

namespace TCC.TemplateSelectors
{
    public class EnemyTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BossDataTemplate { get; set; }
        public DataTemplate MobDataTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            if (((Boss)item).IsBoss) return BossDataTemplate;
            else return MobDataTemplate;
        }
    }
}
