namespace Tera.Game.Messages
{
    public class S_CREATURE_ROTATE : ParsedMessage
    {
        internal S_CREATURE_ROTATE(TeraMessageReader reader) : base(reader)
        {
            Entity = reader.ReadEntityId();
            Heading = reader.ReadAngle();
            NeedTime = reader.ReadInt16();
//            Debug.WriteLine($"{Time.Ticks} {BitConverter.ToString(BitConverter.GetBytes(Entity.Id))}: {Heading} {Time}");
        }

        public EntityId Entity { get; }
        public Angle Heading { get; private set; }
        public int NeedTime { get; private set; }
    }
}