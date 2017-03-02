using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
using System.Windows.Threading;

namespace TCC.UI
{

    /// <summary>
    /// Logica di interazione per SkillIconControl.xaml
    /// </summary>
    public partial class SkillIconControl : UserControl
    {

        public ImageBrush IconBrush
        {
            get { return (ImageBrush)GetValue(IconBrushProperty); }
            set { SetValue(IconBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconBrushProperty =
            DependencyProperty.Register("IconBrush", typeof(ImageBrush), typeof(SkillIconControl));

        public uint Id
        {
            get { return (uint)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(uint), typeof(SkillIconControl));


        public int Cooldown
        {
            get { return (int)GetValue(CooldownProperty); }
            set { SetValue(CooldownProperty, value); }
        }
    

        // Using a DependencyProperty as the backing store for Cooldown.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CooldownProperty =
            DependencyProperty.Register("Cooldown", typeof(int), typeof(SkillIconControl));

        public SkillIconControl()
        {
            InitializeComponent();
            icon.DataContext = this;
            SkillManager.Changed += SkillIconControl_Changed;
        }

        private void SkillIconControl_Changed(object sender, EventArgs e, SkillCooldown s)
        {
            Dispatcher.Invoke(() =>
            {

                if (s.Id == this.Id)
                {
                    double current = (double)s.Cooldown / (double)Cooldown;

                    arc.BeginAnimation(Arc.EndAngleProperty, null);
                    var a = new DoubleAnimation(359.9 * current, 0, TimeSpan.FromMilliseconds(s.Cooldown));
                    var b = new DoubleAnimation(.5, 1, TimeSpan.FromMilliseconds(150));
                    arc.BeginAnimation(Arc.EndAngleProperty, a);
                    this.BeginAnimation(OpacityProperty, b);
                }
            });
        }

        Timer NumberTimer;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var a = new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(Cooldown));
            //var b = new DoubleAnimation(.4, 0, TimeSpan.FromMilliseconds(Cooldown));
            arc.BeginAnimation(Arc.EndAngleProperty, a);
            //fill.BeginAnimation(OpacityProperty, b);
            NumberTimer = new Timer(1000);
            double cd = (double)Cooldown / 1000;
            number.Text = String.Format("{0:N0}",cd);
            NumberTimer.Elapsed += (s, o) => {
                Dispatcher.Invoke(() => {
                    cd --;
                        number.Text = String.Format("{0:N0}", cd); 
                });
            };
            NumberTimer.Enabled = true;
        }
        ~SkillIconControl()
        {
            NumberTimer.Stop();
        }
    }
}
