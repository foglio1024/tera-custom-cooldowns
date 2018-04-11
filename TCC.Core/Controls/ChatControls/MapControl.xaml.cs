using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Data;
using TCC.Data.Databases;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl : UserControl
    {
        public MapControl()
        {
            InitializeComponent();
            an = new DoubleAnimation(1, 12, TimeSpan.FromMilliseconds(700)) { EasingFunction = new QuadraticEase() };
            opan = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(700)) { EasingFunction = new QuadraticEase() };
            loop = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1.5)
            };
            loop.Tick += Loop_Tick;
        }

        private void Loop_Tick(object sender, EventArgs e)
        {
            Animate();
        }

        DoubleAnimation an;
        DoubleAnimation opan;
        DispatcherTimer loop;
        public void Animate()
        {
            Dispatcher.Invoke(() =>
            {
                animatedPath.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, an);
                animatedPath.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, an);
                animatedPath.BeginAnimation(OpacityProperty, opan);
            });
        }
        public void StartAnimation()
        {
            Animate();
            loop.Start();
        }
        public void StopAnimation()
        {
            loop.Stop();
        }
        internal void OnDataChanged()
        {           
            var c = (MessagePiece)DataContext;
            var isDg = MapDatabase.GetDungeon(c.Location);
            if (mapImg.Source == null) return;
            var ratio = mapImg.Source.Width / mapImg.Source.Height;

            if(isDg && ratio != 1)
            {
                mapImg.Height = mapImg.ActualWidth/ratio;
                if (mapImg.Height == 0) mapImg.Height = (double)App.Current.FindResource("MapWidth") / ratio;
                mapImg.Stretch = Stretch.Uniform;
            }
            else if(ratio == 1 && !isDg)
            {
                mapImg.Height = (double)App.Current.FindResource("MapHeight");
                mapImg.Stretch = Stretch.UniformToFill;
            }
        }

    }
}
