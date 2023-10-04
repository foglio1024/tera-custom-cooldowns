using System.Windows;
using System.Windows.Controls;
using TCC.Data.Chat;
using TCC.Utils;

namespace TCC.UI.TemplateSelectors;

public class MessageBodyDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? DefaultBody { get; set; }
    public DataTemplate? BrokerBody { get; set; }
    public DataTemplate? ApplyBody { get; set; }
    public DataTemplate? LfgBody { get; set; }
    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (item is not ChatMessage m) return null;

        return m.Channel switch
        {
            ChatChannel.LFG => LfgBody,
            ChatChannel.Bargain => BrokerBody,
            ChatChannel.Apply => ApplyBody,
            _ => DefaultBody
        };
    }
}