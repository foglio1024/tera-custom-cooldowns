using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{

    public class C_LOGIN_ARBITER : ParsedMessage
    {
        internal C_LOGIN_ARBITER(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(11);
            Language = (LangEnum)reader.ReadUInt32();
            Version = reader.ReadInt32();
            reader.Factory.ReleaseVersion = Version;
            reader.Factory.ReloadSysMsg();
        }

        public LangEnum Language { get; set; }
        public int Version { get; set; }
    }
}
