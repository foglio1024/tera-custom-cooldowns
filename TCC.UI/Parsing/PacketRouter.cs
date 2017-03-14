using DamageMeter.Sniffing;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Messages;
using TCC.Parsing.Messages;
using Tera.Game;

namespace TCC.Parsing
{
    public delegate void ParsedMessageEventHandler(Tera.Game.Messages.ParsedMessage p);
    public delegate void EmptyPacketEventHandler();
    public delegate void UpdateIntStatEventHandler(int statValue);
    public delegate void UpdateFloatStatEventHandler(double statValue);
    public delegate void UpdateStatWithIdEventHandler(ulong id, object statValue);
    public delegate void MessageEventHandler(Tera.Message msg);

    public static class PacketRouter
    {
        public static uint Version;
        public static OpCodeNamer OpCodeNamer;
        public static OpCodeNamer SystemMessageNamer;
        static CharListProcessor CLP = new CharListProcessor();

        //static event ParsedMessageEventHandler CharLogin;
        //static event ParsedMessageEventHandler SkillCooldown;
        //static event ParsedMessageEventHandler ItemCooldown;
        //static event ParsedMessageEventHandler DecreaseSkillCooldown;
        //static event EmptyPacketEventHandler   ReturnToLobby;
        //static event ParsedMessageEventHandler AbnormalityBegin;
        //static event ParsedMessageEventHandler PlayerStatUpdate;
        //static event ParsedMessageEventHandler PlayerMPChanged;
        //static event ParsedMessageEventHandler CreatureHPChanged;
        //static event ParsedMessageEventHandler PlayerStaminaChanged;
        //static event ParsedMessageEventHandler UserStatusChanged;
        //static event ParsedMessageEventHandler FlightEnergyChanged;
        //static event EmptyPacketEventHandler   UserSpawned;
        //static event MessageEventHandler       CharList;
        //static event ParsedMessageEventHandler BossGageReceived;
        //static event ParsedMessageEventHandler NpcStatusChanged;

        public static event UpdateIntStatEventHandler MaxHPUpdated;
        public static event UpdateIntStatEventHandler MaxMPUpdated;
        public static event UpdateIntStatEventHandler MaxSTUpdated;

        public static event UpdateIntStatEventHandler HPUpdated;
        public static event UpdateIntStatEventHandler MPUpdated;
        public static event UpdateIntStatEventHandler STUpdated;

        public static event UpdateFloatStatEventHandler FlightEnergyUpdated;

        public static event UpdateIntStatEventHandler IlvlUpdated;

        public static event UpdateStatWithIdEventHandler BossHPChanged;
        public static event UpdateStatWithIdEventHandler EnragedChanged;


        public static void Init()
        {
            TeraSniffer.Instance.MessageReceived += MessageReceived;

            //CharLogin += OnCharLogin;
            //CharList += OnCharList;
            //SkillCooldown += OnNewSkillCooldown;
            //ItemCooldown += OnNewItemCooldown;
            //DecreaseSkillCooldown += OnDecreaseSkillCooldown;
            //ReturnToLobby += OnReturnToLobby;
            //AbnormalityBegin += OnAbnormalityBegin;
            //PlayerStatUpdate += OnPlayerStatUpdate;
            //PlayerMPChanged += OnPlayerChangeMP;
            //CreatureHPChanged += OnCreatureChangeHP;
            //PlayerStaminaChanged += OnPlayerChangeStamina;
            //UserStatusChanged += OnUserStatusChanged;
            //UserSpawned += OnSpawn;
            //FlightEnergyChanged += OnPlayerChangeFlightEnergy;
            //BossGageReceived += OnGageReceived;
            //NpcStatusChanged += OnNpcStatusChanged;

        }


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

