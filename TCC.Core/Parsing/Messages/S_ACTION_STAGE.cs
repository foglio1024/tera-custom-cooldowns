using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    internal class S_ACTION_STAGE : ParsedMessage
    {
        public ulong GameId { get;  }
        public uint TemplateId { get; }
        public uint Skill { get; }
        public int Stage { get; }
        public float Speed{ get; }
        public uint Id{ get; }
        public ulong Target { get; }
        public S_ACTION_STAGE(TeraMessageReader reader) : base(reader)
        {
            reader.BaseStream.Position = 0;
            GameId = reader.ReadUInt64();
            reader.ReadVector3f();
            reader.ReadAngle();
            reader.Skip(4);
            TemplateId = reader.ReadUInt32();
            //reader.Skip(3);
            Skill = reader.ReadUInt32() -0x04000000;
            Stage = reader.ReadInt32();
            Speed = reader.ReadSingle();
            Id = reader.ReadUInt32();
            reader.Skip(4 + 1 + 12);
            Target = reader.ReadUInt64();

        }
    }
}
