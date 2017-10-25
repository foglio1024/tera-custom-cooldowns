using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        System.Timers.Timer t;
        int itemIndex = 0;
        int icIndex = 0;
        List<ItemsControl> ICs;
        public InfoWindow()
        {
            InitializeComponent();
            t = new System.Timers.Timer(30);
            t.Elapsed += AnimateNextItem;
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
                        DungeonInfoControl dgc = Utils.GetChild<DungeonInfoControl>(container);
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
                //AnimateICitems();
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
        private UniformGrid GetInnerUniformGrid(FrameworkElement element)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
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

    }
}
