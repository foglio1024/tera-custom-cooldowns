using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Parsing.Messages;

namespace TCC.Data
{
    public class ApplyMessage : ChatMessage
    {
        public Class UserClass { get; set; }
        public uint PlayerId { get; set; }
        public short PlayerLevel { get; set; }

        public bool Handled = false;
        public ApplyMessage(S_OTHER_USER_APPLY_PARTY x) : base()
        {
            Channel = ChatChannel.Apply;
            Author = x.Name;
            PlayerId = x.PlayerId;
            PlayerLevel = x.Level;

        }
    }
}
