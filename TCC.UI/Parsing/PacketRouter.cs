using DamageMeter.Sniffing;
using Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using TCC.Data;
using TCC.Messages;
using TCC.Parsing.Messages;
using Tera.Game;
namespace TCC
{
    public delegate void UpdateStatWithIdEventHandler(ulong id, object statValue);

}
namespace TCC.Parsing
{
    public delegate void ParsedMessageEventHandler(Tera.Game.Messages.ParsedMessage p);
    public delegate void EmptyPacketEventHandler();
    public delegate void UpdateIntStatEventHandler(int statValue);
    public delegate void UpdateFloatStatEventHandler(float statValue);
    public delegate void MessageEventHandler(Tera.Message msg);
    public delegate void UpdateBuffEventHandler(ulong target, Abnormality ab, int duration, int stacks);
    public delegate void UpdateBossBuffEventHandler(Boss b, Abnormality ab, int duration, int stacks);

    public static class PacketRouter
    {
        public static uint Version;
        public static OpCodeNamer OpCodeNamer;
        public static OpCodeNamer SystemMessageNamer;
        static List<Character> CurrentAccountCharacters;
        static ConcurrentQueue<Tera.Message> Packets = new ConcurrentQueue<Tera.Message>();


        public static event UpdateBuffEventHandler BuffUpdated;

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
                //PacketInspector.InspectPacket(msg);
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
            SessionManager.CurrentPlayer.Class = p.CharacterClass;
            SessionManager.CurrentPlayer.EntityId = p.entityId;
            SessionManager.CurrentPlayer.Name = p.Name;
            try
            {
                SessionManager.CurrentPlayer.Laurel = CurrentAccountCharacters.First(x => x.Name == p.Name).Laurel;
            }
            catch (Exception)
            {
                SessionManager.CurrentPlayer.Laurel = Laurel.None;
            }
            SessionManager.CurrentPlayer.Level = (int)p.Level;
            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Warrior:
                    WindowManager.CharacterWindow.ShowResolve();
                    break;
                case Class.Lancer:
                    WindowManager.CharacterWindow.ShowResolve();
                    break;
                case Class.Engineer:
                    //WindowManager.InitClassGauge(Class.Engineer);
                    //WindowManager.CharacterWindow.HideResolve();
                    WindowManager.CharacterWindow.ShowResolve(Colors.Orange);
                    break;
                case Class.Fighter:
                    //WindowManager.InitClassGauge(Class.Fighter);
                    //WindowManager.CharacterWindow.HideResolve();
                    WindowManager.CharacterWindow.ShowResolve(Colors.OrangeRed);
                    break;
                case Class.Assassin:
                    //WindowManager.CharacterWindow.HideResolve();
                    //WindowManager.InitClassGauge(Class.Assassin);
                    WindowManager.CharacterWindow.ShowResolve(Color.FromRgb(208, 165, 255));
                    break;
                case Class.Glaiver:
                    //WindowManager.InitClassGauge(Class.Glaiver);
                    //WindowManager.CharacterWindow.HideResolve();
                    WindowManager.CharacterWindow.ShowResolve(Color.FromRgb(204, 224, 255));

