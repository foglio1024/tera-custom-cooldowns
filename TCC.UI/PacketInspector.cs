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
                "S_USER_LOCATION", "S_SOCIAL", "PROJECTILE", "S_PARTY_MATCH_LINK", "C_PLAYER_LOCATION", "S_NPC_LOCATION",
                "S_CHAT", "S_PLAYER_CHANGE_STAMINA", "S_SPAWN_NPC" , "S_START_COOLTIME_SKILL",  "S_ACTION_END" , "S_ACTION_STAGE",
                "S_PLAYER_CHANGE_MP", "S_DESPAWN_NPC", "S_CREATURE_CHANGE_HP", "S_EACH_SKILL_RESULT", "S_SPAWN_COLLECTION", "S_F2P_PremiumUser_Permission",

            };
            var opName = PacketRouter.OpCodeNamer.GetName(msg.OpCode);
            TeraMessageReader tmr = new TeraMessageReader(msg, PacketRouter.OpCodeNamer, PacketRouter.Version, PacketRouter.SystemMessageNamer);
            if (exclusionList.Any(opName.Contains)) return;
//            if(opName.Equals("S_LOAD_TOPO") || opName.Equals("C_LOAD_TOPO_FIN")|| opName.Equals("S_SPAWN_ME"))
                Console.WriteLine(opName);
        }
    }
}
