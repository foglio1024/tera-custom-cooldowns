using System.Collections.Generic;
using TeraDataLite;

namespace TeraPacketParser.Messages;

/*
 array  friends
- uint32 id
- int32  level
- int32  race
- int32  class
- int32  gender
- int32  status
- uint32 location1
- uint32 location2
- uint32 location3
- byte   unk4
- int16  unk5
- int64  lastOnline
- string name


 */

/*
 # majorPatchVersion >= 101

array    friends
- uint32 playerId
- int32  level
- int32  race
- int32  class
- int32  gender
- int32  status
- int32  worldId
- int32  guardId
- int32  sectionId
- bool   updated
- bool   isWorldEventTarget # see S_SPAWN_USER
- bool   summonable
- int64  lastOnline
- string name


 */
public class S_UPDATE_FRIEND_INFO : ParsedMessage
{
    public IReadOnlyCollection<FriendInfoUpdate> FriendUpdates;

    public S_UPDATE_FRIEND_INFO(TeraMessageReader reader) : base(reader)
    {
        var count = reader.ReadUInt16();

        reader.Skip(2); // friendsPtr

        List<FriendInfoUpdate> friends = [];
        for (var i = 0; i < count; i++)
        {
            reader.Skip(4); // curr + next

            var namePtr = reader.ReadUInt16();

            var id = reader.ReadUInt32();
            var level = reader.ReadInt32();
            var race = (Race)reader.ReadInt32();
            var cl = (Class)reader.ReadInt32();
            var gender = reader.ReadInt32();
            var status = (FriendStatus)reader.ReadInt32();
            var world = reader.ReadInt32();
            var guard = reader.ReadInt32();
            var section = reader.ReadInt32();
            var updated = reader.ReadBoolean();
            var isWorldEventTarget = reader.ReadBoolean();
            var isSummonable = reader.ReadBoolean();
            var lastOnline = reader.ReadInt64();

            reader.RepositionAt(namePtr);
            var name = reader.ReadTeraString();

            friends.Add(new FriendInfoUpdate(id, level, race, cl, gender, status, world, guard, section, updated, isWorldEventTarget, isSummonable, lastOnline, name));
        }

        FriendUpdates = friends.AsReadOnly();
    }
}