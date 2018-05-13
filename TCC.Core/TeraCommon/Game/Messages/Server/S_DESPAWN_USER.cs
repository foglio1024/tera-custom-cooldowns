using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class SDespawnUser : ParsedMessage
    {
        internal SDespawnUser(TeraMessageReader reader) : base(reader)
        {
            User = reader.ReadEntityId();
        }

        public EntityId User { get; }
    }
}