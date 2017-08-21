using System;
using System.Globalization;
using System.Windows.Data;
using TCC.ViewModels;

namespace TCC.Converters
{
    public class GroupSizeToTemplateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (int)value;

            if(val > GroupWindowViewModel.GROUP_SIZE_THRESHOLD)
            {
                return App.Current.FindResource("raid");
            }
            else
            {
                return App.Current.FindResource("party");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
