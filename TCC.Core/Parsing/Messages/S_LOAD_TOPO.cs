using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_LOAD_TOPO : ParsedMessage
    {
        public int Zone { get; set; }
        public S_LOAD_TOPO(TeraMessageReader reader) : base(reader)
        {
            Zone = reader.ReadInt32();

        }
    }
}
