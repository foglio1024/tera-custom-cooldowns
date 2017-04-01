using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TCC
{
    public delegate void MaxEdge();
    public delegate void EdgeReset();
    public partial class EdgeGaugeWindow : Window
    {
        public static EdgeGaugeWindow Instance;

        //DispatcherTimer TestTimer;
        Timer ExpireEdge;
        Timer HideTimer;
        public static bool Visible { get; set; }

        public event MaxEdge MaxedEdge;
        public event EdgeReset NormalEdge;

        ObservableCollection<EdgeArc> edgeArcs;

        static Color arcBaseColor = Color.FromArgb(0xff,0xff,0xbf,0);
        public static int spawnTime = 50;



        public int CurrentEdge
        {
            get { return (int)GetValue(CurrentEdgeProperty); }
            set { SetValue(CurrentEdgeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentEdge.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentEdgeProperty =
            DependencyProperty.Register("CurrentEdge", typeof(int), typeof(EdgeGaugeWindow));



        public EdgeGaugeWindow()
        {
            Instance = this;
            InitializeComponent();

            Warrior.Scythed += StartScytheCooldown;
            Warrior.GambleBuff += StartGambleBuff;
            Warrior.GambleCooldown += StartGambleCooldown;

            Left = Properties.Settings.Default.GaugeWindowLeft;
            Top = Properties.Settings.Default.GaugeWindowTop;

            edgeArcs = new ObservableCollection<EdgeArc>();
            ArcsGrid.DataContext = edgeArcs;
            ArcsGrid.ItemsSource = edgeArcs;
            num.DataContext = this;
            scytheCd.Stroke = new SolidColorBrush(Colors.White);

            for (int i = 0; i < 10; i++)
            {
                var a = new EdgeArc();
                a.arc.RenderTransform = new RotateTransform(i * 36);
                edgeArcs.Add(a);
            }

            //foreach (var item in edgeArcs)
            //{
            //    ArcsGrid.Children.Add(item);
            //}

            ExpireEdge = new Timer(7000);
            ExpireEdge.Elapsed += (s, o) => { SetEdge(0); };

            //TestTimer = new DispatcherTimer()
            //{
            //    Interval = TimeSpan.FromSeconds(1)
            //};
            //TestTimer.Tick += TestTimer_Tick;
            //TestTimer.Start();
        }

        private void StartGambleCooldown(int cd)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                gambleCd.BeginAnimation(Arc.EndAngleProperty, EdgeAnimations.GetArcAnimation(cd));
            }));

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

        }

        //static int testEdge = 0;
        //private void TestTimer_Tick(object sender, EventArgs e)
        //{
        //    if(testEdge == 10)
        //    {
        //        testEdge = 0;
        //        StartScytheCooldown(3000);
        //        StartGambleBuff(7000);
        //        StartGambleCooldown(9000);
        //    }
        //    else
        //    {
        //        testEdge++;
        //    }
        //    SetEdge(testEdge);
        //}

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        int oldEdge = 0;
        public static void SetEdge(int edge)
        {
            
            EdgeGaugeWindow.Instance.Dispatcher.Invoke(() =>
            {
                Instance.ExpireEdge.Stop();
                if (edge == 0)
                {
                    foreach (var item in Instance.edgeArcs)
                    {
                        item.IsBuilt = false;
                    }
                    if(Instance.oldEdge == 10)
                    {
                        Instance.NormalEdge?.Invoke();
                    }
                    Instance.baseEll.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, EdgeAnimations.GetColorAnimation(Color.FromArgb(0x00,0,0,0)));
                    Instance.glow.BeginAnimation(DropShadowEffect.OpacityProperty, EdgeAnimations.ShadowOpacityAnimationDown);

                }
                else if(edge == 10)
                {
                    foreach (var item in Instance.edgeArcs)
                    {
                        if (!item.IsBuilt)
                        {
                            item.IsBuilt = true;
                        }
                        Instance.MaxedEdge?.Invoke();
                    }
                    Instance.glow.BeginAnimation(DropShadowEffect.OpacityProperty, EdgeAnimations.ShadowOpacityAnimationUp);
                    Instance.baseEll.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, EdgeAnimations.GetColorAnimation(Colors.Red));

                }
                else
                {
                    for (int i = 0; i < edge; i++)
                    {
                        if (!Instance.edgeArcs[i].IsBuilt)
                        {
                            Instance.edgeArcs[i].IsBuilt = true;
                        }
                    }
                }
                Instance.CurrentEdge = edge;
                Instance.ExpireEdge = new Timer(8000);
                Instance.ExpireEdge.Elapsed += (s, o) => { SetEdge(0); };
                Instance.ExpireEdge.Enabled = true;
            });

            Instance.oldEdge = edge;

        }

        void StartScytheCooldown(int cd)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                scytheCd.Stroke.BeginAnimation(SolidColorBrush.ColorProperty, EdgeAnimations.GetColorAnimation(Colors.Red, Colors.White, Convert.ToInt32(cd*.1)));
                scytheCd.BeginAnimation(Arc.EndAngleProperty, EdgeAnimations.GetArcAnimation(cd));
            }));
        }

        void StartGambleBuff(int duration)
        {
            Dispatcher.BeginInvoke(new Action(() => 
            {
                gambleDuration.BeginAnimation(Arc.EndAngleProperty, EdgeAnimations.GetArcAnimation(duration));
            }));
        }

        static class EdgeAnimations
        {
            public static DoubleAnimation ShadowOpacityAnimationUp = new DoubleAnimation(1, TimeSpan.FromMilliseconds(spawnTime));
            public static DoubleAnimation ShadowOpacityAnimationDown = new DoubleAnimation(0, TimeSpan.FromMilliseconds(spawnTime));

            public static ColorAnimation GetColorAnimation(Color c)
            {
                return new ColorAnimation(c, TimeSpan.FromMilliseconds(spawnTime));
            }
            public static ColorAnimation GetColorAnimation(Color start, Color end, int time)
            {
                return new ColorAnimation(start,end, TimeSpan.FromMilliseconds(time));
            }
            public static DoubleAnimation GetArcAnimation(int time)
            {
                return new DoubleAnimation(359.9, 0, TimeSpan.FromMilliseconds(time));
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Properties.Settings.Default.GaugeWindowLeft = Left;
            Properties.Settings.Default.GaugeWindowTop = Top;
        }
    }
}
