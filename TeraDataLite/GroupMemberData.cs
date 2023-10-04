namespace TeraDataLite;

public class GroupMemberData
{
    public uint PlayerId { get; init; }
    public Class Class { get; init; }
    public string Name { get; init; } = "";
    public uint ServerId { get; init; }
    public uint Level { get; set; }
    public int Order { get; set; }
    public uint GuardId { get; set; }
    public uint SectionId { get; set; }
    public bool IsLeader { get; set; }
    public bool Online { get; set; }
    public ulong EntityId { get; set; }
    public bool CanInvite { get; set; }
    public Laurel Laurel { get; set; }
    public bool Awakened { get; set; }
    public bool Alive { get; set; }
    public long CurrentHP { get; set; }
    public int CurrentMP { get; set; }
    public long MaxHP { get; set; }
    public int MaxMP { get; set; }
    public int CurrentST { get; set; }
    public int MaxST { get; set; }
    public bool InCombat { get; set; }
}

public struct GuildMemberData
{
    public uint PlayerId { get; set; }
    public string Name { get; set; } 

}