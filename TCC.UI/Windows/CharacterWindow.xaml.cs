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
        static int MaxHP, MaxMP, MaxST = 0;
        static int MaxFlightEnergy = 1000;

        Class currentClass = Class.None;
        public Class CurrentClass
        {
            get => currentClass;
            set
            {
                if (value != currentClass)
                {
                    currentClass = value;
                    NotifyPropertyChanged("CurrentClass");
                }
            }
        }

        string currentName;
        public string CurrentName
        {
            get => currentName;
            set
            {
                if (value != currentName)
                {
                    currentName = value;
                    NotifyPropertyChanged("CurrentName");
                }
            }
        }

        Laurel currentLaurel = Laurel.None;
        public Laurel CurrentLaurel
        {
            get => currentLaurel;
            set
            {
                if (value != currentLaurel)
                {
                    currentLaurel = value;
                    NotifyPropertyChanged("CurrentLaurel");
                }
            }
        }

        private int currentLevel;
        public int CurrentLevel
        {
            get => currentLevel; 
            set
            {
                if (currentLevel != value)
                {
                    currentLevel = value;
                    NotifyPropertyChanged("CurrentLevel");
                }

            }
        }

        private int currentIlvl;
        public int CurrentIlvl
        {
            get => currentIlvl;
            set
            {
                if (value != currentIlvl)
                {
                    currentIlvl = value;
                    NotifyPropertyChanged("CurrentIlvl");
                }
            }
        }

        private void NotifyPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public CharacterWindow()
        {
            InitializeComponent();

            _doubleAnimation.EasingFunction = new QuadraticEase();
            _doubleAnimation.Duration = TimeSpan.FromMilliseconds(AnimationTime);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(hwnd);
            FocusManager.HideFromToolBar(hwnd);
            if (Properties.Settings.Default.Transparent)
            {
                FocusManager.MakeTransparent(hwnd);
            }

            this.Top = Properties.Settings.Default.CharacterWindowTop;
            this.Left = Properties.Settings.Default.CharacterWindowLeft;

            PacketRouter.HPUpdated += UpdateHP;
            PacketRouter.MPUpdated += UpdateMP;
            PacketRouter.STUpdated += UpdateST;
            PacketRouter.FlightEnergyUpdated += UpdateFlightEnergy;

            PacketRouter.IlvlUpdated += UpdateIlvl;

            PacketRouter.MaxHPUpdated += SetMaxHP;
            PacketRouter.MaxMPUpdated += SetMaxMP;
            PacketRouter.MaxSTUpdated += SetMaxST;
            //PacketRouter.MaxFlightEnergyUpdated += SetMaxFlightEnergy;

            hpBar.Width = 0;
            mpBar.Width = 0;
            stBar.Width = 0;

            Binding classBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath("CurrentClass"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Converter = new ClassImageConverter(),
            };
            Binding laurelBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath("CurrentLaurel"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Converter = new LaurelImageConverter()
            };
            Binding nameBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath("CurrentName"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            Binding levelBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath("CurrentLevel"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            Binding ilvlBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath("CurrentIlvl"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };

            classIcon.SetBinding(Rectangle.OpacityMaskProperty, classBinding);
            laurel.SetBinding(Rectangle.FillProperty, laurelBinding);
            nameTB.SetBinding(TextBlock.TextProperty, nameBinding);
            levelTB.SetBinding(TextBlock.TextProperty, levelBinding);
            ilvlTB.SetBinding(TextBlock.TextProperty, ilvlBinding);

            var d = new DispatcherTimer();
            d.Interval = TimeSpan.FromMilliseconds(333);
            d.Tick += D_Tick;
            //d.Start();

        }

        private void UpdateIlvl(int statValue)
        {
            CurrentIlvl = statValue;
        }

        internal void ShowResolve()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StaminaRow.Height = GridLength.Auto;
            }));
        }
        internal void HideResolve()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                StaminaRow.Height = new GridLength(0);
            }));
        }

        int currHp = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        private void D_Tick(object sender, EventArgs e)
        {
            UpdateHP(currHp);
            currHp += 3000;
        }

        private void SetMaxHP(int statValue)
        {
            MaxHP = statValue;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                hpMaxTB.Text = statValue.ToString();
            }));
        }
        private void SetMaxMP(int statValue)
        {
            MaxMP = statValue;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                mpMaxTB.Text = statValue.ToString();
            }));

        }
        private void SetMaxST(int statValue)
        {
            MaxST = statValue;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                stMaxTB.Text = statValue.ToString();
            }));
        }

        private void UpdateHP(int newValue)
        {
            Dispatcher.BeginInvoke(new Action(() =>  
            {
                _doubleAnimation.To = ValueToLength(newValue, MaxHP);
                hpBar.BeginAnimation(WidthProperty, _doubleAnimation);

                hpTB.Text = newValue.ToString();
                if (MaxHP != 0)
                {

                    var perc = 100 * newValue / MaxHP;
                    hpPercTB.Text = perc + "%";
                }
                else
                {
                    hpPercTB.Text = "-";
                }
            }));
        }
        private void UpdateMP(int newValue)
        {
            Dispatcher.BeginInvoke(new Action(() => 
            {
                _doubleAnimation.To = ValueToLength(newValue, MaxMP);
                mpBar.BeginAnimation(WidthProperty, _doubleAnimation);

                mpTB.Text = newValue.ToString();
            }));

        }
        private void UpdateST(int newValue)
        {
            Dispatcher.BeginInvoke(new Action(() => 
            {
                _doubleAnimation.To = ValueToLength(newValue, MaxST);
                stBar.BeginAnimation(WidthProperty, _doubleAnimation);

                stTB.Text = newValue.ToString();
            }));
        }

        private void UpdateFlightEnergy(double newValue)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _doubleAnimation.To = dummyFlightBar.ActualWidth * newValue / 1000;
                flightBar.BeginAnimation(WidthProperty, _doubleAnimation);
            }));
        }
        private void SetMaxFlightEnergy(int statValue)
        {
            MaxFlightEnergy = statValue;
        }


        //private DoubleAnimation BarAnimation(double value)
        //{
        //    return new DoubleAnimation(value, TimeSpan.FromMilliseconds(AnimationTime)) { EasingFunction = new QuadraticEase() };
        //}
        DoubleAnimation _doubleAnimation = new DoubleAnimation();
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Properties.Settings.Default.CharacterWindowLeft = Left;
            Properties.Settings.Default.CharacterWindowTop = Top;
        }

        double ValueToLength(double value, int maxValue)
        {
            if(maxValue == 0)
            {
                return 0;
            }
            else
            {
                double n = @base.ActualWidth * ((double)value / (double)maxValue);
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
                MaxHP = 0;
                MaxMP = 0;
                MaxST = 0;
                CurrentLaurel = Laurel.None;
                CurrentName = "";
                CurrentLevel = 1;
                CurrentClass = Class.None;
            }));
        }
    }
    public class LaurelImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Laurel l = (Laurel)value;
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(50, 50);

            switch (l)
            {
                case Laurel.None:
                    break;
                case Laurel.Bronze:
                    bitmap = Properties.Resources.bronze;
                    break;
                case Laurel.Silver:
                    bitmap = Properties.Resources.silver;

                    break;
                case Laurel.Gold:
                    bitmap = Properties.Resources.gold;

                    break;
                case Laurel.Diamond:
                    bitmap = Properties.Resources.diamond;

                    break;
                case Laurel.Champion:
                    bitmap = Properties.Resources.champion;

                    break;
                default:
                    break;
            }
            return new ImageBrush(Bitmap2BitmapImage(bitmap));
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

        private BitmapImage Bitmap2BitmapImage(System.Drawing.Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

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
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(50, 50);
            switch (c)
            {
                case Class.Warrior:
                    bitmap = Properties.Resources.warrior;
                    break;
                case Class.Lancer:
                    bitmap = Properties.Resources.lancer;
                    break;
                case Class.Slayer:
                    bitmap = Properties.Resources.slayer;
                    break;
                case Class.Berserker:
                    bitmap = Properties.Resources.berserker;
                    break;
                case Class.Sorcerer:
                    bitmap = Properties.Resources.sorcerer;
                    break;
                case Class.Archer:
                    bitmap = Properties.Resources.archer;
                    break;
                case Class.Priest:
                    bitmap = Properties.Resources.priest;
                    break;
                case Class.Elementalist:
                    bitmap = Properties.Resources.mystic;
                    break;
                case Class.Soulless:
                    bitmap = Properties.Resources.reaper;
                    break;
                case Class.Engineer:
                    bitmap = Properties.Resources.gunner;
                    break;
                case Class.Fighter:
                    bitmap = Properties.Resources.brawler;
                    break;
                case Class.Assassin:
                    bitmap = Properties.Resources.ninja;
                    break;
                case Class.Moon_Dancer:
                    bitmap = Properties.Resources.glaiver;
                    break;
                default:
                    break;

            }
            return new ImageBrush(Bitmap2BitmapImage(bitmap));
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

        private BitmapImage Bitmap2BitmapImage(System.Drawing.Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
