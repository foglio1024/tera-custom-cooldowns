using System.Windows.Controls;
using System.Windows.Input;

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
