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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCC.Controls.Dashboard
{
    /// <summary>
    /// Logica di interazione per GenericInfoView.xaml
    /// </summary>
    public partial class GenericInfoView : UserControl
    {
        public GenericInfoView()
        {
            InitializeComponent();
        }

        private void CloseInfoPopup(object sender, MouseEventArgs e)
        {
            

        }

        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Utils.GetChild<ScrollViewer>(CharNames).ScrollToVerticalOffset(e.VerticalOffset);
        }
    }
}
