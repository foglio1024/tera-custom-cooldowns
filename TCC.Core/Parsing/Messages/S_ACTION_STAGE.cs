using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_ACTION_STAGE : ParsedMessage
    {
        public ulong GameId { get;  }
        public uint TemplateId { get; }
        public int Skill { get; }
        public int Stage { get; }
        public float Speed{ get; }
        public uint Id{ get; }
        public ulong Target { get; }
        public S_ACTION_STAGE(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(4);
            GameId = reader.ReadUInt64();
            reader.ReadVector3f();
            reader.ReadAngle();
            reader.Skip(4);
            Skill = new SkillId(reader).Id;

            //Stage = reader.ReadInt32();
            //Speed = reader.ReadSingle();
            //Id = reader.ReadUInt32();
            //reader.Skip(4 + 1 + 12);
            //Target = reader.ReadUInt64();

        }
    }
}
