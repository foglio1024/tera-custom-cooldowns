using System;
using System.Collections.Generic;
using TCC.Data;
using TCC.ViewModels;
namespace TCC.Parsing
{
    public static class SystemMessagesProcessor
    {
        private static readonly List<string> ExclusionList = new List<string>
        {
            "SMT_ITEM_ROULETTE_VALUE",
            "SMT_BATTLE_START",
            "SMT_BATTLE_END",
            "SMT_DROPDMG_DAMAGE",
            "SMT_ABANDON_DIVIDE_DICE_PARTYPLAYER",
            "SMT_JOIN_DIVIDE_DICE_PATYPLAYER",
            "SMT_ABANDON_DIVIDE_DICE",
            "SMT_JOIN_DIVIDE_DICE",
            "SMT_INCREASE_SKILL_PROF_MORE"
        };

        private static bool Filter(string opcodeName)
        {
            return !ExclusionList.Contains(opcodeName);
        }
        internal static void AnalyzeMessage(string srvMsg, SystemMessage sysMsg, string opcodeName)
        {
            if (!Filter(opcodeName)) return;

            if (!Process(srvMsg, sysMsg, opcodeName))
            {
                ChatWindowManager.Instance.AddChatMessage(new ChatMessage(srvMsg, sysMsg, (ChatChannel)sysMsg.ChatChannel));
            }
        }
        private static void HandleMaxEnchantSucceed(string x)
        {
            var sysMsg = ChatMessage.BuildEnchantSystemMessage(x);
            ChatWindowManager.Instance.AddChatMessage(sysMsg);
        }
        private static void HandleFriendLogin(string friendName, SystemMessage sysMsg)
        {
            var sysmsg = "@0\vUserName\v" + friendName;
            var msg = new ChatMessage(sysmsg, sysMsg, ChatChannel.Friend) {Author = friendName};
            ChatWindowManager.Instance.AddChatMessage(msg);
        }

        #region Factory

        private static readonly Dictionary<string, Delegate> Processor = new Dictionary<string, Delegate>
        {
            { "SMT_MAX_ENCHANT_SUCCEED", new Action<string, SystemMessage>((srvMsg, sysMsg) => HandleMaxEnchantSucceed(srvMsg)) },
            { "SMT_FRIEND_IS_CONNECTED", new Action<string, SystemMessage>(HandleFriendLogin) },
            { "SMT_FRIEND_WALK_INTO_SAME_AREA", new Action<string, SystemMessage>(HandleFriendInAreaMessage) },
            { "SMT_CHAT_LINKTEXT_DISCONNECT", new Action<string, SystemMessage>(HandleInvalidLink) },

            { "SMT_BATTLE_PARTY_DIE", new Action<string, SystemMessage>(HandleDeathMessage) },
            { "SMT_BATTLE_PARTY_RESURRECT", new Action<string, SystemMessage>(HandleRessMessage) },
            { "SMT_BATTLE_YOU_DIE", new Action<string, SystemMessage>(HandleDeathMessage) },
            { "SMT_BATTLE_RESURRECT", new Action<string, SystemMessage>(HandleRessMessage) },

            { "SMT_ACCEPT_QUEST", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_CANT_START_QUEST", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_COMPLATE_GUILD_QUEST", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_COMPLETE_MISSION", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_COMPLETE_QUEST", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_FAILED_QUEST_COMPENSATION", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_FAILED_QUEST", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_FAILED_QUEST_CANCLE", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_QUEST_FAILED_GET_FLAG_PARTY_LEVEL", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_QUEST_ITEM_DELETED", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_QUEST_RESET_MESSAGE", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_UPDATE_QUEST_TASK", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_QUEST_SHARE_MESSAGE2", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_QUEST_USE_SKILL", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_QUEST_USE_ITEM", new Action<string, SystemMessage>(HandleQuestMessage) },
            { "SMT_GRANT_DUNGEON_COOLTIME_AND_COUNT", new Action<string, SystemMessage>(HandleDungeonEngagedMessage) },
            { "SMT_GQUEST_URGENT_NOTIFY", new Action<string, SystemMessage>(HandleGuilBamSpawn) },

            { "SMT_PARTY_LOOT_ITEM_PARTYPLAYER", new Action<string, SystemMessage>(HandleGroupMemberLoot) },

            { "SMT_GC_SYSMSG_GUILD_CHIEF_CHANGED", new Action<string, SystemMessage>(HandleNewGuildMasterMEssage) },

            { "SMT_ACCOMPLISH_ACHIEVEMENT_GRADE_ALL", new Action<string, SystemMessage>(HandleLaurelMessage) },
            { "SMT_ACCOMPLISH_ACHIEVEMENT_GRADE_GUILD", new Action<string, SystemMessage>(DoNothing) },
            { "SMT_ACCOMPLISH_ACHIEVEMENT_GRADE_PARTY", new Action<string, SystemMessage>(DoNothing) },

        };

