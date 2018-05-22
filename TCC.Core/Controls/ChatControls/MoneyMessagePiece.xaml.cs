using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Logica di interazione per UserControl1.xaml
    /// </summary>
    public partial class MoneyMessagePiece
    {
        public MoneyMessagePiece()
        {
            InitializeComponent();
        }
    }

    public class MoneyAmountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ReSharper disable once PossibleNullReferenceException
            var amount = (long)value;
            return amount == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
