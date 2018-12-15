using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using TCC.ViewModels;

namespace TCC.Windows

{
    /// <summary>
    /// Logica di interazione per Dashboard.xaml
    /// </summary>
    public partial class Dashboard : TccWindow
    {
        public DashboardViewModel VM => Dispatcher.Invoke(() => DataContext as DashboardViewModel);
        public IntPtr Handle => Dispatcher.Invoke(() => new WindowInteropHelper(this).Handle);

        public Dashboard()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel();
            Hidden += () => SessionManager.DungeonDatabase.SaveCustomDefs();
        }


        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            HideWindow();
            VM.SaveCharacters();
        }

        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
