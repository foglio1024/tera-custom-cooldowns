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

namespace TCC
{
    /// <summary>
    /// Logica di interazione per HPbar.xaml
    /// </summary>
    public partial class HPWindow : Window, INotifyPropertyChanged
    {
        static int AnimationTime = 100;
        static int MaxHP, MaxMP, MaxST = 0;

        Class currentClass = Class.None;
        public Class CurrentClass { get => currentClass;
            set
            {
                if (value != currentClass)
                {
                    currentClass = value;
                    NotifyPropertyChanged("CurrentClass");
                }
            }
        }

        private void NotifyPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public HPWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(hwnd);
            FocusManager.HideFromToolBar(hwnd);

            this.Top = Properties.Settings.Default.HPBarTop;
            this.Left = Properties.Settings.Default.HPBarLeft;

            PacketParser.HPUpdated += UpdateHP;
            PacketParser.MPUpdated += UpdateMP;
            PacketParser.STUpdated += UpdateST;

            PacketParser.MaxHPUpdated += SetMaxHP;
            PacketParser.MaxMPUpdated += SetMaxMP;
            PacketParser.MaxSTUpdated += SetMaxST;

            hpBar.Width = 0;
            mpBar.Width = 0;
            stBar.Width = 0;

            Binding b = new Binding
            {
                Source = this,
                Path = new PropertyPath("CurrentClass"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Converter = new ImageConverter(),
            };

            classIcon.SetBinding(Rectangle.OpacityMaskProperty, b);

            var d = new DispatcherTimer();
            d.Interval = TimeSpan.FromMilliseconds(333);
            d.Tick += D_Tick;
            //d.Start();

        }

        internal void ShowStamina()
        {
            Dispatcher.Invoke(() => {
                StaminaRow.Height = GridLength.Auto;
            });
        }
        internal void HideStamina()
        {
            Dispatcher.Invoke(() =>
            {
                StaminaRow.Height = new GridLength(0);
            });
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
            Dispatcher.Invoke(() => {
            hpMaxTB.Text = statValue.ToString();
            });
        }
        private void SetMaxMP(int statValue)
        {
            MaxMP = statValue;
            Dispatcher.Invoke(() => {
            mpMaxTB.Text = statValue.ToString();
            });

        }
        private void SetMaxST(int statValue)
        {
            MaxST = statValue;
            Dispatcher.Invoke(() => {
            stMaxTB.Text = statValue.ToString();
            });
        }

        private void UpdateHP(int newValue)
        {
            Dispatcher.Invoke(() => {
                hpBar.BeginAnimation(WidthProperty, BarAnimation(ValueToLength(newValue, MaxHP)));
            hpTB.Text = newValue.ToString();
            });
        }
        private void UpdateMP(int newValue)
        {
            Dispatcher.Invoke(() => {
            mpBar.BeginAnimation(WidthProperty, BarAnimation(ValueToLength(newValue, MaxMP)));
            mpTB.Text = newValue.ToString();
            });

        }
        private void UpdateST(int newValue)
        {
            Dispatcher.Invoke(() => {
            stBar.BeginAnimation(WidthProperty, BarAnimation(ValueToLength(newValue, MaxST)));
            stTB.Text = newValue.ToString();
            });
        }

        private DoubleAnimation BarAnimation(double value)
        {
            return new DoubleAnimation(value, TimeSpan.FromMilliseconds(AnimationTime)) { EasingFunction = new QuadraticEase() };
        }

        double ValueToLength(int value, int maxValue)
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
            Dispatcher.Invoke(() =>
            {
                UpdateHP(0);
                UpdateMP(0);
                UpdateST(0);
                MaxHP = 0;
                MaxMP = 0;
                MaxST = 0;
            });
        }
    }

    public class ImageConverter : IValueConverter
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
