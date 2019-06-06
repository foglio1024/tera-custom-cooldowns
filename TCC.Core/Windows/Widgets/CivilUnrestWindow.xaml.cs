using System.Windows;
using System.Windows.Input;
using TCC.ViewModels;

namespace TCC.Windows.Widgets
{
    /// <summary>
    /// Logica di interazione per CivilUnrestWindow.xaml
    /// </summary>
    public partial class CivilUnrestWindow 
    {
        public CivilUnrestViewModel VM => Dispatcher.Invoke(() => DataContext as CivilUnrestViewModel);

        public CivilUnrestWindow()
        {
            InitializeComponent();
            MainContent = WindowContent;
            DataContext = new CivilUnrestViewModel();
            Init(App.Settings.CivilUnrestWindowSettings);
            VM.Teleported += OnTeleported;
            ZoneBoundContent.Visibility = Visibility.Collapsed;
        }

        private void OnTeleported()
        {
            ZoneBoundContent.Visibility = SessionManager.CivilUnrestZone ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VM.CopyToClipboard();
        }
    }
}
