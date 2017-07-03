using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_USER_BLOCK_LIST : ParsedMessage
    {
        public List<string> BlockedUsers { get; private set; }

        public S_USER_BLOCK_LIST(TeraMessageReader reader) : base(reader)
        {
            BlockedUsers = new List<string>();

            var count = reader.ReadUInt16();
            var offest = reader.ReadUInt16();

            for (int i = 0; i < count; i++)
            {
                BlockedUsers.Add(ParseBlockedUser(reader));
            }

        }
        private string ParseBlockedUser(TeraMessageReader reader)
        {
            reader.Skip(4);
            var nameOffset = reader.ReadUInt16();
            reader.BaseStream.Position = nameOffset - 4;
            var name = reader.ReadTeraString();
            reader.ReadTeraString(); //skips notes
            return name;
        }


    }
}
