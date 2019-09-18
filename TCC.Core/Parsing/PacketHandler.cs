using System;
using System.IO;
using System.Linq;
using TCC.Data;
using TCC.Data.Chat;
using TCC.Interop.Proxy;
using TCC.Utilities;
using TCC.ViewModels;
using TCC.Windows;

using TeraDataLite;

using TeraPacketParser;
using TeraPacketParser.Messages;

namespace TCC.Parsing
{
    public static class PacketHandler
    {
        #region done
        public static async void HandleCheckVersion(C_CHECK_VERSION p)
        {
            var opcPath = Path.Combine(App.DataPath, $"opcodes/protocol.{p.Versions[0]}.map").Replace("\\", "/");
            OpcodeDownloader.DownloadOpcodesIfNotExist(p.Versions[0], Path.Combine(App.DataPath, "opcodes/"));
            if (!File.Exists(opcPath))
            {
                if (PacketAnalyzer.Sniffer is ToolboxSniffer tbs)
                {
                    if (!await tbs.ControlConnection.DumpMap(opcPath, "protocol"))
                    {
                        TccMessageBox.Show("Unknown client version: " + p.Versions[0], MessageBoxType.Error);
                        App.Close();
                        return;
                    }
                }
                else
                {
                    TccMessageBox.Show("Unknown client version: " + p.Versions[0], MessageBoxType.Error);
                    App.Close();
                    return;
                }
            }
            var opcNamer = new OpCodeNamer(opcPath);
            PacketAnalyzer.Factory = new MessageFactory(p.Versions[0], opcNamer); //SystemMessageNamer = new OpCodeNamer(Path.Combine(App.DataPath, $"opcodes/sysmsg.{PacketAnalyzer.Factory.ReleaseVersion}.map"))
            PacketAnalyzer.Processor?.Update(); //TODO
            PacketAnalyzer.Sniffer.Connected = true;
        }
        public static async void HandleLoginArbiter(C_LOGIN_ARBITER p)
        {
            await ProxyInterface.Instance.Init();

            // 6/6 moved to own hook
            //SessionManager.CurrentAccountName = p.AccountName;

            OpcodeDownloader.DownloadSysmsgIfNotExist(PacketAnalyzer.Factory.Version, Path.Combine(App.DataPath, "opcodes/"), PacketAnalyzer.Factory.ReleaseVersion);
            var path = File.Exists(Path.Combine(App.DataPath, $"opcodes/sysmsg.{PacketAnalyzer.Factory.ReleaseVersion / 100}.map"))
                        ?
                        Path.Combine(App.DataPath, $"opcodes/sysmsg.{PacketAnalyzer.Factory.ReleaseVersion / 100}.map")
                        :
                        File.Exists(Path.Combine(App.DataPath, $"opcodes/sysmsg.{PacketAnalyzer.Factory.Version}.map"))
                            ? Path.Combine(App.DataPath, $"opcodes/sysmsg.{PacketAnalyzer.Factory.Version}.map")
                            : "";

            if (path == "")
            {
                if (PacketAnalyzer.Sniffer.Connected && PacketAnalyzer.Sniffer is ToolboxSniffer tbs)
                {
                    var destPath = Path.Combine(App.DataPath, $"opcodes/sysmsg.{PacketAnalyzer.Factory.Version}.map").Replace("\\", "/");
                    if (await tbs.ControlConnection.DumpMap(destPath, "sysmsg"))
                    {
                        PacketAnalyzer.Factory.SystemMessageNamer = new OpCodeNamer(destPath);
                        return;
                    }
                }
                TccMessageBox.Show($"sysmsg.{PacketAnalyzer.Factory.ReleaseVersion / 100}.map or sysmsg.{PacketAnalyzer.Factory.Version}.map not found.\nWait for update or use tcc-stub to automatically retreive sysmsg files from game client.\nTCC will now close.", MessageBoxType.Error);
                App.Close();
                return;
            }
            PacketAnalyzer.Factory.ReloadSysMsg(path);

            // 6/6 moved to own hook
            //SessionManager.DB.ServerDatabase.Language = p.Language;

            WindowManager.FloatingButton.NotifyExtended("TCC", $"Release Version: {PacketAnalyzer.Factory.ReleaseVersion / 100d}", NotificationType.Normal); //by HQ 20190209
        }
        public static void HandlePlayerStatUpdate(S_PLAYER_STAT_UPDATE p)
        {
            // 6/6 moved to CharacterVM
            //SessionManager.CurrentPlayer.ItemLevel = p.Ilvl;
            //SessionManager.CurrentPlayer.Level = p.Level;
            //SessionManager.CurrentPlayer.CritFactor = p.BonusCritFactor;
            if (PacketAnalyzer.Factory.ReleaseVersion > 8000)
            {
                // 6/6 moved to CharacterVM
                //SessionManager.CurrentPlayer.Coins = p.Coins;
                //SessionManager.CurrentPlayer.MaxCoins = p.MaxCoins;
                //WindowManager.ViewModels.Dashboard.CurrentCharacter.Coins = p.Coins;
                //WindowManager.ViewModels.Dashboard.CurrentCharacter.MaxCoins = p.MaxCoins;
            }

            // 6/6 moved to CharacterVM
            //SessionManager.SetPlayerMaxHp(p.MaxHP);
            //SessionManager.SetPlayerMaxMp(p.MaxMP);
            //SessionManager.SetPlayerMaxSt(p.MaxST + p.BonusST);
            //SessionManager.SetPlayerHp(p.CurrentHP);
            //SessionManager.SetPlayerMp(p.CurrentMP);
            //SessionManager.SetPlayerSt(p.CurrentST);

            //WindowManager.ViewModels.Dashboard.CurrentCharacter.ItemLevel = p.Ilvl;
            //WindowManager.ViewModels.Dashboard.CurrentCharacter.Level = p.Level;

            //switch (Session.Me.Class)
            //{
            //    case Class.Warrior:
            //        if (App.Settings.ClassWindowSettings.Enabled && WindowManager.ViewModels.Class.CurrentManager is WarriorLayoutVM wm)
            //            wm.EdgeCounter.Val = p.Edge;
            //        break;
            //    case Class.Sorcerer:
            //        Session.SetSorcererElements(p.Fire, p.Ice, p.Arcane);
            //        break;
            //}

        }
        public static void HandleNewSkillCooldown(S_START_COOLTIME_SKILL p)
        {
            //SkillManager.AddSkill(p.SkillId, p.Cooldown);
        }
        public static void HandleNewItemCooldown(S_START_COOLTIME_ITEM p)
        {
            //SkillManager.AddItemSkill(p.ItemId, p.Cooldown);
        }
        public static void HandleDecreaseSkillCooldown(S_DECREASE_COOLTIME_SKILL p)
        {
            //SkillManager.ChangeSkillCooldown(p.SkillId, p.Cooldown);
        }
        public static void HandleCreatureChangeHp(S_CREATURE_CHANGE_HP p)
        {
            //if (Session.IsMe(p.Target))
            //{
            //    Session.SetPlayerMaxHp(p.MaxHP);
            //    Session.SetPlayerHp(p.CurrentHP);
            //}
            //else
            //{
            //    EntityManager.UpdateNPC(p.Target, p.CurrentHP, p.MaxHP, p.Source);
            //}
            //ChatWindowManager.Instance.AddDamageReceivedMessage(p.Source, p.Target, p.Diff, p.MaxHP);
        }
        public static void HandlePlayerChangeMp(S_PLAYER_CHANGE_MP p)
        {
            //if (!Session.IsMe(p.Target)) return;
            //Session.SetPlayerMaxMp(p.MaxMP);
            //Session.SetPlayerMp(p.CurrentMP);
        }
        public static void HandlePlayerChangeStamina(S_PLAYER_CHANGE_STAMINA p)
        {
            //Session.SetPlayerSt(p.CurrentST);
        }
        public static void HandlePlayerChangeFlightEnergy(S_PLAYER_CHANGE_FLIGHT_ENERGY p)
        {
            //Session.SetPlayerFe(p.Energy);
        }
        public static void HandleUserStatusChanged(S_USER_STATUS p)
        {
            //if (Session.IsMe(p.EntityId)) Session.Combat = p.IsInCombat;
        }
        public static void HandleBossGageInfo(S_BOSS_GAGE_INFO p)
        {
            //EntityManager.UpdateNPC(p.EntityId, p.CurrentHP, p.MaxHP, (ushort)p.HuntingZoneId, (uint)p.TemplateId);
        }
        public static void HandleNpcStatusChanged(S_NPC_STATUS p)
        {
            //EntityManager.SetNPCStatus(p.EntityId, p.IsEnraged, p.RemainingEnrageTime);
            //if (p.Target == 0)
            //{
            //    WindowManager.ViewModels.NPC.UnsetBossTarget(p.EntityId);
            //}
            //var b = WindowManager.ViewModels.NPC.NpcList.ToSyncList().FirstOrDefault(x => x.EntityId == p.EntityId);
            //if (b == null || !b.Visible) return;
            ////WindowManager.ViewModels.Group.SetAggro(p.Target); // done twice
            //WindowManager.ViewModels.NPC.SetBossAggro(p.EntityId, p.Target);
        }
        public static void HandleUserEffect(S_USER_EFFECT p)
        {
            //WindowManager.ViewModels.NPC.SetBossAggro(p.Source, p.User);
            //WindowManager.ViewModels.Group.SetAggroCircle(p.Circle, p.Action, p.User);
        }
        public static void HandleGetUserList(S_GET_USER_LIST p)
        {
            //Session.Logged = false;
            //SkillManager.Clear();
            //EntityManager.ClearNPC();
            //WindowManager.ViewModels.Group.ClearAll();
            //WindowManager.ViewModels.Dashboard.UpdateBuffs();

            //SessionManager.CurrentPlayer.ClearAbnormalities();

            //Firebase.RegisterWebhook(App.Settings.WebhookUrlGuildBam, false);
            //Firebase.RegisterWebhook(App.Settings.WebhookUrlFieldBoss, false);

            //foreach (var item in p.CharacterList)
            //{
            //    var ch = WindowManager.ViewModels.Dashboard.Characters.FirstOrDefault(x => x.Id == item.Id);
            //    if (ch != null)
            //    {
            //        ch.Name = item.Name;
            //        ch.Laurel = item.Laurel;
            //        ch.Position = item.Position;
            //        ch.GuildName = item.GuildName;
            //        ch.Level = item.Level;
            //        ch.LastLocation = new Location(item.LastWorldId, item.LastGuardId, item.LastSectionId);
            //        ch.LastOnline = item.LastOnline;
            //        ch.ServerName = Session.Server.Name;
            //    }
            //    else
            //    {
            //        WindowManager.ViewModels.Dashboard.Characters.Add(new Character(item));
            //    }
            //}

            //WindowManager.ViewModels.Dashboard.SaveCharacters();

        }
        public static void HandleLogin(S_LOGIN p)
        {
            //Firebase.RegisterWebhook(App.Settings.WebhookUrlGuildBam, true);
            //Firebase.RegisterWebhook(App.Settings.WebhookUrlFieldBoss, true);

            //SessionManager.CurrentPlayer.Class = p.CharacterClass;
            //WindowManager.ReloadPositions();
            //if (App.Settings.ClassWindowSettings.Enabled) WindowManager.ViewModels.Class.CurrentClass = p.CharacterClass;
            //AbnormalityManager.SetAbnormalityTracker(p.CharacterClass);
            //Session.Server = Session.DB.ServerDatabase.GetServer(p.ServerId);
            //Firebase.SendUsageStatAsync();
            //App.Settings.LastLanguage = Session.Language;
            //TimeManager.Instance.SetServerTimeZone(App.Settings.LastLanguage);
            //TimeManager.Instance.SetGuildBamTime(false);
            //Session.InitDatabases(App.Settings.LastLanguage);
            //SkillManager.Clear();
            //WindowManager.ViewModels.Cooldowns.LoadSkills(p.CharacterClass);
            //WindowManager.FloatingButton.SetMoongourdButtonVisibility();
            //EntityManager.ClearNPC();
            //WindowManager.ViewModels.Group.ClearAll();
            //ChatWindowManager.Instance.BlockedUsers.Clear();
            //SessionManager.CurrentPlayer.ClearAbnormalities();

            //Session.Logged = true;
            //Session.LoadingScreen = true;
            //Session.Encounter = false;
            PacketAnalyzer.Processor?.Update(); //TODO
            //SessionManager.CurrentPlayer.EntityId = p.EntityId;
            //SessionManager.CurrentPlayer.PlayerId = p.PlayerId;
            //SessionManager.CurrentPlayer.ServerId = p.ServerId;
            //SessionManager.CurrentPlayer.Name = p.Name;
            //SessionManager.CurrentPlayer.Level = p.Level;
            //SessionManager.SetPlayerLaurel(SessionManager.CurrentPlayer);
            //WindowManager.ViewModels.Dashboard.SetLoggedIn(p.PlayerId);
            //Session.GuildMembersNames.Clear();

            //App.BaseDispatcher.Invoke(() => { WindowManager.ViewModels.LFG.EnqueueListRequest(); });

        }
        public static void HandleLfgList(S_SHOW_PARTY_MATCH_INFO x)
        {
            //if (!App.Settings.LfgWindowSettings.Enabled) return;
            //if (WindowManager.LfgListWindow == null) return;
            //if (WindowManager.ViewModels.LFG == null) return;
            //if (!x.IsLast && App.Settings.LfgWindowSettings.Enabled && ProxyInterface.Instance.IsStubAvailable)
            //    ProxyInterface.Instance.Stub.RequestListingsPage(x.Page + 1);

            //if (!x.IsLast) return;

            //if (S_SHOW_PARTY_MATCH_INFO.Listings.Count == 0)
            //{
            //    WindowManager.ViewModels.LFG.NotifyMyLfg();
            //    WindowManager.LfgListWindow.ShowWindow();
            //    return;
            //}

            //WindowManager.ViewModels.LFG.SyncListings(S_SHOW_PARTY_MATCH_INFO.Listings);

            //WindowManager.ViewModels.LFG.NotifyMyLfg();
            //WindowManager.LfgListWindow.ShowWindow();
        }
        public static void HandleReturnToLobby(S_RETURN_TO_LOBBY p)
        {
            //Session.Logged = false;
            //WindowManager.ViewModels.LFG.ForceStopPublicize();
            //WindowManager.ViewModels.Dashboard.UpdateBuffs();
            //SessionManager.CurrentPlayer.ClearAbnormalities();
            //SkillManager.Clear();
            //EntityManager.ClearNPC();
            //WindowManager.ViewModels.Group.ClearAll();
            //WindowManager.ViewModels.Class.CurrentClass = Class.None;
        }
        public static void HandleLoadTopo(S_LOAD_TOPO x)
        {
            //Session.LoadingScreen = true;
            //Session.Encounter = false;
            //WindowManager.ViewModels.Group.ClearAllAbnormalities();
            //WindowManager.ViewModels.Group.SetAggro(0);
            //SessionManager.CurrentPlayer.ClearAbnormalities();
            //WindowManager.ViewModels.NPC.CurrentHHphase = HarrowholdPhase.None;
            //WindowManager.ViewModels.NPC.ClearGuildTowers();
            //Session.CivilUnrestZone = x.Zone == 152;
            //Session.IsInDungeon = x.Zone >= 8999; // from Salty's server-exposer

            //if (App.Settings.CivilUnrestWindowSettings.Enabled) WindowManager.ViewModels.CivilUnrest.NotifyTeleported();
        }
        public static void HandleStartRoll(S_ASK_BIDDING_RARE_ITEM x)
        {
            //WindowManager.ViewModels.Group.StartRoll();
        }
        public static void HandleRollResult(S_RESULT_BIDDING_DICE_THROW x)
        {
        //    if (!WindowManager.ViewModels.Group.Rolling) WindowManager.ViewModels.Group.StartRoll();
        //    WindowManager.ViewModels.Group.SetRoll(x.EntityId, x.RollResult);
        }
        public static void HandleEndRoll(S_RESULT_ITEM_BIDDING x)
        {
            //WindowManager.ViewModels.Group.EndRoll();
        }
        public static void HandleSpawnMe(S_SPAWN_ME p)
        {
            //EntityManager.ClearNPC();
            //FlyingGuardianDataProvider.Stacks = 0;
            //FlyingGuardianDataProvider.StackType = FlightStackType.None;
            //FlyingGuardianDataProvider.InvokeProgressChanged();
            //var t = new System.Timers.Timer(2000);
            //t.Elapsed += (s, ev) =>
            //{
            //    t.Stop();
            //    Session.LoadingScreen = false;
            //    WindowManager.ForegroundManager.RefreshDim();
            //};
            //t.Enabled = true;
        }
        public static void HandleSpawnNpc(S_SPAWN_NPC p)
        {
            //EntityManager.CheckHarrowholdMode(p.HuntingZoneId, p.TemplateId);
            //EntityManager.SpawnNPC(p.HuntingZoneId, p.TemplateId, p.EntityId, false, p.Villager, p.RemainingEnrageTime);
        }
        public static void HandleSpawnUser(S_SPAWN_USER p)
        {
            //switch (p.Name)
            //{
            //    case "Foglio":
            //    case "Foglietto":
            //    case "Foglia":
            //    case "Myvia":
            //    case "Foglietta.Blu":
            //    case "Foglia.Trancer":
            //    case "Folyria":
            //    case "Folyvia":
            //    case "Fogliolina":
            //    case "Folyemi":
            //    case "Foiya":
            //    case "Fogliarya":
            //        if (p.ServerId != 27) break;
            //        if (Session.CivilUnrestZone) break;
            //        EntityManager.FoglioEid = p.EntityId;
            //        var ab = Session.DB.AbnormalityDatabase.Abnormalities[10241024];
            //        AbnormalityManager.BeginAbnormality(ab.Id, Session.Me.EntityId, 0, int.MaxValue, 1);
            //        var sysMsg = Session.DB.SystemMessagesDatabase.Messages["SMT_BATTLE_BUFF_DEBUFF"];
            //        var msg = $"@0\vAbnormalName\v{ab.Name}";
            //        SystemMessagesProcessor.AnalyzeMessage(msg, sysMsg, "SMT_BATTLE_BUFF_DEBUFF");
            //        break;
            //}

            //EntityManager.SpawnUser(p.EntityId, p.Name);
            //if (!WindowManager.ViewModels.Group.Exists(p.EntityId)) return;
            //WindowManager.ViewModels.Group.UpdateMemberGear(p.PlayerId, p.ServerId, p.Weapon, p.Armor, p.Gloves, p.Boots);
        }
        public static void HandlePartyMemberBuffUpdate(S_PARTY_MEMBER_BUFF_UPDATE x)
        {
            //foreach (var buff in x.Abnormals)
            //{
            //    AbnormalityManager.UpdatePartyMemberAbnormality(x.PlayerId, x.ServerId, buff.Id, buff.Duration, buff.Stacks);
            //}
        }
        public static void HandlePartyMemberAbnormalAdd(S_PARTY_MEMBER_ABNORMAL_ADD x)
        {
            //AbnormalityManager.UpdatePartyMemberAbnormality(x.PlayerId, x.ServerId, x.Id, x.Duration, x.Stacks);
        }
        public static void HandlePartyMemberAbnormalDel(S_PARTY_MEMBER_ABNORMAL_DEL x)
        {
            //AbnormalityManager.EndPartyMemberAbnormality(x.PlayerId, x.ServerId, x.Id);
        }
        public static void HandleRunemark(S_WEAK_POINT x)
        {
            //if (Session.Me.Class != Class.Valkyrie) return;
            //if (TccUtils.CurrentClassVM<ValkyrieLayoutVM>() == null) return;
            //TccUtils.CurrentClassVM<ValkyrieLayoutVM>().RunemarksCounter.Val = x.TotalRunemarks;
        }
        public static void HandleChangeLeader(S_CHANGE_PARTY_MANAGER x)
        {
            //WindowManager.ViewModels.Group.SetNewLeader(x.EntityId, x.Name);
        }
        public static void HandlePartyMemberAbnormalClear(S_PARTY_MEMBER_ABNORMAL_CLEAR x)
        {
            //WindowManager.ViewModels.Group.ClearAbnormality(x.PlayerId, x.ServerId);
        }
        public static void HandlePartyMemberAbnormalRefresh(S_PARTY_MEMBER_ABNORMAL_REFRESH x)
        {
            //AbnormalityManager.UpdatePartyMemberAbnormality(x.PlayerId, x.ServerId, x.Id, x.Duration, x.Stacks);
        }
        public static void HandleChat(S_CHAT x)
        {
            //if ((x.AuthorName == "Foglio" || x.AuthorName == "Myvia" || x.AuthorName == "Foglia" || x.AuthorName == "Foglia.Trancer" || x.AuthorName == "Folyemi" ||
            //    x.AuthorName == "Folyria" || x.AuthorName == "Foglietto") && x.Channel == (uint)ChatChannel.Greet) WindowManager.FloatingButton.NotifyExtended("TCC", "Nice TCC :lul:", NotificationType.Warning);
            //ChatWindowManager.Instance.AddChatMessage(new ChatMessage(x.Channel == 212 ? (ChatChannel)26 : ((ChatChannel)x.Channel), x.AuthorName, x.Message));
        }
        public static void HandleCreatureLife(S_CREATURE_LIFE p)
        {
            //if (SessionManager.IsMe(p.Target))
            //{
            //    SessionManager.CurrentPlayer.IsAlive = p.Alive;
            //}
        }
        public static void HandlePrivateChat(S_PRIVATE_CHAT x)
        {
            //var i = ChatWindowManager.Instance.PrivateChannels.FirstOrDefault(y => y.Id == x.Channel).Index;
            //var ch = (ChatChannel)(ChatWindowManager.Instance.PrivateChannels[i].Index + 11);
            //ChatWindowManager.Instance.AddChatMessage(new ChatMessage(ch, x.AuthorName, x.Message));
        }
        public static void HandleJoinPrivateChat(S_JOIN_PRIVATE_CHANNEL x)
        {
            //ChatWindowManager.Instance.JoinPrivateChannel(x.Id, x.Index, x.Name);
        }
        public static void HandleGuildTowerInfo(S_GUILD_TOWER_INFO x)
        {
            //WindowManager.ViewModels.NPC.AddGuildTower(x.TowerId, x.GuildName, x.GuildId);
        }
        public static void HandleLeavePrivateChat(S_LEAVE_PRIVATE_CHANNEL x)
        {
            //var i = ChatWindowManager.Instance.PrivateChannels.FirstOrDefault(c => c.Id == x.Id).Index;
            //ChatWindowManager.Instance.PrivateChannels[i].Joined = false;
        }
        public static void HandleDungeonCooltimeList(S_DUNGEON_COOL_TIME_LIST x)
        {
            //WindowManager.ViewModels.Dashboard.SetDungeons(x.DungeonCooldowns);
        }
        public static void HandleAccountPackageList(S_ACCOUNT_PACKAGE_LIST x)
        {
            //Session.IsElite = x.IsElite;
        }
        public static void HandleWhisper(S_WHISPER x)
        {
            //var isMe = x.Author == Session.Me.Name;
            //ChatWindowManager.Instance.AddChatMessage(
            //    new ChatMessage(isMe ? ChatChannel.SentWhisper : ChatChannel.ReceivedWhisper, 
            //                    isMe ? x.Recipient : x.Author, 
            //                     x.Message));

        }
        public static void HandleGuardianInfo(S_FIELD_POINT_INFO x)
        {
            //if (WindowManager.ViewModels.Dashboard.CurrentCharacter == null) return;
            //WindowManager.ViewModels.Dashboard.CurrentCharacter.ClaimedGuardianQuests = x.Claimed;
            //WindowManager.ViewModels.Dashboard.CurrentCharacter.ClearedGuardianQuests = x.Cleared;
        }
        public static void HandleVanguardReceived(S_AVAILABLE_EVENT_MATCHING_LIST x)
        {
            //WindowManager.ViewModels.Dashboard.SetVanguard(x.WeeklyDone, x.DailyDone, x.VanguardCredits);
        }
        public static void HandleDungeonClears(S_DUNGEON_CLEAR_COUNT_LIST x)
        {
            //if (x.Failed) return;
            //if (x.PlayerId != Session.Me.PlayerId) return;
            //foreach (var dg in x.DungeonClears)
            //{
            //    WindowManager.ViewModels.Dashboard.CurrentCharacter.SetDungeonClears(dg.Key, dg.Value);
            //}
        }
        public static void HandleDungeonMessage(S_DUNGEON_EVENT_MESSAGE p)
        {
            //switch (p.MessageId)
            //{
            //    case 9950045:
            //        //shield start
            //        foreach (var item in WindowManager.ViewModels.NPC.NpcList.Where(x => x.IsPhase1Dragon))
            //        {
            //            item.StartShield();
            //        }
            //        break;
            //    case 9950113:
            //        //aquadrax interrupted
            //        WindowManager.ViewModels.NPC.NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1103).BreakShield();
            //        break;
            //    case 9950114:
            //        //umbradrax interrupted
            //        WindowManager.ViewModels.NPC.NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1102).BreakShield();
            //        break;
            //    case 9950115:
            //        //ignidrax interrupted
            //        WindowManager.ViewModels.NPC.NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1100).BreakShield();
            //        break;
            //    case 9950116:
            //        //terradrax interrupted
            //        WindowManager.ViewModels.NPC.NpcList.First(x => x.ZoneId == 950 && x.TemplateId == 1101).BreakShield();
            //        break;
            //    case 9950044:
            //        //shield fail
            //        break;
            //}
        }
        public static void HandlePlayerLocation(C_PLAYER_LOCATION p)
        {
            //if (WindowManager.ViewModels.NPC.CurrentHHphase != HarrowholdPhase.Phase1) return;
            //WindowManager.ViewModels.NPC.SelectDragon(p.X, p.Y);
        }
        public static void HandleBrokerOffer(S_TRADE_BROKER_DEAL_SUGGESTED x)
        {
            //ChatWindowManager.Instance.AddChatMessage(new BrokerChatMessage(x.PlayerId, x.Listing, x.Item, x.Amount, x.SellerPrice, x.OfferedPrice, x.Name));
        }
        public static void HandleUserApplyToParty(S_OTHER_USER_APPLY_PARTY x)
        {
            //ChatWindowManager.Instance.AddChatMessage(new ApplyMessage(x.PlayerId, x.Class, x.Level, x.Name)); 
            //if (!App.Settings.LfgWindowSettings.Enabled) return;
            //if (WindowManager.ViewModels.LFG.MyLfg == null) return;
            //var dest = WindowManager.ViewModels.LFG.MyLfg.Applicants;
            //if (dest.Any(u => u.PlayerId == x.PlayerId)) return;
            //dest.Add(new User(WindowManager.LfgListWindow.Dispatcher)
            //{
            //    PlayerId = x.PlayerId,
            //    UserClass = x.Class,
            //    Level = Convert.ToUInt32(x.Level),
            //    Name = x.Name,
            //    Online = true

            //});
        }
        public static void HandleLfgSpam(S_PARTY_MATCH_LINK x)
        {
            //if (x.Message.IndexOf("WTB", 0, StringComparison.InvariantCultureIgnoreCase) != -1) return;
            //if (x.Message.IndexOf("WTS", 0, StringComparison.InvariantCultureIgnoreCase) != -1) return;
            //if (x.Message.IndexOf("WTT", 0, StringComparison.InvariantCultureIgnoreCase) != -1) return;
            //ChatWindowManager.Instance.AddOrRefreshLfg(x.ListingData);
            //ChatWindowManager.Instance.AddChatMessage(new LfgMessage(x.Id, x.Name, x.Message));
        }
        public static void HandleBlockList(S_USER_BLOCK_LIST x)
        {
            //x.BlockedUsers.ForEach(u =>
            //{
            //    if (ChatWindowManager.Instance.BlockedUsers.Contains(u)) return;
            //    ChatWindowManager.Instance.BlockedUsers.Add(u);
            //});
        }
        public static void HandleFriendList(S_FRIEND_LIST x)
        {
            //ChatWindowManager.Instance.Friends = x.Friends;
        }
        public static void HandleCrestMessage(S_CREST_MESSAGE x)
        {
            //if (x.Type != 6) return;
            //SkillManager.ResetSkill(x.SkillId);
        }
        public static void HandleAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            //AbnormalityManager.BeginAbnormality(p.AbnormalityId, p.TargetId, p.CasterId, p.Duration, p.Stacks);
            //if (Session.IsMe(p.TargetId)) FlyingGuardianDataProvider.HandleAbnormal(p);

