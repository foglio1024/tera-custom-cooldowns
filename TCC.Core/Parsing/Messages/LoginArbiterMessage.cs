using System.IO;
using TCC.Tera.Data;

namespace TCC.Parsing.Messages
{
    public class LoginArbiterMessage
    {
        public LoginArbiterMessage(C_LOGIN_ARBITER message)
        {
            if (OpcodeDownloader.DownloadSysmsg(PacketProcessor.Factory.Version, 
                Path.Combine(BasicTeraData.Instance.ResourceDirectory, "data/opcodes/"), PacketProcessor.Factory.ReleaseVersion))
            {
                PacketProcessor.Factory.ReloadSysMsg();
            }

            BasicTeraData.Instance.Servers.Language = message.Language;
        }
    }
}
