using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages

{
    public class S_USER_STATUS : ParsedMessage
    {
        public bool IsInCombat { get; private set; }
        public ulong EntityId { get; private set; }

        public S_USER_STATUS(TeraMessageReader reader) : base(reader)
        {
            EntityId = reader.ReadUInt64();
            if(reader.ReadInt32() == 1)
            {
                IsInCombat = true;
            }
            else
            {
                IsInCombat = false;
            }
        }
    }
}
