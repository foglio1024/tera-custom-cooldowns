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

namespace TCC.Controls
{
    /// <summary>
    /// Logica di interazione per StanceIndicator.xaml
    /// </summary>
    public partial class WarriorStanceIndicator : UserControl
    {
        public WarriorStanceIndicator()
        {
            InitializeComponent();
        }
        StanceTracker<WarriorStance> _context;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _context = (StanceTracker<WarriorStance>)DataContext;
            _context.PropertyChanged += _context_PropertyChanged;
        }

        private void _context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentStance")
            {
                switch (_context.CurrentStance)
                {
                    case WarriorStance.None:
                        rect.Fill = new SolidColorBrush(Colors.Transparent);
                        break;
                    case WarriorStance.Assault:
                        rect.Fill = TryFindResource("AssaultStanceColor") as SolidColorBrush;
                        break;
                    case WarriorStance.Defensive:
                        rect.Fill = TryFindResource("DefensiveStanceColor") as SolidColorBrush;
                        break;
                }
            }
        }
    }
}
