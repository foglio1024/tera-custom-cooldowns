using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TCC.ClassSpecific;
using TCC.Data;
using TCC.Data.Databases;
using TCC.Parsing.Messages;
using TCC.Sniffing;
using TCC.Tera.Data;
using TCC.TeraCommon;
using TCC.TeraCommon.Game;
using TCC.TeraCommon.Game.Services;
using TCC.ViewModels;
using TCC.Windows;
using S_GET_USER_GUILD_LOGO = TCC.TeraCommon.Game.Messages.Server.S_GET_USER_GUILD_LOGO;

namespace TCC.Parsing
{


    public static class PacketProcessor
    {
        public static uint Version;
        public static Server Server;
        private static string Language => new TeraData(Server.Region).GetLanguage(Server.Region);
        public static OpCodeNamer OpCodeNamer;
        public static OpCodeNamer SystemMessageNamer;
        public static MessageFactory Factory;
        private static readonly ConcurrentQueue<Message> Packets = new ConcurrentQueue<Message>();
        private static System.Timers.Timer _x;
        public static void Init()
        {
            TeraSniffer.Instance.MessageReceived += MessageReceived;
            var analysisThread = new Thread(PacketAnalysisLoop);
            analysisThread.Start();
            _x = new System.Timers.Timer { Interval = 1000 };
            _x.Elapsed += (s, ev) =>
            {
                Debug.WriteLine("Q:" + Packets.Count + " P:" + _processed + " D:" + _discarded);
                _processed = 0; _discarded = 0;
            };
            //x.Start();
        }


        private static void MessageReceived(Message obj)
        {
            if (obj.Direction == MessageDirection.ClientToServer && obj.OpCode == 19900)
            {
                var message = new C_CHECK_VERSION_CUSTOM(new CustomReader(obj));
                Version = message.Versions[0];
                OpcodeDownloader.DownloadIfNotExist(Version, Path.Combine(BasicTeraData.Instance.ResourceDirectory, "data/opcodes/"));
                if (!File.Exists(Path.Combine(BasicTeraData.Instance.ResourceDirectory, $"data/opcodes/{message.Versions[0]}.txt")) && !File.Exists(Path.Combine(BasicTeraData.Instance.ResourceDirectory, $"data/opcodes/protocol.{message.Versions[0]}.map"))
                    || !File.Exists(Path.Combine(BasicTeraData.Instance.ResourceDirectory, $"data/opcodes/smt_{message.Versions[0]}.txt")) && !File.Exists(Path.Combine(BasicTeraData.Instance.ResourceDirectory, $"data/opcodes/sysmsg.{message.Versions[0]}.map")))
                {
                    {
                        TccMessageBox.Show("Unknown client version: " + message.Versions[0], MessageBoxType.Error);
                        App.CloseApp();
                        return;
                    }
                }
                OpCodeNamer = new OpCodeNamer(Path.Combine(BasicTeraData.Instance.ResourceDirectory, $"data/opcodes/{message.Versions[0]}.txt"));
                SystemMessageNamer = new OpCodeNamer(Path.Combine(BasicTeraData.Instance.ResourceDirectory, $"data/opcodes/smt_{message.Versions[0]}.txt"));
                Factory = new MessageFactory(OpCodeNamer, Server.Region, message.Versions[0], sysMsgNamer: SystemMessageNamer);
                TeraSniffer.Instance.Connected = true;
                Proxy.ConnectToProxy();
                return;
            }
            Packets.Enqueue(obj);
        }

