using TCC.Data.Chat;

namespace TCC.Windows
{
    public class SystemMessageViewModel
    {
        public SystemMessage SysMsg { get; }
        public string Opcode { get; }

        public SystemMessageViewModel(string opc, SystemMessage msg)
        {
            Opcode = opc;
            SysMsg = msg;
        }
    }
}