using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Parsing;
using Tera;
using Tera.Game;

namespace TCC
{
    public static class PacketInspector 
    {


        public static void InspectPacket(Message msg)
        {
            List<string> exclusionList = new List<string>()
            {
                "S_USER_LOCATION",
                "S_SOCIAL",
                "S_PARTY_MATCH_LINK",
                "C_PLAYER_LOCATION",
                "S_PLAYER_CHANGE_STAMINA",
                "S_START_COOLTIME_SKILL",
                "S_PLAYER_CHANGE_MP",
                "S_CREATURE_CHANGE_HP",
                "S_SPAWN_COLLECTION",
                "S_F2P_PremiumUser_Permission",
                "S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA",
                "C_SHOW_INVEN",
                "S_INVEN",
                "C_SHOW_ITEM_TOOLTIP_EX",
                "S_SHOW_ITEM_TOOLTIP",
                "S_CURRENT_CHANNEL",
                "S_ACTION_END",
                "S_ACTION_STAGE",
                "S_NPC_LOCATION",
                "S_BATTLE_FIELD_SEASON_RANKER",
                "S_ABNORMALITY_BEGIN",
                "S_DIALOG_EVENT"




            };
            var opName = PacketProcessor.OpCodeNamer.GetName(msg.OpCode);
            TeraMessageReader tmr = new TeraMessageReader(msg, PacketProcessor.OpCodeNamer, PacketProcessor.Version, PacketProcessor.SystemMessageNamer);
            if (exclusionList.Any(opName.Contains)) return;
//            if(opName.Equals("S_LOAD_TOPO") || opName.Equals("C_LOAD_TOPO_FIN")|| opName.Equals("S_SPAWN_ME"))
                Console.WriteLine(opName);
        }
    }
}
