using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using TCC.Data.Chat;

namespace TCC.UI.Converters
{
    public class MessageTypeToCursor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (MessagePieceType?)value;
            var c = val switch
            {
                MessagePieceType.Simple => Cursors.Arrow,
                MessagePieceType.Money => Cursors.Arrow,
                MessagePieceType.Item => Cursors.Hand,
                MessagePieceType.Quest => Cursors.Hand,
                MessagePieceType.PointOfInterest => Cursors.Hand,
                MessagePieceType.Url => Cursors.Hand,
                MessagePieceType.Achievement => Cursors.Hand,
                _ => Cursors.Arrow
            };
            return c;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
