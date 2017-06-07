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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCC.Data;
using TCC.ViewModels;

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
                        rect.Fill = TryFindResource("SniperEyeColor") as SolidColorBrush;
                        break;
                }
            }
        }

    }
}
