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
using System.Windows.Shapes;
using TCC.Data;
using TCC.Controls;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC.Converters
{
    public class ClassToFillConverter : IValueConverter
    {
        SolidColorBrush healerColor = new SolidColorBrush(Color.FromRgb(79, 255, 142));
        SolidColorBrush tankColor = new SolidColorBrush(Color.FromRgb(79, 190, 255));
        SolidColorBrush dpsColor = new SolidColorBrush(Color.FromRgb(255, 150, 70));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (Class)value;
            switch (c)
            {
                case Class.Lancer:
                    return App.Current.FindResource("Colors.GroupWindow.Tank");
                case Class.Fighter:
                    return App.Current.FindResource("Colors.GroupWindow.Tank");
                case Class.Priest:
                    return App.Current.FindResource("Colors.GroupWindow.Healer");
                case Class.Elementalist:
                    return App.Current.FindResource("Colors.GroupWindow.Healer");
                default:
                    return App.Current.FindResource("Colors.GroupWindow.Dps");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
namespace TCC.Windows
{
    /// <summary>
    /// Logica di interazione per GroupWindow.xaml
    /// </summary>
    public partial class GroupWindow : TccWindow
    {
        public GroupWindow()
        {
            InitializeComponent();
        }
        private void TccWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitWindow(SettingsManager.GroupWindowSettings, ignoreSize: true);
        }
        private void TccWindow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}
