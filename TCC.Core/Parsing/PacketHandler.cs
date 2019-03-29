using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Data.Pc;
using TCC.Parsing.Messages;
using TCC.Settings;
using TCC.Sniffing;
using TCC.Tera.Data;
using TCC.TeraCommon.Game.Services;
using TCC.ViewModels;
using TCC.Windows;
using S_GET_USER_GUILD_LOGO = TCC.TeraCommon.Game.Messages.Server.S_GET_USER_GUILD_LOGO;

namespace TCC.Parsing
{
    public static class PacketHandler
    {

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
            SessionManager.CurrentPlayer.CritFactor = p.BonusCritFactor;
            SessionManager.SetPlayerMaxHp(p.MaxHP);
            SessionManager.SetPlayerMaxMp(p.MaxMP);
            SessionManager.SetPlayerMaxSt(p.MaxST + p.BonusST);

            SessionManager.SetPlayerHp(p.CurrentHP);
            SessionManager.SetPlayerMp(p.CurrentMP);
            SessionManager.SetPlayerSt(p.CurrentST);

            WindowManager.Dashboard.VM.CurrentCharacter.ItemLevel = p.Ilvl;
            WindowManager.Dashboard.VM.CurrentCharacter.Level = p.Level;

            switch (SessionManager.CurrentPlayer.Class)
            {
                case Class.Warrior:
                    if (SettingsHolder.ClassWindowSettings.Enabled && WindowManager.ClassWindow.VM.CurrentManager is WarriorLayoutVM wm)
                        wm.EdgeCounter.Val = p.Edge;
                    break;
                case Class.Sorcerer:
                    SessionManager.SetSorcererElements(p.Fire, p.Ice, p.Arcane);
                    break;
            }

        }
        public static void HandleCreatureChangeHp(S_CREATURE_CHANGE_HP p)
        {
            if (p.Target.IsMe())
            {
                SessionManager.SetPlayerMaxHp(p.MaxHP);
                SessionManager.SetPlayerHp(p.CurrentHP);
            }
            else
            {
                EntityManager.UpdateNPC(p.Target, p.CurrentHP, p.MaxHP, p.Source);
            }
            ChatWindowManager.Instance.AddDamageReceivedMessage(p.Source, p.Target, p.Diff, p.MaxHP);
        }
        public static void HandlePlayerChangeMp(S_PLAYER_CHANGE_MP p)
        {
            if (!p.Target.IsMe()) return;
            SessionManager.SetPlayerMaxMp(p.MaxMP);
            SessionManager.SetPlayerMp(p.CurrentMP);
        }
        public static void HandlePlayerChangeStamina(S_PLAYER_CHANGE_STAMINA p)
        {
            SessionManager.SetPlayerSt(p.CurrentST);
        }
        public static void HandlePlayerChangeFlightEnergy(S_PLAYER_CHANGE_FLIGHT_ENERGY p)
        {
            SessionManager.SetPlayerFe(p.Energy);
        }
        public static void HandleUserStatusChanged(S_USER_STATUS p)
        {
            if (p.EntityId.IsMe()) SessionManager.Combat = p.IsInCombat;
        }

