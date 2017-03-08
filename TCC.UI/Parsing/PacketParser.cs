using DamageMeter.Sniffing;
using Data;
using System;
using System.Collections.Generic;
using TCC.Messages;
using Tera.Game;

namespace TCC
{
    public delegate void UpdateStatEventHandler(int statValue);

    public static class PacketParser
    {
        public static OpCodeNamer OpCodeNamer;
        public static OpCodeNamer SystemMessageNamer;
        public static uint Version;

        static CharListProcessor CLP = new CharListProcessor();

        public static Dictionary<string, Laurel> LaurelsDictionary;

        public static bool Logged { get; set; }
        
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
        public static string PacketData(Tera.Message msg)
        {
            byte[] data = new byte[msg.Data.Count];
            Array.Copy(msg.Data.Array, 0, data, 2, msg.Data.Count - 2);
            data[0] = (byte)(((short)msg.Data.Count) & 255);
            data[1] = (byte)(((short)msg.Data.Count) >> 8);
            return StringUtils.ByteArrayToString(data).ToUpper();

        }

        public static event UpdateStatEventHandler MaxHPUpdated;
        public static event UpdateStatEventHandler MaxMPUpdated;
        public static event UpdateStatEventHandler MaxSTUpdated;

        public static event UpdateStatEventHandler HPUpdated;
        public static event UpdateStatEventHandler MPUpdated;
        public static event UpdateStatEventHandler STUpdated;

        private static void RoutePacket(Tera.Message msg)
        {
            switch (TeraSniffer.Instance.opn.GetName(msg.OpCode))
            {
                case ("S_LOGIN"):
                    Logged = true;
                    var sLogin = new S_LOGIN(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    CurrentClass = sLogin.CharacterClass;
                    CurrentCharId = sLogin.entityId;
                    WindowManager.SetClass(CurrentClass);
                    WindowManager.SetLaurel(CLP.GetLaurelFromName(sLogin.Name));
                    switch (CurrentClass)
                    {
                        case Class.Warrior:
                            WindowManager.ShowEdgeGauge();
                            WindowManager.HPBars.ShowStamina();
                            break;
                        case Class.Lancer:
                            WindowManager.HPBars.ShowStamina();
                            break;
                        case Class.Slayer:
                            WindowManager.HPBars.HideStamina();
                            break;
                        case Class.Berserker:
                            WindowManager.HPBars.HideStamina();
                            break;
                        case Class.Sorcerer:
                            WindowManager.HPBars.HideStamina();
                            break;
                        case Class.Archer:
                            WindowManager.HPBars.HideStamina();
                            break;
                        case Class.Priest:
                            WindowManager.HPBars.HideStamina();
                            break;
                        case Class.Elementalist:
                            WindowManager.HPBars.HideStamina();
                            break;
                        case Class.Soulless:
                            WindowManager.HPBars.HideStamina();
                            break;
                        case Class.Engineer:
                            WindowManager.HPBars.ShowStamina();
                            break;
                        case Class.Fighter:
                            WindowManager.HPBars.ShowStamina();
                            break;
                        case Class.Assassin:
                            WindowManager.HPBars.ShowStamina();
                            break;
                        case Class.Moon_Dancer:
                            WindowManager.HPBars.ShowStamina();
                            break;
                        case Class.Common:
                            WindowManager.HPBars.HideStamina();
                            break;
                        case Class.None:
                            WindowManager.HPBars.HideStamina();
                            break;
                        default:
                            break;
                    }
                    Console.WriteLine("{0} - {1}", CurrentClass, CurrentCharId);
                    break;
                case ("S_START_COOLTIME_SKILL"):
                    var sStartCooltimeSkill = new S_START_COOLTIME_SKILL(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    SkillManager.AddSkill(new SkillCooldown(sStartCooltimeSkill.SkillId, sStartCooltimeSkill.Cooldown, CooldownType.Skill));
                    switch (CurrentClass)
                    {
                        case Class.Warrior:
                            Warrior.CheckScytheAndGamble(sStartCooltimeSkill);
                            break;
                        default:
                            break;
                    }
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
                    Logged = false;
                    WindowManager.HPBars.Reset();
                    SkillManager.Clear();
                    break;
                case ("S_ABNORMALITY_BEGIN"):
                    var sAbnormalityBegin = new S_ABNORMALITY_BEGIN(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    switch (CurrentClass)
                    {
                        case Class.Warrior:
                            Warrior.CheckGamble(sAbnormalityBegin);
                            break;
                        case Class.Elementalist:
                            if (sAbnormalityBegin.id == SkillManager.HurricaneId && sAbnormalityBegin.casterId == CurrentCharId)
                            {
                                SkillManager.AddSkill(new SkillCooldown(60010, 120000, CooldownType.Skill));
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case ("S_PLAYER_STAT_UPDATE"):
                    var sPlayerStatUpdate = new S_PLAYER_STAT_UPDATE(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    switch (CurrentClass)
                    {
                        case Class.Warrior:
                            EdgeWindow.SetEdge(sPlayerStatUpdate.edge);
                            break;
                        default:
                            break;
                    }
                    MaxHPUpdated?.Invoke(sPlayerStatUpdate.maxHp);
                    MaxMPUpdated?.Invoke(sPlayerStatUpdate.maxMp);
                    MaxSTUpdated?.Invoke(sPlayerStatUpdate.maxRe);
                    HPUpdated?.Invoke(sPlayerStatUpdate.currHp);
                    MPUpdated?.Invoke(sPlayerStatUpdate.currMp);
                    STUpdated?.Invoke(sPlayerStatUpdate.currRe);
                    break;
                case ("S_PLAYER_CHANGE_MP"):
                    var sPlayerChangeMP = new S_PLAYER_CHANGE_MP(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    MPUpdated?.Invoke(sPlayerChangeMP.currentMP);
                    break;
                case ("S_CREATURE_CHANGE_HP"):
                    var sCreatureChangeHP = new S_CREATURE_CHANGE_HP(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    if(sCreatureChangeHP.target == CurrentCharId)
                    {
                        HPUpdated.Invoke(sCreatureChangeHP.currentHP);
                    }
                    break;
                case ("S_PLAYER_CHANGE_STAMINA"):
                    var sPlayerChangeStamina = new S_PLAYER_CHANGE_STAMINA(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                    STUpdated.Invoke(sPlayerChangeStamina.currentStamina);
                    break;
                case ("S_USER_STATUS"):
                    switch (CurrentClass)
                    {
                        case Class.Warrior:
                            var sUserStatus = new S_USER_STATUS(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer));
                            if (sUserStatus.id == CurrentCharId)
                            {
                                if (sUserStatus.isInCombat)
                                {
                                    WindowManager.UndimEdgeGauge();
                                }
                                else
                                {
                                    WindowManager.DimEdgeGauge();
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case ("S_SPAWN_ME"):
                    WindowManager.ShowHPBars();
                    break;
                case ("S_GET_USER_LIST"):
                    CLP.ParseCharacters(PacketData(msg));
                    break;
                default:
                    break;
            }
        }
    }
}
