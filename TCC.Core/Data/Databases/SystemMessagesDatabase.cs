using System.Collections.Generic;
using System.IO;

namespace TCC.Data.Databases
{
    public class SystemMessagesDatabase
    {
        public Dictionary<string, SystemMessage> Messages { get; }

        public SystemMessagesDatabase(string lang)
        {
            var f = File.OpenText($"resources/data/sys_msg/sys_msg-{lang}.tsv");
            Messages = new Dictionary<string, SystemMessage>();
            while (true)
            {
                var line = f.ReadLine();
                if (line == null) break;

                var s = line.Split('\t');

                if(!int.TryParse(s[0], out var ch)) continue;
                var opcodeName = s[1];
                var msg = s[2].Replace("&#xA","\n");

                var sm = new SystemMessage(msg, ch);
                Messages.Add(opcodeName, sm);
            }
        }
    }
}
