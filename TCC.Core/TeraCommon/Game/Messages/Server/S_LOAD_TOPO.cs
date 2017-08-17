namespace Tera.Game.Messages
{
    public class S_LOAD_TOPO : ParsedMessage
    {
        internal S_LOAD_TOPO(TeraMessageReader reader) : base(reader)
        {
            AreaId = reader.ReadInt32();
            Position = reader.ReadVector3f();
        }

        public int AreaId { get; private set; }
        public Vector3f Position { get; private set; }
    }
}