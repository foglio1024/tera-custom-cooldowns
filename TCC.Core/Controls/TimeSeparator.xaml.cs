using System.Windows;

namespace TCC.Controls
{
    public partial class TimeSeparator 
    {
        public string TimeText
        {
            get => (string)GetValue(TimeTextProperty);
            set => SetValue(TimeTextProperty, value);
        }
        public static readonly DependencyProperty TimeTextProperty = DependencyProperty.Register("TimeText", typeof(string), typeof(TimeSeparator));

        public Thickness Borders
        {
            get => (Thickness)GetValue(BordersProperty);
            set => SetValue(BordersProperty, value);
        }
        public static readonly DependencyProperty BordersProperty = DependencyProperty.Register("Borders", typeof(Thickness), typeof(TimeSeparator));

        //private readonly DoubleAnimation _fadeOut;
        //private readonly DoubleAnimation _fadeIn;

        public TimeSeparator()
        {
            InitializeComponent();
            //_fadeIn = new DoubleAnimation(0,1,TimeSpan.FromMilliseconds(10));
            //_fadeOut = new DoubleAnimation(1,0,TimeSpan.FromMilliseconds(200));
        }

/*
        private void MouseEntered(object sender, MouseEventArgs e)
        {
            TimeTb.BeginAnimation(OpacityProperty, _fadeIn);
        }
*/
/*
        private void MouseLeft(object sender, MouseEventArgs e)
        {
            TimeTb.BeginAnimation(OpacityProperty, _fadeOut);
        }
*/
    }
}
