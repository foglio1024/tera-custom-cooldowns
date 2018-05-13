using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class S_MOUNT_VEHICLE_EX : ParsedMessage
    {
        internal S_MOUNT_VEHICLE_EX(TeraMessageReader reader)
            : base(reader)
        {
            Owner = reader.ReadEntityId();
            Id = reader.ReadEntityId();
        }

        public EntityId Id { get; private set; }
        public EntityId Owner { get; private set; }
    }
}