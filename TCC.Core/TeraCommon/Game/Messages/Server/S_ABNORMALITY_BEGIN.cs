namespace Tera.Game.Messages
{
    public class SAbnormalityBegin : ParsedMessage
    {
        internal SAbnormalityBegin(TeraMessageReader reader) : base(reader)
        {
            TargetId = reader.ReadEntityId();
            SourceId = reader.ReadEntityId();
            AbnormalityId = reader.ReadInt32();
            Duration = reader.ReadInt32();
            Stack = reader.ReadInt32();
        }

        public int Duration { get; }

        public int Stack { get; }

        public int AbnormalityId { get; }


        public EntityId TargetId { get; }
        public EntityId SourceId { get; }
    }
}