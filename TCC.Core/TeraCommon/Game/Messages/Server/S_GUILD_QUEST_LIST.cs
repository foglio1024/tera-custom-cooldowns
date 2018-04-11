using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Tera.Game.Messages
{
    public class S_GUILD_QUEST_LIST : ParsedMessage
    {
        public enum GuildQuestType
        {
            Hunt = 1,
            Battleground = 2,
            Gathering = 3

        }

        public enum GuildQuestType2
        {
            Hunt = 0,
            Gathering = 2,
            Battleground = 4

        }

        public enum QuestSizeType {
            Small = 0,
            Medium = 1,
            Big = 2
        }



        public ulong Gold { get; private set; }
        public uint NumberCharacters { get; private set; }
        public uint NumberAccount { get; private set; }
        public string GuildName { get; private set; }
        public string GuildMaster { get; private set; }
        public uint GuildLevel { get; private set; }

        public uint NumberQuestsDone { get; private set; }
        public uint NumberTotalDailyQuest { get; private set; }

        public override string ToString()
        {
            var str =  "Guild name:" + GuildName + "\n" +
                "Guild master:" + GuildMaster + "\n" +
                "Guild level:" + GuildLevel + "\n" +
                "Number accounts:" + NumberAccount + "\n" +
                "Number characters:" + NumberCharacters + "\n" +
                "Gold:" + Gold + "\n"+
                "NumberTotalDailyQuest:"+NumberTotalDailyQuest+"\n"+
                "NumberQuestsDone:"+NumberQuestsDone+"\n"+
                "Current XP:"+ GuildXpCurrent+"\n"+
                "XP for next level:"+ GuildXpNextLevel+"\n"+
                "XP to gain for next level:"+ (GuildXpNextLevel - GuildXpCurrent)+"\n"+
                "Guild size:"+ GuildSize+"\n"+
                "Guild creation date:"+ GuildCreationTime+"\n"
                ;
            foreach(var quest in GuildQuests)
            {
                str += "\n=====\n" + quest;
            }
            return str;

        }

        public GuildQuest ActiveQuest()
        {
            return GuildQuests.Where(x => x.Active == true).FirstOrDefault();
        }

        public List<GuildQuest> ActiveQuests()
        {
            return GuildQuests.Where(x => x.Active).ToList();
        }

        public List<GuildQuest> GuildQuests { get; private set; }

        public ulong GuildXpCurrent { get; private set; }
        public ulong GuildXpNextLevel { get; private set; }

        public GuildSizeType GuildSize { get; private set; }
        public DateTime GuildCreationTime { get; private set; }

        public enum GuildSizeType
        {
            Small = 0,
            Medium = 1,
            Big = 2
        }


        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        internal S_GUILD_QUEST_LIST(TeraMessageReader reader) : base(reader)
        {
            //PrintRaw();
            GuildQuests = new List<GuildQuest>();
            var counter = reader.ReadUInt16();
            var questOffset = reader.ReadUInt16();
            var guildNameOffset = reader.ReadUInt16();
            var guildMasterOffset = reader.ReadUInt16();

            var guildId = reader.ReadUInt32();
            var guildMasterId = reader.ReadInt32();

            GuildLevel = reader.ReadUInt32();
            GuildXpCurrent = reader.ReadUInt64();
            GuildXpNextLevel = reader.ReadUInt64();
            Gold = reader.ReadUInt64();
            NumberCharacters = reader.ReadUInt32();
            NumberAccount = reader.ReadUInt32();
            GuildSize = (GuildSizeType)reader.ReadUInt32();
            GuildCreationTime = UnixTimeStampToDateTime(reader.ReadUInt64());
            NumberQuestsDone = reader.ReadUInt32();
            NumberTotalDailyQuest = reader.ReadUInt32();
            GuildName = reader.ReadTeraString();
            GuildMaster = reader.ReadTeraString();
            
            for (var i = 1; i <= counter; i++)
            {
                reader.BaseStream.Position = questOffset - 4;
                var pointer = reader.ReadUInt16();
                Debug.Assert(pointer == questOffset);//should be the same
                var nextOffset = reader.ReadUInt16();

                var countTargets = reader.ReadUInt16();
                var offsetTargets = reader.ReadUInt16();

                var countUnk2 = reader.ReadUInt16();
                var offsetUnk2 = reader.ReadUInt16();

                var countRewards = reader.ReadUInt16();
                var offsetRewards = reader.ReadUInt16();

                var offsetName = reader.ReadUInt16();
                var offsetDescription = reader.ReadUInt16();
                var offsetGuildName = reader.ReadUInt16();

                var id = reader.ReadUInt32();
                var questType2 = (GuildQuestType2)reader.ReadUInt32();
                var questSize = (QuestSizeType)reader.ReadUInt32();
                var unk3 = reader.ReadByte();
                var unk4 = reader.ReadUInt32();
                var active = reader.ReadByte();
                //Debug.WriteLine(active.ToString("X"));
                var activeBool = active == 1;
                var unk7 = reader.ReadBytes(3);
                
                //in seconds
                var timeRemaining = reader.ReadUInt32();


                var guildQuestType = (GuildQuestType) reader.ReadUInt32();
                var unk5 = reader.ReadByte();
                var unk6 = reader.ReadInt32();
               
                var guildQuestDescriptionLabel = reader.ReadTeraString();
                var guildQuestTitleLabel = reader.ReadTeraString();
                var questguildname = reader.ReadTeraString();
                //Debug.WriteLine(
                // ";unk3:" + unk3 +
                // ";unk4:" + unk4 +
                // ";unk5:" + unk5.ToString("X") +
                // ";unk6:" + unk6 +
                // ";unk7:" + BitConverter.ToString(unk7) +
                // ";countUnk2:" + countUnk2 +
                // ";offsetUnk2:" + offsetUnk2
                // );
                List<GuildQuestTarget> targets = new List<GuildQuestTarget>();
                reader.BaseStream.Position = offsetTargets - 4;
                for (var j = 1; j <= countTargets; j++)
                {
                    var currentPosition = reader.ReadUInt16();
                    var nextTargetOffset = reader.ReadUInt16();
                    var zoneId = reader.ReadUInt32();
                    var targetId = reader.ReadUInt32();
                    var countQuest = reader.ReadUInt32();
                    var totalQuest = reader.ReadUInt32();
                    targets.Add(new GuildQuestTarget(zoneId, targetId, countQuest, totalQuest));
                }

                var nextUnk2Offset = offsetUnk2;
                for (var j = 1; j <= countUnk2; j++)
                {
                    reader.BaseStream.Position = nextUnk2Offset - 4;
                    var currentPosition = reader.ReadUInt16();
                    nextUnk2Offset = reader.ReadUInt16();
                    Debug.WriteLine("unk2:" + reader.ReadByte().ToString("X") + " ;" + j + "/" + countUnk2);
                }

                List<GuildQuestItem> rewards = new List<GuildQuestItem>();
                reader.BaseStream.Position = offsetRewards - 4;
                for (var j = 1; j <= countRewards; j++)
                {
                    var currentPosition = reader.ReadUInt16();
                    var nextRewardOffset = reader.ReadUInt16();
                    var item = reader.ReadUInt32();
                    var amount = reader.ReadUInt64();

                    rewards.Add(new GuildQuestItem(item, amount));
                }

                questOffset = nextOffset;

                var quest = new GuildQuest(
               guildQuestType,
               questType2,
               guildQuestDescriptionLabel,
               guildQuestTitleLabel,
               questguildname,
               targets,
               activeBool,
               rewards,
               timeRemaining,
               questSize

               );
                GuildQuests.Add(quest);
             
            }

           // Debug.WriteLine(ToString());
        }

    }
}

