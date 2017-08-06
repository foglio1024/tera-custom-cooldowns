using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_GUILD_TOWER_INFO : ParsedMessage
    {
        public ulong TowerId { get; set; }
        public string GuildName { get; set; }

        public S_GUILD_TOWER_INFO(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(2);
            var nameOffset = reader.ReadUInt16();
            TowerId = reader.ReadUInt64();
            reader.BaseStream.Position = nameOffset - 4;
            GuildName = reader.ReadTeraString();
        }
    }
}
