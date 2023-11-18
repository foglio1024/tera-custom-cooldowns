using Nostrum.WPF.Factories;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TCC.ViewModels;

namespace TCC.UI.Windows;

public partial class ChatSettingsWindow
{
    public static readonly List<ChatSettingsWindow> OpenWindows = new();

    readonly DoubleAnimation _closeAnimation;
    readonly DoubleAnimation _openAnimation;

    public ChatSettingsWindow(Tab dataContext)
    {
        _closeAnimation = AnimationFactory.CreateDoubleAnimation(150, 0, completed: (_, _) => Close());
        _openAnimation = AnimationFactory.CreateDoubleAnimation(150, 1);

        InitializeComponent();
        DataContext = dataContext;
        Opacity = 0;
    }
    
    void CloseChannelSettings(object sender, RoutedEventArgs e)
    {
        App.Settings.Save();
        FocusManager.ForceFocused = false;
        BeginAnimation(OpacityProperty, _closeAnimation);
        OpenWindows.Remove(this);
    }

    void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    void Window_Loaded(object sender, RoutedEventArgs e)
    {
        FocusManager.ForceFocused = true;
        BeginAnimation(OpacityProperty, _openAnimation);
        OpenWindows.Add(this);
    }
}