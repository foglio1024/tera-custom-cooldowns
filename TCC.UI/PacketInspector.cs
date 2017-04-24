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
                "S_USER_LOCATION", "S_SOCIAL", "PROJECTILE", "S_PARTY_MATCH_LINK", "C_PLAYER_LOCATION", "S_NPC_LOCATION"
            };
            var opName = PacketRouter.OpCodeNamer.GetName(msg.OpCode);
            TeraMessageReader tmr = new TeraMessageReader(msg, PacketRouter.OpCodeNamer, PacketRouter.Version, PacketRouter.SystemMessageNamer);
            if (exclusionList.Any(opName.Contains)) return;
            if (opName.Equals("S_LOAD_HINT"))
            {
                Console.WriteLine("[{0}] ({1})", opName, msg.Payload.Count);
                StringBuilder sb = new StringBuilder();
                foreach (var b in msg.Payload)
                {
                    sb.Append(b);

                }
                int j = 0;
                for (int i = 8; i+j < sb.Length; i+=8)
                {
                    sb.Insert(i+j, " ");
                    j++;
                }
                Console.WriteLine(sb.ToString());
            }
        }
    }
}
