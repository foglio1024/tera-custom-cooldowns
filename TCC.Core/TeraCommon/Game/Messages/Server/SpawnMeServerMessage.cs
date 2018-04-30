using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class SpawnMeServerMessage : ParsedMessage
    {
        internal SpawnMeServerMessage(TeraMessageReader reader)
            : base(reader)
        {
            Id = reader.ReadEntityId();
            Position = reader.ReadVector3f();
            Heading = reader.ReadAngle();
            Dead = (reader.ReadByte() & 1) == 0;
            unk1 = reader.ReadByte();
        }

        public byte unk1 { get; set; }
        public bool Dead { get; set; }
        public Angle Heading { get; set; }
        public Vector3f Position { get; set; }
        public EntityId Id { get; private set; }
    }
}