        public static void HandleBossGageInfo(S_BOSS_GAGE_INFO p)
        {
            EntityManager.UpdateNPC(p.EntityId, p.CurrentHP, p.MaxHP, (ushort)p.HuntingZoneId, (uint)p.TemplateId);
        }
        public static void HandleNpcStatusChanged(S_NPC_STATUS p)
        {
            EntityManager.SetNPCStatus(p.EntityId, p.IsEnraged, p.RemainingEnrageTime);
            if (p.Target == 0)
            {
                WindowManager.BossWindow.VM.UnsetBossTarget(p.EntityId);
            }
            var b = WindowManager.BossWindow.VM.NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == p.EntityId);
            //if (WindowManager.BossWindow.VM.CurrentHHphase == HarrowholdPhase.None) return;
            if (b != null /*&& b.IsBoss*/ && b.Visible)
            {
                WindowManager.GroupWindow.VM.SetAggro(p.Target);
                WindowManager.BossWindow.VM.SetBossAggro(p.EntityId, p.Target);

            }

        }
        public static void HandleUserEffect(S_USER_EFFECT p)
        {
            WindowManager.BossWindow.VM.SetBossAggro(p.Source, p.User);
            WindowManager.GroupWindow.VM.SetAggroCircle(p);
        }

        public static void HandleCharList(S_GET_USER_LIST p)
        {
            /*- Moved from HandleReturnToLobby -*/
            SessionManager.Logged = false;
            SkillManager.Clear();
            EntityManager.ClearNPC();
            WindowManager.GroupWindow.VM.ClearAll();
            WindowManager.Dashboard.VM.UpdateBuffs();
            SessionManager.CurrentPlayer.ClearAbnormalities();

            /*---------------------------------*/

            foreach (var item in p.CharacterList)
            {
                var ch = WindowManager.Dashboard.VM.Characters.FirstOrDefault(x => x.Id == item.Id);
                if (ch != null)
                {
                    ch.Name = item.Name;
                    ch.Laurel = item.Laurel;
                    ch.Position = item.Position;
                    ch.GuildName = item.GuildName;
                    ch.Level = item.Level;
                    ch.LastLocation = item.LastLocation;
                    ch.LastOnline = item.LastOnline;
                    ch.ServerName = SessionManager.Server.Name;
                }
                else
                {
                    WindowManager.Dashboard.VM.Characters.Add(item);
                }
            }

            WindowManager.Dashboard.VM.SaveCharacters();

        }
        public static void HandleLogin(S_LOGIN p)
        {
            SessionManager.CurrentPlayer.Class = p.CharacterClass;
            WindowManager.ReloadPositions();
            //S_IMAGE_DATA.LoadCachedImages(); //TODO: refactor this thing
            if (SettingsHolder.ClassWindowSettings.Enabled) WindowManager.ClassWindow.VM.CurrentClass = p.CharacterClass;
            AbnormalityManager.SetAbnormalityTracker(p.CharacterClass);
            SessionManager.Server = BasicTeraData.Instance.Servers.GetServer(p.ServerId);
            App.SendUsageStat();
            SettingsHolder.LastLanguage = SessionManager.Language;
            TimeManager.Instance.SetServerTimeZone(SettingsHolder.LastLanguage);
            TimeManager.Instance.SetGuildBamTime(false);
            SessionManager.InitDatabases(SettingsHolder.LastLanguage);
            SkillManager.Clear();
            WindowManager.CooldownWindow.VM.LoadSkills(p.CharacterClass);
            WindowManager.FloatingButton.SetMoongourdButtonVisibility();
            EntityManager.ClearNPC();
            WindowManager.GroupWindow.VM.ClearAll();
            ChatWindowManager.Instance.BlockedUsers.Clear();
            SessionManager.CurrentPlayer.ClearAbnormalities();

            SessionManager.LoadingScreen = true;
            SessionManager.Logged = true;
            SessionManager.Encounter = false;
            MessageFactory.Update();
            SessionManager.CurrentPlayer.EntityId = p.EntityId;
            SessionManager.CurrentPlayer.PlayerId = p.PlayerId;
            SessionManager.CurrentPlayer.ServerId = p.ServerId;
            SessionManager.CurrentPlayer.Name = p.Name;
            SessionManager.CurrentPlayer.Level = p.Level;
            SessionManager.SetPlayerLaurel(SessionManager.CurrentPlayer);
            WindowManager.Dashboard.VM.SetLoggedIn(p.PlayerId);
            SessionManager.GuildMembersNames.Clear();

            WindowManager.LfgListWindow.VM.EnqueueListRequest();
            //if (Settings.Settings.LastRegion == "NA")
            //    Task.Delay(20000).ContinueWith(t => ChatWindowManager.Instance.AddTccMessage(App.ThankYou_mEME));

        }

        internal static void HandleLfgList(S_SHOW_PARTY_MATCH_INFO x)
        {
            if (!SettingsHolder.LfgEnabled) return;
            if (WindowManager.LfgListWindow == null) return;
            if (WindowManager.LfgListWindow.VM == null) return;
            if (!x.IsLast) return;

            if (S_SHOW_PARTY_MATCH_INFO.Listings.Count == 0)
            {
                WindowManager.LfgListWindow.VM.NotifyMyLfg();
                WindowManager.LfgListWindow.ShowWindow();
                return;
            }
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
                        WindowManager.LfgListWindow.VM.EnqueueRequest(l.LeaderId);
                    }
                }
                else
                {
                    WindowManager.LfgListWindow.VM.Listings.Add(l);
                    WindowManager.LfgListWindow.VM.EnqueueRequest(l.LeaderId);
                }
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

        /*
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
        */
        public static void HandleReturnToLobby(S_RETURN_TO_LOBBY p)
        {
            SessionManager.Logged = false;
            WindowManager.LfgListWindow.VM.ForceStopPublicize();
            WindowManager.Dashboard.VM.UpdateBuffs();
            SessionManager.CurrentPlayer.ClearAbnormalities();
            SkillManager.Clear();
            EntityManager.ClearNPC();
            WindowManager.GroupWindow.VM.ClearAll();
            WindowManager.ClassWindow.VM.CurrentClass = Class.None;
        }


        public static void HandleLoadTopo(S_LOAD_TOPO x)
        {
            SessionManager.LoadingScreen = true;
            SessionManager.Encounter = false;
            WindowManager.GroupWindow.VM.ClearAllAbnormalities();
            WindowManager.GroupWindow.VM.SetAggro(0);
            SessionManager.CurrentPlayer.ClearAbnormalities();
            WindowManager.BossWindow.VM.CurrentHHphase = HarrowholdPhase.None;
            WindowManager.BossWindow.VM.ClearGuildTowers();
            SessionManager.CivilUnrestZone = x.Zone == 152;
            SessionManager.IsInDungeon = x.Zone >= 8999; // from Salty's server-exposer

            if (SettingsHolder.CivilUnrestWindowSettings.Enabled) WindowManager.CivilUnrestWindow.VM.NotifyTeleported();
        }

        public static void HandleStartRoll(S_ASK_BIDDING_RARE_ITEM x)
        {
            WindowManager.GroupWindow.VM.StartRoll();
        }
        public static void HandleRollResult(S_RESULT_BIDDING_DICE_THROW x)
        {
            if (!WindowManager.GroupWindow.VM.Rolling) WindowManager.GroupWindow.VM.StartRoll();
            WindowManager.GroupWindow.VM.SetRoll(x.EntityId, x.RollResult);
        }
        public static void HandleEndRoll(S_RESULT_ITEM_BIDDING x)
        {
            WindowManager.GroupWindow.VM.EndRoll();
        }

        public static void HandleSpawnMe(S_SPAWN_ME p)
        {
            EntityManager.ClearNPC();
            FlyingGuardianDataProvider.Stacks = 0;
            FlyingGuardianDataProvider.StackType = FlightStackType.None;
            FlyingGuardianDataProvider.InvokeProgressChanged();
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
            EntityManager.CheckHarrowholdMode(p.HuntingZoneId, p.TemplateId);
            EntityManager.SpawnNPC(p.HuntingZoneId, p.TemplateId, p.EntityId, false, p.Villager, p.RemainingEnrageTime);
        }
        public static void HandleSpawnUser(S_SPAWN_USER p)
        {
            switch (p.Name)
            {
                case "Foglio":
                case "Foglietto":
                case "Foglia":
                case "Myvia":
                case "Foglietta.Blu":
                case "Foglia.Trancer":
                case "Folyria":
                case "Folyvia":
                case "Fogliolina":
                case "Folyemi":
                case "Foiya":
                case "Fogliarya":
                    if (p.ServerId != 27) break;
                    if (SessionManager.CivilUnrestZone) break;
                    EntityManager.FoglioEid = p.EntityId;
                    var ab = SessionManager.CurrentDatabase.AbnormalityDatabase.Abnormalities[10241024];
                    AbnormalityManager.BeginAbnormality(ab.Id, SessionManager.CurrentPlayer.EntityId, 0, int.MaxValue, 1);
                    var sysMsg = SessionManager.CurrentDatabase.SystemMessagesDatabase.Messages["SMT_BATTLE_BUFF_DEBUFF"];
                    var msg = $"@0\vAbnormalName\v{ab.Name}";
                    SystemMessagesProcessor.AnalyzeMessage(msg, sysMsg, "SMT_BATTLE_BUFF_DEBUFF");
                    break;
            }
            EntityManager.SpawnUser(p.EntityId, p.Name);
            if (!WindowManager.GroupWindow.VM.Exists(p.EntityId)) return;

            WindowManager.GroupWindow.VM.UpdateMemberGear(p);
        }

        public static void HandlePartyMemberBuffUpdate(S_PARTY_MEMBER_BUFF_UPDATE x)
        {
            foreach (var buff in x.Abnormals)
            {
                AbnormalityManager.UpdatePartyMemberAbnormality(x.PlayerId, x.ServerId, buff.Id, buff.Duration, buff.Stacks);
            }
        }
        public static void HandlePartyMemberAbnormalAdd(S_PARTY_MEMBER_ABNORMAL_ADD x)
        {
            AbnormalityManager.UpdatePartyMemberAbnormality(x.PlayerId, x.ServerId, x.Id, x.Duration, x.Stacks);
        }
        public static void HandlePartyMemberAbnormalDel(S_PARTY_MEMBER_ABNORMAL_DEL x)
        {
            AbnormalityManager.EndPartyMemberAbnormality(x.PlayerId, x.ServerId, x.Id);
        }

        public static void HandleRunemark(S_WEAK_POINT x)
        {
            if (SessionManager.CurrentPlayer.Class != Class.Valkyrie) return;
            if (WindowManager.ClassWindow.VM.CurrentManager.GetType() != typeof(ValkyrieLayoutVM)) return;
            ((ValkyrieLayoutVM)WindowManager.ClassWindow.VM.CurrentManager).RunemarksCounter.Val = x.TotalRunemarks;
        }


        public static void HandleSkillResult(S_EACH_SKILL_RESULT x)
        {
            if (x.Skill != 63005521) return;
            var name = EntityManager.GetUserName(x.Source);
            ChatWindowManager.Instance.AddTccMessage($"Dragon Firework spawned by {name}");
            //bool sourceInParty = WindowManager.GroupWindow.VM.UserExists(x.Source);
            //bool targetInParty = WindowManager.GroupWindow.VM.UserExists(x.Target);
            //if (x.Target == x.Source) return;
            //if (sourceInParty && targetInParty) return;
            //if (sourceInParty || targetInParty) WindowManager.SkillsEnded = false;
            //if (x.Source == SessionManager.CurrentPlayer.EntityId) WindowManager.SkillsEnded = false;
            //if (x.Source == SessionManager.CurrentPlayer.EntityId) return;
            //WindowManager.BossWindow.VM.UpdateShield(x.Target, x.Damage);
            //if (x.Type != 1) return;
            //if (x.Source == SessionManager.CurrentPlayer.EntityId) return;
            //WindowManager.BossWindow.VM.UpdateBySkillResult(x.Target, x.Damage);
        }


        public static void HandleChangeLeader(S_CHANGE_PARTY_MANAGER x)
        {
            WindowManager.GroupWindow.VM.SetNewLeader(x.EntityId, x.Name);
        }

        public static void HandlePartyMemberAbnormalClear(S_PARTY_MEMBER_ABNORMAL_CLEAR x)
        {
            WindowManager.GroupWindow.VM.ClearAbnormality(x.PlayerId, x.ServerId);
        }
        public static void HandlePartyMemberAbnormalRefresh(S_PARTY_MEMBER_ABNORMAL_REFRESH x)
        {
            AbnormalityManager.UpdatePartyMemberAbnormality(x.PlayerId, x.ServerId, x.Id, x.Duration, x.Stacks);
        }

        public static void HandleChat(S_CHAT x)
        {
            Console.WriteLine($"{x.AuthorId} - {x.AuthorName}");
            if ((x.AuthorName == "Foglio" || x.AuthorName == "Myvia" || x.AuthorName == "Foglia" || x.AuthorName == "Foglia.Trancer" || x.AuthorName == "Folyemi" ||
                x.AuthorName == "Folyria" || x.AuthorName == "Foglietto") && x.Channel == ChatChannel.Greet) WindowManager.FloatingButton.NotifyExtended("TCC", "Nice TCC :lul:", NotificationType.Warning);
            //Log.CW(x.Message);
            ChatWindowManager.Instance.AddChatMessage(new ChatMessage(x.Channel, x.AuthorName, x.Message));
        }

        internal static void HandleCreatureLife(S_CREATURE_LIFE p)
        {
            if (p.Target.IsMe())
            {
                SessionManager.CurrentPlayer.IsAlive = p.Alive;
            }
        }

        public static void HandlePrivateChat(S_PRIVATE_CHAT x)
        {
            var i = ChatWindowManager.Instance.PrivateChannels.FirstOrDefault(y => y.Id == x.Channel).Index;
            var ch = (ChatChannel)(ChatWindowManager.Instance.PrivateChannels[i].Index + 11);

            ChatWindowManager.Instance.AddChatMessage(new ChatMessage(ch, x.AuthorName, x.Message));
        }

        public static void HandleProxyOutput(string author, uint channel, string message)
        {
            //if (message.IndexOf('[') != -1 && message.IndexOf(']') != -1)
            //{
            //    //    author = message.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries)[1];
            //    //   message = message.Replace('[' + author + ']', "");
            //}
            if (author == "undefined") author = "System";
            if (!ChatWindowManager.Instance.PrivateChannels.Any(x => x.Id == channel && x.Joined))
                ChatWindowManager.Instance.CachePrivateMessage(channel, author, message);
            else
                ChatWindowManager.Instance.AddChatMessage(
                    new ChatMessage((ChatChannel)ChatWindowManager.Instance.PrivateChannels.FirstOrDefault(x =>
                                        x.Id == channel && x.Joined).Index + 11, author, message));
        }

        public static void HandleActionStage(S_ACTION_STAGE x)
        {
            var name = EntityManager.GetUserName(x.GameId);
            if (x.Skill == 63005521) ChatWindowManager.Instance.AddTccMessage($"Dragon Firework spawned by {name}");
        }

        internal static void HandleFriendIntoArea(S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA x)
        {
            var friend = ChatWindowManager.Instance.Friends.FirstOrDefault(f => f.PlayerId == x.PlayerId);
            if (friend == null) return;
            const string opcode = "SMT_FRIEND_WALK_INTO_SAME_AREA";
            var areaName = x.SectionId.ToString();
            try
            {
                areaName = SessionManager.CurrentDatabase.RegionsDatabase.Names[SessionManager.CurrentDatabase.MapDatabase.Worlds[x.WorldId].Guards[x.GuardId].Sections[x.SectionId].NameId];
            }
            catch (Exception)
            {
                // ignored
            }
            var srvMsg = "@0\vUserName\v" + friend.Name + "\vAreaName\v" + areaName;
            SessionManager.CurrentDatabase.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);

            SystemMessagesProcessor.AnalyzeMessage(srvMsg, m, opcode);
        }

        public static void HandleJoinPrivateChat(S_JOIN_PRIVATE_CHANNEL x)
        {
            ChatWindowManager.Instance.JoinPrivateChannel(x.Id, x.Index, x.Name);
        }

        internal static void HandleGuildTowerInfo(S_GUILD_TOWER_INFO x)
        {
            WindowManager.BossWindow.VM.AddGuildTower(x.TowerId, x.GuildName, x.GuildId);
        }

        public static void HandleLeavePrivateChat(S_LEAVE_PRIVATE_CHANNEL x)
        {
            var i = ChatWindowManager.Instance.PrivateChannels.FirstOrDefault(c => c.Id == x.Id).Index;
            ChatWindowManager.Instance.PrivateChannels[i].Joined = false;
        }

        internal static void HandleDungeonCooltimeList(S_DUNGEON_COOL_TIME_LIST x)
        {
            WindowManager.Dashboard.VM.SetDungeons(x.DungeonCooldowns);
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
            if (WindowManager.Dashboard.VM.CurrentCharacter == null) return;
            WindowManager.Dashboard.VM.CurrentCharacter.ClaimedGuardianQuests = x.Claimed;
            WindowManager.Dashboard.VM.CurrentCharacter.ClearedGuardianQuests = x.Cleared;
            //InfoWindowViewModel.Instance.CurrentCharacter.MaxGuardianQuests = x.MaxPoints;
        }

        internal static void HandleVanguardReceived(S_AVAILABLE_EVENT_MATCHING_LIST x)
        {
            WindowManager.Dashboard.VM.SetVanguard(x);
        }

        internal static void HandleDungeonClears(S_DUNGEON_CLEAR_COUNT_LIST x)
        {
            if (x.Failed) return;
            if (x.PlayerId != SessionManager.CurrentPlayer.PlayerId) return;
            foreach (var dg in x.DungeonClears)
            {
                WindowManager.Dashboard.VM.CurrentCharacter.SetDungeonClears(dg.Key, dg.Value);
            }
        }

        internal static void HandleDungeonMessage(S_DUNGEON_EVENT_MESSAGE p)
        {
            if (p.MessageId == 9950045)
            {
                //shield start
                foreach (var item in WindowManager.BossWindow.VM.NpcList.Where(x => x.IsPhase1Dragon))
                {
                    item.StartShield();
                }
            }
            else if (p.MessageId == 9950113)
            {
                //aquadrax interrupted
                WindowManager.BossWindow.VM.NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1103).BreakShield();
            }
            else if (p.MessageId == 9950114)
            {
                //umbradrax interrupted
                WindowManager.BossWindow.VM.NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1102).BreakShield();

            }
            else if (p.MessageId == 9950115)
            {
                //ignidrax interrupted
                WindowManager.BossWindow.VM.NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1100).BreakShield();

            }
            else if (p.MessageId == 9950116)
            {
                //terradrax interrupted
                WindowManager.BossWindow.VM.NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1101).BreakShield();

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
            if (!SettingsHolder.LfgEnabled) return;
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
            if (SessionManager.CurrentDatabase.SystemMessagesDatabase.Messages.TryGetValue(opcodeName, out var m))
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
            ChatWindowManager.Instance.AddChatMessage(new LfgMessage(x.Id, x.Name, x.Message));
        }

        public static void HandleSystemMessage(S_SYSTEM_MESSAGE x)
        {
            try
            {
                var msg = x.Message.Split('\v');
                var opcode = ushort.Parse(msg[0].Substring(1));
                var opcodeName = PacketAnalyzer.Factory.SystemMessageNamer.GetName(opcode);

                if (SessionManager.CurrentDatabase.SystemMessagesDatabase.Messages.TryGetValue(opcodeName, out var m))
                {
                    SystemMessagesProcessor.AnalyzeMessage(x.Message, m, opcodeName);
                }

            }
            catch (Exception)
            {
                //File.AppendAllText("chat-errors.log", x.Message + "\n");
                Log.F($"Failed to parse system message: {x.Message}");
            }
        }

        internal static void HandleAccomplishAchievement(S_ACCOMPLISH_ACHIEVEMENT x)
        {
            //if (!SessionManager.CurrentDatabase.AchievementDatabase.Achievements.ContainsKey(x.AchievementId)) return;
            if (!SessionManager.CurrentDatabase.SystemMessagesDatabase.Messages.TryGetValue("SMT_ACHIEVEMENT_GRADE0_CLEAR_MESSAGE", out var m)) return;

            var sysMsg = new ChatMessage("@0\vAchievementName\v@achievement:" + x.AchievementId, m, (ChatChannel)m.ChatChannel);
            ChatWindowManager.Instance.AddChatMessage(sysMsg);
        }

        public static void HandleBlockList(S_USER_BLOCK_LIST x)
        {
            x.BlockedUsers.ForEach(u =>
            {
                if (ChatWindowManager.Instance.BlockedUsers.Contains(u)) return;
                ChatWindowManager.Instance.BlockedUsers.Add(u);
            });
        }

        internal static void HandleFriendList(S_FRIEND_LIST x)
        {
            ChatWindowManager.Instance.Friends = x.Friends;
        }

        internal static void HandleAnswerInteractive(S_ANSWER_INTERACTIVE x)
        {
            SessionManager.CurrentDatabase.MonsterDatabase.TryGetMonster(x.Model, 0, out var m);
            WindowManager.FloatingButton.TooltipInfo.Name = x.Name;
            WindowManager.FloatingButton.TooltipInfo.Info = m.Name;
            WindowManager.FloatingButton.TooltipInfo.Level = (int)x.Level;
            WindowManager.FloatingButton.TooltipInfo.SetInfo(x.Model);
            if (x.Name == SessionManager.CurrentPlayer.Name)
            {
                WindowManager.FloatingButton.TooltipInfo.ShowGuildInvite = false;
                WindowManager.FloatingButton.TooltipInfo.ShowPartyInvite = false;
            }
            else
            {
                WindowManager.FloatingButton.TooltipInfo.ShowGuildInvite = !x.HasGuild;
                WindowManager.FloatingButton.TooltipInfo.ShowPartyInvite = !x.HasParty;
            }
            if (!ProxyInterop.Proxy.IsConnected) return;
            WindowManager.FloatingButton.OpenPlayerMenu();
        }

        internal static void HandleCrestMessage(S_CREST_MESSAGE x)
        {
            if (x.Type != 6) return;
            SkillManager.ResetSkill(x.SkillId);
        }

        internal static void HandleSystemMessageLoot(S_SYSTEM_MESSAGE_LOOT_ITEM x)
        {
            try
            {
                var msg = x.SysMessage.Split('\v');
                var opcode = ushort.Parse(msg[0].Substring(1));
                var opcodeName = PacketAnalyzer.Factory.SystemMessageNamer.GetName(opcode);

                if (SessionManager.CurrentDatabase.SystemMessagesDatabase.Messages.TryGetValue(opcodeName, out var m))
                {
                    var sysMsg = new ChatMessage(x.SysMessage, m, (ChatChannel)m.ChatChannel);
                    ChatWindowManager.Instance.AddChatMessage(sysMsg);
                }

            }
            catch (Exception)
            {
                Log.F($"Failed to parse sysmsg: {x.SysMessage}");
            }
        }

        public static void HandleDespawnNpc(S_DESPAWN_NPC p)
        {
            EntityManager.DespawnNPC(p.Target, p.Type);
        }
        public static void HandleDespawnUser(S_DESPAWN_USER p)
        {
            if (p.EntityId == EntityManager.FoglioEid) AbnormalityManager.EndAbnormality(SessionManager.CurrentPlayer.EntityId, 10241024);
            EntityManager.DepawnUser(p.EntityId);
        }

        public static void HandleAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            AbnormalityManager.BeginAbnormality(p.AbnormalityId, p.TargetId, p.CasterId, p.Duration, p.Stacks);
            if (p.TargetId.IsMe()) FlyingGuardianDataProvider.HandleAbnormal(p);

            if (!SettingsHolder.ClassWindowSettings.Enabled) return;
            AbnormalityManager.CurrentAbnormalityTracker?.CheckAbnormality(p);
        }
        public static void HandleAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            AbnormalityManager.BeginAbnormality(p.AbnormalityId, p.TargetId, p.TargetId, p.Duration, p.Stacks);
            if (p.TargetId.IsMe()) FlyingGuardianDataProvider.HandleAbnormal(p);

            if (!SettingsHolder.ClassWindowSettings.Enabled) return;
            AbnormalityManager.CurrentAbnormalityTracker?.CheckAbnormality(p);
        }
        public static void HandleAbnormalityEnd(S_ABNORMALITY_END p)
        {
            if (!AbnormalityManager.EndAbnormality(p.TargetId, p.AbnormalityId)) return;
            if (p.TargetId.IsMe()) FlyingGuardianDataProvider.HandleAbnormal(p);

            if (!SettingsHolder.ClassWindowSettings.Enabled) return;
            AbnormalityManager.CurrentAbnormalityTracker?.CheckAbnormality(p);
        }

        public static void HandlePlayerLocation(C_PLAYER_LOCATION p)
        {
            if (WindowManager.BossWindow.VM.CurrentHHphase != HarrowholdPhase.Phase1) return;
            WindowManager.BossWindow.VM.SelectDragon(p.X, p.Y);
        }
        public static void HandlePartyMemberList(S_PARTY_MEMBER_LIST p)
        {
            var notifyLfg = WindowManager.GroupWindow.VM.Members.Count == 0;

            WindowManager.GroupWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                WindowManager.GroupWindow.VM.SetRaid(p.Raid);
                p.Members.ForEach(WindowManager.GroupWindow.VM.AddOrUpdateMember);
            }));

            if (notifyLfg && WindowManager.LfgListWindow != null && WindowManager.LfgListWindow.VM != null) WindowManager.LfgListWindow.VM.NotifyMyLfg();
            if (!ProxyInterop.Proxy.IsConnected || !SettingsHolder.LfgEnabled || !SessionManager.InGameUiOn) return;
            ProxyInterop.Proxy.RequestCandidates();
            if (WindowManager.LfgListWindow == null || !WindowManager.LfgListWindow.IsVisible) return;
            ProxyInterop.Proxy.RequestLfgList();
        }
        public static void HandlePartyMemberLeave(S_LEAVE_PARTY_MEMBER p)
        {
            WindowManager.GroupWindow.VM.RemoveMember(p.PlayerId, p.ServerId);
        }
        public static void HandlePartyMemberLogout(S_LOGOUT_PARTY_MEMBER p)
        {
            WindowManager.GroupWindow.VM.LogoutMember(p.PlayerId, p.ServerId);
            WindowManager.GroupWindow.VM.ClearAbnormality(p.PlayerId, p.ServerId);

        }
        public static void HandlePartyMemberKick(S_BAN_PARTY_MEMBER p)
        {
            WindowManager.GroupWindow.VM.RemoveMember(p.PlayerId, p.ServerId, true);
        }
        public static void HandlePartyMemberHp(S_PARTY_MEMBER_CHANGE_HP p)
        {
            WindowManager.GroupWindow.VM.UpdateMemberHp(p.PlayerId, p.ServerId, p.CurrentHP, p.MaxHP);
        }
        public static void HandlePartyMemberMp(S_PARTY_MEMBER_CHANGE_MP p)
        {
            WindowManager.GroupWindow.VM.UpdateMemberMp(p.PlayerId, p.ServerId, p.CurrentMP, p.MaxMP);
        }
        public static void HandlePartyMemberStats(S_PARTY_MEMBER_STAT_UPDATE p)
        {
            WindowManager.GroupWindow.VM.UpdateMember(p);
        }
        public static void HandleLeaveParty(S_LEAVE_PARTY x)
        {
            WindowManager.GroupWindow.VM.ClearAll();
            if (SettingsHolder.LfgEnabled) WindowManager.LfgListWindow.VM.NotifyMyLfg();

        }
        public static void HandleKicked(S_BAN_PARTY x)
        {
            WindowManager.GroupWindow.VM.ClearAll();
            if (SettingsHolder.LfgEnabled) WindowManager.LfgListWindow.VM.NotifyMyLfg();

        }

        public static void HandleReadyCheck(S_CHECK_TO_READY_PARTY p)
        {
            WindowManager.GroupWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var member in p.Party)
                {
                    WindowManager.GroupWindow.VM.SetReadyStatus(member);
                }
            }));
        }
        public static void HandleReadyCheckFin(S_CHECK_TO_READY_PARTY_FIN x)
        {
            WindowManager.GroupWindow.VM.EndReadyCheck();
        }

        public static void HandlePartyMemberInfo(S_PARTY_MEMBER_INFO packet)
        {
            ChatWindowManager.Instance.UpdateLfgMembers(packet);
            if (!SettingsHolder.LfgEnabled) return;

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
                    target.Location = member.Location;
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
            if (WindowManager.LfgListWindow.VM.LastClicked != null && WindowManager.LfgListWindow.VM.LastClicked.LeaderId == lfg.LeaderId) lfg.IsExpanded = true;
            lfg.PlayerCount = packet.Members.Count;
            WindowManager.LfgListWindow.VM.NotifyMyLfg();
        }

        public static void HandleInventory(S_INVEN x)
        {
            //TODO: add gear again?
            if (x.Failed) return;
            WindowManager.Dashboard.VM.UpdateInventory(x.Items, x.First);
            //WindowManager.GroupWindow.VM.UpdateMyGear();

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
            WindowManager.GroupWindow.VM.UpdateMemberLocation(sPartyMemberIntervalPosUpdate);
        }

        // todo: add chat message too
        public static void HandleShieldDamageAbsorb(S_ABNORMALITY_DAMAGE_ABSORB p)
        {

            if (p.Target.IsMe())
                SessionManager.SetPlayerShield(p.Damage);
            else if (WindowManager.BossWindow.VM.NpcList.Any(x => x.EntityId == p.Target))
                WindowManager.BossWindow.VM.UpdateShield(p.Target, p.Damage);
        }

        public static void HandleImageData(S_IMAGE_DATA sImageData)
        {


        }

        public static void HandleUserGuildLogo(S_GET_USER_GUILD_LOGO sGetUserGuildLogo)
        {
            if (sGetUserGuildLogo.GuildLogo == null) return;
            S_IMAGE_DATA.Database[sGetUserGuildLogo.GuildId] = sGetUserGuildLogo.GuildLogo;

            if (!Directory.Exists("resources/images/guilds")) Directory.CreateDirectory("resources/images/guilds");
            try
            {
                sGetUserGuildLogo.GuildLogo.Save(
                    Path.Combine(App.ResourcesPath, $"images/guilds/guildlogo_{SessionManager.Server.ServerId}_{sGetUserGuildLogo.GuildId}_{0}.bmp"),
                    System.Drawing.Imaging.ImageFormat.Bmp
                    );
            }
            catch (Exception e)
            {
                Log.F($"Error while saving guild logo: {e}");
            }

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
                SessionManager.InGameChatOpen = chatMode == "1" || chatMode == "true"; //too lazy
            }
            else if (data.StartsWith(uiModeCmd))
            {
                var uiMode = data.Replace(uiModeCmd, "");
                SessionManager.InGameUiOn = uiMode == "1" || uiMode == "true"; //too lazy
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

        public static void HandleShowHp(S_SHOW_HP x)
        {
            WindowManager.BossWindow.VM.AddOrUpdateBoss(x.GameId, x.MaxHp, x.CurrentHp, false, HpChangeSource.CreatureChangeHp);
        }

        public static void HandleDestroyGuildTower(S_DESTROY_GUILD_TOWER p)
        {
            try
            {
                WindowManager.CivilUnrestWindow.VM.AddDestroyedGuildTower(p.SourceGuildId);
            }
            catch
            {
                // ignored
            }
        }

        public static void HandleCityWarMapInfo(S_REQUEST_CITY_WAR_MAP_INFO p)
        {
            try
            {
                p.Guilds.ToList().ForEach(x => WindowManager.CivilUnrestWindow.VM.AddGuild(x));
            }
            catch
            {
                // ignored
            }
        }
        public static void HandleCityWarMapInfoDetail(S_REQUEST_CITY_WAR_MAP_INFO_DETAIL p)
        {
            try
            {
                p.GuildDetails.ToList().ForEach(x => WindowManager.CivilUnrestWindow.VM.SetGuildName(x.Item1, x.Item2));
            }
            catch
            {
                // ignored
            }
        }

        public static void HandleViewWareEx(S_VIEW_WARE_EX p)
        {
            foreach (var page in S_VIEW_WARE_EX.Pages)
            {
                if (page.Index + 1 == S_VIEW_WARE_EX.Pages.Count) break;
                for (var i = page.Index + 1; i < 8; i++)
                {
                    var pg = S_VIEW_WARE_EX.Pages[(int)i];
                    foreach (var item in page.Items)
                    {
                        if (pg.Items.All(x => x.Id != item.Id)) continue;
                        var name = SessionManager.CurrentDatabase.ItemsDatabase.GetItemName((uint)item.Id);
                        Console.WriteLine($"Found duplicate of {name} [{item.Id}] (page {page.Index + 1}) in page {i + 1}");
                    }
                }
            }
        }

        public static void HandleGuardianOnEnter(S_FIELD_EVENT_ON_ENTER obj)
        {
            const string opcode = "SMT_FIELD_EVENT_ENTER";
            SessionManager.CurrentDatabase.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);
            SystemMessagesProcessor.AnalyzeMessage("", m, opcode);

            if (ProxyInterop.Proxy.IsConnected && ProxyInterop.Proxy.IsFpsUtilsAvailable && SettingsHolder.FpsAtGuardian)
            {
                ProxyInterop.Proxy.SendCommand($"fps mode 3");
            }
        }

        public static void HandleGuardianOnLeave(S_FIELD_EVENT_ON_LEAVE obj)
        {
            const string opcode = "SMT_FIELD_EVENT_LEAVE";
            SessionManager.CurrentDatabase.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);
            SystemMessagesProcessor.AnalyzeMessage("", m, opcode);

            if (ProxyInterop.Proxy.IsConnected && ProxyInterop.Proxy.IsFpsUtilsAvailable && SettingsHolder.FpsAtGuardian)
            {
                ProxyInterop.Proxy.SendCommand($"fps mode 1");
            }
        }

        public static void HandleUpdateNpcGuild(S_UPDATE_NPCGUILD p)
        {
            switch (p.Guild)
            {
                case NpcGuild.Vanguard:
                    WindowManager.Dashboard.VM.SetVanguardCredits(p.Credits);
                    break;
                case NpcGuild.Guardian:
                    WindowManager.Dashboard.VM.SetGuardianCredits(p.Credits);
                    break;
            }
        }

        public static void HandleNpcGuildList(S_NPCGUILD_LIST p)
        {
            if (!p.UserId.IsMe()) return;
            p.NpcGuildList.Keys.ToList().ForEach(k =>
            {
                switch (k)
                {
                    case (int)NpcGuild.Vanguard:
                        WindowManager.Dashboard.VM.SetVanguardCredits(p.NpcGuildList[k]);
                        break;
                    case (int)NpcGuild.Guardian:
                        WindowManager.Dashboard.VM.SetGuardianCredits(p.NpcGuildList[k]);
                        break;
                }
            });
        }

        public static void HandleNotifyGuildQuestUrgent(S_NOTIFY_GUILD_QUEST_URGENT p)
        {
            const string opcode = "SMT_GQUEST_URGENT_NOTIFY";
            SessionManager.CurrentDatabase.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);
            switch (p.Type)
            {
                case S_NOTIFY_GUILD_QUEST_URGENT.GuildBamQuestType.Announce:
                    var questName = p.QuestId == 0 ? "Defeat Guild BAM" : SessionManager.CurrentDatabase.GuildQuestDatabase.GuildQuests[p.QuestId].Title;
                    var zone = SessionManager.CurrentDatabase.RegionsDatabase.GetZoneName(p.ZoneId);
                    var name = SessionManager.CurrentDatabase.MonsterDatabase.GetName(p.TemplateId, p.ZoneId);
                    var msg = $"@0\vquestName\v{questName}\vnpcName\v{name}\vzoneName\v{zone}";
                    SystemMessagesProcessor.AnalyzeMessage(msg, m, opcode);
                    break;
                default:
                    return;
            }

        }

        public static void HandleChangeGuildChief(S_CHANGE_GUILD_CHIEF obj)
        {
            const string opcode = "SMT_GC_SYSMSG_GUILD_CHIEF_CHANGED";
            SessionManager.CurrentDatabase.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);
            SystemMessagesProcessor.AnalyzeMessage($"@0\vName\v{SessionManager.GetGuildMemberName(obj.PlayerId)}", m, opcode);
        }

        public static void HandleGuildMembersList(S_GUILD_MEMBER_LIST obj)
        {
            obj.GuildMembersList.ToList().ForEach(m => { SessionManager.GuildMembersNames[m.Key] = m.Value; });

        }

        public static void HandleCheckVersion(C_CHECK_VERSION p)
        {
            OpcodeDownloader.DownloadIfNotExist(p.Versions[0], Path.Combine(App.DataPath, "opcodes/"));
            if (!File.Exists(Path.Combine(App.DataPath, $"opcodes/protocol.{p.Versions[0]}.map")))
            {
                TccMessageBox.Show("Unknown client version: " + p.Versions[0], MessageBoxType.Error);
                App.CloseApp();
                return;
            }
            var opcNamer = new OpCodeNamer(Path.Combine(App.DataPath, $"opcodes/protocol.{p.Versions[0]}.map"));
            PacketAnalyzer.Factory = new MessageFactory(p.Versions[0], opcNamer); //SystemMessageNamer = new OpCodeNamer(Path.Combine(App.DataPath, $"opcodes/sysmsg.{PacketAnalyzer.Factory.ReleaseVersion}.map"))
            TeraSniffer.Instance.Connected = true;
        }
        public static void HandleLoginArbiter(C_LOGIN_ARBITER p)
        {
            SessionManager.CurrentAccountName = p.AccountName;
            if (OpcodeDownloader.DownloadSysmsg(PacketAnalyzer.Factory.Version, Path.Combine(App.DataPath, "opcodes/"), PacketAnalyzer.Factory.ReleaseVersion))
            {
                PacketAnalyzer.Factory.ReloadSysMsg();
            }
            else WindowManager.FloatingButton.NotifyExtended("TCC", "Failed to download sysmsg file. System messages will not work.", NotificationType.Warning, 6000);

            BasicTeraData.Instance.Servers.Language = p.Language;
            ProxyInterop.Proxy.ConnectToProxy();
            WindowManager.FloatingButton.NotifyExtended("TCC", $"Release Version: {PacketAnalyzer.Factory.ReleaseVersion}", NotificationType.Normal); //by HQ 20190209
        }
    }
}
