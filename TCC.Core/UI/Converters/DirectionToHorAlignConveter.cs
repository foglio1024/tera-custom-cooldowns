using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TCC.UI.Converters
{
    public class DirectionToHorAlignConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (FlowDirection?)value;
            return v switch
            {
                FlowDirection.RightToLeft => HorizontalAlignment.Right,
                _ => HorizontalAlignment.Left
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
