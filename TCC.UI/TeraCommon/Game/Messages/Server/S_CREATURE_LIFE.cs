namespace Tera.Game.Messages
{
    public class SCreatureLife : ParsedMessage
    {
        internal SCreatureLife(TeraMessageReader reader) : base(reader)
        {
            User = reader.ReadEntityId();
            Position = reader.ReadVector3f();
            Dead = reader.ReadByte() == 0; // 0=dead;1=alive
        }

        public EntityId User { get; }
        public Vector3f Position { get; private set; }
        public bool Dead { get; }
    }
}