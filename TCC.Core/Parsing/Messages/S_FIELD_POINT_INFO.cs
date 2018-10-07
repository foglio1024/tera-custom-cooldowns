using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{

    public class S_FIELD_POINT_INFO : ParsedMessage
    {
        public uint Points { get; }
        public uint MaxPoints { get; set; }
        public int Claimed { get; set; }
        public int Cleared { get; set; }

        public S_FIELD_POINT_INFO(TeraMessageReader reader) : base(reader)
        {
            Points = reader.ReadUInt32();
            MaxPoints = reader.ReadUInt32();
            Claimed = reader.ReadInt32() + 1;
            Cleared = reader.ReadInt32();
        }
    }
}
