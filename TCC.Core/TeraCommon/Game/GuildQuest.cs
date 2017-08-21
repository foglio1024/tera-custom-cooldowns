using System;
using System.Collections.Generic;
using Tera.Game.Messages;

namespace Tera.Game
{
    public class GuildQuest
    {
        public S_GUILD_QUEST_LIST.GuildQuestType GuildQuestType1 { get; private set; }
        public S_GUILD_QUEST_LIST.GuildQuestType2 GuildQuestType2 { get; private set; }
        public string DescriptionLabel { get; private set; }

        public TimeSpan TimeRemaining { get; private set; }

        public string TitleLabel { get; private set; }
        public string GuildName { get; private set; }
        public bool Active { get; private set; }
        public S_GUILD_QUEST_LIST.QuestSizeType QuestSize { get; private set; }

        public List<GuildQuestItem> Rewards { get; private set; }
   
        public List<GuildQuestTarget> Targets { get; private set; }


        public GuildQuest(
            S_GUILD_QUEST_LIST.GuildQuestType guildQuestType1,
            S_GUILD_QUEST_LIST.GuildQuestType2 guildQuestType2,
            string descriptionLabel,
            string titleLabel,
            string guildName,
            List<GuildQuestTarget> targets,
            bool active,
            List<GuildQuestItem> rewards,
            ulong timeRemaining,
            S_GUILD_QUEST_LIST.QuestSizeType questSize
        
            )
        {

            GuildQuestType1 = guildQuestType1;
            GuildQuestType2 = guildQuestType2;
            DescriptionLabel = descriptionLabel;
            TitleLabel = titleLabel;
            GuildName = guildName;
            Active = active;
            Rewards = rewards;
            Targets = targets;
            TimeRemaining = TimeSpan.FromSeconds(timeRemaining);
            QuestSize = questSize;
        }

        public override string ToString()
        {
            var str = "GuildQuestType1: " + GuildQuestType1 + "\n" +
                "GuildQuestType2:" + GuildQuestType2 + "\n" +
                "GuildQuestDescriptionLabel:" + DescriptionLabel + "\n" +
                "GuildQuestTitleLabel:" + TitleLabel + "\n" +
                "GuildName:" + GuildName + "\n" +
                "Active:" + Active + "\n" +
                "Time remaining:" + TimeRemaining + "\n" +
                "Quest size:" + QuestSize;

            foreach(var target in Targets)
            {
                str += "\n-----\n"+target;
            }

            foreach(var reward in Rewards)
            {
                str += "\n-----\n"+reward;
            }
            return str;
                
        }

    }
}
