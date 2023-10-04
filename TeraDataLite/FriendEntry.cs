namespace TeraDataLite;

public enum FriendEntryType
{
    Friend = 0,
    OutgoingRequest = 1,
    IncomingRequest = 2
}
public enum FriendStatus
{
    Online = 0,
    Busy = 1,
    Offline = 2
}
public readonly record struct FriendInfoUpdate(uint Id,
                                               int Level,
                                               Race Race,
                                               Class Class,
                                               int Gender,
                                               FriendStatus Status,
                                               int World,
                                               int Guard,
                                               int Section,
                                               bool IsUpdated,
                                               bool IsWorldEventTarget,
                                               bool IsSummonable,
                                               long LastOnline,
                                               string Name);


public record struct FriendEntry(uint Id,
                                 int Group,
                                 int Level,
                                 Race Race,
                                 Class Class,
                                 int Gender,
                                 int World,
                                 int Guard,
                                 int Section,
                                 bool IsSummonable,
                                 long LastOnline,
                                 FriendEntryType Type,
                                 int Bonds,
                                 string Name,
                                 string MyNote,
                                 string TheirNote,
                                 FriendStatus Status)
{

    public void UpdateFrom(FriendEntry update)
    {
        Group = update.Group;
        Level = update.Level;
        Race = update.Race;
        Class = update.Class;
        Gender = update.Gender;
        World = update.World;
        Guard = update.Guard;
        Section = update.Section;
        IsSummonable = update.IsSummonable;
        LastOnline = update.LastOnline;
        Type = update.Type;
        Bonds = update.Bonds;
        Name = update.Name;
        MyNote = update.MyNote;
        TheirNote = update.TheirNote;
    }

    public void UpdateFrom(FriendInfoUpdate update)
    {
        Level = update.Level;
        Race = update.Race;
        Class = update.Class;
        Gender = update.Gender;
        Status = update.Status;
        World = update.World;
        Guard = update.Guard;
        Section = update.Section;
        IsSummonable = update.IsSummonable;
        LastOnline = update.LastOnline;
        Name = update.Name;
    }
}
