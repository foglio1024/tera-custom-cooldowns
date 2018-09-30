using TCC.Data;

namespace TCC.Parsing.Messages
{
    public struct ReadyPartyMember
    {
        public uint ServerId;
        public uint PlayerId;
        public ReadyStatus Status;
    }
}