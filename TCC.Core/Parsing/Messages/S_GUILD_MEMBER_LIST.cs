using System.Collections.Generic;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_GUILD_MEMBER_LIST : ParsedMessage
    {
        public Dictionary<uint, string> GuildMembersList { get; }

        public S_GUILD_MEMBER_LIST(TeraMessageReader reader) : base(reader)
        {
            var membersCount = reader.ReadInt16();
            var membersOffset = reader.ReadInt16();
            GuildMembersList = new Dictionary<uint, string>();
            if (membersCount == 0) return;
            reader.BaseStream.Position = membersOffset - 4;

            for (var i = 0; i < membersCount; i++)
            {
                reader.Skip(2); //curr
                var next = reader.ReadUInt16();
                var nameOffset = reader.ReadInt16();
                reader.Skip(2); //noteOffset
                var playerId = reader.ReadUInt32();

                reader.BaseStream.Position = nameOffset - 4;
                var name = reader.ReadTeraString();

                GuildMembersList[playerId] = name;

                if (next == 0) return;
                reader.BaseStream.Position = next - 4;
            }
        }
    }
}