        private static int _processed;
        private static int _discarded;
        private static void PacketAnalysisLoop()
        {
            while (true)
            {
                var successDequeue = Packets.TryDequeue(out var msg);
                if (!successDequeue)
                {
                    Thread.Sleep(1);
                    continue;
                }
                var message = Factory.Create(msg);
                Factory.Process(message);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public static void HandleNewSkillCooldown(S_START_COOLTIME_SKILL p)
        {
            SkillManager.AddSkill(p.SkillId, p.Cooldown);
        }
        public static void HandleNewItemCooldown(S_START_COOLTIME_ITEM p)
        {
            SkillManager.AddItemSkill(p.ItemId, p.Cooldown);
        }
        public static void HandleDecreaseSkillCooldown(S_DECREASE_COOLTIME_SKILL p)
        {
            SkillManager.ChangeSkillCooldown(p.SkillId, p.Cooldown);
        }

        public static void HandlePlayerStatUpdate(S_PLAYER_STAT_UPDATE p)
        {
            SessionManager.CurrentPlayer.ItemLevel = p.Ilvl;
            SessionManager.CurrentPlayer.Level = p.Level;
            CharacterWindowViewModel.Instance.Player.ItemLevel = p.Ilvl;
            CharacterWindowViewModel.Instance.Player.Level = p.Level;

            SessionManager.SetPlayerMaxHp(SessionManager.CurrentPlayer.EntityId, p.MaxHP);
            SessionManager.SetPlayerMaxMp(SessionManager.CurrentPlayer.EntityId, p.MaxMP);
            SessionManager.SetPlayerMaxSt(SessionManager.CurrentPlayer.EntityId, p.MaxST + p.BonusST);

            SessionManager.SetPlayerHp(p.CurrentHP);
            SessionManager.SetPlayerMp(SessionManager.CurrentPlayer.EntityId, p.CurrentMP);
            SessionManager.SetPlayerSt(SessionManager.CurrentPlayer.EntityId, p.CurrentST);

            if (!SettingsManager.ClassWindowSettings.Enabled) return;
            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Warrior:
                    ((WarriorBarManager)ClassWindowViewModel.Instance.CurrentManager).EdgeCounter.Val = p.Edge;
                    break;
            }

        }
        public static void HandleCreatureChangeHp(S_CREATURE_CHANGE_HP p)
        {
            SessionManager.SetPlayerMaxHp(p.Target, p.MaxHP);
            if (p.Target == SessionManager.CurrentPlayer.EntityId)
            {
                SessionManager.SetPlayerHp(p.CurrentHP);
            }
            else
            {
                EntitiesManager.UpdateNPCbyCreatureChangeHP(p.Target, p.CurrentHP, p.MaxHP);
            }
        }
        public static void HandlePlayerChangeMp(S_PLAYER_CHANGE_MP p)
        {
            SessionManager.SetPlayerMaxMp(p.Target, p.MaxMP);
            SessionManager.SetPlayerMp(p.Target, p.CurrentMP);
        }
        public static void HandlePlayerChangeStamina(S_PLAYER_CHANGE_STAMINA p)
        {
            SessionManager.SetPlayerSt(SessionManager.CurrentPlayer.EntityId, p.CurrentST);
        }
        public static void HandlePlayerChangeFlightEnergy(S_PLAYER_CHANGE_FLIGHT_ENERGY p)
        {
            SessionManager.SetPlayerFe(p.Energy);
        }
        public static void HandleUserStatusChanged(S_USER_STATUS p)
        {
            SessionManager.SetCombatStatus(p.EntityId, p.IsInCombat);
        }

        public static void HandleGageReceived(S_BOSS_GAGE_INFO p)
        {
            EntitiesManager.UpdateNPCbyGauge(p.EntityId, p.CurrentHP, p.MaxHP, (ushort)p.HuntingZoneId, (uint)p.TemplateId);
        }
        public static void HandleNpcStatusChanged(S_NPC_STATUS p)
        {
            EntitiesManager.SetNPCStatus(p.EntityId, p.IsEnraged);
            if (p.Target == 0)
            {
                BossGageWindowViewModel.Instance.UnsetBossTarget(p.EntityId);
            }
            var b = BossGageWindowViewModel.Instance.NpcList.ToSyncArray().FirstOrDefault(x => x.EntityId == p.EntityId);
            if (BossGageWindowViewModel.Instance.CurrentHHphase == HarrowholdPhase.None) return;
            if (b != null && b.IsBoss && b.Visible == System.Windows.Visibility.Visible) GroupWindowViewModel.Instance.SetAggro(p.Target);

        }
        public static void HandleUserEffect(S_USER_EFFECT p)
        {
            BossGageWindowViewModel.Instance.SetBossAggro(p.Source, p.Circle, p.User);
            GroupWindowViewModel.Instance.SetAggroCircle(p);
        }

        public static void HandleCharList(S_GET_USER_LIST p)
        {
            /*- Moved from HandleReturnToLobby -*/
            SessionManager.Logged = false;
            SessionManager.CurrentPlayer.ClearAbnormalities();
            BuffBarWindowViewModel.Instance.Player.ClearAbnormalities();
            SkillManager.Clear();
            EntitiesManager.ClearNPC();
            GroupWindowViewModel.Instance.ClearAll();
            /*---------------------------------*/

            foreach (var item in p.CharacterList)
            {
                var ch = InfoWindowViewModel.Instance.Characters.FirstOrDefault(x => x.Id == item.Id);
                if (ch != null)
                {
                    ch.Name = item.Name;
                    ch.Laurel = item.Laurel;
                    ch.Position = item.Position;
                }
                else
                {
                    InfoWindowViewModel.Instance.Characters.Add(item);
                }
            }
            InfoWindowViewModel.Instance.SaveToFile();
        }
        public static void HandleLogin(S_LOGIN p)
        {
            if (SettingsManager.ClassWindowSettings.Enabled) ClassWindowViewModel.Instance.CurrentClass = p.CharacterClass;
            Server = BasicTeraData.Instance.Servers.GetServer(p.ServerId);
            if (!SettingsManager.StatSent) App.SendUsageStat();
            SettingsManager.LastRegion = Language;
            TimeManager.Instance.SetServerTimeZone(SettingsManager.LastRegion);
            TimeManager.Instance.SetGuildBamTime(false);
            SessionManager.InitDatabases(SettingsManager.LastRegion);
            CooldownWindowViewModel.Instance.ClearSkills();
            SessionManager.CurrentPlayer.Class = p.CharacterClass;
            CooldownWindowViewModel.Instance.LoadSkills(Utils.ClassEnumToString(p.CharacterClass).ToLower() + "-skills.xml", p.CharacterClass);
            ChatWindowManager.Instance.SetMgButtonVis();
            EntitiesManager.ClearNPC();
            GroupWindowViewModel.Instance.ClearAll();

            BuffBarWindowViewModel.Instance.Player.ClearAbnormalities();
            SkillManager.Clear();

            SessionManager.LoadingScreen = true;
            SessionManager.Logged = true;
            SessionManager.Encounter = false;
            MessageFactory.Update();
            SessionManager.CurrentPlayer.EntityId = p.entityId;
            SessionManager.CurrentPlayer.PlayerId = p.PlayerId;
            SessionManager.CurrentPlayer.ServerId = p.ServerId;
            SessionManager.CurrentPlayer.Name = p.Name;
            SessionManager.CurrentPlayer.Level = p.Level;
            SessionManager.SetPlayerLaurel(SessionManager.CurrentPlayer);

            CharacterWindowViewModel.Instance.Player.Class = p.CharacterClass;
            CharacterWindowViewModel.Instance.Player.Name = p.Name;
            CharacterWindowViewModel.Instance.Player.Level = p.Level;
            SessionManager.SetPlayerLaurel(CharacterWindowViewModel.Instance.Player);
            InfoWindowViewModel.Instance.SetLoggedIn(p.PlayerId);
        }

        internal static void HandleLfgList(S_SHOW_PARTY_MATCH_INFO x)
        {
            if (!SettingsManager.LfgEnabled) return;
            if (WindowManager.LfgListWindow == null) return;
            if (WindowManager.LfgListWindow.VM == null) return;
            if (!x.IsLast) return;
            //WindowManager.LfgListWindow.VM.Listings.Clear();
            S_SHOW_PARTY_MATCH_INFO.Listings.ForEach(l =>
            {
                if (WindowManager.LfgListWindow.VM.Listings.Any(toFind => toFind.LeaderId == l.LeaderId))
                {
                    var target = WindowManager.LfgListWindow.VM.Listings.FirstOrDefault(t => t.LeaderId == l.LeaderId);
                    if (target == null) return;
                    target.LeaderId = l.LeaderId;
                    target.Message = l.Message;
                    target.IsRaid = l.IsRaid;
                    target.LeaderName = l.LeaderName;
                    if (target.PlayerCount != l.PlayerCount)
                    {
                        Proxy.RequestPartyInfo(l.LeaderId);
                    }
                }
                else WindowManager.LfgListWindow.VM.Listings.Add(l);
            });
            var toRemove = new List<uint>();
            WindowManager.LfgListWindow.VM.Listings.ToList().ForEach(l =>
            {
                if (S_SHOW_PARTY_MATCH_INFO.Listings.All(f => f.LeaderId != l.LeaderId)) toRemove.Add(l.LeaderId);
            });
            toRemove.ForEach(r =>
            {
                var target = WindowManager.LfgListWindow.VM.Listings.FirstOrDefault(rm => rm.LeaderId == r);
                if (target != null) WindowManager.LfgListWindow.VM.Listings.Remove(target);
            });
            WindowManager.LfgListWindow.VM.NotifyMyLfg();
            WindowManager.LfgListWindow.ShowWindow();
        }

        private static Stopwatch _st;
        internal static void HandleActionStage(S_ACTION_STAGE x)
        {
            _st.Stop();
            Console.WriteLine("- S_ACTION_STAGE -");
            Console.WriteLine($"GameId: {x.GameId}");
            Console.WriteLine($"Target: {x.Target}");
            Console.WriteLine($"Id:     {x.Id}");
            Console.WriteLine($"Speed:  {x.Speed * 1.1}");
            Console.WriteLine($"Elaps:  {_st.ElapsedMilliseconds}");
            _st.Restart();
        }

        public static void SendTestMessage()
        {
            //var str = "@3947questNameDefeat HumedraszoneName@zoneName:181npcName@creature:181#2050";
            //var str = "@3789cityname@cityWar:20guildFated";
            //var str = "@1773ItemName@item:152141ItemName1@item:447ItemCount5";
            const string str = "@3821userNametestNameguildQuestName@GuildQuest:31007001value1targetValue3";
            var toBytes = Encoding.Unicode.GetBytes(str);
            var arr = new byte[toBytes.Length + 2 + 4];
            for (var i = 0; i < toBytes.Length - 1; i++)
            {
                arr[i + 4] = toBytes[i];
            }

            var seg = new ArraySegment<byte>(arr);

            var sysMsg = new S_SYSTEM_MESSAGE(new TeraMessageReader(new Message(DateTime.Now, MessageDirection.ServerToClient, seg), OpCodeNamer, Factory, SystemMessageNamer));
            HandleSystemMessage(sysMsg);

        }
        public static void HandleReturnToLobby(S_RETURN_TO_LOBBY p)
        {
            SessionManager.Logged = false;
            SessionManager.CurrentPlayer.ClearAbnormalities();
            BuffBarWindowViewModel.Instance.Player.ClearAbnormalities();
            SkillManager.Clear();
            EntitiesManager.ClearNPC();
            GroupWindowViewModel.Instance.ClearAll();

        }


        public static void HandleLoadTopo(S_LOAD_TOPO x)
        {
            SessionManager.LoadingScreen = true;
            SessionManager.Encounter = false;
            GroupWindowViewModel.Instance.ClearAllAbnormalities();
            GroupWindowViewModel.Instance.SetAggro(0);
            BuffBarWindowViewModel.Instance.Player.ClearAbnormalities();
            BossGageWindowViewModel.Instance.CurrentHHphase = HarrowholdPhase.None;
            BossGageWindowViewModel.Instance.ClearGuildTowers();
        }
        public static void HandleLoadTopoFin(C_LOAD_TOPO_FIN x)
        {
            //SessionManager.LoadingScreen = false;
        }

        public static void HandleStartRoll(S_ASK_BIDDING_RARE_ITEM x)
        {
            GroupWindowViewModel.Instance.StartRoll();
        }
        public static void HandleRollResult(S_RESULT_BIDDING_DICE_THROW x)
        {
            if (!GroupWindowViewModel.Instance.Rolling) GroupWindowViewModel.Instance.StartRoll();
            GroupWindowViewModel.Instance.SetRoll(x.EntityId, x.RollResult);
        }
        public static void HandleEndRoll(S_RESULT_ITEM_BIDDING x)
        {
            GroupWindowViewModel.Instance.EndRoll();
        }

        public static void HandleSpawnMe(S_SPAWN_ME p)
        {
            EntitiesManager.ClearNPC();
            var t = new System.Timers.Timer(2000);
            t.Elapsed += (s, ev) =>
            {
                t.Stop();
                SessionManager.LoadingScreen = false;
                WindowManager.ForegroundManager.RefreshDim();

            };
            t.Enabled = true;
        }
        public static void HandleSpawnNpc(S_SPAWN_NPC p)
        {
            EntitiesManager.CheckHarrowholdMode(p.HuntingZoneId, p.TemplateId);
            EntitiesManager.SpawnNPC(p.HuntingZoneId, p.TemplateId, p.EntityId, System.Windows.Visibility.Collapsed);
        }
        public static void HandleSpawnUser(S_SPAWN_USER p)
        {
            if (!GroupWindowViewModel.Instance.Exists(p.EntityId)) return;

            GroupWindowViewModel.Instance.UpdateMemberGear(p);
        }

        public static void HandlePartyMemberBuffUpdate(S_PARTY_MEMBER_BUFF_UPDATE x)
        {
            foreach (var buff in x.Abnormals)
            {
                AbnormalityManager.BeginOrRefreshPartyMemberAbnormality(x.PlayerId, x.ServerId, buff.Id, buff.Duration, buff.Stacks);
            }
        }
        public static void HandlePartyMemberAbnormalAdd(S_PARTY_MEMBER_ABNORMAL_ADD x)
        {
            AbnormalityManager.BeginOrRefreshPartyMemberAbnormality(x.PlayerId, x.ServerId, x.Id, x.Duration, x.Stacks);
        }
        public static void HandlePartyMemberAbnormalDel(S_PARTY_MEMBER_ABNORMAL_DEL x)
        {
            AbnormalityManager.EndPartyMemberAbnormality(x.PlayerId, x.ServerId, x.Id);
        }

        public static void HandleRunemark(S_WEAK_POINT x)
        {
            if (SessionManager.CurrentPlayer.Class != Class.Valkyrie) return;
            if (ClassWindowViewModel.Instance.CurrentManager.GetType() != typeof(ValkyrieBarManager)) return;
            ((ValkyrieBarManager)ClassWindowViewModel.Instance.CurrentManager).RunemarksCounter.Val = (int)x.TotalRunemarks;
        }

        public static void HandleSkillResult(S_EACH_SKILL_RESULT x)
        {
            //bool sourceInParty = GroupWindowViewModel.Instance.UserExists(x.Source);
            //bool targetInParty = GroupWindowViewModel.Instance.UserExists(x.Target);
            //if (x.Target == x.Source) return;
            //if (sourceInParty && targetInParty) return;
            //if (sourceInParty || targetInParty) WindowManager.SkillsEnded = false;
            //if (x.Source == SessionManager.CurrentPlayer.EntityId) WindowManager.SkillsEnded = false;
            if (x.Source == SessionManager.CurrentPlayer.EntityId) return;
            BossGageWindowViewModel.Instance.UpdateShield(x.Target, x.Damage);
        }

        public static void HandleChangeLeader(S_CHANGE_PARTY_MANAGER x)
        {
            GroupWindowViewModel.Instance.SetNewLeader(x.EntityId, x.Name);
        }

        public static void HandlePartyMemberAbnormalClear(S_PARTY_MEMBER_ABNORMAL_CLEAR x)
        {
            GroupWindowViewModel.Instance.ClearAbnormality(x.PlayerId, x.ServerId);
        }
        public static void HandlePartyMemberAbnormalRefresh(S_PARTY_MEMBER_ABNORMAL_REFRESH x)
        {
            AbnormalityManager.BeginOrRefreshPartyMemberAbnormality(x.PlayerId, x.ServerId, x.Id, x.Duration, x.Stacks);
        }

        public static void HandleChat(S_CHAT x)
        {
            ChatWindowManager.Instance.AddChatMessage(new ChatMessage(x.Channel, x.AuthorName, x.Message));
        }

        public static void HandlePrivateChat(S_PRIVATE_CHAT x)
        {
            var i = ChatWindowManager.Instance.PrivateChannels.FirstOrDefault(y => y.Id == x.Channel).Index;
            var ch = (ChatChannel)(ChatWindowManager.Instance.PrivateChannels[i].Index + 11);

            ChatWindowManager.Instance.AddChatMessage(new ChatMessage(ch, x.AuthorName, x.Message));
        }
        public static void HandleCommandOutput(string msg)
        {
            var ch = (ChatChannel)(ChatWindowManager.Instance.PrivateChannels[7].Index + 11);

            ChatWindowManager.Instance.AddChatMessage(new ChatMessage(ch, "System", msg));
        }

        internal static void HandleFriendIntoArea(S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA x)
        {
            var friend = ChatWindowManager.Instance.Friends.FirstOrDefault(f => f.PlayerId == x.PlayerId);
            if (friend == null) return;
            const string opcode = "SMT_FRIEND_WALK_INTO_SAME_AREA";
            var areaName = x.SectionId.ToString();
            try
            {
                areaName = SessionManager.MapDatabase.Names[SessionManager.MapDatabase.Worlds[x.WorldId].Guards[x.GuardId].Sections[x.SectionId].NameId];
            }
            catch (Exception)
            {
                // ignored
            }
            var srvMsg = "@0\vUserName\v" + friend.Name + "\vAreaName\v" + areaName;
            SessionManager.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);

            SystemMessagesProcessor.AnalyzeMessage(srvMsg, m, opcode);
        }

