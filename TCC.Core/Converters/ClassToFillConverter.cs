using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TCC.Data;
using TeraDataLite;

namespace TCC.Converters
{
    public class ClassToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (Class?)value ?? Class.Common;
            var color = targetType == typeof(Color); 
            switch (c)
            {
                case Class.Lancer:
                    return Application.Current.FindResource($"TankRole{(color ? "Color" : "Brush")}");
                case Class.Brawler:
                    return Application.Current.FindResource($"TankRole{(color ? "Color" : "Brush")}");
                case Class.Priest:
                    return Application.Current.FindResource($"HealerRole{(color ? "Color" : "Brush")}");
                case Class.Mystic:
                    return Application.Current.FindResource($"HealerRole{(color ? "Color" : "Brush")}");
                default:
                    return Application.Current.FindResource($"DpsRole{(color ? "Color" : "Brush")}");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}