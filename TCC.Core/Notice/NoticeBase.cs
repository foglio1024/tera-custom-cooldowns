using TCC.Data;

namespace TCC.Notice;

public class NoticeBase
{
    public bool Enabled { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public NoticeTrigger Trigger { get; set; }
    public bool Fired { get; private set; }

    public virtual void Fire()
    {
        Fired = true;
    }
}