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

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per ArcherFocusControl.xaml
    /// </summary>
    public partial class ArcherFocusControl : UserControl
    {
        public ArcherFocusControl()
        {
            InitializeComponent();
        }

        ArcherFocusTracker _context;

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
            externalArc.Stroke = new SolidColorBrush(Colors.White);
            externalArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(0, TimeSpan.FromMilliseconds(100)) {EasingFunction = new QuadraticEase() });
        }

        private void AnimateArc()
        {
            externalArc.Stroke = new SolidColorBrush(Color.FromRgb(0xff, 0xcc, 0x30));

            externalArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(359.9,0, TimeSpan.FromMilliseconds(_context.Duration)));
        }
        private void AnimateArcPartial(int newStacks)
        {
            externalArc.BeginAnimation(Arc.EndAngleProperty, new DoubleAnimation(newStacks*36, TimeSpan.FromMilliseconds(100)) { EasingFunction = new QuadraticEase() });
        }
    }
}
