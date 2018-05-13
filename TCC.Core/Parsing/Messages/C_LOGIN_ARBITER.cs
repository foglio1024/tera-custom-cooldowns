using System;
using System.IO;
using TCC.Tera.Data;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class LoginArbiterMessage
    {
        public LoginArbiterMessage(C_LOGIN_ARBITER message)
        {
            if (OpcodeDownloader.DownloadSysmsg(PacketProcessor.Factory.Version, 
                Path.Combine(BasicTeraData.Instance.ResourceDirectory, $"data/opcodes/"), PacketProcessor.Factory.ReleaseVersion))
            {
                PacketProcessor.Factory.ReloadSysMsg();
            };
            BasicTeraData.Instance.Servers.Language = message.Language;
        }
    }

    public enum LangEnum : UInt32
    {
        INT = 0,
        KR = 1,
        USA = 2,
        JPN = 3,
        GER = 4,
        FR = 5,
        EN = 6,
        TW = 7,
        RUS = 8,
        CHN = 9,
        THA = 10
    }


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
