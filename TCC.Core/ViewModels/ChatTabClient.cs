using System;
using System.Windows;
using Dragablz;
using TCC.Data;
using TCC.Settings.WindowSettings;
using TCC.UI.Windows.Widgets;
using TCC.Utils;
using TCC.ViewModels.Widgets;

namespace TCC.ViewModels;

public class ChatTabClient : IInterTabClient
{
    public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
    {
        var ws = new ChatWindowSettings(0, 0, 200, 500, true, ClickThruMode.Never, 1, false, 1, false, true, false)
            { HideTimeout = 10, BackgroundOpacity = .3, FadeOut = true, LfgOn = false };
        var model = new ChatViewModel(ws);
        var view = new ChatWindow(model);
        ChatManager.Instance.ChatWindows.Add(view);
        return new NewTabHost<Window>(view, view.TabControl);

    }
    public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
    {
        try
        {
            ChatManager.Instance.ChatWindows.Remove((ChatWindow) window);
            window.Close();
        }
        catch (Exception e)
        {
            Log.F($"Error while removing empty chat window: {e} ");
        }
        return TabEmptiedResponse.DoNothing;
    }
}