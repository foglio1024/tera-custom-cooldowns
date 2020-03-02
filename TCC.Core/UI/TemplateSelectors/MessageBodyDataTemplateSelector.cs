using System.Windows;
using System.Windows.Controls;
using TCC.Data;
using TCC.Data.Chat;

namespace TCC.UI.TemplateSelectors
{
    public class MessageBodyDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultBody { get; set; }
        public DataTemplate BrokerBody { get; set; }
        public DataTemplate ApplyBody { get; set; }
        public DataTemplate LfgBody { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var m = item as ChatMessage;
            if (m == null) return null;

            switch (m.Channel)
            {
                case ChatChannel.LFG:
                    return LfgBody;
                case ChatChannel.Bargain:
                    return BrokerBody;
                case ChatChannel.Apply:
                    return ApplyBody;
                default:
                    return DefaultBody;
            }
        }
    }
}
