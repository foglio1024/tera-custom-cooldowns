using System;
using System.Collections.Generic;
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

namespace TCC.UI_elements
{
    /// <summary>
    /// Logica di interazione per AbnormalityIndicator.xaml
    /// </summary>
    public partial class AbnormalityIndicator : UserControl
    {
        public AbnormalityIndicator()
        {
            InitializeComponent();

            //abnormalityId.DataContext = this;
            abnormalityName.DataContext = this;
            abnormalityIcon.DataContext = this;
        }



        public uint AbnormalityId
        {
            get { return (uint)GetValue(AbnormalityIdProperty); }
            set { SetValue(AbnormalityIdProperty, value); }
        }
        public static readonly DependencyProperty AbnormalityIdProperty =
            DependencyProperty.Register("AbnormalityId", typeof(uint), typeof(AbnormalityIndicator));

        public string AbnormalityName
        {
            get { return (string)GetValue(AbnormalityNameProperty); }
            set { SetValue(AbnormalityNameProperty, value); }
        }
        public static readonly DependencyProperty AbnormalityNameProperty =
            DependencyProperty.Register("AbnormalityName", typeof(string), typeof(AbnormalityIndicator));

        public ImageBrush Icon
        {
            get { return (ImageBrush)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageBrush), typeof(AbnormalityIndicator));

        public int Duration
        {
            get { return (int)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(int), typeof(AbnormalityIndicator));

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            arc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(Duration)));
        }
    }
}
