namespace TeraPacketParser.Messages
{
    public class S_PARTY_MEMBER_CHANGE_STAMINA : ParsedMessage
    {
        public uint ServerId { get; }

        public uint PlayerId { get; }

        public int CurrentST { get; }

        public int MaxST { get; }


        public S_PARTY_MEMBER_CHANGE_STAMINA(TeraMessageReader reader) : base(reader)
        {
            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();

            CurrentST = reader.ReadInt32();
            MaxST = reader.ReadInt32();
        }
    }
}