        static void RoutePacket(Tera.Message msg)
        {
            switch (OpCodeNamer.GetName(msg.OpCode))
            {
                case ("S_START_COOLTIME_SKILL"):
                    HandleNewSkillCooldown(new TCC.Messages.S_START_COOLTIME_SKILL(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_DECREASE_COOLTIME_SKILL"):
                    HandleDecreaseSkillCooldown(new S_DECREASE_COOLTIME_SKILL(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_ABNORMALITY_BEGIN"):
                    HandleAbnormalityBegin(new S_ABNORMALITY_BEGIN(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_CREATURE_CHANGE_HP"):
                    HandleCreatureChangeHP(new S_CREATURE_CHANGE_HP(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_PLAYER_CHANGE_MP"):
                    HandlePlayerChangeMP(new S_PLAYER_CHANGE_MP(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_BOSS_GAGE_INFO"):
                    HandleGageReceived(new TCC.Messages.S_BOSS_GAGE_INFO(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_PLAYER_CHANGE_STAMINA"):
                    HandlePlayerChangeStamina(new S_PLAYER_CHANGE_STAMINA(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_PLAYER_CHANGE_FLIGHT_ENERGY"):
                    HandlePlayerChangeFlightEnergy(new S_PLAYER_CHANGE_FLIGHT_ENERGY(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_START_COOLTIME_ITEM"):
                    HandleNewItemCooldown(new S_START_COOLTIME_ITEM(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_PLAYER_STAT_UPDATE"):
                    HandlePlayerStatUpdate(new TCC.Messages.S_PLAYER_STAT_UPDATE(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_USER_STATUS"):
                    HandleUserStatusChanged(new S_USER_STATUS(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_NPC_STATUS"):
                    HandleNpcStatusChanged(new S_NPC_STATUS(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_SPAWN_ME"):
                    HandleSpawn();
                    break;
                case ("S_GET_USER_LIST"):
                    HandleCharList(msg);
                    break;
                case ("S_RETURN_TO_LOBBY"):
                    HandleReturnToLobby();
                    break;
                case ("S_LOGIN"):
                    HandleCharLogin(new S_LOGIN(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                default:
                    break;
            }
        }

        static void HandleCharLogin(S_LOGIN p)
        {
            var sLogin = (S_LOGIN)p;
            SessionManager.Logged = true;
            SessionManager.CurrentClass = sLogin.CharacterClass;
            SessionManager.CurrentCharId = sLogin.entityId;
            SessionManager.CurrentCharName = sLogin.Name;
            SessionManager.CurrentLaurel = CLP.GetLaurelFromName(sLogin.Name);
            SessionManager.CurrentLevel = (int)sLogin.Level;
            WindowManager.SetCharInfo();
            switch (SessionManager.CurrentClass)
            {
                case Class.Warrior:
                    WindowManager.InitClassGauge(Class.Warrior);
                    WindowManager.CharacterWindow.ShowResolve();
                    break;
                case Class.Lancer:
                    WindowManager.CharacterWindow.ShowResolve();
                    break;
                case Class.Engineer:
                    WindowManager.InitClassGauge(Class.Engineer);
                    WindowManager.CharacterWindow.HideResolve();
                    break;
                case Class.Fighter:
                    WindowManager.InitClassGauge(Class.Fighter);
                    WindowManager.CharacterWindow.HideResolve();
                    break;
                case Class.Assassin:
                    WindowManager.CharacterWindow.HideResolve();
                    WindowManager.InitClassGauge(Class.Assassin);
                    break;
                case Class.Moon_Dancer:
                    WindowManager.InitClassGauge(Class.Moon_Dancer);
                    WindowManager.CharacterWindow.HideResolve();
                    break;
                default:
                    WindowManager.CharacterWindow.HideResolve();
                    break;
            }
        }
        static void HandleNewSkillCooldown(S_START_COOLTIME_SKILL p)
        {
            var sStartCooltimeSkill = (TCC.Messages.S_START_COOLTIME_SKILL)p;

            SkillManager.AddSkill(sStartCooltimeSkill);
            switch (SessionManager.CurrentClass)
            {
                case Class.Warrior:
                    Warrior.CheckWarriorsSkillCooldown(sStartCooltimeSkill);
                    break;
                default:
                    break;
            }
        }
        static void HandleNewItemCooldown(S_START_COOLTIME_ITEM p)
        {
            var sStartCooltimeItem = (S_START_COOLTIME_ITEM)p;
            SkillManager.AddBrooch(sStartCooltimeItem);
        }
        static void HandleDecreaseSkillCooldown(S_DECREASE_COOLTIME_SKILL p)
        {
            var sDecreaseCooltimeSkill = (S_DECREASE_COOLTIME_SKILL)p;
            SkillManager.ChangeSkillCooldown(sDecreaseCooltimeSkill);
        }
        static void HandleReturnToLobby()
        {
            SessionManager.Logged = false;
            WindowManager.CharacterWindow.Reset();
            SkillManager.Clear();
        }
        static void HandleAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            var sAbnormalityBegin = (S_ABNORMALITY_BEGIN)p;
            switch (SessionManager.CurrentClass)
            {
                case Class.Warrior:
                    Warrior.CheckGambleBuff(sAbnormalityBegin);
                    break;
                case Class.Elementalist:
                    Mystic.CheckHurricane(sAbnormalityBegin);
                    break;
                default:
                    break;
            }
        }
        static void HandlePlayerStatUpdate(S_PLAYER_STAT_UPDATE p)
        {
            var sPlayerStatUpdate = (TCC.Messages.S_PLAYER_STAT_UPDATE)p;

            switch (SessionManager.CurrentClass)
            {
                case Class.Warrior:
                    EdgeGaugeWindow.SetEdge(sPlayerStatUpdate.edge);
                    MaxSTUpdated?.Invoke(sPlayerStatUpdate.maxRe + sPlayerStatUpdate.bonusRe);
                    STUpdated?.Invoke(sPlayerStatUpdate.currRe);
                    break;
                case Class.Lancer:
                    MaxSTUpdated?.Invoke(sPlayerStatUpdate.maxRe + sPlayerStatUpdate.bonusRe);
                    STUpdated?.Invoke(sPlayerStatUpdate.currRe);
                    break;
                case Class.Engineer:
                    MaxSTUpdated?.Invoke(sPlayerStatUpdate.maxRe + sPlayerStatUpdate.bonusRe);
                    STUpdated?.Invoke(sPlayerStatUpdate.currRe);
                    break;
                case Class.Fighter:
                    MaxSTUpdated?.Invoke(sPlayerStatUpdate.maxRe + sPlayerStatUpdate.bonusRe);
                    STUpdated?.Invoke(sPlayerStatUpdate.currRe);
                    break;
                case Class.Assassin:
                    MaxSTUpdated?.Invoke(sPlayerStatUpdate.maxRe + sPlayerStatUpdate.bonusRe);
                    STUpdated?.Invoke(sPlayerStatUpdate.currRe);
                    break;
                case Class.Moon_Dancer:
                    MaxSTUpdated?.Invoke(sPlayerStatUpdate.maxRe + sPlayerStatUpdate.bonusRe);
                    STUpdated?.Invoke(sPlayerStatUpdate.currRe);
                    break;
                default:
                    break;
            }
            MaxHPUpdated?.Invoke(sPlayerStatUpdate.maxHp);
            MaxMPUpdated?.Invoke(sPlayerStatUpdate.maxMp);
            HPUpdated?.Invoke(sPlayerStatUpdate.currHp);
            MPUpdated?.Invoke(sPlayerStatUpdate.currMp);
            IlvlUpdated?.Invoke(sPlayerStatUpdate.ilvl);

        }
        static void HandlePlayerChangeMP(S_PLAYER_CHANGE_MP p)
        {
            var sPlayerChangeMP = (S_PLAYER_CHANGE_MP)p;
            MPUpdated?.Invoke(sPlayerChangeMP.currentMP);
        }
        static void HandleCreatureChangeHP(S_CREATURE_CHANGE_HP p)
        {
            var sCreatureChangeHP = (S_CREATURE_CHANGE_HP)p;

            if (sCreatureChangeHP.target == SessionManager.CurrentCharId)
            {
                HPUpdated.Invoke(sCreatureChangeHP.currentHP);
            }

        }
        static void HandlePlayerChangeStamina(S_PLAYER_CHANGE_STAMINA p)
        {
            var sPlayerChangeStamina = (S_PLAYER_CHANGE_STAMINA)p;
            switch (SessionManager.CurrentClass)
            {
                case Class.Warrior:
                    STUpdated.Invoke(sPlayerChangeStamina.currentStamina);
                    break;
                case Class.Lancer:
                    STUpdated.Invoke(sPlayerChangeStamina.currentStamina);
                    break;
                case Class.Engineer:
                    STUpdated.Invoke(sPlayerChangeStamina.currentStamina);
                    break;
                case Class.Fighter:
                    STUpdated.Invoke(sPlayerChangeStamina.currentStamina);
                    break;
                case Class.Assassin:
                    STUpdated.Invoke(sPlayerChangeStamina.currentStamina);
                    break;
                case Class.Moon_Dancer:
                    STUpdated.Invoke(sPlayerChangeStamina.currentStamina);
                    break;
                default:
                    break;
            }

        }
        static void HandleUserStatusChanged(S_USER_STATUS p)
        {
            var sUserStatus = (S_USER_STATUS)p;
            if (sUserStatus.id == SessionManager.CurrentCharId)
            {
                if (sUserStatus.isInCombat)
                {
                    //WindowManager.ShowWindow(WindowManager.ClassSpecificGauge);
                    SessionManager.Combat = true;
                }
                else
                {
                    //WindowManager.HideWindow(WindowManager.ClassSpecificGauge);
                    SessionManager.Combat = false;
                }
            }

        }
        static void HandleSpawn()
        {
            WindowManager.ShowWindow(WindowManager.CharacterWindow);
        }
        static void HandleCharList(Tera.Message msg)
        {
            CLP.ParseCharacters(PacketData(msg));

        }
        static void HandlePlayerChangeFlightEnergy(S_PLAYER_CHANGE_FLIGHT_ENERGY p)
        {
            var sPlayerChangeFlightEnergy = (S_PLAYER_CHANGE_FLIGHT_ENERGY)p;
            FlightEnergyUpdated?.Invoke(sPlayerChangeFlightEnergy.energy);
        }
        static void HandleGageReceived(S_BOSS_GAGE_INFO p)
        {
            var sBossGageInfo = (TCC.Messages.S_BOSS_GAGE_INFO)p;
            if(SessionManager.CurrentBosses.Where(x => x.EntityId == sBossGageInfo.EntityId).Count() > 0)
            {
                if (SessionManager.CurrentBosses.Where(x => x.EntityId == sBossGageInfo.EntityId).Single().CurrentHP != sBossGageInfo.CurrentHP)
                {
                    if(sBossGageInfo.CurrentHP == 0)
                    {
                        App.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {

                            SessionManager.CurrentBosses.Remove(SessionManager.CurrentBosses.Where(x => x.EntityId == sBossGageInfo.EntityId).Single());
                        }));
                        return;
                    }
                    SessionManager.CurrentBosses.Where(x => x.EntityId == sBossGageInfo.EntityId).Single().CurrentHP = sBossGageInfo.CurrentHP;
                    BossHPChanged?.Invoke(sBossGageInfo.EntityId, sBossGageInfo.CurrentHP);
                }
            }
            else
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    SessionManager.CurrentBosses.Add(new Boss(sBossGageInfo.EntityId, sBossGageInfo.Type, sBossGageInfo.Npc, sBossGageInfo.CurrentHP, sBossGageInfo.MaxHP));
                }));
            }

            //CHandlesole.WriteLine("type:{0} - npc:{1}", sBossGageInfo.Type, sBossGageInfo.Npc);
            //if first time => retreive name and show window
            //event => update hp
        }
        static void HandleNpcStatusChanged(S_NPC_STATUS p)
        {
            var sNpcStatus = (S_NPC_STATUS)p;
            if (SessionManager.CurrentBosses.Where(x => x.EntityId == sNpcStatus.EntityId).Count() > 0)
            {
                if (SessionManager.CurrentBosses.Where(x => x.EntityId == sNpcStatus.EntityId).Single().Enraged != sNpcStatus.IsEnraged)
                {
                    SessionManager.CurrentBosses.Where(x => x.EntityId == sNpcStatus.EntityId).Single().Enraged = sNpcStatus.IsEnraged;
                    EnragedChanged?.Invoke(sNpcStatus.EntityId, sNpcStatus.IsEnraged);
                }
            }


            //if (sNpcStatus.EntityId != SessiHandleManager.CurrentBossEntityId) return;
            //WindowManager.BossGauge.SetEnraged(sNpcStatus.IsEnraged);
        }

    }
}

