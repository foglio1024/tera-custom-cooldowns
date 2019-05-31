using System.Collections.Generic;
using System.IO;
using TCC.Data.Chat;

namespace TCC.Data.Databases
{
    public class SystemMessagesDatabase : DatabaseBase
    {
        public Dictionary<string, SystemMessage> Messages { get; }

        protected override string FolderName => "sys_msg";
        protected override string Extension => "tsv";

        public SystemMessagesDatabase(string lang) : base(lang)
        {
            Messages = new Dictionary<string, SystemMessage>();
        }

        public override void Load()
        {
            Messages.Clear();
            //var f = File.OpenText(FullPath);
            var lines = File.ReadAllLines(FullPath);
            foreach (var line in lines)
            {
                //var line = f.ReadLine();
                if (line == null) break;

                var s = line.Split('\t');

                if (!int.TryParse(s[0], out var ch)) continue;
                var opcodeName = s[1];
                var msg = s[2].Replace("&#xA", "\n");

                var sm = new SystemMessage(msg, ch);
                Messages[opcodeName] = sm;
            }

            AddCustom();
        }

        private void AddCustom()
        {
            var guildieLogin = Messages["SMT_GUILD_MEMBER_LOGON_NO_MESSAGE"];
            var guildieLogout = Messages["SMT_GUILD_MEMBER_LOGOUT"];

            var memberLogin = new SystemMessage(guildieLogin.Message, (int)ChatChannel.GroupAlerts);
            var memberLogout = new SystemMessage(guildieLogout.Message, (int)ChatChannel.GroupAlerts);

            Messages["TCC_PARTY_MEMBER_LOGON"] = memberLogin;
            Messages["TCC_PARTY_MEMBER_LOGOUT"] = memberLogout;
        }
    }
}
