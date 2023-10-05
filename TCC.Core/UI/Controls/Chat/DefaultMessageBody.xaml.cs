using System.Windows;
using Dragablz;
using TCC.Data.Chat;
using TCC.ViewModels;

namespace TCC.UI.Controls.Chat;

/// <summary>
/// Interaction logic for DefaultMessageBody.xaml
/// </summary>
public partial class DefaultMessageBody
{
    public DefaultMessageBody()
    {
        InitializeComponent();

    }

    void PinBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var dc = DataContext as ChatMessage;
        foreach (var w in ChatManager.Instance.ChatWindows)
        {
            if (!w.IsMouseOver) continue;
            var currTabVm = w.TabControl.SelectedItem as HeaderedItemViewModel;
            if (currTabVm?.Content is not Tab currTab) continue;
            currTab.PinnedMessage = currTab.PinnedMessage == dc ? null : dc;
        }
    }

    void CopyBtn_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Clipboard.SetText(((ChatMessage)DataContext).ToString());
        }
        catch
        {
            // ignored
        }
    }
}