            //if (!App.Settings.ClassWindowSettings.Enabled) return;
            //AbnormalityManager.CurrentAbnormalityTracker?.CheckAbnormality(p);
        }
        public static void HandleAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            //AbnormalityManager.BeginAbnormality(p.AbnormalityId, p.TargetId, p.TargetId, p.Duration, p.Stacks);
            //if (Session.IsMe(p.TargetId)) FlyingGuardianDataProvider.HandleAbnormal(p);

            //if (!App.Settings.ClassWindowSettings.Enabled) return;
            //AbnormalityManager.CurrentAbnormalityTracker?.CheckAbnormality(p);
        }
        public static void HandleAbnormalityEnd(S_ABNORMALITY_END p)
        {
            //if (!AbnormalityManager.EndAbnormality(p.TargetId, p.AbnormalityId)) return;
            //if (Session.IsMe(p.TargetId)) FlyingGuardianDataProvider.HandleAbnormal(p);

            //if (!App.Settings.ClassWindowSettings.Enabled) return;
            //AbnormalityManager.CurrentAbnormalityTracker?.CheckAbnormality(p);
        }
        public static void HandlePartyMemberList(S_PARTY_MEMBER_LIST p)
        {
            //var notifyLfg = WindowManager.ViewModels.Group.Members.Count == 0;

            //WindowManager.GroupWindow.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    WindowManager.ViewModels.Group.SetRaid(p.Raid);
            //    p.Members.ForEach(WindowManager.ViewModels.Group.AddOrUpdateMember);
            //}));

            //if (notifyLfg && WindowManager.LfgListWindow != null && WindowManager.ViewModels.LFG != null) WindowManager.ViewModels.LFG.NotifyMyLfg();
            //if (!ProxyInterface.Instance.IsStubAvailable || !App.Settings.LfgWindowSettings.Enabled || !Session.InGameUiOn) return;
            //ProxyInterface.Instance.Stub.RequestListingCandidates(); 
            //if (WindowManager.LfgListWindow == null || !WindowManager.LfgListWindow.IsVisible) return;
            //ProxyInterface.Instance.Stub.RequestListings(); 
        }
        public static void HandleReadyCheck(S_CHECK_TO_READY_PARTY p)
        {
            //WindowManager.GroupWindow.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    foreach (var member in p.Party)
            //    {
            //        WindowManager.ViewModels.Group.SetReadyStatus(member);
            //    }
            //}));
        }
        public static void HandlePartyMemberLeave(S_LEAVE_PARTY_MEMBER p)
        {
            //WindowManager.ViewModels.Group.RemoveMember(p.PlayerId, p.ServerId);
        }
        public static void HandlePartyMemberLogout(S_LOGOUT_PARTY_MEMBER p)
        {
            //WindowManager.ViewModels.Group.LogoutMember(p.PlayerId, p.ServerId);
            //WindowManager.ViewModels.Group.ClearAbnormality(p.PlayerId, p.ServerId);
        }
        public static void HandlePartyMemberKick(S_BAN_PARTY_MEMBER p)
        {
            //WindowManager.ViewModels.Group.RemoveMember(p.PlayerId, p.ServerId, true);
        }
        public static void HandlePartyMemberHp(S_PARTY_MEMBER_CHANGE_HP p)
        {
            //WindowManager.ViewModels.Group.UpdateMemberHp(p.PlayerId, p.ServerId, p.CurrentHP, p.MaxHP);
        }
        public static void HandlePartyMemberMp(S_PARTY_MEMBER_CHANGE_MP p)
        {
            //WindowManager.ViewModels.Group.UpdateMemberMp(p.PlayerId, p.ServerId, p.CurrentMP, p.MaxMP);
        }
        public static void HandlePartyMemberStats(S_PARTY_MEMBER_STAT_UPDATE p)
        {
            //WindowManager.ViewModels.Group.UpdateMember(p.PartyMemberData);
        }
        public static void HandleLeaveParty(S_LEAVE_PARTY x)
        {
            //WindowManager.ViewModels.Group.ClearAll();
            //if (App.Settings.LfgWindowSettings.Enabled) WindowManager.ViewModels.LFG.NotifyMyLfg();
        }
        public static void HandleKicked(S_BAN_PARTY x)
        {
            //WindowManager.ViewModels.Group.ClearAll();
            //if (App.Settings.LfgWindowSettings.Enabled) WindowManager.ViewModels.LFG.NotifyMyLfg();
        }
        public static void HandleReadyCheckFin(S_CHECK_TO_READY_PARTY_FIN x)
        {
            //WindowManager.ViewModels.Group.EndReadyCheck();
        }
        public static void HandleInventory(S_INVEN x)
        {
            //TODO: add gear again?
            //if (x.Failed) return;
            //WindowManager.ViewModels.Dashboard.UpdateInventory(x.Items, x.First);
        }
        public static void HandleShieldDamageAbsorb(S_ABNORMALITY_DAMAGE_ABSORB p)
        {

            //if (Session.IsMe(p.Target)) Session.SetPlayerShield(p.Damage);
            //else if (WindowManager.ViewModels.NPC.NpcList.ToSyncList().Any(x => x.EntityId == p.Target))
            //{
            //    WindowManager.ViewModels.NPC.UpdateShield(p.Target, p.Damage);
            //}
        }
        public static void HandleShowHp(S_SHOW_HP x)
        {
            //WindowManager.ViewModels.NPC.AddOrUpdateNpc(x.GameId, x.MaxHp, x.CurrentHp, false, HpChangeSource.CreatureChangeHp);
        }
        public static void HandleDestroyGuildTower(S_DESTROY_GUILD_TOWER p)
        {
            //try
            //{
            //    WindowManager.ViewModels.CivilUnrest.AddDestroyedGuildTower(p.SourceGuildId);
            //}
            //catch
            //{
            //    // ignored
            //}
        }
        public static void HandleCityWarMapInfo(S_REQUEST_CITY_WAR_MAP_INFO p)
        {
            //try
            //{
            //    p.Guilds.ToList().ForEach(x => WindowManager.ViewModels.CivilUnrest.AddGuild(x));
            //}
            //catch
            //{
            //    // ignored
            //}
        }
        public static void HandleCityWarMapInfoDetail(S_REQUEST_CITY_WAR_MAP_INFO_DETAIL p)
        {
            //try
            //{
            //    p.GuildDetails.ToList().ForEach(x => WindowManager.ViewModels.CivilUnrest.SetGuildName(x.Item1, x.Item2));
            //}
            //catch
            //{
            //    // ignored
            //}
        }
        public static void HandleUpdateNpcGuild(S_UPDATE_NPCGUILD p)
        {
            //switch (p.Guild)
            //{
            //    case NpcGuild.Vanguard:
            //        WindowManager.ViewModels.Dashboard.SetVanguardCredits(p.Credits);
            //        break;
            //    case NpcGuild.Guardian:
            //        WindowManager.ViewModels.Dashboard.SetGuardianCredits(p.Credits);
            //        break;
            //}
        }
        public static void HandleNpcGuildList(S_NPCGUILD_LIST p)
        {
            //if (!Session.IsMe(p.UserId)) return;
            //p.NpcGuildList.Keys.ToList().ForEach(k =>
            //{
            //    switch (k)
            //    {
            //        case (int)NpcGuild.Vanguard:
            //            WindowManager.ViewModels.Dashboard.SetVanguardCredits(p.NpcGuildList[k]);
            //            break;
            //        case (int)NpcGuild.Guardian:
            //            WindowManager.ViewModels.Dashboard.SetGuardianCredits(p.NpcGuildList[k]);
            //            break;
            //    }
            //});
        }
        public static void HandleGuildMembersList(S_GUILD_MEMBER_LIST obj)
        {
            //obj.GuildMembersList.ToList().ForEach(m => { Session.GuildMembersNames[m.Key] = m.Value; });
        }
        public static void HandlePlayerChangeExp(S_PLAYER_CHANGE_EXP p)
        {
            //var msg = $"<font>You gained </font>";
            //msg += $"<font color='{R.Colors.GoldColor.ToHex()}'>{p.GainedTotalExp - p.GainedRestedExp:N0}</font>";
            //msg += $"<font>{(p.GainedRestedExp > 0 ? $" + </font><font color='{R.Colors.ChatMegaphoneColor.ToHex()}'>{p.GainedRestedExp:N0}" : "")} </font>";
            //msg += $"<font>(</font>";
            //msg += $"<font color='{R.Colors.GoldColor.ToHex()}'>";
            //msg += $"{(p.GainedTotalExp) / (double)(p.NextLevelExp):P3}</font>";
            //msg += $"<font>) XP.</font>";
            //msg += $"<font> Total: </font>";
            //msg += $"<font color='{R.Colors.GoldColor.ToHex()}'>{p.LevelExp / (double)(p.NextLevelExp):P3}</font>";
            //msg += $"<font>.</font>";

            //ChatWindowManager.Instance.AddChatMessage(new ChatMessage(ChatChannel.Exp, "System", msg));
        }
        public static void HandleLoadEpInfo(S_LOAD_EP_INFO p)
        {
            //if (p.Perks.TryGetValue(851010, out var level))
            //{
            //    EpDataProvider.SetManaBarrierPerkLevel(level);
            //}
        }
        public static void HandleLearnEpPerk(S_LEARN_EP_PERK p)
        {
            //if (p.Perks.TryGetValue(851010, out var level))
            //{
            //    EpDataProvider.SetManaBarrierPerkLevel(level);
            //}
        }
        public static void HandleResetEpPerk(S_RESET_EP_PERK p)
        {
            //if (p.Success) EpDataProvider.SetManaBarrierPerkLevel(0);
        }
        public static void HandleImageData(S_IMAGE_DATA sImageData)
        {


        }
        public static void HandlePartyMemberInfo(S_PARTY_MEMBER_INFO packet)
        {
            //ChatWindowManager.Instance.UpdateLfgMembers(packet.Id, packet.Members.Count);

            //if (!App.Settings.LfgWindowSettings.Enabled) return;
            //var lfg = WindowManager.ViewModels.LFG.Listings.FirstOrDefault(listing => listing.LeaderId == packet.Id || packet.Members.Any(member => member.PlayerId == listing.LeaderId));
            //if (lfg == null) return;
            ////lfg.Players.Clear();
            //packet.Members.ForEach(member =>
            //{
            //    if (lfg.Players.Any(toFind => toFind.PlayerId == member.PlayerId))
            //    {
            //        var target = lfg.Players.FirstOrDefault(player => player.PlayerId == member.PlayerId);
            //        if (target == null) return;
            //        target.IsLeader = member.IsLeader;
            //        target.Online = member.Online;
            //        target.Location = Session.DB.GetSectionName(member.GuardId, member.SectionId);
            //    }
            //    else lfg.Players.Add(new User(member));
            //});
            //var toDelete = new List<uint>();
            //lfg.Players.ToList().ForEach(player =>
            //{
            //    if (packet.Members.All(newMember => newMember.PlayerId != player.PlayerId)) toDelete.Add(player.PlayerId);
            //    toDelete.ForEach(targetId => lfg.Players.Remove(lfg.Players.FirstOrDefault(playerToRemove => playerToRemove.PlayerId == targetId)));
            //});
            //lfg.LeaderId = packet.Id;
            //var leader = lfg.Players.FirstOrDefault(u => u.IsLeader);
            //if (leader != null) lfg.LeaderName = leader.Name;
            //if (WindowManager.ViewModels.LFG.LastClicked != null && WindowManager.ViewModels.LFG.LastClicked.LeaderId == lfg.LeaderId) lfg.IsExpanded = true;
            //lfg.PlayerCount = packet.Members.Count;
            //WindowManager.ViewModels.LFG.NotifyMyLfg();
        }
        public static void HandlePartyMemberIntervalPosUpdate(S_PARTY_MEMBER_INTERVAL_POS_UPDATE p)
        {
            //WindowManager.ViewModels.Group.UpdateMemberLocation(p.PlayerId, p.ServerId, p.Channel, p.ContinentId);
        }
        public static void HandleUserGuildLogo(S_GET_USER_GUILD_LOGO sGetUserGuildLogo)
        {
            //if (sGetUserGuildLogo.GuildLogo == null) return;
            //S_IMAGE_DATA.Database[sGetUserGuildLogo.GuildId] = sGetUserGuildLogo.GuildLogo;

            //if (!Directory.Exists("resources/images/guilds")) Directory.CreateDirectory("resources/images/guilds");
            //try
            //{
            //    sGetUserGuildLogo.GuildLogo.Save(
            //        Path.Combine(App.ResourcesPath, $"images/guilds/guildlogo_{Session.Server.ServerId}_{sGetUserGuildLogo.GuildId}_{0}.bmp"),
            //        System.Drawing.Imaging.ImageFormat.Bmp
            //        );
            //}
            //catch (Exception e)
            //{
            //    Log.F($"Error while saving guild logo: {e}");
            //}

        }
        public static void HandleApplicantsList(S_SHOW_CANDIDATE_LIST p)
        {
            //if (WindowManager.ViewModels.LFG == null) return;
            //if (WindowManager.ViewModels.LFG.MyLfg == null) return;
            //var dest = WindowManager.ViewModels.LFG.MyLfg.Applicants;
            ////TODO refactoring: method that does this "merge" thing
            //foreach (var applicant in p.Candidates)
            //{
            //    if (dest.All(x => x.PlayerId != applicant.PlayerId)) dest.Add(new User(applicant));
            //}

            //var toRemove = new List<User>();
            //foreach (var user in dest)
            //{
            //    if (p.Candidates.All(x => x.PlayerId != user.PlayerId)) toRemove.Add(user);
            //}
            //toRemove.ForEach(r => dest.Remove(r));
        }
        public static void HandleGuardianOnEnter(S_FIELD_EVENT_ON_ENTER obj)
        {
            //const string opcode = "SMT_FIELD_EVENT_ENTER";
            //Session.DB.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);
            //SystemMessagesProcessor.AnalyzeMessage("", m, opcode);

            //if (/*ProxyOld.IsConnected */ ProxyInterface.Instance.IsStubAvailable && ProxyInterface.Instance.IsFpsUtilsAvailable && App.Settings.FpsAtGuardian)
            //{
            //    ProxyInterface.Instance.Stub.InvokeCommand("fps mode 3"); //ProxyOld.SendCommand($"fps mode 3");
            //}
        }
        public static void HandleGuardianOnLeave(S_FIELD_EVENT_ON_LEAVE obj)
        {
            //const string opcode = "SMT_FIELD_EVENT_LEAVE";
            //Session.DB.SystemMessagesDatabase.Messages.TryGetValue(opcode, out var m);
            //SystemMessagesProcessor.AnalyzeMessage("", m, opcode);

            //if (/*ProxyOld.IsConnected */ ProxyInterface.Instance.IsStubAvailable && ProxyInterface.Instance.IsFpsUtilsAvailable && App.Settings.FpsAtGuardian)
            //{
            //    ProxyInterface.Instance.Stub.InvokeCommand("fps mode 1"); //ProxyOld.SendCommand($"fps mode 1");
            //}
        }

        #endregion // ---------------------------------------------------

        //public static void HandleViewWareEx(S_VIEW_WARE_EX p)
        //{
        //    foreach (var page in S_VIEW_WARE_EX.Pages)
        //    {
        //        if (page.Index + 1 == S_VIEW_WARE_EX.Pages.Count) break;
        //        for (var i = page.Index + 1; i < 8; i++)
        //        {
        //            var pg = S_VIEW_WARE_EX.Pages[(int)i];
        //            foreach (var item in page.Items)
        //            {
        //                if (pg.Items.All(x => x.Id != item.Id)) continue;
        //                var name = Session.DB.ItemsDatabase.GetItemName((uint)item.Id);
        //                Console.WriteLine($"Found duplicate of {name} [{item.Id}] (page {page.Index + 1}) in page {i + 1}");
        //            }
        //        }
        //    }
        //}
        //public static void HandleGpkData(string data)
        //{
        //    const string chatModeCmd = ":tcc-chatMode:";
        //    const string uiModeCmd = ":tcc-uiMode:";
        //    const string unkString = "Unknown command ";
        //    data = data.Replace(unkString, "").Replace("\"", "").Replace(".", "");
        //    if (data.StartsWith(chatModeCmd))
        //    {
        //        var chatMode = data.Replace(chatModeCmd, "");
        //        Session.InGameChatOpen = chatMode == "1" || chatMode == "true"; //too lazy
        //    }
        //    else if (data.StartsWith(uiModeCmd))
        //    {
        //        var uiMode = data.Replace(uiModeCmd, "");
        //        Session.InGameUiOn = uiMode == "1" || uiMode == "true"; //too lazy
        //    }
        //}
        //public static void HandleSkillResult(S_EACH_SKILL_RESULT x)
        //{
        //    if (x.Skill != 63005521) return;
        //    var name = EntityManager.GetUserName(x.Source);
        //    ChatWindowManager.Instance.AddTccMessage($"Dragon Firework spawned by {name}");
        //    //bool sourceInParty = WindowManager.ViewModels.Group.UserExists(x.Source);
        //    //bool targetInParty = WindowManager.ViewModels.Group.UserExists(x.Target);
        //    //if (x.Target == x.Source) return;
        //    //if (sourceInParty && targetInParty) return;
        //    //if (sourceInParty || targetInParty) WindowManager.SkillsEnded = false;
        //    //if (x.Source == SessionManager.CurrentPlayer.EntityId) WindowManager.SkillsEnded = false;
        //    //if (x.Source == SessionManager.CurrentPlayer.EntityId) return;
        //    //WindowManager.ViewModels.NPC.UpdateShield(x.Target, x.Damage);
        //    //if (x.Type != 1) return;
        //    //if (x.Source == SessionManager.CurrentPlayer.EntityId) return;
        //    //WindowManager.ViewModels.NPC.UpdateBySkillResult(x.Target, x.Damage);
        //}
        //public static void SendTestMessage()
        //{
        //    var str = "@3947questNameDefeat HumedraszoneName@zoneName:181npcName@creature:181#2050";
        //    var str = "@3789cityname@cityWar:20guildFated";
        //    var str = "@1773ItemName@item:152141ItemName1@item:447ItemCount5";
        //    const string str = "@3821userNametestNameguildQuestName@GuildQuest:31007001value1targetValue3";
        //    var toBytes = Encoding.Unicode.GetBytes(str);
        //    var arr = new byte[toBytes.Length + 2 + 4];
        //    for (var i = 0; i < toBytes.Length - 1; i++)
        //    {
        //        arr[i + 4] = toBytes[i];
        //    }
        //    var seg = new ArraySegment<byte>(arr);
        //    var sysMsg = new S_SYSTEM_MESSAGE(new TeraMessageReader(new Message(DateTime.Now, MessageDirection.ServerToClient, seg), OpCodeNamer, Factory, SystemMessageNamer));
        //    HandleSystemMessage(sysMsg);
        //}
        //public static void HandleProxyOutput(string author, uint channel, string message)
        //{
        //    //if (message.IndexOf('[') != -1 && message.IndexOf(']') != -1)
        //    //{
        //    //    //    author = message.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries)[1];
        //    //    //   message = message.Replace('[' + author + ']', "");
        //    //}
        //    if (author == "undefined") author = "System";
        //    if (!ChatWindowManager.Instance.PrivateChannels.Any(x => x.Id == channel && x.Joined))
        //        ChatWindowManager.Instance.CachePrivateMessage(channel, author, message);
        //    else
        //        ChatWindowManager.Instance.AddChatMessage(
        //            new ChatMessage((ChatChannel)ChatWindowManager.Instance.PrivateChannels.FirstOrDefault(x =>
        //                                x.Id == channel && x.Joined).Index + 11, author, message));
        //}

        //public static void HandleActionStage(S_ACTION_STAGE x)
        //{
        //    var name = EntityManager.GetUserName(x.GameId);
        //    if (x.Skill == 63005521) ChatWindowManager.Instance.AddTccMessage($"Dragon Firework spawned by {name}");
        //}
        public static void OnSystemMessageLootItem(S_SYSTEM_MESSAGE_LOOT_ITEM obj)
        {
            throw new NotImplementedException();
        }

        public static void OnAccomplishAchievement(S_ACCOMPLISH_ACHIEVEMENT obj)
        {
            throw new NotImplementedException();
        }

        public static void OnUpdateFriendInfo(S_UPDATE_FRIEND_INFO obj)
        {
            throw new NotImplementedException();
        }

        public static void OnNotifyToFriendsWalkIntoSameArea(S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA obj)
        {
            throw new NotImplementedException();
        }

        public static void OnAnswerInteractive(S_ANSWER_INTERACTIVE obj)
        {
            throw new NotImplementedException();
        }

        public static void OnDespawnUser(S_DESPAWN_USER obj)
        {
            throw new NotImplementedException();
        }

        public static void OnDespawnNpc(S_DESPAWN_NPC obj)
        {
            throw new NotImplementedException();
        }

        public static void OnSystemMessage(S_SYSTEM_MESSAGE obj)
        {
            throw new NotImplementedException();
        }
    }

}
