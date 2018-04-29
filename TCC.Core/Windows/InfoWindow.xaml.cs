using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Controls;
using TCC.ViewModels;

namespace TCC.Windows
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        private System.Timers.Timer t;
        private int itemIndex = 0;
        private int icIndex = 0;
        private List<ItemsControl> ICs;
        private ColorAnimation mainBorderfadeIn, mainBorderFadeOut;
        private Color borderFull, borderTransp;
        public InfoWindow()
        {
            InitializeComponent();
            t = new System.Timers.Timer(30);
            t.Elapsed += AnimateNextItem;
            var animTime = TimeSpan.FromMilliseconds(300);
            _expW = new DoubleAnimation(0, animTime) { EasingFunction = new QuadraticEase() };
            _expH = new DoubleAnimation(0, animTime) { EasingFunction = new QuadraticEase() };
            _expM = new ThicknessAnimation(new Thickness(0), animTime) { EasingFunction = new QuadraticEase() };
            _expO = new DoubleAnimation(0, 1, animTime) { BeginTime = TimeSpan.FromMilliseconds(300) };
            borderFull = Color.FromArgb(0xff, 0xf0, 0xf0, 0xf0);
            borderTransp = Color.FromArgb(0x00, 0xf0, 0xf0, 0xf0);
            mainBorderfadeIn = new ColorAnimation(borderTransp, borderFull, TimeSpan.FromMilliseconds(200));
            mainBorderFadeOut = new ColorAnimation(borderFull, borderTransp, TimeSpan.FromMilliseconds(100)) { BeginTime = animTime - TimeSpan.FromMilliseconds(50) };
            //_expO.Completed += (s1, ev1) => { b.Visibility = Visibility.Collapsed; _expO.BeginAnimation(OpacityProperty, new DoubleAnimation(1, TimeSpan.FromMilliseconds(1))); };

        }

        private void AnimateNextItem(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (icIndex >= ICs.Count)
            {
                t.Stop();
                icIndex = 0;
                itemIndex = 0;
                return;
            }
            if (itemIndex < ICs[icIndex].Items.Count)
            {
                Dispatcher.Invoke(() =>
                {

                    var container = ICs[icIndex].ItemContainerGenerator.ContainerFromIndex(itemIndex);
                    if (container != null)
                    {
                        var dgc = Utils.GetChild<DungeonInfoControl>(container);
                        if (dgc != null) dgc.AnimateIn();
                    }

                });
                itemIndex++;
            }
            else
            {
                icIndex++;
                itemIndex = 0;
            }

        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            InfoWindowViewModel.Instance.SaveToFile();
            var a = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));
            a.Completed += (s, ev) => { Hide(); InfoWindowViewModel.Instance.SaveToFile(); };
            this.BeginAnimation(OpacityProperty, a);
            //this.Hide();
        }
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception)
            {


            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var _handle = new WindowInteropHelper(this).Handle;
            FocusManager.HideFromToolBar(_handle);
        }
        internal void ShowWindow()
        {
            Dispatcher.Invoke(() =>
            {
                Topmost = false; Topmost = true;
                Opacity = 0;
                Show();
                Activate();
                BeginAnimation(Window.OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200)));
            });
        }

        public void AnimateICitems()
        {
            //icIndex = 0;
            //itemIndex = 0;

            //ICs = new List<ItemsControl>();
            //foreach (ItemsControl ic in dngICs.Children)
            //{
            //    if(ic.Visibility == Visibility.Visible) ICs.Add(ic);
            //}
            //Task.Delay(50).ContinueWith(task => t.Start());
        }

        private DoubleAnimation _expW, _expH, _expO;

        private ThicknessAnimation _expM;
        //internal void ExpandCharacter(Point relativePoint, double actualWidth, double actualHeight)
        //{
        //    relPoint = relativePoint;
        //    b.Width = actualWidth;
        //    b.Height = actualHeight;
        //    var startThickness = new Thickness(relativePoint.X - 15, relativePoint.Y - 15, 0, 0);
        //    w = actualWidth; h = actualHeight;
        //    var endThickness = new Thickness(10);
        //    var endH = g2.ActualHeight - 20;
        //    var endW = g2.ActualWidth - 20;
        //    _expW.To = endW;
        //    _expH.To = endH;
        //    _expM.To = endThickness;
        //    _expM.From = startThickness;
        //    _expO.To = 1;
        //    _expO.From = 0;

        //    b.BeginAnimation(WidthProperty, _expW);
        //    b.BeginAnimation(HeightProperty, _expH);
        //    b.BeginAnimation(MarginProperty, _expM);
        //    (b.Child as FrameworkElement).BeginAnimation(OpacityProperty, _expO);
        //    //mainBorder.Background.BeginAnimation(SolidColorBrush.ColorProperty, mainBorderFadeOut);
        //    Task.Delay(300).ContinueWith(t => Dispatcher.Invoke(() => (b.Child as FrameworkElement).Visibility = Visibility.Visible));
        //    mainBorder.BeginAnimation(MarginProperty, new ThicknessAnimation(new Thickness(20), TimeSpan.FromMilliseconds(500))
        //    {
        //        BeginTime = TimeSpan.FromMilliseconds(300),
        //        EasingFunction = new QuadraticEase()
        //    });

        //    b.Opacity = 1;
        //    b.Visibility = Visibility.Visible;
        //}

        private UniformGrid GetInnerUniformGrid(FrameworkElement element)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child == null) continue;


                if (child is UniformGrid) return child as UniformGrid;

                var panel = GetInnerUniformGrid(child);

                if (panel != null)
                    return panel;
            }

            return null;

        }

        //private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    ExpandCharacter(Mouse.GetPosition(this), 50, 50);
        //}
        private Point relPoint = new Point();

        private double w, h = 0;
        //private void b_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    b.BeginAnimation(WidthProperty, new DoubleAnimation(w, TimeSpan.FromMilliseconds(150))
        //    {
        //        EasingFunction = new QuarticEase(),
        //        BeginTime = TimeSpan.FromMilliseconds(150)
        //    });
        //    b.BeginAnimation(HeightProperty, new DoubleAnimation(h, TimeSpan.FromMilliseconds(150))
        //    {
        //        EasingFunction = new QuarticEase(),
        //        BeginTime = TimeSpan.FromMilliseconds(150)

        //    });
        //    b.BeginAnimation(MarginProperty, new ThicknessAnimation(new Thickness(relPoint.X - 15, relPoint.Y - 15, 0, 0), 
        //        TimeSpan.FromMilliseconds(150))
        //    {
        //        EasingFunction = new QuarticEase(),
        //        BeginTime = TimeSpan.FromMilliseconds(150)
        //    });
        //    (b.Child as FrameworkElement).BeginAnimation(OpacityProperty, 
        //        new DoubleAnimation (1 , 0 ,TimeSpan.FromMilliseconds(150)));
        //    //mainBorder.Background.BeginAnimation(SolidColorBrush.ColorProperty, mainBorderfadeIn);
        //    mainBorder.BeginAnimation(MarginProperty, new ThicknessAnimation(new Thickness(0), TimeSpan.FromMilliseconds(150)) { EasingFunction = new QuadraticEase() });
        //    Task.Delay(300).ContinueWith(t => Dispatcher.Invoke(() => b.Visibility = Visibility.Collapsed));
        //    Task.Delay(150).ContinueWith(t => Dispatcher.Invoke(() => b.Child.Visibility = Visibility.Collapsed));

        //}
    }
}
