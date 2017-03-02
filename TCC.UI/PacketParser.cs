using DamageMeter.Sniffing;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Messages;
using TCC.UI.Messages;
using Tera.Game;

namespace TCC.UI
{
    public static class PacketParser
    {

        public static OpCodeNamer OpCodeNamer;
        public static OpCodeNamer SystemMessageNamer;
        public static uint Version;

        public static CharListProcessor clp = new CharListProcessor();
        public static CharLoginProcessor cl = new CharLoginProcessor();

        public static Class CurrentClass;

        public static void MessageReceived(global::Tera.Message obj)
        {
            if (obj.Direction == Tera.MessageDirection.ClientToServer && obj.OpCode == 19900)
            {
                var msg = new C_CHECK_VERSION_CUSTOM(new CustomReader(obj));
                Version = msg.Versions[0];
                TeraSniffer.Instance.opn = new OpCodeNamer(System.IO.Path.Combine(BasicTeraData.Instance.ResourceDirectory, $"data/opcodes/{Version}.txt"));
                OpCodeNamer = TeraSniffer.Instance.opn;
                SystemMessageNamer = new OpCodeNamer(System.IO.Path.Combine(BasicTeraData.Instance.ResourceDirectory, $"data/opcodes/smt_{Version}.txt"));
            }
            RoutePacket(obj);
        }
        static string PacketData(Tera.Message msg)
        {
            byte[] data = new byte[msg.Data.Count];
            Array.Copy(msg.Data.Array, 0, data, 2, msg.Data.Count - 2);
            data[0] = (byte)(((short)msg.Data.Count) & 255);
            data[1] = (byte)(((short)msg.Data.Count) >> 8);
            return StringUtils.ByteArrayToString(data).ToUpper();

        }
        private static void RoutePacket(Tera.Message msg)
        {
            switch (TeraSniffer.Instance.opn.GetName(msg.OpCode))
            {
                case ("S_GET_USER_LIST"):
                    clp.ParseCharacters(PacketData(msg));
                    break;
                case ("S_LOGIN"):
                    CurrentClass = clp.GetClassFromName(cl.GetName(PacketData(msg)));
                    clp.Clear();
                    Console.WriteLine(CurrentClass);
                    break;
                case ("S_START_COOLTIME_SKILL"):
                    var m = new S_START_COOLTIME_SKILL(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    SkillManager.AddSkill(new SkillCooldown(m.SkillId, m.Cooldown, CooldownType.Skill));
                    break;
                case ("S_START_COOLTIME_ITEM"):
                    var i = new S_START_COOLTIME_ITEM(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    SkillManager.AddSkill(new SkillCooldown(i.ItemId, i.Cooldown, CooldownType.Item));
                    break;
                case ("S_DECREASE_COOLTIME_SKILL"):
                    var n = new S_DECREASE_COOLTIME_SKILL(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    SkillManager.ChangeSkillCooldown(new SkillCooldown(n.SkillId, n.Cooldown, CooldownType.Skill));
                    break;
                case ("S_RETURN_TO_LOBBY"):
                    SkillManager.Clear();
                    break;
                default:
                    break;
            }
        }
    }
}
