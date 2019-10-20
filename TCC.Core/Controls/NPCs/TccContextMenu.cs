using System.Windows;
using System.Windows.Controls;

namespace TCC.Controls{
    public class TccContextMenu : ContextMenu
    {
        protected override void OnOpened(RoutedEventArgs e)
        {
            FocusManager.PauseTopmost = true;
            base.OnOpened(e);
        }

        protected override void OnClosed(RoutedEventArgs e)
        {
            FocusManager.PauseTopmost = false;
            base.OnClosed(e);
        }
    }

}