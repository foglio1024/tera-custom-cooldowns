using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class SNpcLocation : ParsedMessage
    {
        internal SNpcLocation(TeraMessageReader reader) : base(reader)
        {
            Entity = reader.ReadEntityId();
            Start = reader.ReadVector3f();
            Heading = reader.ReadAngle();
            Speed = reader.ReadInt16();
            Finish = reader.ReadVector3f();
            Ltype = reader.ReadInt32(); //0 = Move, 7= Rotate standing
//            Debug.WriteLine($"{Time.Ticks} {BitConverter.ToString(BitConverter.GetBytes(Entity.Id))}: {Start} {Heading} -> {Finish}, S:{Speed} ,{Ltype}");
        }

        public EntityId Entity { get; }
        public Vector3f Start { get; private set; }
        public Angle Heading { get; private set; }
        public short Speed { get; private set; }
        public Vector3f Finish { get; private set; }
        public int Ltype { get; private set; }
    }
}