        public static void HandleJoinPrivateChat(S_JOIN_PRIVATE_CHANNEL x)
        {
            ChatWindowManager.Instance.PrivateChannels[x.Index] = new PrivateChatChannel(x.Id, x.Name, x.Index);
        }

        internal static void HandleGuildTowerInfo(S_GUILD_TOWER_INFO x)
        {
            BossGageWindowViewModel.Instance.AddGuildTower(x.TowerId, x.GuildName, x.GuildId);
        }

        public static void HandleLeavePrivateChat(S_LEAVE_PRIVATE_CHANNEL x)
        {
            var i = ChatWindowManager.Instance.PrivateChannels.FirstOrDefault(c => c.Id == x.Id).Index;
            ChatWindowManager.Instance.PrivateChannels[i].Joined = false;
        }

        internal static void HandleDungeonCooltimeList(S_DUNGEON_COOL_TIME_LIST x)
        {
            InfoWindowViewModel.Instance.SetDungeons(x.DungeonCooldowns);
        }

        internal static void HandleAccountPackageList(S_ACCOUNT_PACKAGE_LIST x)
        {
            SessionManager.IsElite = x.IsElite;
        }

        public static void HandleWhisper(S_WHISPER x)
        {
            if (x.Author == SessionManager.CurrentPlayer.Name)
            {
                ChatWindowManager.Instance.AddChatMessage(new ChatMessage(ChatChannel.SentWhisper, x.Recipient,
                    x.Message));
            }
            else
            {
                ChatWindowManager.Instance.AddChatMessage(new ChatMessage(ChatChannel.ReceivedWhisper, x.Author,
                    x.Message));
            }
        }

