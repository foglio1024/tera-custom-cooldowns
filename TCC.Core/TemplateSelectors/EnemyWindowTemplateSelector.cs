using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TCC.TemplateSelectors
{
    public class EnemyWindowTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Default { get; set; }
        public DataTemplate Phase1 { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Console.WriteLine("Switching template");
            if (item == null) return null;
            //if (((ViewModels.BossGageWindowViewModel)item).CurrentHHphase == HarrowholdPhase.Phase1) return Phase1;
            if ((HarrowholdPhase)item == HarrowholdPhase.Phase1) return Phase1;
            else return Default;
        }
    }
}
