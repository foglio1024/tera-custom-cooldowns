using System.Windows;
using Dragablz;
using TCC.Data;
using TCC.Settings;
using TCC.Utils;
using TCC.ViewModels.Widgets;
using TCC.Windows.Widgets;

namespace TCC.ViewModels
{
    public class ChatTabClient : IInterTabClient
    {
        public static ChatWindow LastSource;
        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            LastSource = Window.GetWindow(source) as ChatWindow;
            var ws = new ChatWindowSettings(0, 0, 200, 500, true, ClickThruMode.Never, 1, false, 1, false, true, false)
            { HideTimeout = 10, BackgroundOpacity = .3, FadeOut = true, LfgOn = false };
            var model = new ChatViewModel(ws);
            var view = new ChatWindow(ws, model);
            ChatWindowManager.Instance.ChatWindows.Add(view);
            return new NewTabHost<Window>(view, view.TabControl);

        }
        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            try
            {
                ChatWindowManager.Instance.ChatWindows.Remove(window as ChatWindow);
                window.Close();
            }
            catch (System.Exception e)
            {
                Log.F($"Error while removing empty chat window: {e} ");
            }
            return TabEmptiedResponse.DoNothing;
        }
    }
}