using System.Windows;
using Dragablz;
using TCC.Data;
using TCC.Settings;
using TCC.Windows.Widgets;

namespace TCC.ViewModels
{
    public class ChatTabClient : IInterTabClient
    {
        public static ChatWindow LastSource;
        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            LastSource = Window.GetWindow(source) as ChatWindow;
            var model = new ChatViewModel();
            var view = new ChatWindow(new ChatWindowSettings(0, 0, 200, 500, true, ClickThruMode.Never,
                1, false, 1, false, true, false), model);
            ChatWindowManager.Instance.ChatWindows.Add(view);
            return new NewTabHost<Window>(view, view.TabControl);

        }
        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            ChatWindowManager.Instance.ChatWindows.Remove(window as ChatWindow);
            window.Close();
            return TabEmptiedResponse.DoNothing;
        }
    }
}