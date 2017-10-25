using System.Diagnostics;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_EACH_SKILL_RESULT : ParsedMessage
    {
        public ulong Source { get; private set; }
        public ulong Target { get; private set; }
        public uint Damage { get; private set; }
        public S_EACH_SKILL_RESULT(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(4);
            Source = reader.ReadUInt64();
            reader.Skip(8);
            Target = reader.ReadUInt64();
            reader.Skip(4+4+4+4+4+4);
            Damage = reader.ReadUInt32();
            Debug.WriteLine(Damage);

        }
    }
}