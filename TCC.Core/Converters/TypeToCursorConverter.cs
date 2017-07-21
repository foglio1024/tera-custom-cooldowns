using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using TCC.Data;

namespace TCC.Converters
{
    public class MessageTypeToCursor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (MessagePieceType)value;
            Cursor c = Cursors.Arrow;
            switch (val)
            {
                case MessagePieceType.Simple:
                    c = Cursors.Arrow;
                    break;
                case MessagePieceType.Item:
                    c = Cursors.Hand;
                    break;
                case MessagePieceType.Quest:
                    c = Cursors.Hand;
                    break;
                case MessagePieceType.Point_of_interest:
                    c = Cursors.Hand;
                    break;
                case MessagePieceType.Money:
                    c = Cursors.Arrow;
                    break;
                case MessagePieceType.Url:
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
