using System.Windows;

namespace TCC.ResourceDictionaries
{
    public partial class Styles
    {
        private void ToolTip_Opened(object sender, RoutedEventArgs e)
        {
            FocusManager.PauseTopmost = true;
            //FocusManager.ForceVisible = true; //FocusTimer.Enabled = false;
        }
        private void ToolTip_Closed(object sender, RoutedEventArgs e)
        {
            FocusManager.PauseTopmost = false;

            //FocusManager.ForceVisible = false;  //FocusTimer.Enabled = true;
        }
    }
}
