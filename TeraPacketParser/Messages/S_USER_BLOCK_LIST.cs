using System.Collections.Generic;

namespace TeraPacketParser.Messages;

public class S_USER_BLOCK_LIST : ParsedMessage
{
    public List<string> BlockedUsers { get; private set; }

    public S_USER_BLOCK_LIST(TeraMessageReader reader) : base(reader)
    {
        BlockedUsers = new List<string>();

        var count = reader.ReadUInt16();
        reader.Skip(2); //var offset = reader.ReadUInt16();

        for (var i = 0; i < count; i++)
        {
            BlockedUsers.Add(ParseBlockedUser(reader));
        }

    }

    private string ParseBlockedUser(TeraMessageReader reader)
    {
        reader.Skip(4);
        var nameOffset = reader.ReadUInt16();
        reader.RepositionAt(nameOffset);
        var name = reader.ReadTeraString();
        reader.ReadTeraString(); //skips notes
        return name;
    }


}