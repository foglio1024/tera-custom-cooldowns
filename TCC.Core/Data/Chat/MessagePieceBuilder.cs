using System;
using System.Text;
using HtmlAgilityPack;
using Nostrum.Extensions;
using Nostrum.WPF.Extensions;
using TCC.R;
using TCC.Utilities;
using TCC.Utils;

namespace TCC.Data.Chat;

public static class SystemMessageParser
{
    public static string ParseSysMsgZone(string msgText)
    {
        var dictionary = ChatUtils.BuildParametersDictionary(msgText);
        var zoneId = uint.Parse(dictionary["zoneName"]);
        return Game.DB!.MonsterDatabase.GetZoneName(zoneId);
    }
    public static string ParseSysMsgCreature(string msgText)
    {
        var dictionary = ChatUtils.BuildParametersDictionary(msgText);

        var creatureId = dictionary["creature"];
        var creatureSplit = creatureId.Split('#');
        var zoneId = uint.Parse(creatureSplit[0]);
        var templateId = uint.Parse(creatureSplit[1]);

        var txt = creatureId;

        if (Game.DB!.MonsterDatabase.TryGetMonster(templateId, zoneId, out var m))
        {
            txt = m.Name;
        }

        return txt;
    }
    public static string ParseSysMsgItem(string msgText)
    {
        var dictionary = ChatUtils.BuildParametersDictionary(msgText);
        var id = ChatUtils.GetId(dictionary, "item");
        var name = $"Unknown item [{id}]";
        if (Game.DB!.ItemsDatabase.Items.TryGetValue(id, out var i)) name = i.Name;
        return $"<{name}>";
    }
    public static string ParseSysMsgAchi(string msgText)
    {
        var dictionary = ChatUtils.BuildParametersDictionary(msgText);

        var id = ChatUtils.GetId(dictionary, "achievement");
        var achiName = id.ToString();
        if (Game.DB!.AchievementDatabase.TryGetAchievementName(id, out var g2))
        {
            achiName = $"[{g2}]";
        }

        return achiName;
    }
    public static string ParseSysMsgQuest(string msgText)
    {
        var dictionary = ChatUtils.BuildParametersDictionary(msgText);
        var id = ChatUtils.GetId(dictionary, "quest");
        var txt = id.ToString();
        if (Game.DB!.QuestDatabase.Quests.TryGetValue(id, out var q)) txt = q;
        return txt;
    }
    public static string ParseSysMsgAchiGrade(string msgText)
    {
        var dictionary = ChatUtils.BuildParametersDictionary(msgText);
        var id = ChatUtils.GetId(dictionary, "achievementgradeinfo");
        var txt = id.ToString();
        if (Game.DB!.AchievementGradeDatabase.Grades.TryGetValue(id, out var g)) txt = g;
        return txt;
    }
    public static string ParseSysMsgDungeon(string msgText)
    {
        var dictionary = ChatUtils.BuildParametersDictionary(msgText);
        var id = ChatUtils.GetId(dictionary, "dungeon");
        var txt = id.ToString();
        if (Game.DB!.DungeonDatabase.Dungeons.TryGetValue(id, out var dung)) txt = dung.Name;
        return txt;
    }
    public static string ParseSysMsgAccBenefit(string msgText)
    {
        var dictionary = ChatUtils.BuildParametersDictionary(msgText);
        var id = ChatUtils.GetId(dictionary, "accountbenefit");
        var txt = id.ToString();
        if (Game.DB!.AccountBenefitDatabase.Benefits.TryGetValue(id, out var ab)) txt = ab;
        return txt;
    }
    public static string ParseSysMsgGuildQuest(string msgText)
    {
        var dictionary = ChatUtils.BuildParametersDictionary(msgText);

        var id = ChatUtils.GetId(dictionary, "guildquest");
        var questName = id.ToString();
        if (Game.DB!.GuildQuestDatabase.GuildQuests.TryGetValue(id, out var q))
        {
            questName = q.Title;
        }
        return questName;
    }
    public static string ParseSysMsgRegion(string inPiece)
    {
        var dictionary = ChatUtils.BuildParametersDictionary(inPiece);
        var regId = dictionary["rgn"];
        var msgText = Game.DB!.RegionsDatabase.GetZoneName(Convert.ToUInt32(regId));
        return msgText;
    }
}

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

        var enchant = dictionary.TryGetValue("enchantCount", out var enchCount)
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
            return new SimpleMessagePiece(txt)
            {
                Color = col
            };
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

    static ActionMessagePiece ParseHtmlAchievement(HtmlNode node)
    {
        return new ActionMessagePiece(node.InnerText.UnescapeHtml(), node.GetAttributeValue("param", ""));
    }

    static ActionMessagePiece ParseHtmlItem(HtmlNode node)
    {
        return new ActionMessagePiece(node.InnerText.UnescapeHtml(), node.GetAttributeValue("param", ""));
    }

    static ActionMessagePiece ParseHtmlQuest(HtmlNode node)
    {
        return new ActionMessagePiece(node.InnerText.UnescapeHtml(), node.GetAttributeValue("param", ""));
    }

    static ActionMessagePiece ParseHtmlLocation(HtmlNode node)
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
        var sb = new StringBuilder();

        var guardName = guard.NameId != 0 ? Game.DB.RegionsDatabase.GetZoneName(guard.NameId) : "";
        var sectionName = Game.DB.RegionsDatabase.GetZoneName(section.NameId);

        sb.Append("<");
        sb.Append(guardName);
        if (guardName != sectionName)
        {
            if (guardName != "") sb.Append(" - ");
            sb.Append(sectionName);
        }

        sb.Append(">");

        return new ActionMessagePiece(sb.ToString(), linkData);
    }
}