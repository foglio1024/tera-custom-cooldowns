using System.Windows;
using System.Windows.Media;

namespace TCC.Controls.Dashboard
{
    /// <summary>
    /// Logica di interazione per RectangleBarGauge.xaml
    /// </summary>
    public partial class RectangleBarGauge
    {
        public RectangleBarGauge()
        {
            InitializeComponent();
        }

        public new double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }
        public new static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(RectangleBarGauge), new PropertyMetadata(0d));

        public new double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }
        public new  static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(RectangleBarGauge), new PropertyMetadata(0d));

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(RectangleBarGauge));

        public double Factor
        {
            get { return (double)GetValue(FactorProperty); }
            set { SetValue(FactorProperty, value); }
        }
        public static readonly DependencyProperty FactorProperty =
            DependencyProperty.Register("Factor", typeof(double), typeof(RectangleBarGauge), new PropertyMetadata(0d));


    }

}
