using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCC.Controls
{
    public partial class HPGauge : UserControl
    {

        private int animTime = 200;
        private DoubleAnimation a;
        private DependencyPropertyWatcher<double> watcher; //https://blogs.msdn.microsoft.com/flaviencharlon/2012/12/07/getting-change-notifications-from-any-dependency-property-in-windows-store-apps/
        public HPGauge()
        {
            InitializeComponent();

            watcher = new DependencyPropertyWatcher<double>(this, "Value");
            watcher.PropertyChanged += ValueWatcher_PropertyChanged;
            a = new DoubleAnimation(watcher.Value, TimeSpan.FromMilliseconds(animTime)) { EasingFunction = new QuadraticEase() };
            bar.RenderTransform = new ScaleTransform(1, 1, 0, .5);

        }

        private void ValueWatcher_PropertyChanged(object sender, EventArgs e)
        {
            if(watcher.Value > 1)
            {
                a.To = 1;
            }
            else
            {
                a.To = watcher.Value;
            }
            Console.WriteLine(watcher.Value.ToString() + " " + a.To.ToString());
            bar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, a);
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(double), typeof(HPGauge));

        public SolidColorBrush BarColor
        {
            get { return (SolidColorBrush)GetValue(BarColorProperty); }
            set { SetValue(BarColorProperty, value); }
        }
        public static readonly DependencyProperty BarColorProperty = DependencyProperty.Register("BarColor", typeof(SolidColorBrush), typeof(HPGauge));
               
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
