using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TCC.Parsing;

namespace TCC
{
    /// <summary>
    /// Logica di interazione per HPbar.xaml
    /// </summary>
    public partial class CharacterWindow : Window
    {
        static int AnimationTime = 150;
        public CharacterWindow()
        {
            InitializeComponent();

            
            //hpBar.RenderTransform = new ScaleTransform(1, 1, 0, .5);
            //mpBar.RenderTransform = new ScaleTransform(1, 1, 0, .5);
            //stBar.RenderTransform = new ScaleTransform(1, 1, 0, .5);
            //flightBar.RenderTransform = new ScaleTransform(1, 1, 0, .5);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(hwnd);
            FocusManager.HideFromToolBar(hwnd);
            Topmost = true;
            Opacity = 0;
            ContextMenu = new ContextMenu();
            var HideButton = new MenuItem() { Header = "Hide" };
            HideButton.Click += (s, ev) =>
            {
                this.Visibility = Visibility.Collapsed;
            };
            ContextMenu.Items.Add(HideButton);


            //SessionManager.CurrentPlayer.HPUpdated += UpdateHP;
            //SessionManager.CurrentPlayer.MPUpdated += UpdateMP;
            //SessionManager.CurrentPlayer.STUpdated += UpdateST;
            //SessionManager.CurrentPlayer.FlightEnergyUpdated += UpdateFlightEnergy;

            

            //hpBar.Width = @base.Width;
            //mpBar.Width = @base.Width;
            //stBar.Width = @base.Width;
            //flightBar.Width = dummyFlightBar.Width;
        }

        internal void ShowResolve()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //StaminaRow.Height = GridLength.Auto;
                //stBar.Fill = new SolidColorBrush(Color.FromRgb(0x66, 0xbb, 0xff));
                //UpdateST(0);
            }));
        }
        internal void ShowResolve(Color c)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //StaminaRow.Height = GridLength.Auto;
                //stBar.Fill = new SolidColorBrush(c);
                //UpdateST(0);

            }));
        }
        internal void HideResolve()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //StaminaRow.Height = new GridLength(0);
            }));
        }

        private void UpdateHP(float newValue)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                hpBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(ValueToLength((double)newValue, (double)SessionManager.CurrentPlayer.MaxHP), TimeSpan.FromMilliseconds(AnimationTime)) { EasingFunction = new QuadraticEase() });
            }));
        }
        private void UpdateMP(float newValue)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                mpBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(ValueToLength((double)newValue, (double)SessionManager.CurrentPlayer.MaxMP), TimeSpan.FromMilliseconds(AnimationTime)) { EasingFunction = new QuadraticEase() });
            }));

        }
        private void UpdateST(float newValue)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                stBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(ValueToLength((double)newValue, (double)SessionManager.CurrentPlayer.MaxST), TimeSpan.FromMilliseconds(AnimationTime)) { EasingFunction = new QuadraticEase() });
            }));
        }

        private void UpdateFlightEnergy(float newValue)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                flightBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(ValueToLength((double)newValue, 1000), TimeSpan.FromMilliseconds(AnimationTime)) { EasingFunction = new QuadraticEase() });
            }));
        }


        //private DoubleAnimation BarAnimation(double value)
        //{
        //    return new DoubleAnimation(value, TimeSpan.FromMilliseconds(AnimationTime)) { EasingFunction = new QuadraticEase() };
        //}
        //DoubleAnimation _doubleAnimation = new DoubleAnimation();
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        double ValueToLength(double value, double maxValue)
        {
            if (maxValue == 0)
            {
                return 0;
            }
            else
            {
                double n = ((double)value / (double)maxValue);
                return n;
            }

        }

        internal void Reset()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //UpdateHP(0);
                //UpdateMP(0);
                //UpdateST(0);
            }));
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}

