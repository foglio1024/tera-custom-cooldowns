using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class S_ACTION_END : ParsedMessage
    {
        internal S_ACTION_END(TeraMessageReader reader) : base(reader)
        {
            
            Entity = reader.ReadEntityId();
            Position = reader.ReadVector3f();
            Heading = reader.ReadAngle();
            Model = reader.ReadUInt32();
            SkillId = reader.ReadInt32() & 0x3FFFFFF;
            unk = reader.ReadInt32();
            Id = reader.ReadUInt32();
//            Debug.WriteLine($"{Time.Ticks} {BitConverter.ToString(BitConverter.GetBytes(Entity.Id))}: {Start} {Heading} -> {Finish}, S:{Speed} ,{Ltype} {unk1} {unk2}" );
        }

        public uint Id { get; set; }
        public int unk { get; set; }
        public int SkillId { get; set; }
        public uint Model { get; set; }
        public EntityId Entity { get; }
        public Vector3f Position { get; private set; }
        public Angle Heading { get; private set; }
    }
}