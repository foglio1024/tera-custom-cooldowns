using Nostrum.WPF.ThreadSafe;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeraDataLite;

namespace TCC.Data;

public class GuildInfo
{
    readonly ThreadSafeObservableCollection<GuildMemberData> _members = [];

    public bool InGuild { get; private set; }
    public bool AmIMaster { get; private set; }
    public GuildMemberData Master { get; private set; }

    public string NameOf(uint id)
    {
        var found = _members.ToSyncList().FirstOrDefault(m => m.PlayerId == id);
        return found.Name != ""
            ? found.Name
            : "Unknown player";
    }

    public void Set(List<GuildMemberData> newMembers, uint masterId, string masterName)
    {
        foreach (var member in newMembers.Where(x => !Has(x.Name)))
        {
            _members.Add(member);
        }
        InGuild = true;
        SetMaster(masterId, masterName);
    }

    public bool Has(string name)
    {
        return _members.ToSyncList().Exists(m => m.Name == name);
    }

    public void Clear()
    {
        Task.Run(() => _members.Clear());
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