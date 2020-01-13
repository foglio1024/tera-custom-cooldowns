using TCC.ViewModels;

namespace TCC.Controls.Classes
{
    /// <summary>
    /// Logica di interazione per MysticLayout.xaml
    /// </summary>
    public partial class MysticLayout
    {
        private MysticLayoutVM _dc;

        public MysticLayout()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!(DataContext is MysticLayoutVM dc)) return;
            _dc = dc;
            dc.Auras.AuraChanged += OnAurasChanged;
        }

        private void OnAurasChanged()
        {
            if (_dc == null) return;
            Dispatcher?.Invoke(() =>
            {
                //if (_dc.Auras.AllMissing)
                //{
                //    CritAura.Opacity = 1;
                //    SwiftAura.Opacity = 1;
                //    ManaAura.Opacity = 1;
                //    CritResAura.Opacity = 1;
                //    return;
                //}
                //CritAura.Opacity = _dc.Auras.CritAura ? 1 : _dc.Auras.SwiftAura ? .5 : 1;
                //SwiftAura.Opacity = _dc.Auras.SwiftAura ? 1 : _dc.Auras.CritAura ? .5 : 1;
                //ManaAura.Opacity = _dc.Auras.ManaAura ? 1 : _dc.Auras.CritResAura ? .5 : 1;
                //CritResAura.Opacity = _dc.Auras.CritResAura ? 1 : _dc.Auras.ManaAura ? .5 : 1;
            });
        }
    }
}
