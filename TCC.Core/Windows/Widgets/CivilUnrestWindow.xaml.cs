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
        private CivilUnrestViewModel VM { get; }
    

        public CivilUnrestWindow(CivilUnrestViewModel vm)
        {
            InitializeComponent();
            MainContent = WindowContent;
            DataContext = vm;
            VM = DataContext as CivilUnrestViewModel;

            Init(App.Settings.CivilUnrestWindowSettings);
            VM.Teleported += OnTeleported;
            ZoneBoundContent.Visibility = Visibility.Collapsed;
        }

        private void OnTeleported()
        {
            //TODO: move to vm and bind
            ZoneBoundContent.Visibility = Session.CivilUnrestZone ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VM.CopyToClipboard();
        }
    }
}
