using System.Windows;
using System.Windows.Controls;
using TCC.Data;

namespace TCC.TemplateSelectors
{
    public class SkillTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RoundTemplate { get; set; }
        public DataTemplate SquareTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return App.Settings.SkillShape == ControlShape.Round ? RoundTemplate : SquareTemplate;
        }

    }
}
