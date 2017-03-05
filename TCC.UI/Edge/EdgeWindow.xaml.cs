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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TCC
{

    public partial class EdgeWindow : Window
    {
        DispatcherTimer t;
        Timer ExpireEdge;
        static Color arcBaseColor = Colors.Orange;
        public static int spawnTime = 100;
        List<EdgeArc> edgeArcs;
        public static EdgeWindow Instance;
        public EdgeWindow()
        {
            Instance = this;
            InitializeComponent();

            Left = Properties.Settings.Default.ELeft;
            Top = Properties.Settings.Default.ETop;

            edgeArcs = new List<EdgeArc>();
            for (int i = 0; i < 10; i++)
            {
                var a = new EdgeArc();
                a.arc.RenderTransform = new RotateTransform(i * 36);
                a.arc.Stroke = new SolidColorBrush(arcBaseColor);
                edgeArcs.Add(a);
            }
            foreach (var item in edgeArcs)
            {
                MainGrid.Children.Add(item);
            }

            ExpireEdge = new Timer(8000);
            ExpireEdge.Elapsed += (s, o) => { SetEdge(0); };

            t = new DispatcherTimer();
            t.Interval = TimeSpan.FromSeconds(.5);
            t.Tick += T_Tick;
            //t.Start();



        }
        static int i = 0;
        private void T_Tick(object sender, EventArgs e)
        {
            if(i == 10)
            {
                i = 0;
            }
            else
            {
                i++;
            }
            SetEdge(i);

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


        public static void SetEdge(int edge)
        {
            
            EdgeWindow.Instance.Dispatcher.Invoke(() =>
            {
                Instance.ExpireEdge.Stop();
                if (edge == 0)
                {
                    var a = new ColorAnimation(arcBaseColor, TimeSpan.FromMilliseconds(spawnTime));
                    var sb = new SolidColorBrush(Colors.Red);
                    foreach (var item in Instance.edgeArcs)
                    {
                        item.IsBuilt = false;

                        item.arc.Stroke = sb;
                        sb.BeginAnimation(SolidColorBrush.ColorProperty, a);

                    }
                }
                else if(edge == 10)
                {
                    var a = new ColorAnimation(Colors.Red, TimeSpan.FromMilliseconds(spawnTime));
                    var sb = new SolidColorBrush( arcBaseColor);
                    foreach (var item in Instance.edgeArcs)
                    {
                        if (!item.IsBuilt)
                        {
                            item.IsBuilt = true;
                        }
                        item.arc.Stroke = sb;
                        sb.BeginAnimation(SolidColorBrush.ColorProperty, a);
                        
                    }
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
                Instance.num.Text = edge.ToString();
                Instance.ExpireEdge = new Timer(8000);
                Instance.ExpireEdge.Elapsed += (s, o) => { SetEdge(0); };
                Instance.ExpireEdge.Enabled = true;
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(hwnd);
            FocusManager.HideFromToolBar(hwnd);
        }
    }

    public class RotOffset
    {
        public double Rot { get; set; }
        public RotOffset(double r)
        {
            Rot = r;
        }
    }

}
