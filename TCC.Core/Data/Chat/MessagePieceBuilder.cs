using HtmlAgilityPack;
using Nostrum.Extensions;
using Nostrum.WPF.Extensions;
using System;
using System.Text;
using TCC.R;
using TCC.Utilities;
using TCC.Utils;

namespace TCC.Data.Chat;

public static class MessagePieceBuilder
{
    public static SimpleMessagePiece BuildSysMsgZone(string msgText)
    {
        return new SimpleMessagePiece(SystemMessageParser.ParseSysMsgZone(msgText));
    }

    public static SimpleMessagePiece BuildSysMsgCreature(string msgText)
    {
        return new SimpleMessagePiece(SystemMessageParser.ParseSysMsgCreature(msgText));
    }

    public static SimpleMessagePiece BuildSysMsgItem(string msgText)
    {
        var dictionary = ChatUtils.BuildParametersDictionary(msgText);

        var id = ChatUtils.GetId(dictionary, "item");
        var uid = ChatUtils.GetItemUid(dictionary);
        var rawLink = new StringBuilder("1#####");
        rawLink.Append(id.ToString());

        if (uid != 0)
        {
            rawLink.Append("@" + uid);
        }

        if (dictionary.TryGetValue("UserName", out var username)) rawLink.Append("@" + username);

        var name = $"Unknown item [{id}]";
        var grade = RareGrade.Common;
        if (Game.DB!.ItemsDatabase.Items.TryGetValue(id, out var i))
        {
            name = i.Name;
            grade = i.RareGrade;
        }

        var enchant = dictionary.TryGetValue("enchantcount", out var enchCount)
            ? $"+{enchCount} "
            : "";

        return new ActionMessagePiece($"<{enchant}{name}>", rawLink.ToString())
        {
            Color = TccUtils.GradeToColorString(grade)
        };
    }

    public static SimpleMessagePiece BuildSysMsgAchi(string msgText)
    {
        return new SimpleMessagePiece(SystemMessageParser.ParseSysMsgAchi(msgText));
    }

    public static SimpleMessagePiece BuildSysMsgQuest(string msgText)
    {
        return new SimpleMessagePiece(SystemMessageParser.ParseSysMsgQuest(msgText));
    }

    public static SimpleMessagePiece BuildSysMsgAchiGrade(string msgText)
    {
        var dictionary = ChatUtils.BuildParametersDictionary(msgText);
        var id = ChatUtils.GetId(dictionary, "achievementgradeinfo");
        var txt = id.ToString();
        var col = "fcb06f";

        if (!Game.DB!.AchievementGradeDatabase.Grades.TryGetValue(id, out var g))
            return new SimpleMessagePiece(txt) { Color = col };

        txt = g;
        col = id switch
        {
            104 => Colors.ChatDiamondLaurelColor.ToHex(false, false),
            105 => Colors.ChatChampionLaurelColor.ToHex(false, false),
            _ => col
        };

        return new SimpleMessagePiece(txt)
        {
            Color = col
        };
    }

    public static SimpleMessagePiece BuildSysMsgDungeon(string msgText)
    {
        return new SimpleMessagePiece(SystemMessageParser.ParseSysMsgDungeon(msgText));
    }

    public static SimpleMessagePiece BuildSysMsgAccBenefit(string msgText)
    {
        return new SimpleMessagePiece(SystemMessageParser.ParseSysMsgAccBenefit(msgText));
    }

    public static SimpleMessagePiece BuildSysMsgGuildQuest(string msgText)
    {
        return new SimpleMessagePiece(SystemMessageParser.ParseSysMsgGuildQuest(msgText));
    }

    public static SimpleMessagePiece BuildSysMsgRegion(string inPiece)
    {
        return new SimpleMessagePiece(SystemMessageParser.ParseSysMsgRegion(inPiece));
    }

    public static SimpleMessagePiece ParseChatLinkAction(HtmlNode chatLinkAction)
    {
        var param = chatLinkAction.GetAttributeValue("param", "");
        var type = int.Parse(param[..1]);
        var mp = type switch
        {
            1 => ParseHtmlItem(chatLinkAction),
            2 => ParseHtmlQuest(chatLinkAction),
            3 => ParseHtmlLocation(chatLinkAction),
            7 => ParseHtmlAchievement(chatLinkAction),
            _ => throw new Exception()
        };

        return mp;
    }

    private static ActionMessagePiece ParseHtmlAchievement(HtmlNode node)
    {
        return new ActionMessagePiece(node.InnerText.UnescapeHtml(), node.GetAttributeValue("param", ""));
    }

    private static ActionMessagePiece ParseHtmlItem(HtmlNode node)
    {
        return new ActionMessagePiece(node.InnerText.UnescapeHtml(), node.GetAttributeValue("param", ""));
    }

    private static ActionMessagePiece ParseHtmlQuest(HtmlNode node)
    {
        return new ActionMessagePiece(node.InnerText.UnescapeHtml(), node.GetAttributeValue("param", ""));
    }

    private static ActionMessagePiece ParseHtmlLocation(HtmlNode node)
    {
        var linkData = node.GetAttributeValue("param", "");
        var pars = linkData[6..].Split('@');
        var locTree = pars[0].Split('_');
        var worldId = uint.Parse(locTree[0]);
        var guardId = uint.Parse(locTree[1]);
        var sectionId = uint.Parse(locTree[2]);
        if (worldId == 1 && guardId == 2 && sectionId == 9) sectionId = 7;
        //var coords = pars[2].Split(',');

        //var x = double.Parse(coords[0], CultureInfo.InvariantCulture);
        //var y = double.Parse(coords[1], CultureInfo.InvariantCulture);

        var world = Game.DB!.MapDatabase.Worlds[worldId];
        var guard = world.Guards[guardId];
        var section = guard.Sections[sectionId];
        var guardName = guard.NameId != 0 ? Game.DB.RegionsDatabase.GetZoneName(guard.NameId) : "";
        var sectionName = Game.DB.RegionsDatabase.GetZoneName(section.NameId);

        var sb = new StringBuilder();
        sb.Append('<');
        sb.Append(guardName);
        if (guardName != sectionName)
        {
            if (guardName != "") sb.Append(" - ");
            sb.Append(sectionName);
        }
        sb.Append('>');

        return new ActionMessagePiece(sb.ToString(), linkData);
    }
}