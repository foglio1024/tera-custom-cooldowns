using Nostrum.WPF.Extensions;
using System.Windows;
using System.Windows.Input;

namespace TCC.UI.Windows;

public partial class WelcomeWindow
{
    public WelcomeWindow()
    {
        InitializeComponent();
    }

    void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    void OnWikiButtonClick(object sender, RoutedEventArgs e)
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

    void OnTeraDpsDiscordButtonClick(object sender, RoutedEventArgs e)
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

    void OnToolboxDiscordButtonClick(object sender, RoutedEventArgs e)
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

    void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        this.TryDragMove();
    }
}