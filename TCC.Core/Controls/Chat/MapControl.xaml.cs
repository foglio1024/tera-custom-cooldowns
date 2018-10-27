using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TCC.Data;

namespace TCC.Controls.ChatControls
{
    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl
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

        private DoubleAnimation an;
        private DoubleAnimation opan;
        private DispatcherTimer loop;
        public void Animate()
        {
            Dispatcher.Invoke(() =>
            {
                AnimatedPath.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, an);
                AnimatedPath.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, an);
                AnimatedPath.BeginAnimation(OpacityProperty, opan);
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
            var isDg = SessionManager.MapDatabase.GetDungeon(c.Location);
            if (MapImg.Source == null) return;
            var ratio = MapImg.Source.Width / MapImg.Source.Height;

            if(isDg && ratio != 1)
            {
                MapImg.Height = MapImg.ActualWidth/ratio;
                // ReSharper disable once PossibleNullReferenceException
                if (MapImg.Height == 0) MapImg.Height = (double)Application.Current?.FindResource("MapWidth") / ratio;
                MapImg.Stretch = Stretch.Uniform;
            }
            else if(ratio == 1 && !isDg)
            {
                // ReSharper disable once PossibleNullReferenceException
                MapImg.Height = (double)Application.Current.FindResource("MapHeight");
                MapImg.Stretch = Stretch.UniformToFill;
            }
        }

    }
}
