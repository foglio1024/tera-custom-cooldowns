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
            var c = Cursors.Arrow;
            switch (val)
            {
                case MessagePieceType.Simple:
                case MessagePieceType.Money:
                    c = Cursors.Arrow;
                    break;
                case MessagePieceType.Item:
                case MessagePieceType.Quest:
                case MessagePieceType.PointOfInterest:
                case MessagePieceType.Url:
                case MessagePieceType.Achievement:
                    c = Cursors.Hand;
                    break;
            }
            return c;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
