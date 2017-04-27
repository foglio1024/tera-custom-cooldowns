using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
    public partial class CharacterWindow : Window, INotifyPropertyChanged
    {
        static int AnimationTime = 200;

        private void NotifyPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public CharacterWindow()
        {
            InitializeComponent();
            this.DataContext = SessionManager.CurrentPlayer;
            stBar.Fill = new SolidColorBrush(Color.FromRgb(0x6, 0xb, 0xf));
            
            hpBar.RenderTransform = new ScaleTransform(1, 1, 0, .5);
            mpBar.RenderTransform = new ScaleTransform(1, 1, 0, .5);
            stBar.RenderTransform = new ScaleTransform(1, 1, 0, .5);
            flightBar.RenderTransform = new ScaleTransform(1, 1, 0, .5);

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


            SessionManager.CurrentPlayer.HPUpdated += UpdateHP;
            SessionManager.CurrentPlayer.MPUpdated += UpdateMP;
            SessionManager.CurrentPlayer.STUpdated += UpdateST;
            SessionManager.CurrentPlayer.FlightEnergyUpdated += UpdateFlightEnergy;

            hpBar.Width = @base.Width;
            mpBar.Width = @base.Width;
            stBar.Width = @base.Width;
            flightBar.Width = dummyFlightBar.Width;
        }

        internal void ShowResolve()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StaminaRow.Height = GridLength.Auto;
                stBar.Fill = new SolidColorBrush(Color.FromRgb(0x66, 0xbb, 0xff));
                UpdateST(0);

            }));
        }
        internal void ShowResolve(Color c)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StaminaRow.Height = GridLength.Auto;
                stBar.Fill = new SolidColorBrush(c);
                UpdateST(0);

            }));
        }
        internal void HideResolve()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StaminaRow.Height = new GridLength(0);
            }));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void D_Tick(object sender, EventArgs e)
        {
            //UpdateHP(currHp);
            //currHp += 3000;
        }

        //private void SetMaxHP(int statValue)
        //{
        //    MaxHP = statValue;
        //    Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        hpMaxTB.Text = statValue.ToString();
        //    }));
        //}
        //private void SetMaxMP(int statValue)
        //{
        //    MaxMP = statValue;
        //    Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        mpMaxTB.Text = statValue.ToString();
        //    }));

        //}
        //private void SetMaxST(int statValue)
        //{
        //    MaxST = statValue;
        //    Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        stMaxTB.Text = statValue.ToString();
        //    }));
        //}

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
            Properties.Settings.Default.CharacterWindowLeft = Left;
            Properties.Settings.Default.CharacterWindowTop = Top;
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
                UpdateHP(0);
                UpdateMP(0);
                UpdateST(0);
                //MaxHP = 0;
                //MaxMP = 0;
                //MaxST = 0;
                //CurrentLevel = 1;
            }));
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }
    }
}
namespace TCC.Converters
{
    public class LaurelImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Laurel l = (Laurel)value;
            //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(50, 50);
            string laurel = "";
            switch (l)
            {
                case Laurel.None:
                    laurel = "blank";
                    break;
                case Laurel.Bronze:
                    laurel = "bronze";
                    break;
                case Laurel.Silver:
                    laurel = "silver";
                    break;
                case Laurel.Gold:
                    laurel = "gold";
                    break;
                case Laurel.Diamond:
                    laurel = "diamond";
                    break;
                case Laurel.Champion:
                    laurel = "champion";
                    break;
            }
            // return new ImageBrush(CharacterWindow.Bitmap2BitmapImage(bitmap));
            return "/resources/images/Icon_Laurels/" + laurel + ".png";
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ClassImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Class c = (Class)value;
            //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(50, 50);
            string className = "common";
            switch (c)
            {
                case Class.Warrior:
                    className = "warrior";
                    break;
                case Class.Lancer:
                    className = "lancer";
                    break;
                case Class.Slayer:
                    className = "slayer";
                    break;
                case Class.Berserker:
                    className = "berserker";
                    break;
                case Class.Sorcerer:
                    className = "sorcerer";
                    break;
                case Class.Archer:
                    className = "archer";
                    break;
                case Class.Priest:
                    className = "priest";
                    break;
                case Class.Elementalist:
                    className = "mystic";
                    break;
                case Class.Soulless:
                    className = "reaper";
                    break;
                case Class.Engineer:
                    className = "gunner";
                    break;
                case Class.Fighter:
                    className = "brawler";
                    break;
                case Class.Assassin:
                    className = "ninja";
                    break;
                case Class.Glaiver:
                    className = "glaiver";
                    break;
                default:
                    className = "common";
                    break;

            }
            //return new ImageBrush(CharacterWindow.Bitmap2BitmapImage(bitmap));
            return "/resources/images/Icon_Classes/" + className + ".png";
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class CombatToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool c = (bool)value;

            if (c)
            {
                return new SolidColorBrush(Colors.Red);
            }
            else
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class HP_PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float hp = (float)value;
            if(SessionManager.CurrentPlayer.MaxHP > 0)
            {
                return hp / SessionManager.CurrentPlayer.MaxHP;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HPbarColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return new SolidColorBrush(Color.FromRgb(0xb7, 0x4b, 0xe5));
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(0xd0, 0, 0));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
