using TCC.ViewModels;

namespace TCC.Controls.ClassBars
{
    /// <summary>
    /// Logica di interazione per LancerBar.xaml
    /// </summary>
    public partial class LancerBar
    {
        private LancerBarManager _dc;

        public LancerBar()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _dc = DataContext as LancerBarManager;
            _dc.LH.PropertyChanged += OnLineHeldPropertyChanged;
        }

        private void OnLineHeldPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_dc.LH.Val)) return;
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < _dc.LH.Max; i++)
                {
                    LineHeldContainer.Children[i].Opacity = i <= _dc.LH.Val - 1 ? 1 : 0;
                }
            });
        }
    }
}
