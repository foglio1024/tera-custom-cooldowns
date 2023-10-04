using TCC.Utils;

namespace TCC.Notice;

public class NotificationNotice : NoticeBase
{
    public int Duration { get; set; }
    public NotificationType NotificationType { get; set; }

    public override void Fire()
    {
        Log.N(Title, Content, NotificationType, Duration * 1000);
        base.Fire();
    }

    public NotificationNotice(NoticeBase b)
    {
        Title = b.Title;
        Content = b.Content;
        Trigger = b.Trigger;
    }
}