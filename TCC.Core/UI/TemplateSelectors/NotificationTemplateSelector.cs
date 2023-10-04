using System.Windows;
using System.Windows.Controls;
using TCC.UI.Windows.Widgets;
using TCC.Utils;

namespace TCC.UI.TemplateSelectors;

public class NotificationTemplateSelector : DataTemplateSelector
{
    public DataTemplate? Default { get; set; }
    public DataTemplate? Progress { get; set; }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        var n = (NotificationInfoBase?) item;

        return n != null
            ? n.NotificationTemplate switch
            {
                NotificationTemplate.Progress => Progress,
                _ => Default
            }
            : Default;
    }
}