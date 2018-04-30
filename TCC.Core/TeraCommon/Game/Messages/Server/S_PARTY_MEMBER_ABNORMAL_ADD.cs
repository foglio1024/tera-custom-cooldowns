using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class SPartyMemberAbnormalAdd : ParsedMessage
    {
        internal SPartyMemberAbnormalAdd(TeraMessageReader reader) : base(reader)
        {
            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();
            AbnormalityId = reader.ReadInt32();
            Duration = reader.ReadInt32();
            Stack = reader.ReadInt32();
            //  Debug.WriteLine("target = " + TargetId + ";Abnormality:" + AbnormalityId + ";Duration:" + Duration +
            //                  ";Stack:" + Stack);
        }

        public uint ServerId { get; }
        public uint PlayerId { get; }

        public int AbnormalityId { get; }

        public int Duration { get; }
        public int Stack { get; }
    }
}