                    break;
                default:
                    WindowManager.CharacterWindow.HideResolve();
                    break;
            }
            App.Current.Dispatcher.Invoke(() =>
            {
                WindowManager.ChangeClickThru(WindowManager.Transparent);
            });
        }
        public static void HandleNewSkillCooldown(S_START_COOLTIME_SKILL p)
        {
            SkillManager.AddSkill(p);
            switch (SessionManager.CurrentPlayer.Class)
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
            App.Current.Dispatcher.Invoke(() =>
            {
                SessionManager.CurrentPlayer.Buffs.Clear();
            });
        }
        public static void HandleAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Warrior:
                    Warrior.CheckGambleBuff(p);
                    break;
                case Class.Elementalist:
                    Mystic.CheckHurricane(p);
                    break;
                case Class.Assassin:
                    //redirect intense focus to 10154032
                    //if (p.id == 10154031) p.id++;
                    break;
                default:
                    break;
            }
           
            if (AbnormalityDatabase.Abnormalities.TryGetValue(p.id, out Abnormality ab))
            {
                if (ab.Name.Contains("BTS") || ab.ToolTip.Contains("BTS") || !ab.IsShow) return;
                if (ab.Name.Contains("(Hidden)") || ab.Name.Equals("Unknown") || ab.Name.Equals(string.Empty)) return;
                //Console.WriteLine("{1} {0}",ab.Name, ab.Property);
                if (p.targetId == SessionManager.CurrentPlayer.EntityId)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        if (ab.Type == AbnormalityType.Buff)
                        {
                            if (ab.Infinity)
                            {
                                if (SessionManager.CurrentPlayer.InfBuffs.Any(x => x.Abnormality == ab))
                                {
                                    BuffUpdated?.Invoke(p.targetId, ab, -1, p.stacks);
                                }
                                else
                                {
                                    SessionManager.CurrentPlayer.InfBuffs.Add(new AbnormalityDuration(ab, -1, p.stacks, p.targetId));
                                }
                            }
                            else
                            {
                                if (SessionManager.CurrentPlayer.Buffs.Any(x => x.Abnormality == ab))
                                {
                                    BuffUpdated?.Invoke(p.targetId, ab, p.duration, p.stacks);
                                }
                                else
                                {
                                    SessionManager.CurrentPlayer.Buffs.Add(new AbnormalityDuration(ab, p.duration, p.stacks, p.targetId));
                                }
                            }
                        }
                        else
                        {
                            if (ab.Infinity)
                            {
                                if (SessionManager.CurrentPlayer.Debuffs.Any(x => x.Abnormality == ab))
                                {
                                    BuffUpdated?.Invoke(p.targetId, ab, -1, p.stacks);
                                }
                                else
                                {
                                    SessionManager.CurrentPlayer.Debuffs.Insert(0, new AbnormalityDuration(ab, -1, p.stacks, p.targetId));
                                }
                            }
                            else
                            {
                                if (SessionManager.CurrentPlayer.Debuffs.Any(x => x.Abnormality == ab))
                                {
                                    BuffUpdated?.Invoke(p.targetId, ab, p.duration, p.stacks);
                                }
                                else
                                {
                                    SessionManager.CurrentPlayer.Debuffs.Add(new AbnormalityDuration(ab, p.duration, p.stacks, p.targetId));
                                }
                            }
                        }
                    });
                }
                else
                {
                    if (SessionManager.TryGetBossById(p.targetId, out Boss b))
                {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            if (b.HasBuff(ab))
                            {
                                BuffUpdated?.Invoke(b.EntityId, ab, p.duration, p.stacks);
                            }
                            else
                            {
                                if(!ab.Infinity)
                                {
                                    SessionManager.CurrentBosses.Where(x => x.EntityId == p.targetId).First().Buffs.Add(new AbnormalityDuration(ab, p.duration, p.stacks, p.targetId));
                                }
                                else
                                {
                                    SessionManager.CurrentBosses.Where(x => x.EntityId == p.targetId).First().Buffs.Insert(0,new AbnormalityDuration(ab, -1, p.stacks, p.targetId));
                                }

                            }
                        });
                
                }
                }
            }

            
        }
        public static void HandleAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            if (AbnormalityDatabase.Abnormalities.TryGetValue(p.AbnormalityId, out Abnormality ab))
            {
                if (ab.Name.Contains("BTS") || ab.ToolTip.Contains("BTS") || !ab.IsShow) return;
                if (ab.Name.Contains("(Hidden)") || ab.Name.Equals("Unknown") || ab.Name.Equals(string.Empty)) return;
                //Console.WriteLine("{1} {0}", ab.Name, ab.Property);

                if (p.TargetId == SessionManager.CurrentPlayer.EntityId)
                {

                    App.Current.Dispatcher.Invoke(() =>
                {
                    if (ab.Type == AbnormalityType.Buff)
                    {
                        if (ab.Infinity)
                        {
                            if (SessionManager.CurrentPlayer.InfBuffs.Any(x => x.Abnormality == ab))
                            {
                                BuffUpdated?.Invoke(p.TargetId, ab, -1, p.Stacks);
                            }
                            else
                            {
                                SessionManager.CurrentPlayer.InfBuffs.Add(new AbnormalityDuration(ab, -1, p.Stacks, p.TargetId));
                            }
                        }
                        else
                        {
                            if (SessionManager.CurrentPlayer.Buffs.Any(x => x.Abnormality == ab))
                            {
                                BuffUpdated?.Invoke(p.TargetId, ab, p.Duration, p.Stacks);
                            }
                            else
                            {
                                SessionManager.CurrentPlayer.Buffs.Add(new AbnormalityDuration(ab, p.Duration, p.Stacks, p.TargetId));
                            }
                        }
                    }
                    else
                    {
                        if (ab.Infinity)
                        {
                            if (SessionManager.CurrentPlayer.Debuffs.Any(x => x.Abnormality == ab))
                            {
                                BuffUpdated?.Invoke(p.TargetId, ab, -1, p.Stacks);
                            }
                            else
                            {
                                SessionManager.CurrentPlayer.Debuffs.Insert(0, new AbnormalityDuration(ab, -1, p.Stacks, p.TargetId));
                            }
                        }
                        else
                        {
                            if (SessionManager.CurrentPlayer.Debuffs.Any(x => x.Abnormality == ab))
                            {
                                BuffUpdated?.Invoke(p.TargetId, ab, p.Duration, p.Stacks);
                            }
                            else
                            {
                                SessionManager.CurrentPlayer.Debuffs.Add(new AbnormalityDuration(ab, p.Duration, p.Stacks, p.TargetId));
                            }
                        }
                    }
                });
                }
                else
                {

                if (SessionManager.TryGetBossById(p.TargetId, out Boss b))
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        if (b.HasBuff(ab))
                        {
                            BuffUpdated?.Invoke(b.EntityId, ab, p.Duration, p.Stacks);
                        }
                        else
                        {
                            if (!ab.Infinity)
                            {
                                SessionManager.CurrentBosses.Where(x => x.EntityId == p.TargetId).First().Buffs.Add(new AbnormalityDuration(ab, p.Duration, p.Stacks, p.TargetId));
                            }
                            else
                            {
                                SessionManager.CurrentBosses.Where(x => x.EntityId == p.TargetId).First().Buffs.Insert(0, new AbnormalityDuration(ab, -1, p.Stacks, p.TargetId));
                            }

                        }
                    });

                }
                }
            }
        }
        public static void HandleAbnormalityEnd(S_ABNORMALITY_END p)
        {
            if (AbnormalityDatabase.Abnormalities.TryGetValue(p.id, out Abnormality ab))
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if(p.target == SessionManager.CurrentPlayer.EntityId)
                    {
                        SessionManager.CurrentPlayer.Buffs.Remove(SessionManager.CurrentPlayer.Buffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id));
                        SessionManager.CurrentPlayer.Debuffs.Remove(SessionManager.CurrentPlayer.Debuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id));
                        SessionManager.CurrentPlayer.InfBuffs.Remove(SessionManager.CurrentPlayer.InfBuffs.FirstOrDefault(x => x.Abnormality.Id == ab.Id));

                        return;
                    }
                    if (SessionManager.TryGetBossById(p.target, out Boss b) && b.HasBuff(ab))
                    {
                        b.EndBuff(ab);
                    }
                });
            }

        }
        public static void HandlePlayerStatUpdate(S_PLAYER_STAT_UPDATE p)
        {
            //switch (SessionManager.CurrentPlayer.Class)
            //{
            //    case Class.Warrior:
            //        EdgeGaugeWindow.SetEdge(p.edge);
            //        break;
            //    default:
            //        break;
            //}

            SessionManager.CurrentPlayer.MaxHP = p.maxHp;
            SessionManager.CurrentPlayer.MaxMP = p.maxMp;
            SessionManager.CurrentPlayer.MaxST = p.maxRe + p.bonusRe;
            SessionManager.CurrentPlayer.ItemLevel = p.ilvl;
            SessionManager.CurrentPlayer.CurrentST = p.currRe;
            SessionManager.CurrentPlayer.CurrentHP = p.currHp;
            SessionManager.CurrentPlayer.CurrentMP = p.currMp;


        }
        public static void HandlePlayerChangeMP(S_PLAYER_CHANGE_MP p)
        {
            if (p.target != SessionManager.CurrentPlayer.EntityId)
            {
                return;
            }
            else
            {
                SessionManager.CurrentPlayer.CurrentMP = p.currentMP;

            }
            
        }
        public static void HandleCreatureChangeHP(S_CREATURE_CHANGE_HP p)
        {
            if (p.target != SessionManager.CurrentPlayer.EntityId)
            {
                return;
            }
            else
            {
                SessionManager.CurrentPlayer.CurrentHP = p.currentHP;
            }
            
        }
        public static void HandlePlayerChangeStamina(S_PLAYER_CHANGE_STAMINA p)
        {
            SessionManager.CurrentPlayer.CurrentST = p.currentStamina;

            //switch (SessionManager.CurrentPlayer.Class)
            //{
            //    case Class.Warrior:
            //        SessionManager.CurrentPlayer.CurrentST = p.currentStamina;
            //        break;
            //    case Class.Lancer:
            //        STUpdated.Invoke(p.currentStamina);
            //        break;
            //    case Class.Engineer:
            //        STUpdated.Invoke(p.currentStamina);
            //        break;
            //    case Class.Fighter:
            //        STUpdated.Invoke(p.currentStamina);
            //        break;
            //    case Class.Assassin:
            //        STUpdated.Invoke(p.currentStamina);
            //        break;
            //    case Class.Valkyrie:
            //        STUpdated.Invoke(p.currentStamina);
            //        break;
            //    default:
            //        break;
            //}

        }
        public static void HandleUserStatusChanged(S_USER_STATUS p)
        {
            if (p.id == SessionManager.CurrentPlayer.EntityId)
            {
                if (p.isInCombat)
                {
                    SessionManager.CurrentPlayer.IsInCombat = true;
                }
                else
                {
                    SessionManager.CurrentPlayer.IsInCombat = false;
                }
            }

        }
        public static void HandleSpawn(S_SPAWN_ME p)
        {
            WindowManager.ShowWindow(WindowManager.CharacterWindow);
            //Console.WriteLine("[S_SPAWN_ME]");
            App.Current.Dispatcher.Invoke(() =>
            {
                SessionManager.CurrentBosses.Clear();


            });
        }
        //public static void HandleCharList(Tera.Message msg)
        //{
        //    CLP.ParseCharacters(PacketData(msg));

        //}
        public static void HandleCharList(S_GET_USER_LIST p)
        {
            CurrentAccountCharacters = p.CharacterList;
            //foreach (var c in p.RawCharacters)
            //{
            //    Console.WriteLine(c);
            //}
        }
        public static void HandlePlayerChangeFlightEnergy(S_PLAYER_CHANGE_FLIGHT_ENERGY p)
        {
            SessionManager.CurrentPlayer.FlightEnergy = p.energy;
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
                        //Console.WriteLine("SPAWNED: {0} - {1}   [{2}]", m.Name, m.MaxHP, SessionManager.CurrentBosses.Count);
                    });
                }

                else
                {
                    //Console.WriteLine("SKIPPED: {0} - {1}   [{2}]", m.Name, m.MaxHP, SessionManager.CurrentBosses.Count);
                }
            }
        }

        public static void Debug(bool x)
        {
            SessionManager.TryGetBossById(10, out Boss b);
            if (x)
            {
                b.CurrentHP = b.MaxHP/2;
            }
            else
            {
                b.CurrentHP = b.MaxHP;
            }

        }
        public static void DebugEnrage(bool e)
        {
            SessionManager.TryGetBossById(10, out Boss b);

            b.Enraged = e;
            //EnragedChanged?.Invoke(10, e);

        }
    }

}

