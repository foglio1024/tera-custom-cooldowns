using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per UserControl1.xaml
    /// </summary>
    public partial class MoneyMessagePiece : UserControl
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
            var amount = (int)value;
            if(amount == 0)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
