using System.Windows;
using System.Windows.Controls;
using TCC.Data.Chat;

namespace TCC.TemplateSelectors
{
    public class PiecePopupSelector : DataTemplateSelector
    {
        public DataTemplate MapTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;
            if(((MessagePiece)item).Type == MessagePieceType.PointOfInterest)
            {
                return MapTemplate;
            }
            else
            {
                return null;
            }
        }
    }
}
