using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TCC.Data;

namespace TCC.Controls
{
    /// <inheritdoc cref="UserControl" />
    /// <summary>
    /// Logica di interazione per ArcherFocusControl.xaml
    /// </summary>
    public partial class ArcherFocusControl
    {
        public ArcherFocusControl()
        {
            InitializeComponent();
        }

        private ArcherFocusTracker _context;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _context = (ArcherFocusTracker)DataContext;
            _context.PropertyChanged += _context_PropertyChanged;
        }

        private void _context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Refresh")
            {
                AnimateArcPartial(_context.Stacks);
            }
            else if(e.PropertyName == "StartFocus")
            {
                AnimateArcPartial(_context.Stacks);
            }
            else if(e.PropertyName == "StartFocusX")
            {
                AnimateArc();
            }
            else if(e.PropertyName == "Ended")
            {
                ResetArc();
            }
        }

        private void ResetArc()
        {
            InternalArc.BeginAnimation(Arc.StartAngleProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(100)) {EasingFunction = new QuadraticEase() });
        }

        private void AnimateArc()
        {
            ExternalArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9,0, TimeSpan.FromMilliseconds(_context.Duration)));
        }
        private void AnimateArcPartial(int newStacks)
        {
            InternalArc.BeginAnimation(Arc.StartAngleProperty, new DoubleAnimation(newStacks*36, TimeSpan.FromMilliseconds(100)) { EasingFunction = new QuadraticEase() });
        }
    }
}
