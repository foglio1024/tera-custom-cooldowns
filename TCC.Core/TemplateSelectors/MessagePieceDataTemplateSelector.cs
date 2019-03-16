using System.Windows;
using System.Windows.Controls;
using TCC.Data.Chat;

namespace TCC.TemplateSelectors
{
    public class MessagePieceDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SimpleTemplate { get; set; }
        public DataTemplate ItemTemplate { get; set; }
        public DataTemplate QuestTemplate { get; set; }
        public DataTemplate AchievementTemplate { get; set; }
        public DataTemplate PointOfInterestTemplate { get; set; }
        public DataTemplate MoneyTemplate { get; set; }
        public DataTemplate EmojiTemplate { get; set; }
        public DataTemplate IconTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var m = item as MessagePiece;
            if (m == null) return null;
            switch (m.Type)
            {
                case MessagePieceType.Item:
                    return ItemTemplate;
                case MessagePieceType.Quest:
                case MessagePieceType.Achievement:
                    return QuestTemplate;
                case MessagePieceType.PointOfInterest:
                    return PointOfInterestTemplate;
                case MessagePieceType.Emoji:
                    return EmojiTemplate;
                case MessagePieceType.Icon:
                    return IconTemplate;
                case MessagePieceType.Money:
                    return MoneyTemplate;
                default:
                    return SimpleTemplate;
            }
        }
    }
}