        internal static void HandleGuardianInfo(S_FIELD_POINT_INFO x)
        {
            InfoWindowViewModel.Instance.CurrentCharacter.GuardianPoints = x.Points;
            InfoWindowViewModel.Instance.CurrentCharacter.MaxGuardianPoints = x.MaxPoints;
        }

        internal static void HandleVanguardReceived(S_AVAILABLE_EVENT_MATCHING_LIST x)
        {
            InfoWindowViewModel.Instance.SetVanguard(x);
        }

        internal static void HandleDungeonClears(S_DUNGEON_CLEAR_COUNT_LIST x)
        {
            foreach (var dg in x.DungeonClears)
            {
                if(InfoWindowViewModel.Instance.SelectedCharacter != null)
                    InfoWindowViewModel.Instance.SelectedCharacter.SetDungeonTotalRuns(dg.Key, dg.Value);
            }
        }

        internal static void HandleDungeonMessage(S_DUNGEON_EVENT_MESSAGE p)
        {
            if (p.MessageId == 9950045)
            {
                //shield start
                foreach (var item in BossGageWindowViewModel.Instance.NpcList.Where(x => x.IsPhase1Dragon))
                {
                    item.StartShield();
                }
            }
            else if (p.MessageId == 9950113)
            {
                //aquadrax interrupted
                BossGageWindowViewModel.Instance.NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1103).BreakShield();
            }
            else if (p.MessageId == 9950114)
            {
                //umbradrax interrupted
                BossGageWindowViewModel.Instance.NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1102).BreakShield();

            }
            else if (p.MessageId == 9950115)
            {
                //ignidrax interrupted
                BossGageWindowViewModel.Instance.NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1100).BreakShield();

            }
            else if (p.MessageId == 9950116)
            {
                //terradrax interrupted
                BossGageWindowViewModel.Instance.NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1101).BreakShield();

            }
            else if (p.MessageId == 9950044)
            {
                //shield fail
            }
        }

        internal static void HandleBrokerOffer(S_TRADE_BROKER_DEAL_SUGGESTED x)
        {
            ChatWindowManager.Instance.AddChatMessage(new BrokerChatMessage(x));
        }

        internal static void HandleUserApplyToParty(S_OTHER_USER_APPLY_PARTY x)
        {
            ChatWindowManager.Instance.AddChatMessage(new ApplyMessage(x)); //TODO: got NullRefEx here
            if (WindowManager.LfgListWindow.VM.MyLfg == null) return;
            var dest = WindowManager.LfgListWindow.VM.MyLfg.Applicants;
            if (dest.Any(u => u.PlayerId == x.PlayerId)) return;
            dest.Add(new User(WindowManager.LfgListWindow.Dispatcher)
            {
                PlayerId = x.PlayerId,
                UserClass = x.Class,
                Level = Convert.ToUInt32(x.Level),
                Name = x.Name,
                Online = true

            });
        }

        internal static void HandleFriendStatus(S_UPDATE_FRIEND_INFO x)
        {
            var opcodeName = "SMT_FRIEND_IS_CONNECTED";
            if (!x.Online) return;
            if (SessionManager.SystemMessagesDatabase.Messages.TryGetValue(opcodeName, out var m))
            {
                SystemMessagesProcessor.AnalyzeMessage(x.Name, m, opcodeName);
            }
        }

        internal static void HandleLfgSpam(S_PARTY_MATCH_LINK x)
        {
            if (x.Message.IndexOf("WTB", 0, StringComparison.InvariantCultureIgnoreCase) != -1) return;
            if (x.Message.IndexOf("WTS", 0, StringComparison.InvariantCultureIgnoreCase) != -1) return;
            if (x.Message.IndexOf("WTT", 0, StringComparison.InvariantCultureIgnoreCase) != -1) return;
            ChatWindowManager.Instance.AddOrRefreshLfg(x);
        }

        public static void HandleSystemMessage(S_SYSTEM_MESSAGE x)
        {
            try
            {
                var msg = x.Message.Split('\v');
                var opcode = ushort.Parse(msg[0].Substring(1));
                var opcodeName = SystemMessageNamer.GetName(opcode);

                if (SessionManager.SystemMessagesDatabase.Messages.TryGetValue(opcodeName, out var m))
                {
                    SystemMessagesProcessor.AnalyzeMessage(x.Message, m, opcodeName);
                }

            }
            catch (Exception)
            {
                File.AppendAllText("chat-errors.log", x.Message + "\n");
            }
        }

        internal static void HandleAccomplishAchievement(S_ACCOMPLISH_ACHIEVEMENT x)
        {
            if (!SessionManager.AchievementDatabase.Achievements.ContainsKey(x.AchievementId)) return;
            if (!SessionManager.SystemMessagesDatabase.Messages.TryGetValue("SMT_ACHIEVEMENT_GRADE0_CLEAR_MESSAGE", out var m)) return;

            var sysMsg = new ChatMessage("@0\vAchievementName\v@achievement:" + x.AchievementId, m, (ChatChannel)m.ChatChannel);
            ChatWindowManager.Instance.AddChatMessage(sysMsg);
        }

        public static void HandleBlockList(S_USER_BLOCK_LIST x)
        {
            ChatWindowManager.Instance.BlockedUsers = x.BlockedUsers;
        }

        internal static void HandleFriendList(S_FRIEND_LIST x)
        {
            ChatWindowManager.Instance.Friends = x.Friends;
        }

        internal static void HandleAnswerInteractive(S_ANSWER_INTERACTIVE x)
        {
            SessionManager.MonsterDatabase.TryGetMonster(x.Model, 0, out var m);
            ChatWindowManager.Instance.TooltipInfo.Name = x.Name;
            ChatWindowManager.Instance.TooltipInfo.Info = m.Name;
            ChatWindowManager.Instance.TooltipInfo.Level = (int)x.Level;
            ChatWindowManager.Instance.TooltipInfo.SetInfo(x.Model);
            if (x.Name == SessionManager.CurrentPlayer.Name)
            {
                ChatWindowManager.Instance.TooltipInfo.ShowGuildInvite = false;
                ChatWindowManager.Instance.TooltipInfo.ShowPartyInvite = false;
            }
            else
            {
                ChatWindowManager.Instance.TooltipInfo.ShowGuildInvite = !x.HasGuild;
                ChatWindowManager.Instance.TooltipInfo.ShowPartyInvite = !x.HasParty;
            }
            if (!Proxy.IsConnected) return;
            ChatWindowManager.Instance.OpenTooltip();
        }

        internal static void HandleCrestMessage(S_CREST_MESSAGE x)
        {

        }

        internal static void HandleSystemMessageLoot(S_SYSTEM_MESSAGE_LOOT_ITEM x)
        {
            try
            {
                var msg = x.SysMessage.Split('\v');
                var opcode = ushort.Parse(msg[0].Substring(1));
                var opcodeName = SystemMessageNamer.GetName(opcode);

                if (SessionManager.SystemMessagesDatabase.Messages.TryGetValue(opcodeName, out var m))
                {
                    var sysMsg = new ChatMessage(x.SysMessage, m, (ChatChannel)m.ChatChannel);
                    ChatWindowManager.Instance.AddChatMessage(sysMsg);
                }

            }
            catch (Exception)
            {

                File.AppendAllText("chat-errors.log", x.SysMessage + "\n");
            }
        }

        public static void HandleDespawnNpc(S_DESPAWN_NPC p)
        {
            EntitiesManager.DespawnNPC(p.Target, p.Type);
        }


        public static void HandleDespawnUser(S_DESPAWN_USER p)
        {
        }

        public static void HandleAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            AbnormalityManager.BeginAbnormality(p.AbnormalityId, p.TargetId, p.Duration, p.Stacks);
            if (!SettingsManager.ClassWindowSettings.Enabled) return;
            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Mystic:
                    Mystic.CheckHurricane(p);
                    Mystic.CheckBuff(p);
                    break;
                case Class.Warrior:
                    Warrior.CheckBuff(p);
                    break;
                case Class.Valkyrie:
                    Valkyrie.CheckRagnarok(p);
                    break;
                case Class.Archer:
                    Archer.CheckFocus(p);
                    Archer.CheckFocusX(p);
                    Archer.CheckSniperEye(p);
                    break;
                case Class.Lancer:
                    Lancer.CheckArush(p);
                    Lancer.CheckGshout(p);
                    Lancer.CheckLineHeld(p);
                    break;
                case Class.Priest:
                    Priest.CheckBuff(p);
                    break;
                case Class.Brawler:
                    Brawler.CheckBrawlerAbnormal(p);
                    break;
                case Class.Ninja:
                    Ninja.CheckFocus(p);
                    break;
                case Class.Sorcerer:
                    Sorcerer.CheckBuff(p);
                    break;
                case Class.Reaper:
                    Reaper.CheckBuff(p);
                    break;
                case Class.Slayer:
                    Slayer.CheckBuff(p);
                    break;
                case Class.Berserker:
                    Berserker.CheckBuff(p);
                    break;

            }
        }
        public static void HandleAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            AbnormalityManager.BeginAbnormality(p.AbnormalityId, p.TargetId, p.Duration, p.Stacks);
            if (!SettingsManager.ClassWindowSettings.Enabled) return;
            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Warrior:
                    Warrior.CheckBuff(p);
                    break;
                case Class.Archer:
                    Archer.CheckFocus(p);
                    Archer.CheckFocusX(p);
                    Archer.CheckSniperEye(p);
                    break;
                case Class.Lancer:
                    Lancer.CheckLineHeld(p);
                    break;
                case Class.Priest:
                    Priest.CheckBuff(p);
                    break;
                case Class.Mystic:
                    Mystic.CheckBuff(p);
                    break;
                case Class.Sorcerer:
                    Sorcerer.CheckBuff(p);
                    break;
                case Class.Reaper:
                    Reaper.CheckBuff(p);
                    break;
                case Class.Slayer:
                    Slayer.CheckBuff(p);
                    break;
                case Class.Berserker:
                    Berserker.CheckBuff(p);
                    break;
                case Class.Brawler:
                    Brawler.CheckBrawlerAbnormal(p);
                    break;


            }
        }
        public static void HandleAbnormalityEnd(S_ABNORMALITY_END p)
        {
            AbnormalityManager.EndAbnormality(p.TargetId, p.AbnormalityId);
            if (!SettingsManager.ClassWindowSettings.Enabled) return;

            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Archer:
                    Archer.CheckFocusEnd(p);
                    Archer.CheckSniperEyeEnd(p);
                    break;
                case Class.Warrior:
                    Warrior.CheckBuffEnd(p);
                    break;
                case Class.Lancer:
                    Lancer.CheckLineHeldEnd(p);
                    Lancer.CheckArushEnd(p);
                    Lancer.CheckGshoutEnd(p);
                    break;
                case Class.Mystic:
                    Mystic.CheckBuffEnd(p);
                    break;
                case Class.Brawler:
                    Brawler.CheckBrawlerAbnormalEnd(p);
                    break;
                case Class.Ninja:
                    Ninja.CheckFocusEnd(p);
                    break;
                case Class.Priest:
                    Priest.CheckBuffEnd(p);
                    break;
                case Class.Sorcerer:
                    Sorcerer.CheckBuffEnd(p);
                    break;
                case Class.Reaper:
                    Reaper.CheckBuffEnd(p);
                    break;
                case Class.Slayer:
                    Slayer.CheckBuffEnd(p);
                    break;
                case Class.Berserker:
                    Berserker.CheckBuffEnd(p);
                    break;



            }
        }

        public static void HandlePlayerLocation(C_PLAYER_LOCATION p)
        {
            if (BossGageWindowViewModel.Instance.CurrentHHphase == HarrowholdPhase.Phase1)
            {
                BossGageWindowViewModel.Instance.SelectDragon(EntitiesManager.CheckCurrentDragon(new System.Windows.Point(p.X, p.Y)));

            }
        }
        public static void HandlePartyMemberList(S_PARTY_MEMBER_LIST p)
        {
            var notifyLfg = GroupWindowViewModel.Instance.Members.Count == 0;

            GroupWindowViewModel.Instance.SetRaid(p.Raid);

            foreach (var user in p.Members)
                GroupWindowViewModel.Instance.AddOrUpdateMember(user);

            if (notifyLfg && WindowManager.LfgListWindow != null && WindowManager.LfgListWindow.VM != null) WindowManager.LfgListWindow.VM.NotifyMyLfg();
            if (Proxy.IsConnected)
            {
                Proxy.RequestCandidates();
                if(WindowManager.LfgListWindow != null) if(WindowManager.LfgListWindow.IsVisible) Proxy.RequestLfgList();
            }
        }
        public static void HandlePartyMemberLeave(S_LEAVE_PARTY_MEMBER p)
        {
            GroupWindowViewModel.Instance.RemoveMember(p.PlayerId, p.ServerId);
        }
        public static void HandlePartyMemberLogout(S_LOGOUT_PARTY_MEMBER p)
        {
            GroupWindowViewModel.Instance.LogoutMember(p.PlayerId, p.ServerId);
            GroupWindowViewModel.Instance.ClearAbnormality(p.PlayerId, p.ServerId);

        }
        public static void HandlePartyMemberKick(S_BAN_PARTY_MEMBER p)
        {
            GroupWindowViewModel.Instance.RemoveMember(p.PlayerId, p.ServerId, true);
        }
        public static void HandlePartyMemberHp(S_PARTY_MEMBER_CHANGE_HP p)
        {
            GroupWindowViewModel.Instance.UpdateMemberHp(p.PlayerId, p.ServerId, p.CurrentHP, p.MaxHP);
        }
        public static void HandlePartyMemberMp(S_PARTY_MEMBER_CHANGE_MP p)
        {
            GroupWindowViewModel.Instance.UpdateMemberMp(p.PlayerId, p.ServerId, p.CurrentMP, p.MaxMP);
        }
        public static void HandlePartyMemberStats(S_PARTY_MEMBER_STAT_UPDATE p)
        {
            GroupWindowViewModel.Instance.UpdateMember(p);
        }
        public static void HandleLeaveParty(S_LEAVE_PARTY x)
        {
            GroupWindowViewModel.Instance.ClearAll();
        }
        internal static void HandleKicked(S_BAN_PARTY x)
        {
            GroupWindowViewModel.Instance.ClearAll();
        }

        public static void HandleReadyCheck(S_CHECK_TO_READY_PARTY p)
        {
            foreach (var item in p.Party)
            {
                GroupWindowViewModel.Instance.SetReadyStatus(item);
            }
        }
        public static void HandleReadyCheckFin(S_CHECK_TO_READY_PARTY_FIN x)
        {
            GroupWindowViewModel.Instance.EndReadyCheck();
        }

        public static void HandlePartyMemberInfo(S_PARTY_MEMBER_INFO packet)
        {
            ChatWindowManager.Instance.UpdateLfgMembers(packet);
            if (!SettingsManager.LfgEnabled) return;

            var lfg = WindowManager.LfgListWindow.VM.Listings.FirstOrDefault(listing => listing.LeaderId == packet.Id || packet.Members.Any(member => member.PlayerId == listing.LeaderId));
            if (lfg == null) return;
            //lfg.Players.Clear();
            packet.Members.ForEach(member =>
            {
                if (lfg.Players.Any(toFind => toFind.PlayerId == member.PlayerId))
                {
                    var target = lfg.Players.FirstOrDefault(player => player.PlayerId == member.PlayerId);
                    if (target == null) return;
                    target.IsLeader = member.IsLeader;
                    target.Online = member.Online;
                }
                else lfg.Players.Add(member);
            });
            var toDelete = new List<uint>();
            lfg.Players.ToList().ForEach(player =>
            {
                if (packet.Members.All(newMember => newMember.PlayerId != player.PlayerId)) toDelete.Add(player.PlayerId);
                toDelete.ForEach(targetId => lfg.Players.Remove(lfg.Players.FirstOrDefault(playerToRemove => playerToRemove.PlayerId == targetId)));
            });
            lfg.LeaderId = packet.Id;
            var leader = lfg.Players.FirstOrDefault(u => u.IsLeader);
            if (leader != null) lfg.LeaderName = leader.Name;
            if (WindowManager.LfgListWindow.VM._lastClicked != null && WindowManager.LfgListWindow.VM._lastClicked.LeaderId == lfg.LeaderId) lfg.IsExpanded = true;
            lfg.PlayerCount = packet.Members.Count;
            WindowManager.LfgListWindow.VM.NotifyMyLfg();
        }

        public static void HandleInventory(S_INVEN x)
        {
            if (!x.IsOpen) return;
            if (x.First && x.More) return;
            if (S_INVEN.Items == null) return;
            if (InfoWindowViewModel.Instance.CurrentCharacter == null) return;
            InfoWindowViewModel.Instance.CurrentCharacter.ClearGear();
            foreach (var tuple in S_INVEN.Items)
            {
                if (InventoryManager.TryParseGear(tuple.Item1, out var parsedPiece))
                {
                    var i = new GearItem(tuple.Item1, parsedPiece.Item1, parsedPiece.Item2, tuple.Item2, tuple.Item3);
                    Console.WriteLine($"Item: {i}");
                    InfoWindowViewModel.Instance.CurrentCharacter.Gear.Add(i);
                }
            }
            InfoWindowViewModel.Instance.SelectCharacter(InfoWindowViewModel.Instance.SelectedCharacter);
            InfoWindowViewModel.Instance.CurrentCharacter.ElleonMarks = S_INVEN.ElleonMarks;
            GroupWindowViewModel.Instance.UpdateMyGear();
            //88273 - 88285 L weapons
            //88286 - 88298 L armors
            //88299 - 88301 L gloves
            //88302 - 88304 L boots
            //88305 L belt

            //88306 - 88318 M weapons
            //88319 - 88331 M armors
            //88332 - 88334 M gloves
            //88335 - 88337 M boots
            //88338 M belt

            //88339 - 88351 H weapons
            //88352 - 88364 H armors
            //88365 - 88367 H gloves
            //88368 - 88370 H boots
            //88371 H belt

            //88372 - 88384 T weapons
            //88385 - 88397 T armors
            //88398 - 88400 T gloves
            //88401 - 88403 T boots
            //88404 T belt

            //88405 - 88407 L crit  set (neck/earr/ring)
            //88408 - 88410 L power set
            //88411 L circlet

            //88412 - 88414 M crit  set (neck/earr/ring)
            //88415 - 88417 M power set
            //88418 M circlet

            //88419 - 88421 H crit  set (neck/earr/ring)
            //88422 - 88424 H power set
            //88425 H circlet

            //88426 - 88428 T crit  set (neck/earr/ring)
            //88429 - 88431 T power set
            //88432 T circlet

        }

        public static void HandlePartyMemberIntervalPosUpdate(S_PARTY_MEMBER_INTERVAL_POS_UPDATE sPartyMemberIntervalPosUpdate)
        {
            GroupWindowViewModel.Instance.UpdateMemberLocation(sPartyMemberIntervalPosUpdate);
        }

        public static void HandleShieldDamageAbsorb(S_ABNORMALITY_DAMAGE_ABSORB sAbnormalityDamageAbsorb)
        {
            if (sAbnormalityDamageAbsorb.Target == SessionManager.CurrentPlayer.EntityId)
            {
                SessionManager.SetPlayerShield(sAbnormalityDamageAbsorb.Damage);
                return;
            }
            if (BossGageWindowViewModel.Instance.NpcList.Any(x => x.EntityId == sAbnormalityDamageAbsorb.Target))
            {
                BossGageWindowViewModel.Instance.UpdateShield(sAbnormalityDamageAbsorb.Target,
                    sAbnormalityDamageAbsorb.Damage);
            }
        }

        public static void HandleImageData(S_IMAGE_DATA sImageData)
        {


        }

        public static void HandleUserGuildLogo(S_GET_USER_GUILD_LOGO sGetUserGuildLogo)
        {
            if (S_IMAGE_DATA.Database.ContainsKey(sGetUserGuildLogo.GuildId)) return;
            S_IMAGE_DATA.Database.Add(sGetUserGuildLogo.GuildId, sGetUserGuildLogo.GuildLogo);
        }

        public static void HandleGpkData(string data)
        {
            const string chatModeCmd = ":tcc-chatMode:";
            const string uiModeCmd = ":tcc-uiMode:";
            const string unkString = "Unknown command ";
            data = data.Replace(unkString, "").Replace("\"", "").Replace(".", "");
            if (data.StartsWith(chatModeCmd))
            {
                var chatMode = data.Replace(chatModeCmd, "");
                SessionManager.InGameChatOpen = chatMode == "1"; //too lazy
            }
            else if (data.StartsWith(uiModeCmd))
            {
                var uiMode = data.Replace(uiModeCmd, "");
                SessionManager.InGameUiOn = uiMode == "1"; //too lazy
            }
        }

        public static void HandleApplicantsList(S_SHOW_CANDIDATE_LIST p)
        {
            if (WindowManager.LfgListWindow.VM == null) return;
            if (WindowManager.LfgListWindow.VM.MyLfg == null) return;
            var dest = WindowManager.LfgListWindow.VM.MyLfg.Applicants;
            //TODO refactoring: method that does this "merge" thing
            foreach (var applicant in p.Candidates)
            {
                if (dest.All(x => x.PlayerId != applicant.PlayerId)) dest.Add(applicant);
            }

            var toRemove = new List<User>();
            foreach (var user in dest)
            {
                if (p.Candidates.All(x => x.PlayerId != user.PlayerId)) toRemove.Add(user);
            }
            toRemove.ForEach(r => dest.Remove(r));
        }
    }
}
