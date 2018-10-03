using System.Windows;

namespace TCC.ResourceDictionaries
{
    public partial class Styles
    {
        private void ToolTip_Opened(object sender, RoutedEventArgs e)
        {
            FocusManager.FocusTimer.Enabled = false;
        }
        private void ToolTip_Closed(object sender, RoutedEventArgs e)
        {
            FocusManager.FocusTimer.Enabled = true;
        }
    }
}