        private static void HandleNewGuildMasterMEssage(string srvMsg, SystemMessage sysMsg)
        {
            var msg = new ChatMessage(srvMsg, sysMsg, ChatChannel.GuildNotice);
            ChatWindowManager.Instance.AddChatMessage(msg);
            WindowManager.FloatingButton.NotifyExtended("Guild", msg.ToString(), NotificationType.Success);

        }

        private static void DoNothing(string srvMsg, SystemMessage sysMsg)
        {

        }

        private static void HandleLaurelMessage(string srvMsg, SystemMessage sysMsg)
        {
            var msg = new ChatMessage(srvMsg, sysMsg, ChatChannel.Laurel);
            ChatWindowManager.Instance.AddChatMessage(msg);
        }

        private static void HandleGroupMemberLoot(string srvMsg, SystemMessage sysMsg)
        {
            var msg = new ChatMessage(srvMsg, sysMsg, ChatChannel.Loot);
            ChatWindowManager.Instance.AddChatMessage(msg);
        }
        private static void HandleGuilBamSpawn(string srvMsg, SystemMessage sysMsg)
        {
            TimeManager.Instance.UploadGuildBamTimestamp();
            TimeManager.Instance.SetGuildBamTime(true);
            TimeManager.Instance.SendWebhookMessageOld();
            var msg = new ChatMessage(srvMsg, sysMsg, (ChatChannel)sysMsg.ChatChannel);
            ChatWindowManager.Instance.AddChatMessage(msg);
            WindowManager.FloatingButton.NotifyExtended("Guild BAM", msg.ToString(), NotificationType.Normal);
        }
        private static void HandleDungeonEngagedMessage(string srvMsg, SystemMessage sysMsg)
        {
            const string s = "dungeon:";
            var dgId = Convert.ToUInt32(srvMsg.Substring(srvMsg.IndexOf(s, StringComparison.Ordinal) + s.Length));
            InfoWindowViewModel.Instance.EngageDungeon(dgId);

            var msg = new ChatMessage(srvMsg, sysMsg, (ChatChannel)sysMsg.ChatChannel);
            ChatWindowManager.Instance.AddChatMessage(msg);
        }
        private static void HandleFriendInAreaMessage(string srvMsg, SystemMessage sysMsg)
        {
            var msg = new ChatMessage(srvMsg, sysMsg, ChatChannel.Friend);
            var start = srvMsg.IndexOf("UserName\v", StringComparison.InvariantCultureIgnoreCase) + "UserName\v".Length;
            var end = srvMsg.IndexOf("\v", start, StringComparison.InvariantCultureIgnoreCase);
            var friendName = srvMsg.Substring(start, end - start);
            msg.Author = friendName;
            ChatWindowManager.Instance.AddChatMessage(msg);
        }
        private static void HandleQuestMessage(string srvMsg, SystemMessage sysMsg)
        {
            var msg = new ChatMessage(srvMsg, sysMsg, ChatChannel.Quest);
            ChatWindowManager.Instance.AddChatMessage(msg);
        }
        private static void HandleRessMessage(string srvMsg, SystemMessage sysMsg)
        {
            var msg = new ChatMessage(srvMsg, sysMsg, ChatChannel.Ress);
            ChatWindowManager.Instance.AddChatMessage(msg);
        }
        private static void HandleDeathMessage(string srvMsg, SystemMessage sysMsg)
        {
            var msg = new ChatMessage(srvMsg, sysMsg, ChatChannel.Death);
            ChatWindowManager.Instance.AddChatMessage(msg);
        }
        private static void HandleInvalidLink(string srvMsg, SystemMessage sysMsg)
        {
            ChatWindowManager.Instance.AddChatMessage(new ChatMessage(srvMsg, sysMsg, (ChatChannel)sysMsg.ChatChannel));
            ChatWindowManager.Instance.RemoveDeadLfg();
            if (Settings.LfgEnabled) WindowManager.LfgListWindow.VM.RemoveDeadLfg();
        }

        private static bool Process(string serverMsg, SystemMessage sysMsg, string opcodeName)
        {
            Processor.TryGetValue(opcodeName, out var type);
            if (type == null) return false;
            type.DynamicInvoke(serverMsg, sysMsg);
            return true;
        }
        #endregion
    }
}