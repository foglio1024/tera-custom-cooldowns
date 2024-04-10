using System.Windows;
using System.Windows.Input;
using Nostrum.WPF.Extensions;

namespace TCC.UI.Windows;

public partial class WelcomeWindow
{
    public WelcomeWindow()
    {
        InitializeComponent();
    }

    private void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnWikiButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Utils.Utilities.OpenUrl("https://github.com/Foglio1024/Tera-custom-cooldowns/wiki");
        }
        catch
        {
            // ignored
        }
    }

    private void OnTeraDpsDiscordButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Utils.Utilities.OpenUrl("https://discord.gg/anUXQTp");
        }
        catch 
        {
            // ignored
        }
    }

    private void OnToolboxDiscordButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Utils.Utilities.OpenUrl("https://discord.gg/dUNDDtw");
        }
        catch 
        {
            // ignored
        }
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        this.TryDragMove();
    }
}