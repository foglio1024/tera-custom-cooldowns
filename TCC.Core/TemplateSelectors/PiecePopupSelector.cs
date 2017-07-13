using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TCC.Data;

namespace TCC.TemplateSelectors
{
    public class PiecePopupSelector : DataTemplateSelector
    {
        public DataTemplate MapTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;
            if(((MessagePiece)item).Type == MessagePieceType.Point_of_interest)
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
