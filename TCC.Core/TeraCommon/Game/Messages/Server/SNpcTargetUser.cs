using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class SNpcTargetUser : ParsedMessage

    {
        internal SNpcTargetUser(TeraMessageReader reader) : base(reader)
        {
            NPC = reader.ReadEntityId();
        }

        public EntityId NPC { get; private set; }
    }
}