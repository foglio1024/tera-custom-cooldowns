using DamageMeter.Sniffing;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Data;
using TCC.Messages;
using TCC.Parsing.Messages;
using Tera.Game;

namespace TCC.Parsing
{
    public delegate void ParsedMessageEventHandler(Tera.Game.Messages.ParsedMessage p);
    public delegate void EmptyPacketEventHandler();
    public delegate void UpdateIntStatEventHandler(int statValue);
    public delegate void UpdateFloatStatEventHandler(float statValue);
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

        public static event UpdateFloatStatEventHandler HPUpdated;
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
                case ("S_DESPAWN_NPC"):
                    HandlerNpcDespawn(new S_DESPAWN_NPC(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
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
            SessionManager.Logged = true;
            SessionManager.CurrentClass = p.CharacterClass;
            SessionManager.CurrentCharId = p.entityId;
            SessionManager.CurrentCharName = p.Name;
            SessionManager.CurrentLaurel = CLP.GetLaurelFromName(p.Name);
            SessionManager.CurrentLevel = (int)p.Level;
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
            SkillManager.AddSkill(p);
            switch (SessionManager.CurrentClass)
            {
                case Class.Warrior:
                    Warrior.CheckWarriorsSkillCooldown(p);
                    break;
                default:
                    break;
            }
        }
        static void HandleNewItemCooldown(S_START_COOLTIME_ITEM p)
        {
            SkillManager.AddBrooch(p);
        }
        static void HandleDecreaseSkillCooldown(S_DECREASE_COOLTIME_SKILL p)
        {
            SkillManager.ChangeSkillCooldown(p);
        }
        static void HandleReturnToLobby()
        {
            SessionManager.Logged = false;
            WindowManager.CharacterWindow.Reset();
            SkillManager.Clear();
        }
        static void HandleAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            switch (SessionManager.CurrentClass)
            {
                case Class.Warrior:
                    Warrior.CheckGambleBuff(p);
                    break;
                case Class.Elementalist:
                    Mystic.CheckHurricane(p);
                    break;
                default:
                    break;
            }
            if(SessionManager.CurrentBosses.Where(x => x.EntityId == p.targetId).Count() > 0)
            {
                if(AbnormalityDatabase.TryGetAbnormality(p.id, out Abnormality ab))
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        if (SessionManager.CurrentBosses.Where(x => x.EntityId == p.targetId).First().Buffs.Where(x => x.Buff.Id == p.id).Count() > 0)
                        {
                            Console.WriteLine("{0} already present, skipping", ab.Name);
                            return;
                        }
                        else
                        {
                            SessionManager.CurrentBosses.Where(x => x.EntityId == p.targetId).First().Buffs.Add(new BuffDuration(ab, p.duration, p.stacks));
                            Console.WriteLine("Added {0} to {1}", ab.Name, SessionManager.CurrentBosses.Where(x => x.EntityId == p.targetId).First().Name);
                        }
                    });
                }
            }
        }



        static void HandlePlayerStatUpdate(S_PLAYER_STAT_UPDATE p)
        {
            switch (SessionManager.CurrentClass)
            {
                case Class.Warrior:
                    EdgeGaugeWindow.SetEdge(p.edge);
                    MaxSTUpdated?.Invoke(p.maxRe + p.bonusRe);
                    STUpdated?.Invoke(p.currRe);
                    break;
                case Class.Lancer:
                    MaxSTUpdated?.Invoke(p.maxRe + p.bonusRe);
                    STUpdated?.Invoke(p.currRe);
                    break;
                case Class.Engineer:
                    MaxSTUpdated?.Invoke(p.maxRe + p.bonusRe);
                    STUpdated?.Invoke(p.currRe);
                    break;
                case Class.Fighter:
                    MaxSTUpdated?.Invoke(p.maxRe + p.bonusRe);
                    STUpdated?.Invoke(p.currRe);
                    break;
                case Class.Assassin:
                    MaxSTUpdated?.Invoke(p.maxRe + p.bonusRe);
                    STUpdated?.Invoke(p.currRe);
                    break;
                case Class.Moon_Dancer:
                    MaxSTUpdated?.Invoke(p.maxRe + p.bonusRe);
                    STUpdated?.Invoke(p.currRe);
                    break;
                default:
                    break;
            }
            MaxHPUpdated?.Invoke(p.maxHp);
            MaxMPUpdated?.Invoke(p.maxMp);
            HPUpdated?.Invoke(p.currHp);
            MPUpdated?.Invoke(p.currMp);
            IlvlUpdated?.Invoke(p.ilvl);

        }
        static void HandlePlayerChangeMP(S_PLAYER_CHANGE_MP p)
        {
            MPUpdated?.Invoke(p.currentMP);
            
        }
        static void HandleCreatureChangeHP(S_CREATURE_CHANGE_HP p)
        {
            //if (SessionManager.CurrentBosses.Where(x => x.EntityId == p.target).Count() > 0)
            //{
            //    if (SessionManager.CurrentBosses.Where(x => x.EntityId == p.target).Single().CurrentHP != p.currentHP)
            //    {
            //        if (p.currentHP == 0)
            //        {
            //            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            //            {
            //                SessionManager.CurrentBosses.Remove(SessionManager.CurrentBosses.Where(x => x.EntityId == p.target).Single());
            //            }));
            //            return;
            //        }
            //        SessionManager.CurrentBosses.Where(x => x.EntityId == p.target).Single().CurrentHP = p.currentHP;
            //        BossHPChanged?.Invoke(p.target, p.currentHP);
            //    }
            //}

            if (p.target == SessionManager.CurrentCharId)
            {
                HPUpdated.Invoke(p.currentHP);
            }

        }
        static void HandlePlayerChangeStamina(S_PLAYER_CHANGE_STAMINA p)
        {
            switch (SessionManager.CurrentClass)
            {
                case Class.Warrior:
                    STUpdated.Invoke(p.currentStamina);
                    break;
                case Class.Lancer:
                    STUpdated.Invoke(p.currentStamina);
                    break;
                case Class.Engineer:
                    STUpdated.Invoke(p.currentStamina);
                    break;
                case Class.Fighter:
                    STUpdated.Invoke(p.currentStamina);
                    break;
                case Class.Assassin:
                    STUpdated.Invoke(p.currentStamina);
                    break;
                case Class.Moon_Dancer:
                    STUpdated.Invoke(p.currentStamina);
                    break;
                default:
                    break;
            }

        }
        static void HandleUserStatusChanged(S_USER_STATUS p)
        {
            if (p.id == SessionManager.CurrentCharId)
            {
                if (p.isInCombat)
                {
                    SessionManager.Combat = true;
                }
                else
                {
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
            FlightEnergyUpdated?.Invoke(p.energy);
        }
        static void HandleGageReceived(S_BOSS_GAGE_INFO p)
        {
            if(SessionManager.CurrentBosses.Where(x => x.EntityId == p.EntityId).Count() > 0)
            {
                //return;
                if (SessionManager.CurrentBosses.Where(x => x.EntityId == p.EntityId).Single().CurrentHP != p.CurrentHP)
                {
                    //if (p.CurrentHP == 0)
                    //{
                    //    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    //    {
                    //        SessionManager.CurrentBosses.Remove(SessionManager.CurrentBosses.Where(x => x.EntityId == p.EntityId).Single());
                    //    }));
                    //    return;
                    //}
                    SessionManager.CurrentBosses.Where(x => x.EntityId == p.EntityId).Single().CurrentHP = p.CurrentHP;
                    BossHPChanged?.Invoke(p.EntityId, p.CurrentHP);
                }
            }
            else
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    SessionManager.CurrentBosses.Add(new Boss(p.EntityId, p.Type, p.Npc, p.CurrentHP, p.MaxHP));
                }));
            }
        }
        static void HandleNpcStatusChanged(S_NPC_STATUS p)
        {
            if (SessionManager.CurrentBosses.Where(x => x.EntityId == p.EntityId).Count() > 0)
            {
                if (SessionManager.CurrentBosses.Where(x => x.EntityId == p.EntityId).Single().Enraged != p.IsEnraged)
                {
                    SessionManager.CurrentBosses.Where(x => x.EntityId == p.EntityId).Single().Enraged = p.IsEnraged;
                    EnragedChanged?.Invoke(p.EntityId, p.IsEnraged);
                }
            }
        }
        static void HandlerNpcDespawn(S_DESPAWN_NPC p)
        {
            if(SessionManager.CurrentBosses.Where(x => x.EntityId == p.target).Count() > 0)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    SessionManager.CurrentBosses.Remove(SessionManager.CurrentBosses.Where(x => x.EntityId == p.target).Single());
                });
            }
        }
    }
}

