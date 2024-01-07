using Nostrum.WPF.Extensions;
using System.Collections.Generic;
using System.IO;
using TCC.Data.Chat;
using TCC.R;
using TCC.Utils;
using TeraPacketParser.Analysis;

namespace TCC.Data.Databases;

public class SystemMessagesDatabase : DatabaseBase
{
    protected override string FolderName => "sys_msg";
    protected override string Extension => "tsv";

    readonly List<string> _handledInternally = ["SMT_FIELD_EVENT_REWARD_AVAILABLE"];

    public Dictionary<string, SystemMessageData> Messages { get; } = [];

    public SystemMessagesDatabase(string lang) : base(lang)
    {
    }

    public override void Load()
    {
        Messages.Clear();
        var lines = File.ReadAllLines(FullPath);
        foreach (var line in lines)
        {
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

    void AddCustom()
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
                      ChatUtils.Font(" crit", Colors.ItemSuperiorColor.ToHex(true)) +
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
            ChatUtils.Font(" crit", Colors.ItemSuperiorColor.ToHex(true)) +
            ChatUtils.Font(" damage.", "cccccc");

        var damageReceivedUnknownCrit = new SystemMessageData(msgUnkCrit, (int)ChatChannel.Damage);
        Messages["TCC_DAMAGE_RECEIVED_UNKNOWN_CRIT"] = damageReceivedUnknownCrit;

        // ---------------------
        var ench = Messages["SMT_MAX_ENCHANT_SUCCEED"];
        var newEnch = new SystemMessageData(ChatUtils.Font(ench.Template, Colors.ChatSystemGenericColor.ToHex()), ench.ChatChannel);
        Messages["SMT_MAX_ENCHANT_SUCCEED"] = newEnch;

        // ---------------------
        var msgBomb =
            ChatUtils.Font("{UserName}", R.Colors.ChatSystemErrorColor.ToHex(sharp: false)) +
            ChatUtils.Font(" used ") +
            ChatUtils.Font("{ItemName}", R.Colors.ItemSuperiorColor.ToHex(sharp: false)) +
            ChatUtils.Font(".");

        var bomb = new SystemMessageData(msgBomb, (int)ChatChannel.Group);
        Messages["TCC_MT_USER_THROW_BOMB"] = bomb;
    }

    public bool IsHandledInternally(string msg)
    {
        try
        {
            var pars = msg.Split('\v');
            var opc = ushort.Parse(pars[0][1..]);
            var opcName = PacketAnalyzer.Factory!.SystemMessageNamer.GetName(opc);
            return _handledInternally.Contains(opcName);
        }
        catch
        {
            // ignored
        }

        return false;
    }
}