using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_CREST_MESSAGE : ParsedMessage
    {
        public uint Type { get; private set; }
        public uint SkillId { get; private set; }

        public S_CREST_MESSAGE(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(4);
            Type = reader.ReadUInt32();
            SkillId = reader.ReadUInt32();

            Console.WriteLine(nameof(S_CREST_MESSAGE));
            Console.WriteLine("\t Type: {0}", Type);
            Console.WriteLine("\t SkillId: {0}", SkillId);
        }
    }
}
