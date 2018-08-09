using System.Windows;
using System.Windows.Controls;
using TCC.Data;

namespace TCC.TemplateSelectors
{
    public class MessagePieceDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SimpleTemplate { get; set; }
        public DataTemplate ItemTemplate { get; set; }
        public DataTemplate QuestTemplate { get; set; }
        public DataTemplate PointOfInterestTemplate { get; set; }
        public DataTemplate MoneyTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var m = item as MessagePiece;
            if (m == null) return null;
            switch (m.Type)
            {
                case MessagePieceType.Simple:
                    return SimpleTemplate;
                case MessagePieceType.Item:
                    return ItemTemplate;
                case MessagePieceType.Quest:
                    return QuestTemplate;
                case MessagePieceType.PointOfInterest:
                    return PointOfInterestTemplate;
                case MessagePieceType.Money:
                    return MoneyTemplate;
                default:
                    return SimpleTemplate;
            }
        }
    }
}
