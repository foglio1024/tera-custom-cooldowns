using TCC.Data.Chat;

namespace TCC.UI.Windows
{
    public class SystemMessageViewModel
    {
        public SystemMessageData SysMsg { get; }
        public string Opcode { get; }

        public SystemMessageViewModel(string opc, SystemMessageData msg)
        {
            Opcode = opc;
            SysMsg = msg;
        }
    }
}