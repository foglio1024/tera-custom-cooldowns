using System.Timers;
using Nostrum.WPF.ThreadSafe;
using TCC.ViewModels;

namespace TCC.Data.Chat;

public class LFG : ThreadSafeObservableObject
{
    uint _id;
    string _name = "";
    string _message = "";
    bool _raid;
    string _dungeonName = "";
    int _membersCount;
    readonly Timer _removeTimer;

    public uint Id
    {
        get => _id; set
        {
            if (_id == value) return;
            _id = value;
            N();
        }
    }

    public uint ServerId { get; }

    public string Name
    {
        get => _name; set
        {
            if (_name == value) return;
            _name = value;
            N();
        }
    }
    public string Message
    {
        get => _message; set
        {
            if (_message == value) return;
            _message = value;
            UpdateDungeonName();
            N();
        }
    }
    public bool Raid
    {
        get => _raid; set
        {
            if (_raid == value) return;
            _raid = value;
            N();
        }
    }

    public string DungeonName
    {
        get => _dungeonName; set
        {
            if (_dungeonName == value) return;
            _dungeonName = value;
            N();
        }
    }

    public int MembersCount
    {
        get => _membersCount; set
        {
            if (_membersCount == value) return;
            _membersCount = value;
            N();
            N(nameof(MembersCountLabel));
        }
    }
    public string MembersCountLabel => MembersCount == 0 ? "" : MembersCount.ToString();

    public LFG(uint id, string name, string msg, bool raid, uint serverId)
    {
        Dispatcher = ChatManager.Instance.Dispatcher;

        Id = id;
        ServerId = serverId;
        Name = name;
        Message = msg;
        Raid = raid;
        MembersCount = 0;
        UpdateDungeonName();

        _removeTimer = new Timer(3 * 60 * 1000);
        _removeTimer.Elapsed += _removeTimer_Elapsed;
        _removeTimer.Start();

    }

    void _removeTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        ChatManager.Instance.RemoveLfg(this);
    }
    public void Refresh()
    {
        try
        {
            _removeTimer.Stop();
            _removeTimer.Start();
            N();
        }
        catch
        {
            // ignored
        }
    }

    void UpdateDungeonName()
    {
        var a = Message.Split(' ');
        if (a[0].Length <= 5)
        {
            DungeonName = a[0];
        }
        else
        {
            DungeonName = "LFG";
        }
    }
    public void Dispose()
    {
        _removeTimer.Elapsed -= _removeTimer_Elapsed;
        _removeTimer.Stop();
        _removeTimer.Dispose();
    }
    public override string ToString()
    {
        return $"[{Id}] {Name}: {Message}";
    }
}