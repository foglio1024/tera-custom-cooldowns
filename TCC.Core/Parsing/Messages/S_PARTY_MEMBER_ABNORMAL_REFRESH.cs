using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_PARTY_MEMBER_ABNORMAL_REFRESH : ParsedMessage
    {
        public S_PARTY_MEMBER_ABNORMAL_REFRESH(TeraMessageReader reader) : base(reader)
        {
            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();
            Id = reader.ReadUInt32();
            Duration = reader.ReadUInt32();
            reader.Skip(4);
            Stacks = reader.ReadInt32();
        }
        public uint ServerId { get; private set; }
        public uint PlayerId { get; private set; }
        public uint Id { get; private set; }
        public uint Duration { get; private set; }
        public int Stacks { get; private set; }

    }
}
