using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class S_ACTION_STAGE : ParsedMessage
    {
        internal S_ACTION_STAGE(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(4); //Effects array count and offset
            Entity = reader.ReadEntityId();
            Position = reader.ReadVector3f();
            Heading = reader.ReadAngle();
            Model = reader.ReadUInt32();
            SkillId = reader.ReadInt32() & 0x3FFFFFF;
            Stage = reader.ReadUInt32();
            Speed = reader.ReadSingle();
            Id = reader.ReadUInt32();
            unk = reader.ReadSingle();
//            Debug.WriteLine($"{Time.Ticks} {BitConverter.ToString(BitConverter.GetBytes(Entity.Id))}: {Start} {Heading}, S:{Speed}, {SkillId} {Stage} {Model} {unk} {Id}" );
        }

        public float unk { get; set; }
        public uint Id { get; set; }
        public uint Stage { get; set; }
        public int SkillId { get; set; }
        public uint Model { get; set; }
        public EntityId Entity { get; }
        public Vector3f Position { get; private set; }
        public Angle Heading { get; private set; }
        public float Speed { get; private set; }
    }
}