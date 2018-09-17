using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using TCC.ViewModels;

namespace TCC.Windows
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
            MainContent = content;
            this.DataContext = new CivilUnrestViewModel();
            Init(Settings.CivilUnrestWindowSettings, perClassPosition:false);
            VM.Teleported += OnTeleported;
            ZoneBoundContent.Visibility = Visibility.Collapsed;
        }

        private void OnTeleported()
        {
            ZoneBoundContent.Visibility = VM.CuZone ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VM.CopyToClipboard();
        }
    }
}
