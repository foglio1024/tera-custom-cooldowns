using DamageMeter.Sniffing;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
        
        //public static CharListProcessor clp = new CharListProcessor();
        public static CharLoginProcessor cl = new CharLoginProcessor();

        public static Class CurrentClass;
        public static ulong CurrentCharId;

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
                case ("S_LOGIN"):
                    var sLogin = new S_LOGIN(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    CurrentClass = sLogin.GetClass();
                    CurrentCharId = sLogin.entityId;
                    if(CurrentClass == Class.Warrior)
                    {
                        MainWindow.ShowEdgeGauge();
                    }
                    Console.WriteLine("{0} - {1}", CurrentClass, CurrentCharId);
                    break;
                case ("S_START_COOLTIME_SKILL"):
                    var sStartCooltimeSkill = new S_START_COOLTIME_SKILL(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    SkillManager.AddSkill(new SkillCooldown(sStartCooltimeSkill.SkillId, sStartCooltimeSkill.Cooldown, CooldownType.Skill));
                    break;
                case ("S_START_COOLTIME_ITEM"):
                    var sStartCooltimeItem = new S_START_COOLTIME_ITEM(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    SkillManager.AddSkill(new SkillCooldown(sStartCooltimeItem.ItemId, sStartCooltimeItem.Cooldown, CooldownType.Item));
                    break;
                case ("S_DECREASE_COOLTIME_SKILL"):
                    var sDecreaseCooltimeSkill = new S_DECREASE_COOLTIME_SKILL(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    SkillManager.ChangeSkillCooldown(new SkillCooldown(sDecreaseCooltimeSkill.SkillId, sDecreaseCooltimeSkill.Cooldown, CooldownType.Skill));
                    break;
                case ("S_RETURN_TO_LOBBY"):
                    SkillManager.Clear();
                    break;
                case ("S_ABNORMALITY_BEGIN"):
                    var sAbnormalityBegin = new S_ABNORMALITY_BEGIN(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    if (sAbnormalityBegin.isHurricane && sAbnormalityBegin.casterId == CurrentCharId)
                    {
                        SkillManager.AddSkill(new SkillCooldown(60010, 120000, CooldownType.Skill));
                    }
                    break;
                case ("S_PLAYER_STAT_UPDATE"):
                    if (CurrentClass == Class.Warrior)
                    {
                        var sPlayerStatUpdate = new S_PLAYER_STAT_UPDATE(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                        EdgeWindow.SetEdge(sPlayerStatUpdate.edge);
                    }
                    break;
                case ("S_USER_STATUS"):
                    var sUserStatus = new S_USER_STATUS(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    if (sUserStatus.id == CurrentCharId && sUserStatus.isInCombat)
                    {
                        MainWindow.UndimEdgeGauge();
                    }
                    else
                    {
                        MainWindow.DimEdgeGauge();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
