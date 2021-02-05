namespace TeraPacketParser.Messages
{
    public class S_ANSWER_INTERACTIVE : ParsedMessage
    {
        public string Name { get; }
        public uint PlayerId { get; }
        public uint ServerId { get; }
        public bool HasGuild { get; }
        public bool HasParty { get; }
        public int Level { get; }
        public int TemplateId { get; }

        public S_ANSWER_INTERACTIVE(TeraMessageReader reader) : base(reader)
        {
            var nameOffset = reader.ReadUInt16();
            reader.Skip(4); // type
            if (reader.Factory.ReleaseVersion / 100 >= 103)
            {
                PlayerId = reader.ReadUInt32();
            }
            TemplateId = reader.ReadInt32();
            Level = reader.ReadInt32();
            HasParty = reader.ReadBoolean();
            HasGuild = reader.ReadBoolean();
            ServerId = reader.ReadUInt32();
            reader.RepositionAt(nameOffset);
            Name = reader.ReadTeraString();
        }
    }
}