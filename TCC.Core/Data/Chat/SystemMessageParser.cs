using System;
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

        return Game.DB!.RegionsDatabase.GetZoneName(Convert.ToUInt32(regId));
    }
}