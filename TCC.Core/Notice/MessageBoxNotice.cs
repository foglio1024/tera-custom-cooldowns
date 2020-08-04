using System.Windows;
using TCC.UI.Windows;

namespace TCC.Notice
{
    public class MessageBoxNotice : NoticeBase
    {
        public override void Fire()
        {
            TccMessageBox.Show(Title, Content, MessageBoxButton.OK);
            base.Fire();
        }

        public MessageBoxNotice(NoticeBase b)
        {
            Enabled = b.Enabled;
            Title = b.Title;
            Content = b.Content;
            Trigger = b.Trigger;
        }
    }
}