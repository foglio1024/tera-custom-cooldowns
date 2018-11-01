using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCC.Data;

namespace TCC.Controls.Classes.Elements
{
    /// <summary>
    /// Logica di interazione per TraverseCutControl.xaml
    /// </summary>
    public partial class TraverseCutControl : UserControl
    {
        private DoubleAnimation _toZeroAnimation;
        private DoubleAnimation _anim;
        private StatTracker _dc;
        private bool _isAnimating;
        public string IconName { get; } = "icon_skills.dualrapidpiercing_tex";

        public TraverseCutControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _dc = (StatTracker) DataContext;
            _anim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(100));
            _toZeroAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(0));
            _anim.Completed += (_, __) => _isAnimating = false;

            _dc.ToZero += OnToZero;
            _dc.PropertyChanged += OnPropertyChanged;
        }

        private void OnToZero(uint duration)
        {
            var delay = (uint) Dispatcher.Invoke(() => _anim.Duration.TimeSpan.Milliseconds + 10);
            if (_isAnimating)
            {
                Task.Delay(TimeSpan.FromMilliseconds(delay)).ContinueWith(t => OnToZero(duration - delay));
                return;
            }

            Dispatcher.Invoke(() =>
            {
                _toZeroAnimation.Duration = TimeSpan.FromMilliseconds(duration);
                _toZeroAnimation.From = _dc.Factor * 359.9;
                ExternalArc.BeginAnimation(Arc.EndAngleProperty, _toZeroAnimation);
            });
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StatTracker.Factor))
            {
                _anim.To = _dc.Factor * 359.9;
                ExternalArc.BeginAnimation(Arc.EndAngleProperty, _anim);
                _isAnimating = true;
            }
        }
    }
}
