


namespace TeraPacketParser.Messages
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
        private uint _type;

        public ulong Target { get; private set; }
        public int TotalRunemarks { get; private set; }
        public uint RemovedRunemarks { get; private set; }
        public RunemarksActionType Type => (RunemarksActionType)_type;
        public uint SkillId { get; private set; }

        public S_WEAK_POINT(TeraMessageReader reader) : base(reader)
        {
            Target = reader.ReadUInt64();
            RemovedRunemarks = reader.ReadUInt32();
            TotalRunemarks = reader.ReadInt32();
            _type = reader.ReadUInt32();
            SkillId = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return $"T:{Target} | total:{TotalRunemarks} | moved:{RemovedRunemarks} | {Type} | S:{SkillId}";
        }
    }
}
