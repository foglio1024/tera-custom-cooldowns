using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        private StanceTracker<WarriorStance> _context;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _context = (StanceTracker<WarriorStance>)DataContext;
            _context.PropertyChanged += _context_PropertyChanged;
        }

        private void _context_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                if (e.PropertyName == "CurrentStance")
                {
                    switch (_context.CurrentStance)
                    {
                        case WarriorStance.None:
                            rect.Background = new SolidColorBrush(Colors.Transparent);
                            break;
                        case WarriorStance.Assault:
                            rect.Background = TryFindResource("AssaultStanceColor") as SolidColorBrush;
                            break;
                        case WarriorStance.Defensive:
                            rect.Background = TryFindResource("DefensiveStanceColor") as SolidColorBrush;
                            break;
                    }
                }
            }, System.Windows.Threading.DispatcherPriority.DataBind);
        }
    }
}
