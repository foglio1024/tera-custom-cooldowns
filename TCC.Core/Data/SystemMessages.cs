using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Data
{
    public static class SystemMessages
    {
        public static Dictionary<string, SystemMessage> Messages;

        public static void Load()
        {
            var f = File.OpenText(Environment.CurrentDirectory + "/resources/data/sys-messages.tsv");
            Messages = new Dictionary<string, SystemMessage>();
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;

                var s = line.Split('\t');

                var ch = Int32.Parse(s[0]);
                var opcodeName = s[1];
                var msg = s[2].Replace("&#xA","\n");

                var sm = new SystemMessage(msg, ch);
                Messages.Add(opcodeName, sm);
            }
        }
    }
    public struct SystemMessage
    {
        public string Message;
        public int ChatChannel;

        public SystemMessage(string s, int ch)
        {
            Message = s;
            ChatChannel = ch;
        }

    }
}
