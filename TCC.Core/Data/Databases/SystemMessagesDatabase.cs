using System.Collections.Generic;
using System.IO;
using Nostrum.Extensions;
using TCC.Data.Chat;
using TCC.Parsing;

namespace TCC.Data.Databases
{
    public class SystemMessagesDatabase : DatabaseBase
    {
        public Dictionary<string, SystemMessageData> Messages { get; }
        private List<string> _handledInternally = new List<string>{ "SMT_FIELD_EVENT_REWARD_AVAILABLE"};
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
                if (line == null) break;

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
            var msg = "<font color=\"#cccccc\">Received </font> " +
                      "<font>{Amount}</font> " +
                      "<font color=\"#cccccc\"> (</font>" +
                      "<font>{Perc}</font>" +
                      "<font color=\"#cccccc\">)</font> " +
                      "<font color=\"#cccccc\"> damage</font>" +
                      "<font color=\"#cccccc\"> from </font>" +
                      "<font>{Source}</font>" +
                      "<font color=\"#cccccc\">.</font>";

            var damageReceived = new SystemMessageData(msg, (int)ChatChannel.Damage);
            Messages["TCC_DAMAGE_RECEIVED"] = damageReceived;

            // ---------------------
            var msgCrit = "<font color=\"#cccccc\">Received </font> " +
                          "<font>{Amount}</font> " +
                          "<font color=\"#cccccc\"> (</font>" +
                          "<font>{Perc}</font>" +
                          "<font color=\"#cccccc\">)</font> " +
                         $"<font color=\"{R.Colors.ItemSuperiorColor.ToHex(true)}\"> crit</font>" +
                          "<font color=\"#cccccc\"> damage</font>" +
                          "<font color=\"#cccccc\"> from </font>" +
                          "<font>{Source}</font>" +
                          "<font color=\"#cccccc\">.</font>";

            var damageReceivedCrit = new SystemMessageData(msgCrit, (int)ChatChannel.Damage);
            Messages["TCC_DAMAGE_RECEIVED_CRIT"] = damageReceivedCrit;

            // ---------------------
            var msgUnk = "<font color=\"#cccccc\">Received </font> " +
                         "<font>{Amount}</font> " +
                         "<font color=\"#cccccc\"> (</font>" +
                         "<font>{Perc}</font>" +
                         "<font color=\"#cccccc\">)</font> " +
                         "<font color=\"#cccccc\"> damage</font>" +
                         "<font color=\"#cccccc\">.</font>";
            var damageReceivedUnknown = new SystemMessageData(msgUnk, (int)ChatChannel.Damage);
            Messages["TCC_DAMAGE_RECEIVED_UNKNOWN"] = damageReceivedUnknown;

            // ---------------------
            //var msgUnkCrit = "<font color=\"#cccccc\">Received </font> " +
            //                 "<font>{Amount}</font> " +
            //                 "<font color=\"#cccccc\"> (</font>" +
            //                 "<font>{Perc}</font>" +
            //                 "<font color=\"#cccccc\">)</font> " +
            //                $"<font color=\"{R.Colors.ItemSuperiorColor.ToHex(true)}\"> crit</font>" +
            //                 "<font color=\"#cccccc\"> damage</font>" +
            //                 "<font color=\"#cccccc\">.</font>";
            //var damageReceivedUnknownCrit = new SystemMessageData(msgUnkCrit, (int)ChatChannel.Damage);
            Messages["TCC_DAMAGE_RECEIVED_UNKNOWN_CRIT"] = damageReceivedUnknown;

            // ---------------------
            var ench = Messages["SMT_MAX_ENCHANT_SUCCEED"];
            var newEnch = new SystemMessageData($"<font color=\"{R.Colors.ChatSystemGenericColor.ToHex()}\">{ench.Template}</font>", ench.ChatChannel);
            Messages["SMT_MAX_ENCHANT_SUCCEED"] = newEnch;
        }

        public bool IsHandledInternally(string msg)
        {
            try
            {
                var pars = msg.Split('\v');
                var opc = ushort.Parse(pars[0].Substring(1));
                var opcName = PacketAnalyzer.Factory.SystemMessageNamer.GetName(opc);
                return _handledInternally.Contains(opcName);
            }
            catch { }
            return false;
        }
    }
}
