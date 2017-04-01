using DamageMeter.Sniffing;
using Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    public delegate void UpdateBuffEventHandler(Boss b, Abnormality ab, int duration, int stacks);

    public static class PacketRouter
    {
        public static uint Version;
        public static OpCodeNamer OpCodeNamer;
        public static OpCodeNamer SystemMessageNamer;
        static CharListProcessor CLP = new CharListProcessor();
        static ConcurrentQueue<Tera.Message> Packets = new ConcurrentQueue<Tera.Message>();
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

        public static event UpdateBuffEventHandler BossBuffUpdated;

        public static void Init()
        {
            TeraSniffer.Instance.MessageReceived += MessageReceived;
            var analysisThread = new Thread(PacketAnalysisLoop);
            analysisThread.Start();
        }

        static void PacketAnalysisLoop()
        {
            while (true)
            {
                Tera.Message msg;
                var successDequeue = Packets.TryDequeue(out msg);
                if (!successDequeue)
                {
                    Thread.Sleep(1);
                    continue;
                }
                var message = MessageFactory.Create(msg);
                if (message.GetType() == typeof(Tera.Game.Messages.UnknownMessage)) continue;

                if (!MessageFactory.Process(message))
                {
                }

            }
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
                MessageFactory.Init();

            }
            //RoutePacket(obj);
            Packets.Enqueue(obj);
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
                    HandleNewSkillCooldown(new S_START_COOLTIME_SKILL(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
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
                    HandleGageReceived(new S_BOSS_GAGE_INFO(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
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
                    HandlePlayerStatUpdate(new S_PLAYER_STAT_UPDATE(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_USER_STATUS"):
                    HandleUserStatusChanged(new S_USER_STATUS(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_NPC_STATUS"):
                    HandleNpcStatusChanged(new S_NPC_STATUS(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_DESPAWN_NPC"):
                    HandleNpcDespawn(new S_DESPAWN_NPC(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_SPAWN_NPC"):
                    HandleNpcSpawn(new S_SPAWN_NPC(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_ABNORMALITY_REFRESH"):
                    HandleAbnormalityRefresh(new S_ABNORMALITY_REFRESH(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_ABNORMALITY_END"):
                    HandleAbnormalityEnd(new S_ABNORMALITY_END(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                case ("S_SPAWN_ME"):
                    //HandleSpawn();
                    break;
                case ("S_GET_USER_LIST"):
                    HandleCharList(msg);
                    break;
                case ("S_RETURN_TO_LOBBY"):
                    //HandleReturnToLobby();
                    break;
                case ("S_LOGIN"):
                    HandleCharLogin(new S_LOGIN(new TeraMessageReader(msg, OpCodeNamer, Version, SystemMessageNamer)));
                    break;
                default:
                    break;
            }
        }
        class EventQuestion : Tera.Game.Messages.ParsedMessage
        {
            public EventQuestion(TeraMessageReader reader) : base(reader)
            {
                reader.Skip(10);
                Console.WriteLine(reader.ReadTeraString());
            }
        }
        public static void HandleCharLogin(S_LOGIN p)
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
        public static void HandleNewSkillCooldown(S_START_COOLTIME_SKILL p)
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
        public static void HandleNewItemCooldown(S_START_COOLTIME_ITEM p)
        {
            SkillManager.AddBrooch(p);
        }
        public static void HandleDecreaseSkillCooldown(S_DECREASE_COOLTIME_SKILL p)
        {
            SkillManager.ChangeSkillCooldown(p);
        }
        public static void HandleReturnToLobby(S_RETURN_TO_LOBBY p)
        {
            SessionManager.Logged = false;
            WindowManager.CharacterWindow.Reset();
            SkillManager.Clear();
        }
        public static void HandleAbnormalityBegin(S_ABNORMALITY_BEGIN p)
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
            if(SessionManager.TryGetBossById(p.targetId, out Boss b))
            {
                if(AbnormalityDatabase.Abnormalities.TryGetValue(p.id, out Abnormality ab))
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        if (ab.Name.Contains("(Hidden)") || ab.Name.Equals("Unknown") || ab.Name.Equals(string.Empty))
                        {
                            return;
                        }
                        if (b.HasBuff(ab))
                        {
                            BossBuffUpdated?.Invoke(b, ab, p.duration, p.stacks);
                        }
                        else
                        {
                            if(p.duration < 200000000)
                            {
                                SessionManager.CurrentBosses.Where(x => x.EntityId == p.targetId).First().Buffs.Add(new BuffDuration(ab, p.duration, p.stacks, p.targetId));
                            }
                            else
                            {
                                SessionManager.CurrentBosses.Where(x => x.EntityId == p.targetId).First().Buffs.Insert(0,new BuffDuration(ab, p.duration, p.stacks, p.targetId));
                            }

                        }
                    });
                }
            }
        }
        public static void HandleAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            if (AbnormalityDatabase.Abnormalities.TryGetValue(p.AbnormalityId, out Abnormality ab))
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (SessionManager.TryGetBossById(p.TargetId, out Boss b))
                    {
                        if (b.HasBuff(ab))
                        {
                            BossBuffUpdated?.Invoke(b, ab, p.Duration, p.Stacks);

                            //SessionManager.CurrentBosses.Where(x => x.EntityId == p.TargetId).First().Buffs.Where(x => x.Buff.Id == p.AbnormalityId).FirstOrDefault().Duration = p.Duration;
                            b.Buffs.Where(x => x.Buff.Id == p.AbnormalityId).FirstOrDefault().Stacks = p.Stacks;
                        }
                        else
                        {
                            if (ab.Name.Contains("(Hidden)") || ab.Name.Equals("Unknown") || ab.Name.Equals(string.Empty))
                            {
                                //Console.WriteLine("Skipping refresh: {0}", ab.Name);
                                return;
                            }
                            if (p.Duration < 2000000000)
                            {
                                b.Buffs.Add(new BuffDuration(ab, p.Duration, p.Stacks, p.TargetId));
                            }
                            else
                            {
                                b.Buffs.Insert(0,new BuffDuration(ab, p.Duration, p.Stacks, p.TargetId));
                            }

                        }
                    }
                });
            }

        }
        public static void HandleAbnormalityEnd(S_ABNORMALITY_END p)
        {
            if (AbnormalityDatabase.Abnormalities.TryGetValue(p.id, out Abnormality ab))
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (SessionManager.TryGetBossById(p.target, out Boss b) && b.HasBuff(ab))
                    {
                        b.EndBuff(ab);
                    }
                });
            }

        }
        public static void HandlePlayerStatUpdate(S_PLAYER_STAT_UPDATE p)
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
        public static void HandlePlayerChangeMP(S_PLAYER_CHANGE_MP p)
        {
            if (p.target != SessionManager.CurrentCharId) return;
            MPUpdated?.Invoke(p.currentMP);
            
        }
        public static void HandleCreatureChangeHP(S_CREATURE_CHANGE_HP p)
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

            if (p.target != SessionManager.CurrentCharId) return;
            
                HPUpdated.Invoke(p.currentHP);
            

        }
        public static void HandlePlayerChangeStamina(S_PLAYER_CHANGE_STAMINA p)
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
        public static void HandleUserStatusChanged(S_USER_STATUS p)
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
        public static void HandleSpawn(S_SPAWN_ME p)
        {
            WindowManager.ShowWindow(WindowManager.CharacterWindow);
            Console.WriteLine("[S_SPAWN_ME]");
            App.Current.Dispatcher.Invoke(() =>
            {

                SessionManager.CurrentBosses.Clear();
            });
        }
        public static void HandleCharList(Tera.Message msg)
        {
            CLP.ParseCharacters(PacketData(msg));

        }
        public static void HandlePlayerChangeFlightEnergy(S_PLAYER_CHANGE_FLIGHT_ENERGY p)
        {
            FlightEnergyUpdated?.Invoke(p.energy);
        }
        public static void HandleGageReceived(S_BOSS_GAGE_INFO p)
        {
            if(SessionManager.TryGetBossById(p.EntityId, out Boss b))
            {
                b.MaxHP = p.MaxHP;
                b.Visible = System.Windows.Visibility.Visible;

                if (b.CurrentHP != p.CurrentHP)
                {
                    b.CurrentHP = p.CurrentHP;
                    BossHPChanged?.Invoke(p.EntityId, p.CurrentHP);
                }
            }
            else
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    SessionManager.CurrentBosses.Add(new Boss(p.EntityId, (uint)p.Type, (uint)p.Npc, p.CurrentHP, p.MaxHP, System.Windows.Visibility.Visible));

                });
            }

            //Console.WriteLine("GAGE: {0} - {1}", MonsterDatabase.GetName((uint)p.Npc, (uint)p.Type), p.MaxHP);

        }
        public static void HandleNpcStatusChanged(S_NPC_STATUS p)
        {
            if (SessionManager.TryGetBossById(p.EntityId, out Boss b))
            {
                if (b.Enraged != p.IsEnraged)
                {
                    b.Enraged = p.IsEnraged;
                    EnragedChanged?.Invoke(p.EntityId, p.IsEnraged);
                }
            }
        }
        public static void HandleNpcDespawn(S_DESPAWN_NPC p)
        {
            if(SessionManager.TryGetBossById(p.target, out Boss b))
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    SessionManager.CurrentBosses.Remove(b);
                });
            }
        }
        public static void HandleNpcSpawn(S_SPAWN_NPC p)
        {
            if (MonsterDatabase.TryGetMonster(p.Npc, (uint)p.Type, out Monster m))
            {
                if (m.IsBoss || m.MaxHP >= 40000000)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        SessionManager.CurrentBosses.Add(new Boss(p.EntityId, (uint)p.Type, p.Npc, System.Windows.Visibility.Collapsed));
                    //Console.WriteLine("SPAWNED: {0} - {1}", m.Name, m.MaxHP);
                });
                }
            }
        }
    }
}

