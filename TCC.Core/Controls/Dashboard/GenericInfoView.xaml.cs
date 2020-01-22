using System.Windows.Controls;
using Nostrum.Extensions;

namespace TCC.Controls.Dashboard
{
    /// <summary>
    /// Logica di interazione per GenericInfoView.xaml
    /// </summary>
    public partial class GenericInfoView
    {
        public GenericInfoView()
        {
            InitializeComponent();
        }

        private void ContentListOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            CharNames.GetChild<ScrollViewer>().ScrollToVerticalOffset(e.VerticalOffset);
        }

        private void CharNames_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ContentList.GetChild<ScrollViewer>().ScrollToVerticalOffset(e.VerticalOffset);
        }
    }
}
