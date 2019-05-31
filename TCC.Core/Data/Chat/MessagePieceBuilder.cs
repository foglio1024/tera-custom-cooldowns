using System;
using System.Globalization;
using System.Text;
using HtmlAgilityPack;
using TCC.Data.Map;
using FoglioUtils.Extensions;

namespace TCC.Data.Chat
{
    public static class MessagePieceBuilder
    {
        public static MessagePiece BuildSysMsgZone(string msgText)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(msgText);
            var zoneId = uint.Parse(dictionary["zoneName"]);
            var zoneName = SessionManager.DB.MonsterDatabase.GetZoneName(zoneId);
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

            if (SessionManager.DB.MonsterDatabase.TryGetMonster(templateId, zoneId, out var m))
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
                rawLink.Append("@" + uid);
            }

            if (dictionary.TryGetValue("UserName", out var username)) rawLink.Append("@" + username);
            else username = SessionManager.CurrentPlayer.Name;

            var name = $"Unknown item [{id}]";
            var grade = RareGrade.Common;
            if (SessionManager.DB.ItemsDatabase.Items.TryGetValue(id, out var i))
            {
                name = i.Name;
                grade = i.RareGrade;
            }
            var mp = new MessagePiece($"<{name}>")
            {
                Type = MessagePieceType.Item,
                //BoundType = i.BoundType,
                ItemId = id,
                ItemUid = uid,
                OwnerName = username,
                RawLink = rawLink.ToString()
            };
            mp.SetColor(ChatUtils.GradeToColorString(grade));
            return mp;
        }
        public static MessagePiece BuildSysMsgAchi(string msgText)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(msgText);

            var id = ChatUtils.GetId(dictionary, "achievement");
            var achiName = id.ToString();
            if (SessionManager.DB.AchievementDatabase.Achievements.TryGetValue(id * 1000 + 1, out var g2))
            {
                achiName = $"[{g2}]";

            }

            return new MessagePiece(achiName) { Type = MessagePieceType.Simple, };
        }
        public static MessagePiece BuildSysMsgQuest(string msgText)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(msgText);

            var id = ChatUtils.GetId(dictionary, "quest");
            var txt = id.ToString();
            if (SessionManager.DB.QuestDatabase.Quests.TryGetValue(id, out var q))
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

            if (SessionManager.DB.AchievementGradeDatabase.Grades.TryGetValue(id, out var g))
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
            if (SessionManager.DB.DungeonDatabase.Dungeons.TryGetValue(id, out var dung))
            {
                txt = dung.Name;
            }
            return new MessagePiece(txt) { Type = MessagePieceType.Simple };
        }
        public static MessagePiece BuildSysMsgAccBenefit(string msgText)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(msgText);

            var id = ChatUtils.GetId(dictionary, "accountBenefit");
            var txt = id.ToString();
            if (SessionManager.DB.AccountBenefitDatabase.Benefits.TryGetValue(id, out var ab))
            {
                txt = ab;
            }
            return new MessagePiece(txt) { Type = MessagePieceType.Simple };
        }
        public static MessagePiece BuildSysMsgGuildQuest(string msgText)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(msgText);

            var id = ChatUtils.GetId(dictionary, "GuildQuest");
            var questName = id.ToString();
            if (SessionManager.DB.GuildQuestDatabase.GuildQuests.TryGetValue(id, out var q))
            {
                questName = q.Title;
            }
            return new MessagePiece(questName) { Type = MessagePieceType.Simple };
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
                case 7:
                    mp = ParseHtmlAchievement(chatLinkAction);
                    break;
                default:
                    throw new Exception();
            }
            return mp;
        }
        private static MessagePiece ParseHtmlAchievement(HtmlNode node)
        {
            return new MessagePiece(node.InnerText.UnescapeHtml())
            {
                Type = MessagePieceType.Achievement,
                RawLink = node.GetAttributeValue("param", "")
            };
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


            var result = new MessagePiece(node.InnerText.UnescapeHtml())
            {
                ItemId = id,
                ItemUid = uid,
                OwnerName = owner,
                Type = MessagePieceType.Item,
                RawLink = linkData
            };
            return result;
        }
        private static MessagePiece ParseHtmlQuest(HtmlNode node)
        {
            return new MessagePiece(node.InnerText.UnescapeHtml())
            {
                Type = MessagePieceType.Quest,
                RawLink = node.GetAttributeValue("param", "")
            };
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

            var world = SessionManager.DB.MapDatabase.Worlds[worldId];
            var guard = world.Guards[guardId];
            var section = guard.Sections[sectionId];
            var sb = new StringBuilder();

            var guardName = guard.NameId != 0 ? SessionManager.DB.RegionsDatabase.Names[guard.NameId] : "";
            var sectionName = SessionManager.DB.RegionsDatabase.Names[section.NameId];

            sb.Append("<");
            sb.Append(guardName);
            if (guardName != sectionName)
            {
                if (guardName != "") sb.Append(" - ");
                sb.Append(sectionName);
            }
            sb.Append(">");

            return new MessagePiece(sb.ToString())
            {
                Type = MessagePieceType.PointOfInterest,
                Location = new Location(worldId, guardId, sectionId, x, y),
                RawLink = linkData
            };
        }

        public static MessagePiece BuildSysMsgRegion(string inPiece)
        {
            var dictionary = ChatUtils.BuildParametersDictionary(inPiece);
            var regId = dictionary["rgn"];
            var msgText = regId;
            if (SessionManager.DB.RegionsDatabase.Names.TryGetValue(Convert.ToUInt32(regId), out var regName))
            {
                msgText = regName;
            }
            return new MessagePiece(msgText) { Type = MessagePieceType.Simple };

        }
    }
}
