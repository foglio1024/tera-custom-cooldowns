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
using Tera.Game.Messages;

namespace TCC.Parsing
{
    public delegate void ParsedMessageEventHandler(ParsedMessage p);
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

        static event ParsedMessageEventHandler CharLogin;
        static event ParsedMessageEventHandler SkillCooldown;
        static event ParsedMessageEventHandler ItemCooldown;
        static event ParsedMessageEventHandler DecreaseSkillCooldown;
        static event EmptyPacketEventHandler   ReturnToLobby;
        static event ParsedMessageEventHandler AbnormalityBegin;
        static event ParsedMessageEventHandler PlayerStatUpdate;
        static event ParsedMessageEventHandler PlayerMPChanged;
        static event ParsedMessageEventHandler CreatureHPChanged;
        static event ParsedMessageEventHandler PlayerStaminaChanged;
        static event ParsedMessageEventHandler UserStatusChanged;
        static event ParsedMessageEventHandler FlightEnergyChanged;
        static event EmptyPacketEventHandler   UserSpawned;
        static event MessageEventHandler       CharList;
        static event ParsedMessageEventHandler BossGageReceived;
        static event ParsedMessageEventHandler NpcStatusChanged;

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

            CharLogin += OnCharLogin;
            SkillCooldown += OnNewSkillCooldown;
            ItemCooldown += OnNewItemCooldown;
            DecreaseSkillCooldown += OnDecreaseSkillCooldown;
            ReturnToLobby += OnReturnToLobby;
            AbnormalityBegin += OnAbnormalityBegin;
            PlayerStatUpdate += OnPlayerStatUpdate;
            PlayerMPChanged += OnPlayerChangeMP;
            CreatureHPChanged += OnCreatureChangeHP;
            PlayerStaminaChanged += OnPlayerChangeStamina;
            UserStatusChanged += OnUserStatusChanged;
            UserSpawned += OnSpawn;
            CharList += OnCharList;
            FlightEnergyChanged += OnPlayerChangeFlightEnergy;
            BossGageReceived += OnGageReceived;
            NpcStatusChanged += OnNpcStatusChanged;

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
                    SkillCooldown?.Invoke(new TCC.Messages.S_START_COOLTIME_SKILL(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_DECREASE_COOLTIME_SKILL"):
                    DecreaseSkillCooldown?.Invoke(new S_DECREASE_COOLTIME_SKILL(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_ABNORMALITY_BEGIN"):
                    AbnormalityBegin?.Invoke(new S_ABNORMALITY_BEGIN(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_CREATURE_CHANGE_HP"):
                    CreatureHPChanged?.Invoke(new S_CREATURE_CHANGE_HP(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_PLAYER_CHANGE_MP"):
                    PlayerMPChanged?.Invoke(new S_PLAYER_CHANGE_MP(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_BOSS_GAGE_INFO"):
                    BossGageReceived?.Invoke(new TCC.Messages.S_BOSS_GAGE_INFO(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_PLAYER_CHANGE_STAMINA"):
                    PlayerStaminaChanged?.Invoke(new S_PLAYER_CHANGE_STAMINA(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_PLAYER_CHANGE_FLIGHT_ENERGY"):
                    FlightEnergyChanged?.Invoke(new S_PLAYER_CHANGE_FLIGHT_ENERGY(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_START_COOLTIME_ITEM"):
                    ItemCooldown?.Invoke(new S_START_COOLTIME_ITEM(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_PLAYER_STAT_UPDATE"):
                    PlayerStatUpdate?.Invoke(new TCC.Messages.S_PLAYER_STAT_UPDATE(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_USER_STATUS"):
                    UserStatusChanged?.Invoke(new S_USER_STATUS(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_NPC_STATUS"):
                    NpcStatusChanged?.Invoke(new S_NPC_STATUS(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_SPAWN_ME"):
                    UserSpawned?.Invoke();
                    break;
                case ("S_GET_USER_LIST"):
                    CharList?.Invoke(msg);
                    break;
                case ("S_RETURN_TO_LOBBY"):
                    ReturnToLobby?.Invoke();
                    break;
                case ("S_LOGIN"):
                    CharLogin?.Invoke(new S_LOGIN(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                default:
                    break;
            }
        }

        static void OnCharLogin(ParsedMessage p)
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
                    WindowManager.ShowWindow(WindowManager.ClassSpecificGauge);
                    WindowManager.CharacterWindow.ShowResolve();
                    break;
                case Class.Lancer:
                    WindowManager.CharacterWindow.ShowResolve();
                    break;
                case Class.Engineer:
                    WindowManager.InitClassGauge(Class.Engineer);
                    WindowManager.ShowWindow(WindowManager.ClassSpecificGauge);
                    WindowManager.CharacterWindow.HideResolve();
                    break;
                case Class.Fighter:
                    WindowManager.InitClassGauge(Class.Fighter);
                    WindowManager.ShowWindow(WindowManager.ClassSpecificGauge);
                    WindowManager.CharacterWindow.HideResolve();
                    break;
                case Class.Assassin:
                    WindowManager.CharacterWindow.HideResolve();
                    WindowManager.InitClassGauge(Class.Assassin);
                    WindowManager.ShowWindow(WindowManager.ClassSpecificGauge);
                    break;
                case Class.Moon_Dancer:
                    WindowManager.InitClassGauge(Class.Moon_Dancer);
                    WindowManager.ShowWindow(WindowManager.ClassSpecificGauge);
                    WindowManager.CharacterWindow.HideResolve();
                    break;
                default:
                    WindowManager.CharacterWindow.HideResolve();
                    break;
            }
        }
        static void OnNewSkillCooldown(ParsedMessage p)
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
        static void OnNewItemCooldown(ParsedMessage p)
        {
            var sStartCooltimeItem = (S_START_COOLTIME_ITEM)p;
            SkillManager.AddBrooch(sStartCooltimeItem);
        }
        static void OnDecreaseSkillCooldown(ParsedMessage p)
        {
            var sDecreaseCooltimeSkill = (S_DECREASE_COOLTIME_SKILL)p;
            SkillManager.ChangeSkillCooldown(sDecreaseCooltimeSkill);
        }
        static void OnReturnToLobby()
        {
            SessionManager.Logged = false;
            WindowManager.CharacterWindow.Reset();
            SkillManager.Clear();
        }
        static void OnAbnormalityBegin(ParsedMessage p)
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
        static void OnPlayerStatUpdate(ParsedMessage p)
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
        static void OnPlayerChangeMP(ParsedMessage p)
        {
            var sPlayerChangeMP = (S_PLAYER_CHANGE_MP)p;
            MPUpdated?.Invoke(sPlayerChangeMP.currentMP);
        }
        static void OnCreatureChangeHP(ParsedMessage p)
        {
            var sCreatureChangeHP = (S_CREATURE_CHANGE_HP)p;

            if (sCreatureChangeHP.target == SessionManager.CurrentCharId)
            {
                HPUpdated.Invoke(sCreatureChangeHP.currentHP);
            }

        }
        static void OnPlayerChangeStamina(ParsedMessage p)
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
        static void OnUserStatusChanged(ParsedMessage p)
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
        static void OnSpawn()
        {
            WindowManager.ShowWindow(WindowManager.CharacterWindow);
        }
        static void OnCharList(Tera.Message msg)
        {
            CLP.ParseCharacters(PacketData(msg));

        }
        static void OnPlayerChangeFlightEnergy(ParsedMessage p)
        {
            var sPlayerChangeFlightEnergy = (S_PLAYER_CHANGE_FLIGHT_ENERGY)p;
            FlightEnergyUpdated?.Invoke(sPlayerChangeFlightEnergy.energy);
        }
        static void OnGageReceived(ParsedMessage p)
        {
            var sBossGageInfo = (TCC.Messages.S_BOSS_GAGE_INFO)p;
            if(SessionManager.CurrentBosses.Where(x => x.EntityId == sBossGageInfo.EntityId).Count() > 0)
            {
                if (SessionManager.CurrentBosses.Where(x => x.EntityId == sBossGageInfo.EntityId).Single().CurrentHP != sBossGageInfo.CurrentHP)
                {
                    if(sBossGageInfo.CurrentHP == 0)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {

                            SessionManager.CurrentBosses.Remove(SessionManager.CurrentBosses.Where(x => x.EntityId == sBossGageInfo.EntityId).Single());
                        });
                        return;
                    }
                    SessionManager.CurrentBosses.Where(x => x.EntityId == sBossGageInfo.EntityId).Single().CurrentHP = sBossGageInfo.CurrentHP;
                    BossHPChanged?.Invoke(sBossGageInfo.EntityId, sBossGageInfo.CurrentHP);
                }
            }
            else
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    SessionManager.CurrentBosses.Add(new Boss(sBossGageInfo.EntityId, sBossGageInfo.Type, sBossGageInfo.Npc, sBossGageInfo.CurrentHP, sBossGageInfo.MaxHP));
                });
            }

            //Console.WriteLine("type:{0} - npc:{1}", sBossGageInfo.Type, sBossGageInfo.Npc);
            //if first time => retreive name and show window
            //event => update hp
        }
        static void OnNpcStatusChanged(ParsedMessage p)
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


            //if (sNpcStatus.EntityId != SessionManager.CurrentBossEntityId) return;
            //WindowManager.BossGauge.SetEnraged(sNpcStatus.IsEnraged);
        }

    }
}

