using System.Windows.Controls;

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

        private void ContentListOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Utils.GetChild<ScrollViewer>(CharNames).ScrollToVerticalOffset(e.VerticalOffset);
        }

        private void CharNames_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Utils.GetChild<ScrollViewer>(ContentList).ScrollToVerticalOffset(e.VerticalOffset);
        }
    }
}
