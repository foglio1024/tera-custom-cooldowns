using System.Windows;
using System.Windows.Controls;
using TCC.Data.Chat;

namespace TCC.TemplateSelectors
{
    public class MessagePieceDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SimpleTemplate { get; set; }
        public DataTemplate ActionTemplate { get; set; }
        public DataTemplate MoneyTemplate { get; set; }
        public DataTemplate IconTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (item)
            {
                case ActionMessagePiece _: return ActionTemplate;
                case IconMessagePiece _: return IconTemplate;
                case MoneyMessagePiece _: return MoneyTemplate;
                default: return SimpleTemplate;
            }
        }
    }
}
