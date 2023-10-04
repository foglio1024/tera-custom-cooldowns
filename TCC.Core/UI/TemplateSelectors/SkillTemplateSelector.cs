using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using TCC.Data;

namespace TCC.UI.TemplateSelectors;

public class SkillTemplateSelector : DataTemplateSelector
{
    public DataTemplate? RoundTemplate { get; set; }
    public DataTemplate? SquareTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (DesignerProperties.GetIsInDesignMode(container)) return RoundTemplate;

        return App.Settings.SkillShape == ControlShape.Round ? RoundTemplate : SquareTemplate;
    }
}