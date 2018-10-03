using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public enum RunemarksActionType
    {
        Normal = 0,
        Detonate = 1,
        Expired = 2,
        Reclaimed = 3
    }
    public class S_WEAK_POINT : ParsedMessage
    {
        private uint type;

        public ulong Target { get; private set; }
        public uint TotalRunemarks { get; private set; }
        public uint RemovedRunemarks { get; private set; }
        public RunemarksActionType Type => (RunemarksActionType)type;
        public uint SkillId { get; private set; }

        public S_WEAK_POINT(TeraMessageReader reader) : base(reader)
        {
            Target = reader.ReadUInt64();
            RemovedRunemarks = reader.ReadUInt32();
            TotalRunemarks = reader.ReadUInt32();
            type = reader.ReadUInt32();
            SkillId = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return $"T:{Target} | total:{TotalRunemarks} | moved:{RemovedRunemarks} | {Type} | S:{SkillId}";
        }
    }
}
