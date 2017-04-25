using DamageMeter.Sniffing;
using Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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


    public static class PacketRouter
    {
        public static uint Version;
        public static OpCodeNamer OpCodeNamer;
        public static OpCodeNamer SystemMessageNamer;
        static ConcurrentQueue<Tera.Message> Packets = new ConcurrentQueue<Tera.Message>();

        public static void Init()
        {
            TeraSniffer.Instance.MessageReceived += MessageReceived;
            var analysisThread = new Thread(PacketAnalysisLoop);
            analysisThread.Start();
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
            Packets.Enqueue(obj);
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
                PacketInspector.InspectPacket(msg);
                if (message.GetType() == typeof(Tera.Game.Messages.UnknownMessage)) continue;

                if (!MessageFactory.Process(message))
                {
                }

            }
        }


        public static void HandleCharLogin(S_LOGIN p)
        {
            SessionManager.Logged = true;
            SessionManager.CurrentPlayer.Class = p.CharacterClass;
            SessionManager.CurrentPlayer.EntityId = p.entityId;
            SessionManager.CurrentPlayer.Name = p.Name;
            SessionManager.CurrentPlayer.Level = p.Level;
            SessionManager.SetPlayerLaurel(p.Name);

            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Warrior:
                    WindowManager.CharacterWindow.ShowResolve();
                    break;
                case Class.Lancer:
                    WindowManager.CharacterWindow.ShowResolve();
                    break;
                case Class.Engineer:
                    WindowManager.CharacterWindow.ShowResolve(Colors.Orange);
                    break;
                case Class.Fighter:
                    WindowManager.CharacterWindow.ShowResolve(Colors.OrangeRed);
                    break;
                case Class.Assassin:
                    WindowManager.CharacterWindow.ShowResolve(Color.FromRgb(208, 165, 255));
                    break;
                case Class.Glaiver:
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
        public static void HandlePlayerLocation(C_PLAYER_LOCATION p)
        {
            if (WindowManager.BossGauge.HarrowholdMode)
            {
                EntitiesManager.CheckCurrentDragon(new System.Windows.Point(p.X, p.Y));
            }
        }
        public static void HandleNewSkillCooldown(S_START_COOLTIME_SKILL p)
        {
            SkillManager.AddSkill(p);
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
            SessionManager.ClearPlayersAbnormalities();
            EntitiesManager.ClearNPC();
        }
        public static void HandleAbnormalityEnd(S_ABNORMALITY_END p)
        {
            AbnormalityManager.EndAbnormality(p.target, p.id);
        }
        public static void HandleAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            AbnormalityManager.BeginAbnormality(p.AbnormalityId, p.TargetId, p.Duration, p.Stacks);
        }
        public static void HandleAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            AbnormalityManager.BeginAbnormality(p.id, p.targetId, p.duration, p.stacks);

            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Elementalist:
                    Mystic.CheckHurricane(p);
                    break;
                default:
                    break;
            }                   
        }

        public static void HandleLoadTopo(S_LOAD_TOPO x)
        {
            SessionManager.LoadingScreen = true;
        }

        public static void HandleLoadTopoFin(C_LOAD_TOPO_FIN x)
        {
            //SessionManager.LoadingScreen = false;
        }

        public static void HandlePlayerStatUpdate(S_PLAYER_STAT_UPDATE p)
        {
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
            SessionManager.SetPlayerMP(p.target, p.currentMP); 
        }
        public static void HandleCreatureChangeHP(S_CREATURE_CHANGE_HP p)
        {
            SessionManager.SetPlayerHP(p.target, p.currentHP);
        }
        public static void HandlePlayerChangeStamina(S_PLAYER_CHANGE_STAMINA p)
        {
            SessionManager.CurrentPlayer.CurrentST = p.currentStamina;
        }
        public static void HandleUserStatusChanged(S_USER_STATUS p)
        {
            SessionManager.SetCombatStatus(p.id, p.isInCombat);
        }
        public static void HandleSpawnMe(S_SPAWN_ME p)
        {
            //WindowManager.ShowWindow(WindowManager.CharacterWindow);
            EntitiesManager.ClearNPC();
            System.Timers.Timer t = new System.Timers.Timer(2000);
            t.Elapsed += (s, ev) =>
            {
                SessionManager.LoadingScreen = false;
                t.Stop();
            };
            t.Enabled = true;
        }
        public static void HandleCharList(S_GET_USER_LIST p)
        {
            SessionManager.CurrentAccountCharacters = p.CharacterList;
        }
        public static void HandlePlayerChangeFlightEnergy(S_PLAYER_CHANGE_FLIGHT_ENERGY p)
        {
            SessionManager.CurrentPlayer.FlightEnergy = p.energy;
        }
        public static void HandleGageReceived(S_BOSS_GAGE_INFO p)
        {
            EntitiesManager.UpdateNPCbyGauge(p.EntityId, p.CurrentHP, p.MaxHP, (ushort)p.HuntingZoneId, (uint)p.TemplateId);
        }
        public static void HandleNpcStatusChanged(S_NPC_STATUS p)
        {
            EntitiesManager.SetNPCStatus(p.EntityId, p.IsEnraged);
        }
        public static void HandleNpcDespawn(S_DESPAWN_NPC p)
        {
            EntitiesManager.DespawnNPC(p.target);
        }
        public static void HandleNpcSpawn(S_SPAWN_NPC p)
        {
            EntitiesManager.SpawnNPC(p.HuntingZoneId, p.TemplateId, p.EntityId, System.Windows.Visibility.Collapsed);
            EntitiesManager.CheckHarrowholdMode(p.HuntingZoneId, p.TemplateId);
        }

        //public static void Debug(bool x)
        //{
        //    SessionManager.TryGetBossById(10, out Boss b);
        //    if (x)
        //    {
        //        b.CurrentHP = b.MaxHP/2;
        //    }
        //    else
        //    {
        //        b.CurrentHP = b.MaxHP;
        //    }

        //}
        //public static void DebugEnrage(bool e)
        //{
        //    SessionManager.TryGetBossById(10, out Boss b);

        //    b.Enraged = e;
        //    //EnragedChanged?.Invoke(10, e);

        //}

    }

}
