using Nostrum.WPF.ThreadSafe;
using System.Collections.Generic;
using System.Linq;
using TeraDataLite;

namespace TCC.Data;

public class GuildInfo
{
    readonly object _lock = new object();
    readonly ThreadSafeObservableCollection<GuildMemberData> _members = [];

    public bool InGuild { get; private set; }
    public bool AmIMaster { get; private set; }
    public GuildMemberData Master { get; private set; }

    public string NameOf(uint id)
    {
        GuildMemberData found;
        lock (_lock)
        {
            found = _members.ToSyncList().FirstOrDefault(m => m.PlayerId == id);
        }
        return found.Name != ""
            ? found.Name
            : "Unknown player";
    }

    public void Set(List<GuildMemberData> newMembers, uint masterId, string masterName)
    {
        lock (_lock)
        {
            foreach (var member in newMembers.Where(x => !Has(x.Name)))
            {
                _members.Add(member);
            }
        }
        InGuild = true;
        SetMaster(masterId, masterName);
    }

    public bool Has(string name)
    {
        bool ret;

        lock (_lock)
        {
            ret = _members.ToSyncList().Exists(m => m.Name == name);
        }
        return ret;
    }

    public void Clear()
    {
        lock (_lock)
        {
            _members.Clear();
        }

        InGuild = false;
        Master = default;
        AmIMaster = false;
    }

    public void SetMaster(uint playerId, string playerName)
    {
        Master = new GuildMemberData { Name = playerName, PlayerId = playerId };
        AmIMaster = Master.Name == Game.Me.Name;
    }
}