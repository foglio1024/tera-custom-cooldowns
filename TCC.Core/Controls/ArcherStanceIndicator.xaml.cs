using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TCC.Data;

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per ArcherStanceIndicator.xaml
    /// </summary>
    public partial class ArcherStanceIndicator : UserControl
    {
        public ArcherStanceIndicator()
        {
            InitializeComponent();
        }

        StanceTracker<ArcherStance> _context;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _context = (StanceTracker<ArcherStance>)DataContext;
            _context.PropertyChanged += _context_PropertyChanged;

        }
        private void _context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentStance")
            {
                switch (_context.CurrentStance)
                {
                    case ArcherStance.None:
                        rect.Fill = new SolidColorBrush(Colors.Transparent);
                        break;
                    case ArcherStance.SniperEye:
                        rect.Fill = TryFindResource("Colors.ClassWindow.SniperEye") as SolidColorBrush;
                        break;
                }
            }
        }

    }
}
