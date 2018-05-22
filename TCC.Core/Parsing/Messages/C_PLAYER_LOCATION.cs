using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class C_PLAYER_LOCATION : ParsedMessage
    {
        private float x;
        public float X
        {
            get { return x; }
            set { x = value; }
        }

        private float y;
        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public C_PLAYER_LOCATION(TeraMessageReader reader) : base(reader)
        {
            x = reader.ReadSingle();
            y = reader.ReadSingle();
        }
    }
}
