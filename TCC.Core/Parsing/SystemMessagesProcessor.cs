using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Data;
using TCC.Data.Databases;
using TCC.Parsing.Messages;
using TCC.ViewModels;
namespace TCC.Parsing
{
    public static class SystemMessagesProcessor
    {
        static List<string> ExclusionList = new List<string>
        {
            "SMT_ITEM_ROULETTE_VALUE",
            "SMT_BATTLE_START",
            "SMT_BATTLE_END",
            "SMT_DROPDMG_DAMAGE",
            "SMT_BATTLE_PARTY_DIE",
            "SMT_BATTLE_PARTY_RESURRECT",
            "SMT_BATTLE_YOU_DIE",
            "SMT_BATTLE_RESURRECT",
            "SMT_ABANDON_DIVIDE_DICE_PARTYPLAYER",
            "SMT_JOIN_DIVIDE_DICE_PATYPLAYER"
        };
        public static bool Filter(string opcodeName)
        {
            return !ExclusionList.Contains(opcodeName);
        }
        internal static void AnalyzeMessage(string srvMsg, SystemMessage sysMsg, string opcodeName)
        {
            if (!Filter(opcodeName)) return;

            if (!Process(srvMsg, sysMsg, opcodeName))
            {
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(srvMsg, sysMsg));
            }
        }
        private static void HandleMaxEnchantSucceed(string x)
        {
            ChatMessage sysMsg = ChatMessage.BuildEnchantSystemMessage(x);
            ChatWindowViewModel.Instance.AddChatMessage(sysMsg);
        }
        private static void HandleFriendLogin(string friendName, SystemMessage sysMsg)
        {
            var sysmsg = "@0\vUserName\v" + friendName;
            var msg = new ChatMessage(sysmsg, sysMsg);
            ChatWindowViewModel.Instance.AddChatMessage(msg);
        }

        #region Factory
        static Dictionary<string, Delegate> Processor = new Dictionary<string, Delegate>
        {
            { "SMT_MAX_ENCHANT_SUCCEED", new Action<string, SystemMessage>((srvMsg, sysMsg) => HandleMaxEnchantSucceed(srvMsg)) },
            { "SMT_FRIEND_IS_CONNECTED", new Action<string, SystemMessage>((srvMsg, sysMsg) => HandleFriendLogin(srvMsg, sysMsg)) },
            { "SMT_CHAT_LINKTEXT_DISCONNECT", new Action<string, SystemMessage>((srvMsg, sysMsg) => HandleInvalidLink(srvMsg, sysMsg)) },
            { "SMT_BATTLE_PARTY_DIE", new Action<string, SystemMessage>((srvMsg, sysMsg) => HandlePartyMemberDeath(srvMsg, sysMsg)) },
            { "SMT_BATTLE_PARTY_RESURRECT", new Action<string, SystemMessage>((srvMsg, sysMsg) => HandlePartyMemberRess(srvMsg, sysMsg)) },
        };

        private static void HandlePartyMemberRess(string srvMsg, SystemMessage sysMsg)
        {
            var msg = new ChatMessage(srvMsg, sysMsg);
            ChatMessage.SetChannel(msg, ChatChannel.Ress);
            ChatWindowViewModel.Instance.AddChatMessage(msg);

        }

        private static void HandlePartyMemberDeath(string srvMsg, SystemMessage sysMsg)
        {
            var msg = new ChatMessage(srvMsg, sysMsg);
            ChatMessage.SetChannel(msg, ChatChannel.Death);
            ChatWindowViewModel.Instance.AddChatMessage(msg);
        }

        private static void HandleInvalidLink(string srvMsg, SystemMessage sysMsg)
        {
            ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(srvMsg, sysMsg));
            ChatWindowViewModel.Instance.RemoveDeadLfg();
        }

        public static bool Process(string serverMsg, SystemMessage sysMsg, string opcodeName)
        {
            Delegate type;
            Processor.TryGetValue(opcodeName, out type);
            if (type == null) return false;
            type.DynamicInvoke(serverMsg, sysMsg);
            return true;
        }
        #endregion
    }
}