using System;
using System.Globalization;
using System.Text;
using HtmlAgilityPack;
using TCC.Data.Map;
using TCC.Parsing;

namespace TCC.Data.Chat
{
    public static class MessagePieceBuilder
    {
        public static MessagePiece BuildSysMsgZone(string msgText)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(msgText);
            var zoneId = uint.Parse(dictionary["zoneName"]);
            var zoneName = SessionManager.MonsterDatabase.GetZoneName(zoneId);
            var txt = zoneId.ToString();
            if (zoneName != null) txt = zoneName;
            var mp = new MessagePiece(txt)
            {
                Type = MessagePieceType.Simple
            };
            return mp;
        }
        public static MessagePiece BuildSysMsgCreature(string msgText)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(msgText);

            var creatureId = dictionary["creature"];
            var creatureSplit = creatureId.Split('#');
            var zoneId = uint.Parse(creatureSplit[0]);
            var templateId = uint.Parse(creatureSplit[1]);

            var txt = creatureId;

            if (SessionManager.MonsterDatabase.TryGetMonster(templateId, zoneId, out var m))
            {
                txt = m.Name;
            }

            var mp = new MessagePiece(txt)
            {
                Type = MessagePieceType.Simple
            };
            return mp;
        }
        public static MessagePiece BuildSysMsgItem(string msgText)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(msgText);

            var id = ChatUtils.GetId(dictionary, "item");
            var uid = ChatUtils.GetItemUid(dictionary);

            var rawLink = new StringBuilder("1#####");
            rawLink.Append(id.ToString());
            if (uid != 0)
            {
                rawLink.Append("@" + uid.ToString());
            }

            var username = SessionManager.CurrentPlayer.Name;
            if (dictionary.ContainsKey("UserName"))
            {
                username = dictionary["UserName"];
                rawLink.Append("@" + username);
            }
            var mp = new MessagePiece(id.ToString());
            if (SessionManager.ItemsDatabase.Items.TryGetValue(id, out var i))
            {
                var txt = $"<{i.Name}>";
                mp = new MessagePiece(txt)
                {
                    Type = MessagePieceType.Item,
                    //BoundType = i.BoundType,
                    ItemId = id,
                    ItemUid = uid,
                    OwnerName = username,
                    RawLink = rawLink.ToString()
                };
                mp.SetColor(ChatUtils.GetItemColor(i));
            }
            return mp;
        }
        public static MessagePiece BuildSysMsgAchi(string msgText)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(msgText);

            var id = ChatUtils.GetId(dictionary, "achievement");
            var achiName = id.ToString();
            if (SessionManager.AchievementDatabase.Achievements.TryGetValue(id, out var g))
            {
                achiName = $"[{g}]";
            }

            return new MessagePiece(achiName) { Type = MessagePieceType.Simple, };
        }
        public static MessagePiece BuildSysMsgQuest(string msgText)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(msgText);

            var id = ChatUtils.GetId(dictionary, "quest");
            var txt = id.ToString();
            if (SessionManager.QuestDatabase.Quests.TryGetValue(id, out var q))
            {
                txt = q;
            }
            return new MessagePiece(txt) { Type = MessagePieceType.Simple };
        }
        public static MessagePiece BuildSysMsgAchiGrade(string msgText)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(msgText);

            var id = ChatUtils.GetId(dictionary, "AchievementGradeInfo");
            var txt = id.ToString();
            var col = "fcb06f";

            if (SessionManager.AchievementGradeDatabase.Grades.TryGetValue(id, out var g))
            {
                txt = g;
                if (id == 104) col = "38bde5"; //TODO: use resources
                if (id == 105) col = "ff264b";
            }
            var ret = new MessagePiece(txt) { Type = MessagePieceType.Simple };
            ret.SetColor(col);
            return ret;
        }
        public static MessagePiece BuildSysMsgDungeon(string msgText)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(msgText);

            var id = ChatUtils.GetId(dictionary, "dungeon");
            var txt = id.ToString();
            if (SessionManager.DungeonDatabase.Dungeons.TryGetValue(id, out var dung))
            {
                txt = dung.Name;
            }
            return new MessagePiece(txt) { Type = MessagePieceType.Simple};
        }
        public static MessagePiece BuildSysMsgAccBenefit(string msgText)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(msgText);

            var id = ChatUtils.GetId(dictionary, "accountBenefit");
            var txt = id.ToString();
            if (SessionManager.AccountBenefitDatabase.Benefits.TryGetValue(id, out var ab))
            {
                txt = ab;
            }
            return new MessagePiece(txt) {Type = MessagePieceType.Simple};
        }
        public static MessagePiece BuildSysMsgGuildQuest(string msgText)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(msgText);

            var id = ChatUtils.GetId(dictionary, "GuildQuest");
            var questName = id.ToString();
            if (SessionManager.GuildQuestDatabase.GuildQuests.TryGetValue(id, out var q))
            {
                questName = q.Title;
            }
            return new MessagePiece(questName) {Type = MessagePieceType.Simple};
        }

        public static MessagePiece ParseChatLinkAction(HtmlNode chatLinkAction)
        {
            var param = chatLinkAction.GetAttributeValue("param", "");
            var type = int.Parse(param.Substring(0, 1));
            MessagePiece mp;
            switch (type)
            {
                case 1:
                    mp = ParseHtmlItem(chatLinkAction);
                    break;
                case 2:
                    mp = ParseHtmlQuest(chatLinkAction);
                    break;
                case 3:
                    mp = ParseHtmlLocation(chatLinkAction);
                    break;
                default:
                    throw new Exception();
            }
            return mp;
        }
        private static MessagePiece ParseHtmlItem(HtmlNode node)
        {
            var linkData = node.GetAttributeValue("param", "");
            var pars = linkData.Substring(6).Split('@');
            var id = uint.Parse(pars[0]);
            var uid = long.Parse(pars[1]);
            var owner = "";
            try { owner = pars[2]; }
            catch
            {
                // ignored
            }


            var text = node.InnerText;

            var result = new MessagePiece(StringUtils.ReplaceHtmlEscapes(text))
            {
                ItemId = id,
                ItemUid = uid,
                OwnerName = owner,
                Type = MessagePieceType.Item
            };
            result.RawLink = linkData;
            return result;
        }
        private static MessagePiece ParseHtmlQuest(HtmlNode node)
        {
            var linkData = node.GetAttributeValue("param", "");
            //parsing only name

            var text = node.InnerText;
            text = StringUtils.ReplaceHtmlEscapes(text);

            var result = new MessagePiece(text)
            {
                Type = MessagePieceType.Quest
            };
            result.RawLink = linkData;

            return result;
        }
        private static MessagePiece ParseHtmlLocation(HtmlNode node)
        {
            var linkData = node.GetAttributeValue("param", "");

            var pars = linkData.Substring(6).Split('@');
            var locTree = pars[0].Split('_');
            var worldId = uint.Parse(locTree[0]);
            var guardId = uint.Parse(locTree[1]);
            var sectionId = uint.Parse(locTree[2]);
            if (worldId == 1 && guardId == 2 && sectionId == 9) sectionId = 7;
            var coords = pars[2].Split(',');
            var x = double.Parse(coords[0], CultureInfo.InvariantCulture);
            var y = double.Parse(coords[1], CultureInfo.InvariantCulture);

            var world = SessionManager.MapDatabase.Worlds[worldId];
            var guard = world.Guards[guardId];
            var section = guard.Sections[sectionId];
            var sb = new StringBuilder();

            var guardName = guard.NameId != 0 ? SessionManager.MapDatabase.Names[guard.NameId] : "";
            var sectionName = SessionManager.MapDatabase.Names[section.NameId];

            sb.Append("<");
            sb.Append(guardName);
            if (guardName != sectionName)
            {
                if (guardName != "") sb.Append(" - ");
                sb.Append(sectionName);
            }
            sb.Append(">");

            var result = new MessagePiece(sb.ToString())
            {
                Type = MessagePieceType.PointOfInterest,
                Location = new Location(worldId, guardId, sectionId, x, y),
                RawLink = linkData
            };
            return result;
        }
    }
}
