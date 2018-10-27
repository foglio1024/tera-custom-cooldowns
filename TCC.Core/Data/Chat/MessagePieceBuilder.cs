using System.Collections.Generic;
using System.Text;

namespace TCC.Data.Chat
{
    public class MessagePieceBuilder
    {
        private readonly ChatChannel _channel;
        public MessagePieceBuilder(ChatChannel channel)
        {
            _channel = channel;
        }

        public MessagePiece ParseSysMsgZone(Dictionary<string, string> dictionary)
        {
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
        public MessagePiece ParseSysMsgCreature(Dictionary<string, string> dictionary)
        {
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
        public MessagePiece ParseSysMsgItem(Dictionary<string, string> info)
        {
            var id = ChatUtils.GetId(info, "item");
            var uid = ChatUtils.GetItemUid(info);

            var rawLink = new StringBuilder("1#####");
            rawLink.Append(id.ToString());
            if (uid != 0)
            {
                rawLink.Append("@" + uid.ToString());
            }

            var username = SessionManager.CurrentPlayer.Name;
            if (info.ContainsKey("UserName"))
            {
                username = info["UserName"];
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
        public MessagePiece ParseSysMsgAchi(Dictionary<string, string> info)
        {
            var id = ChatUtils.GetId(info, "achievement");
            var achiName = id.ToString();
            if (SessionManager.AchievementDatabase.Achievements.TryGetValue(id, out var g))
            {
                achiName = $"[{g}]";
            }
            return new MessagePiece(achiName, MessagePieceType.Simple, _channel, Settings.Settings.FontSize, false);
        }
        public MessagePiece ParseSysMsgQuest(Dictionary<string, string> info)
        {
            var id = ChatUtils.GetId(info, "quest");
            var txt = id.ToString();
            if (SessionManager.QuestDatabase.Quests.TryGetValue(id, out var q))
            {
                txt = q;
            }
            return new MessagePiece(txt, MessagePieceType.Simple, _channel, Settings.Settings.FontSize, false);
        }
        public MessagePiece ParseSysMsgAchiGrade(Dictionary<string, string> info)
        {
            var id = ChatUtils.GetId(info, "AchievementGradeInfo");
            var txt = id.ToString();
            var col = "fcb06f";

            if (SessionManager.AchievementGradeDatabase.Grades.TryGetValue(id, out var g))
            {
                txt = g;
                if (id == 104) col = "38bde5";
                if (id == 105) col = "ff264b";

            }
            return new MessagePiece(txt, MessagePieceType.Simple, _channel, Settings.Settings.FontSize, false, col);
        }
        public MessagePiece ParseSysMsgDungeon(Dictionary<string, string> info)
        {
            var id = ChatUtils.GetId(info, "dungeon");
            var txt = id.ToString();
            if (SessionManager.DungeonDatabase.DungeonNames.TryGetValue(id, out var dngName))
            {
                txt = dngName;
            }
            return new MessagePiece(txt, MessagePieceType.Simple, _channel, Settings.Settings.FontSize, false);
        }
        public MessagePiece ParseSysMsgAccBenefit(Dictionary<string, string> info)
        {
            var id = ChatUtils.GetId(info, "accountBenefit");
            var txt = id.ToString();
            if (SessionManager.AccountBenefitDatabase.Benefits.TryGetValue(id, out var ab))
            {
                txt = ab;
            }
            return new MessagePiece(txt, MessagePieceType.Simple, _channel, Settings.Settings.FontSize, false);
        }
        public MessagePiece ParseSysMsgGuildQuest(Dictionary<string, string> info)
        {
            var id = ChatUtils.GetId(info, "GuildQuest");
            var questName = id.ToString();
            if (SessionManager.GuildQuestDatabase.GuildQuests.TryGetValue(id, out var q))
            {
                questName = q.Title;
            }
            return new MessagePiece(questName, MessagePieceType.Simple, _channel, Settings.Settings.FontSize, false);
        }


    }
}
