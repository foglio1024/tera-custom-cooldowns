using System.Windows;
using TCC.UI;

namespace TCC.ResourceDictionaries;

public partial class Styles
{
    void ToolTip_Opened(object sender, RoutedEventArgs e)
    {
        FocusManager.PauseTopmost = true;
        //FocusManager.ForceVisible = true; //FocusTimer.Enabled = false;
    }

    void ToolTip_Closed(object sender, RoutedEventArgs e)
    {
        FocusManager.PauseTopmost = false;

        //FocusManager.ForceVisible = false;  //FocusTimer.Enabled = true;
    }
}