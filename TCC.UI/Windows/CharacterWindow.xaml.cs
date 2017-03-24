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
        int MaxHP, MaxMP, MaxST = 0;
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

        private float currentHP;
        public float CurrentHP
        {
            get
            {
                return currentHP;
            }
            set
            {
                if(currentHP != value)
                {
                    currentHP = value;
                    NotifyPropertyChanged("CurrentHP");
                }
            }
        }
        private float currentMP;
        public float CurrentMP
        {
            get
            {
                return currentMP;
            }
            set
            {
                if (currentMP != value)
                {
                    currentMP = value;
                    NotifyPropertyChanged("CurrentMP");
                }
            }
        }
        private float currentST;
        public float CurrentST
        {
            get
            {
                return currentST;
            }
            set
            {
                if (currentST != value)
                {
                    currentST = value;
                    NotifyPropertyChanged("CurrentST");
                }
            }
        }

        double percentageHP;
        public double PercentageHP
        {
            get { return percentageHP; }
            set
            {
                if(percentageHP != value)
                {
                    percentageHP = value;
                    NotifyPropertyChanged("PercentageHP");
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
            this.DataContext = this;
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

            hpBar.RenderTransform = new ScaleTransform(1, 1, 0, .5);
            mpBar.RenderTransform = new ScaleTransform(1, 1, 0, .5);
            stBar.RenderTransform = new ScaleTransform(1, 1, 0, .5);
            flightBar.RenderTransform = new ScaleTransform(1, 1, 0, .5);

            hpBar.Width = @base.Width;
            mpBar.Width = @base.Width;
            stBar.Width = @base.Width;
            flightBar.Width = dummyFlightBar.Width;

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
            //Binding nameBinding = new Binding
            //{
            //    Source = this,
            //    Path = new PropertyPath("CurrentName"),
            //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            //};
            //Binding levelBinding = new Binding
            //{
            //    Source = this,
            //    Path = new PropertyPath("CurrentLevel"),
            //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            //};
            //Binding ilvlBinding = new Binding
            //{
            //    Source = this,
            //    Path = new PropertyPath("CurrentIlvl"),
            //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            //};

            classIcon.SetBinding(Rectangle.OpacityMaskProperty, classBinding);
            laurel.SetBinding(Rectangle.FillProperty, laurelBinding);
            //nameTB.SetBinding(TextBlock.TextProperty, nameBinding);
            //levelTB.SetBinding(TextBlock.TextProperty, levelBinding);
            //ilvlTB.SetBinding(TextBlock.TextProperty, ilvlBinding);

            

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


        public event PropertyChangedEventHandler PropertyChanged;

        private void D_Tick(object sender, EventArgs e)
        {
            //UpdateHP(currHp);
            //currHp += 3000;
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

        private void UpdateHP(float newValue)
        {
            CurrentHP = newValue;
            PercentageHP = newValue / MaxHP;
            Dispatcher.BeginInvoke(new Action(() =>  
            {
                hpBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(ValueToLength((double)newValue, (double)MaxHP), TimeSpan.FromMilliseconds(AnimationTime)) { EasingFunction = new QuadraticEase() });

                //if (MaxHP != 0)
                //{
                //    var perc = 100 * newValue / MaxHP;
                //    hpPercTB.Text = String.Format("{0:0.0}%", perc);
                //}
                //else
                //{
                //    hpPercTB.Text = "-";
                //}
            }));
        }
        private void UpdateMP(int newValue)
        {
            CurrentMP = newValue;
            Dispatcher.BeginInvoke(new Action(() => 
            {
                //_doubleAnimation.To = ValueToLength(newValue, MaxMP);
                mpBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(ValueToLength((double)newValue, (double)MaxMP), TimeSpan.FromMilliseconds(AnimationTime)) { EasingFunction = new QuadraticEase() });
                //mpTB.Text = newValue.ToString();
            }));

        }
        private void UpdateST(int newValue)
        {
            CurrentST = newValue;
            Dispatcher.BeginInvoke(new Action(() => 
            {
                //_doubleAnimation.To = ValueToLength(newValue, MaxST);
                stBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(ValueToLength((double)newValue, (double)MaxST), TimeSpan.FromMilliseconds(AnimationTime)) { EasingFunction = new QuadraticEase() });
                //stTB.Text = newValue.ToString();
            }));
        }

        private void UpdateFlightEnergy(float newValue)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                flightBar.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(ValueToLength((double)newValue, 1000), TimeSpan.FromMilliseconds(AnimationTime)) { EasingFunction = new QuadraticEase() });
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
            if(maxValue == 0)
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
                MaxHP = 0;
                MaxMP = 0;
                MaxST = 0;
                CurrentLaurel = Laurel.None;
                CurrentName = "";
                CurrentLevel = 1;
                CurrentClass = Class.None;
            }));
        }
        public static BitmapImage Bitmap2BitmapImage(System.Drawing.Bitmap bitmap)
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
                    bitmap = Properties.Icon_Laurels.bronze;
                    break;
                case Laurel.Silver:
                    bitmap = Properties.Icon_Laurels.silver;

                    break;
                case Laurel.Gold:
                    bitmap = Properties.Icon_Laurels.gold;

                    break;
                case Laurel.Diamond:
                    bitmap = Properties.Icon_Laurels.diamond;

                    break;
                case Laurel.Champion:
                    bitmap = Properties.Icon_Laurels.champion;

                    break;
                default:
                    break;
            }
            return new ImageBrush(CharacterWindow.Bitmap2BitmapImage(bitmap));
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
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(50, 50);
            switch (c)
            {
                case Class.Warrior:
                    bitmap = Properties.Icon_Classes.warrior;
                    break;
                case Class.Lancer:
                    bitmap = Properties.Icon_Classes.lancer;
                    break;
                case Class.Slayer:
                    bitmap = Properties.Icon_Classes.slayer;
                    break;
                case Class.Berserker:
                    bitmap = Properties.Icon_Classes.berserker;
                    break;
                case Class.Sorcerer:
                    bitmap = Properties.Icon_Classes.sorcerer;
                    break;
                case Class.Archer:
                    bitmap = Properties.Icon_Classes.archer;
                    break;
                case Class.Priest:
                    bitmap = Properties.Icon_Classes.priest;
                    break;
                case Class.Elementalist:
                    bitmap = Properties.Icon_Classes.mystic;
                    break;
                case Class.Soulless:
                    bitmap = Properties.Icon_Classes.reaper;
                    break;
                case Class.Engineer:
                    bitmap = Properties.Icon_Classes.gunner;
                    break;
                case Class.Fighter:
                    bitmap = Properties.Icon_Classes.brawler;
                    break;
                case Class.Assassin:
                    bitmap = Properties.Icon_Classes.ninja;
                    break;
                case Class.Moon_Dancer:
                    bitmap = Properties.Icon_Classes.glaiver;
                    break;
                default:
                    break;

            }
            return new ImageBrush(CharacterWindow.Bitmap2BitmapImage(bitmap));
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
