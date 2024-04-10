using System.Timers;
using Nostrum.WPF.ThreadSafe;
using TCC.ViewModels;

namespace TCC.Data.Chat;

public class Lfg : ThreadSafeObservableObject
{
    private string _name = "";
    private string _message = "";
    private bool _raid;
    private string _dungeonName = "";
    private int _membersCount;
    private readonly Timer _removeDelay;

    public uint Id { get; }
    public uint ServerId { get; }
    public string Name
    {
        get => _name; 
        set => RaiseAndSetIfChanged(value, ref _name);
    }
    public string Message
    {
        get => _message; set
        {
            if (!RaiseAndSetIfChanged(value, ref _message)) return;
            UpdateDungeonName();
        }
    }
    public bool Raid
    {
        get => _raid; set => RaiseAndSetIfChanged(value, ref _raid);

    }
    public string DungeonName
    {
        get => _dungeonName;
        private set => RaiseAndSetIfChanged(value, ref _dungeonName);

    }
    public int MembersCount
    {
        get => _membersCount; set
        {
            if (!RaiseAndSetIfChanged(value, ref _membersCount)) return;
            InvokePropertyChanged(nameof(MembersCountLabel));
        }
    }
    public string MembersCountLabel => MembersCount == 0 ? "" : MembersCount.ToString();

    public Lfg(uint id, string name, string msg, bool raid, uint serverId)
    {
        Dispatcher = ChatManager.Instance.Dispatcher;

        Id = id;
        ServerId = serverId;
        Name = name;
        Message = msg;
        Raid = raid;
        MembersCount = 0;
        UpdateDungeonName();

        _removeDelay = new Timer(3 * 60 * 1000);
        _removeDelay.Elapsed += RemoveDelayElapsed;
        _removeDelay.Start();
    }

    private void RemoveDelayElapsed(object? sender, ElapsedEventArgs e)
    {
        ChatManager.Instance.RemoveLfg(this);
    }

    public void Refresh()
    {
        try
        {
            _removeDelay.Stop();
            _removeDelay.Start();
        }
        catch
        {
            // ignored
        }
    }

    private void UpdateDungeonName()
    {
        var a = Message.Split(' ');
        DungeonName = a[0].Length <= 5 ? a[0] : "LFG";
    }

    public void Dispose()
    {
        _removeDelay.Elapsed -= RemoveDelayElapsed;
        _removeDelay.Stop();
        _removeDelay.Dispose();
    }

    public override string ToString()
    {
        return $"[{Id}] {Name}: {Message}";
    }
}