using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class S_OTHER_USER_APPLY_PARTY : ParsedMessage
    {
        internal S_OTHER_USER_APPLY_PARTY(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(7);
            var clas = reader.ReadInt16();
            PlayerClass = (PlayerClass) (clas + 1);
            reader.Skip(4);
            Lvl = reader.ReadInt16();
            reader.Skip(1);
            PlayerName = reader.ReadTeraString();
        }

        public string PlayerName { get; set; }
        public PlayerClass PlayerClass { get; set; }
        public int Lvl { get; set; }
    }
}
