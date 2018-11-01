using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCC.Data;

namespace TCC.Controls.Classes.Elements
{
    /// <summary>
    /// Logica di interazione per RhombEdgeControl.xaml
    /// </summary>
    public partial class RhombEdgeControl : UserControl
    {
        public RhombEdgeControl()
        {
            InitializeComponent();
        }

        public Color GlowColor
        {
            get => (Color)GetValue(GlowColorProperty);
            set => SetValue(GlowColorProperty, value);
        }
        public static readonly DependencyProperty GlowColorProperty = DependencyProperty.Register("GlowColor", typeof(Color), typeof(RhombEdgeControl));

        public Brush FillBrush
        {
            get => (Brush)GetValue(FillBrushProperty);
            set => SetValue(FillBrushProperty, value);
        }
        public static readonly DependencyProperty FillBrushProperty = DependencyProperty.Register("FillBrush", typeof(Brush), typeof(RhombEdgeControl));

        public Counter EdgeCounter
        {
            get => (Counter)GetValue(EdgeCounterProperty);
            set => SetValue(EdgeCounterProperty, value);
        }
        public static readonly DependencyProperty EdgeCounterProperty = DependencyProperty.Register("EdgeCounter", typeof(Counter), typeof(RhombEdgeControl));

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (EdgeCounter == null) return;
            EdgeCounter.PropertyChanged += OnEdgeChanged;
        }

        private void OnEdgeChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Counter.Val)) return;
            for (var i = 0; i < EdgeCounter.MaxValue; i++)
            {
                Container.Children[i].Opacity = i < EdgeCounter.Val ? 1 : 0;
            }
        }
    }
}
