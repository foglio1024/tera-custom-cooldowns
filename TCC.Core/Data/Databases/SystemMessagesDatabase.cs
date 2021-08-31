using System.Collections.Generic;
using System.IO;
using Nostrum.Extensions;
using Nostrum.WPF.Extensions;
using TCC.Data.Chat;
using TCC.Utils;
using TeraPacketParser.Analysis;

namespace TCC.Data.Databases
{
    public class SystemMessagesDatabase : DatabaseBase
    {
        public Dictionary<string, SystemMessageData> Messages { get; }
        private List<string> _handledInternally = new() { "SMT_FIELD_EVENT_REWARD_AVAILABLE"};
        protected override string FolderName => "sys_msg";
        protected override string Extension => "tsv";

        public SystemMessagesDatabase(string lang) : base(lang)
        {
            Messages = new Dictionary<string, SystemMessageData>();
        }

        public override void Load()
        {
            Messages.Clear();
            //var f = File.OpenText(FullPath);
            var lines = File.ReadAllLines(FullPath);
            foreach (var line in lines)
            {
                //var line = f.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) break;

                var s = line.Split('\t');

                if (!int.TryParse(s[0], out var ch)) continue;
                var opcodeName = s[1];
                var msg = s[2].Replace("&#xA", "\n");

                var sm = new SystemMessageData(msg, ch);
                Messages[opcodeName] = sm;
            }

            AddCustom();
        }

        private void AddCustom()
        {
            // party member login/out ------------------------------------------------------------------------

            var guildieLogin = Messages["SMT_GUILD_MEMBER_LOGON_NO_MESSAGE"];
            var guildieLogout = Messages["SMT_GUILD_MEMBER_LOGOUT"];

            var memberLogin = new SystemMessageData(guildieLogin.Template, (int)ChatChannel.GroupAlerts);
            var memberLogout = new SystemMessageData(guildieLogout.Template, (int)ChatChannel.GroupAlerts);

            Messages["TCC_PARTY_MEMBER_LOGON"] = memberLogin;
            Messages["TCC_PARTY_MEMBER_LOGOUT"] = memberLogout;

            // damage received -------------------------------------------------------------------------------
            var msg = ChatUtils.Font("Received ", "cccccc") +
                      ChatUtils.Font("{Amount}") +
                      ChatUtils.Font(" (", "cccccc") +
                      ChatUtils.Font("{Perc}") +
                      ChatUtils.Font(")", "cccccc") +
                      ChatUtils.Font(" damage from ", "cccccc") +
                      ChatUtils.Font("{Source}") +
                      ChatUtils.Font(".", "cccccc");

            var damageReceived = new SystemMessageData(msg, (int)ChatChannel.Damage);
            Messages["TCC_DAMAGE_RECEIVED"] = damageReceived;

            // ---------------------
            var msgCrit = ChatUtils.Font("Received ", "cccccc") +
                          ChatUtils.Font("{Amount}") +
                          ChatUtils.Font(" (", "cccccc") +
                          ChatUtils.Font("{Perc}") +
                          ChatUtils.Font(")", "cccccc") +
                          ChatUtils.Font(" crit", R.Colors.ItemSuperiorColor.ToHex(true)) +
                          ChatUtils.Font(" damage from ", "cccccc") +
                          ChatUtils.Font("{Source}") +
                          ChatUtils.Font(".", "cccccc");

            var damageReceivedCrit = new SystemMessageData(msgCrit, (int)ChatChannel.Damage);
            Messages["TCC_DAMAGE_RECEIVED_CRIT"] = damageReceivedCrit;

            // ---------------------
            var msgUnk =
                ChatUtils.Font("Received ", "cccccc") +
                ChatUtils.Font("{Amount}") +
                ChatUtils.Font(" (", "cccccc") +
                ChatUtils.Font("{Perc}") +
                ChatUtils.Font(")", "cccccc") +
                ChatUtils.Font(" damage.", "cccccc");

            var damageReceivedUnknown = new SystemMessageData(msgUnk, (int)ChatChannel.Damage);
            Messages["TCC_DAMAGE_RECEIVED_UNKNOWN"] = damageReceivedUnknown;

            // ---------------------
            var msgUnkCrit =
                ChatUtils.Font("Received ", "cccccc") +
                ChatUtils.Font("{Amount}") +
                ChatUtils.Font(" (", "cccccc") +
                ChatUtils.Font("{Perc}") +
                ChatUtils.Font(")", "cccccc") +
                ChatUtils.Font(" crit", R.Colors.ItemSuperiorColor.ToHex(true)) +
                ChatUtils.Font(" damage.", "cccccc");

            var damageReceivedUnknownCrit = new SystemMessageData(msgUnkCrit, (int)ChatChannel.Damage);
            Messages["TCC_DAMAGE_RECEIVED_UNKNOWN_CRIT"] = damageReceivedUnknownCrit;

            // ---------------------
            var ench = Messages["SMT_MAX_ENCHANT_SUCCEED"];
            var newEnch = new SystemMessageData(ChatUtils.Font(ench.Template, R.Colors.ChatSystemGenericColor.ToHex()), ench.ChatChannel);
            Messages["SMT_MAX_ENCHANT_SUCCEED"] = newEnch;
        }

        public bool IsHandledInternally(string msg)
        {
            try
            {
                var pars = msg.Split('\v');
                var opc = ushort.Parse(pars[0].Substring(1));
                var opcName = PacketAnalyzer.Factory!.SystemMessageNamer.GetName(opc);
                return _handledInternally.Contains(opcName);
            }
            catch { }
            return false;
        }
    }
}
