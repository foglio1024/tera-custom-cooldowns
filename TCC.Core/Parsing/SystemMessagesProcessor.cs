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
            "SMT_BATTLE_END"
        };
        public static bool Filter(string opcodeName)
        {
            return !ExclusionList.Contains(opcodeName);
        }
        internal static void AnalyzeMessage(S_SYSTEM_MESSAGE srvMsg, SystemMessage sysMsg, string opcodeName)
        {
            if (!Filter(opcodeName)) return;

            if (!Process(srvMsg, sysMsg, opcodeName))
            {
                ChatWindowViewModel.Instance.AddChatMessage(new ChatMessage(srvMsg.Message, sysMsg, opcodeName));
            }
        }
        private static void HandleMaxEnchantSucceed(S_SYSTEM_MESSAGE x)
        {
            ChatMessage sysMsg = ChatMessage.BuildEnchantSystemMessage(x.Message);
            ChatWindowViewModel.Instance.AddChatMessage(sysMsg);
        }

        #region Factory
        static Dictionary<string, Delegate> Processor = new Dictionary<string, Delegate>
        {
            { "SMT_MAX_ENCHANT_SUCCEED", new Action<S_SYSTEM_MESSAGE, SystemMessage>((srvMsg, sysMsg) => HandleMaxEnchantSucceed(srvMsg)) },
        };
        public static bool Process(S_SYSTEM_MESSAGE serverMsg, SystemMessage sysMsg, string opcodeName)
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