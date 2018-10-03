//using TCC.TeraCommon.Game.Services;

//namespace TCC.TeraCommon.Game.Messages.Server
//{
//    public class S_USER_LOCATION : ParsedMessage
//    {
//        internal S_USER_LOCATION(TeraMessageReader reader) : base(reader)
//        {
//            Entity = reader.ReadEntityId();
//            Start = reader.ReadVector3f();
//            Heading = reader.ReadAngle();
//            unk1 = reader.ReadInt16();
//            Speed = reader.ReadInt16();
//            Finish = reader.ReadVector3f();
//            Ltype = reader.ReadInt32();
//            unk2 = reader.ReadByte();
////            Debug.WriteLine($"{Time.Ticks} {BitConverter.ToString(BitConverter.GetBytes(Entity.Id))}: {Start} {Heading} -> {Finish}, S:{Speed} ,{Ltype} {unk1} {unk2}" );
//        }

//        public byte unk2 { get; set; }
//        public short unk1 { get; set; }
//        public EntityId Entity { get; }
//        public Vector3f Start { get; private set; }
//        public Angle Heading { get; private set; }
//        public short Speed { get; private set; }
//        public Vector3f Finish { get; private set; }
//        public int Ltype { get; private set; }
//    }
//}