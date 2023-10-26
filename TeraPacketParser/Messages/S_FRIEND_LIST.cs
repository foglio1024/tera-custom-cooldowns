using System.Collections.Generic;
using TeraDataLite;

namespace TeraPacketParser.Messages;

public class S_FRIEND_LIST : ParsedMessage
{
    public List<FriendEntry> Friends { get; } = new();
    public S_FRIEND_LIST(TeraMessageReader reader) : base(reader)
    {
        var friendsCount = reader.ReadUInt16();
        var friendsPtr = reader.ReadUInt16();
        if (friendsPtr == 0) return;
        reader.RepositionAt(friendsPtr);

        for (var i = 0; i < friendsCount; i++)
        {
            try
            {
                Friends.Add(ParseFriend(reader));
            }
            catch
            {
                // ignored
            }
        }
    }

    static FriendEntry ParseFriend(TeraMessageReader reader)
    {
        reader.Skip(4); // curr + next

        var namePtr = reader.ReadUInt16();

        reader.Skip(4); // my note ptr + their note ptr

        var id = reader.ReadUInt32();
        var group = reader.ReadInt32();
        var level = reader.ReadInt32();
        var race = (Race)reader.ReadInt32();
        var cl = (Class)reader.ReadInt32();
        var gender = reader.ReadInt32();
        var world = reader.ReadInt32();
        var guard = reader.ReadInt32();
        var section = reader.ReadInt32();
        var summonable = reader.ReadBoolean();
        var lastOnline = reader.ReadInt64();
        var type = (FriendEntryType)reader.ReadInt32(); // # 0: friend, 1: outgoing friend request, 2: incoming friend request ?
        var bonds = reader.ReadInt32();

        reader.RepositionAt(namePtr);
        var name = reader.ReadTeraString();
        var myNote = reader.ReadTeraString();
        var theirNote = reader.ReadTeraString();

        return new FriendEntry(id, group, level, race, cl, gender, world, guard, section, summonable, lastOnline, type, bonds, name, myNote, theirNote, FriendStatus.Offline);
    }

    /*
    # menma

    array  friends
    - uint32 id
    - int32 level
    - int32 race
    - int32 class
    - int32 gender
    - int32 status
    - uint32 location1
    - uint32 location2
    - uint32 location3
    - byte unk4
    - int16 unk5
    - int64 lastOnline
    - string name

    # majorPatchVersion >= 101

    ref      friends
    ref      personalNote
    string   personalNote
    array    friends
    - ref    name
    - ref    myNote
    - ref    theirNote
    - uint32 playerId
    - int32  group
    - int32  level
    - int32  race
    - int32  class
    - int32  gender
    - int32  worldId
    - int32  guardId
    - int32  sectionId
    - int32  dungeonGauntletDifficultyId
    - bool   summonable
    - int64  lastOnline
    - uint32 type # 0: friend, 1: outgoing friend request, 2: incoming friend request
    - int32  bonds
    - string name
    - string myNote
    - string theirNote

    # majorPatchVersion < 101

ref friends
ref personalNote

string personalNote
array  friends
- ref name
- ref myNote
- ref theirNote
- uint32 id
- int32  group
- int32  level
- int32  race
- int32  class
- int32  gender
- uint32 location1
- uint32 location2
- uint32 location3
- byte   unk1
- int64  lastOnline
- int32  unk2
- int32  bonds
- string name
- string myNote
- string theirNote

             */
}
