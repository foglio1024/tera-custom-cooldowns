using Nostrum;
using Nostrum.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TCC.Analysis;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Databases;
using TCC.Data.Map;
using TCC.Data.Pc;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.Processing;
using TCC.UI;
using TCC.UI.Windows;
using TCC.Update;
using TCC.Utilities;
using TCC.Utils;
using TeraDataLite;
using TeraPacketParser;
using TeraPacketParser.Messages;
using Player = TCC.Data.Pc.Player;
using Server = TeraPacketParser.TeraCommon.Game.Server;

namespace TCC
{
    public static class Game
    {
        private static ulong _foglioEid;
        private static bool _logged;
        private static bool _loadingScreen = true;
        private static bool _encounter;
        private static bool _inGameChatOpen;
        private static bool _inGameUiOn = true;

        public static readonly Dictionary<ulong, string> NearbyNPC = new Dictionary<ulong, string>();
        public static readonly Dictionary<ulong, Tuple<string, Class>> NearbyPlayers = new Dictionary<ulong, Tuple<string, Class>>();
        public static readonly GroupInfo Group = new GroupInfo();
        public static readonly GuildInfo Guild = new GuildInfo();
        public static Server Server { get; private set; } = new Server("Unknown", "Unknown", "0.0.0.0", 0);
        public static Account Account { get; set; } = new Account();
        public static string Language => DB.ServerDatabase.StringLanguage;

        public static bool LoadingScreen
        {
            get => _loadingScreen;
            set
            {
                if (_loadingScreen == value) return;
                _loadingScreen = value;
                App.BaseDispatcher.InvokeAsync(() => LoadingScreenChanged?.Invoke());
            }
        }

        public static bool Encounter
        {
            get => _encounter;
            set
            {
                if (_encounter == value) return;
                _encounter = value;
                App.BaseDispatcher.InvokeAsync(() => EncounterChanged?.Invoke());
            }
        }

        public static bool Combat
        {
            get => Me?.IsInCombat ?? false;
            set
            {
                if (Combat == value) return;
                Me.IsInCombat = value;
                App.BaseDispatcher.InvokeAsync(() => CombatChanged?.Invoke()); // check logs for other exceptions here
            }
        }

        public static bool Logged
        {
            get => _logged;
            set
            {
                if (_logged == value) return;
                _logged = value;
                App.BaseDispatcher.InvokeAsync(() => LoggedChanged?.Invoke());
            }
        }

        public static bool InGameUiOn
        {
            get => _inGameUiOn;
            set
            {
                if (_inGameUiOn == value) return;
                _inGameUiOn = value;
                GameUiModeChanged?.Invoke();
            }
        }

        public static bool InGameChatOpen
        {
            get => _inGameChatOpen;
            set
            {
                if (_inGameChatOpen == value) return;
                _inGameChatOpen = value;
                ChatModeChanged?.Invoke();
            }
        }

        public static int CurrentZoneId { get; private set; }
        public static List<FriendData> FriendList { get; private set; } = new List<FriendData>();
        public static List<string> BlockList { get; } = new List<string>();
        public static AbnormalityTracker CurrentAbnormalityTracker { get; private set; } = new AbnormalityTracker();

        public static bool IsMe(ulong eid)
        {
            return eid == Me.EntityId;
        }

        public static event Action ChatModeChanged = null!;
        public static event Action GameUiModeChanged = null!;
        public static event Action EncounterChanged = null!;
        public static event Action CombatChanged = null!;
        public static event Action LoadingScreenChanged = null!;
        public static event Action LoggedChanged = null!;
        public static event Action DatabaseLoaded = null!;
        public static event Action Teleported = null!;
        public static event Action SkillStarted = null!;

        public static Player Me { get; } = new Player();
        public static TccDatabase DB { get; private set; } = null!;

        public static bool CivilUnrestZone => CurrentZoneId == 152;
        public static bool IsInDungeon => DB.MapDatabase.IsDungeon(CurrentZoneId);
        public static string CurrentAccountNameHash { get; private set; } = "";


        public static async Task InitAsync()
        {
            PacketAnalyzer.ProcessorReady += InstallHooks;
            await InitDatabasesAsync(string.IsNullOrEmpty(App.Settings.LastLanguage)
                ? "EU-EN"
                : App.Settings.LastLanguage);
            KeyboardHook.Instance.RegisterCallback(App.Settings.ReturnToLobbyHotkey, OnReturnToLobbyHotkeyPressed);
        }

        private static async Task InitDatabasesAsync(string lang)
        {
            await Task.Factory.StartNew(() => InitDatabases(lang));
            DatabaseLoaded?.Invoke();
        }

        private static void InitDatabases(string lang)
        {
            UpdateManager.CheckServersFile();
            var samedb = DB?.Language == lang;
            var updated = false;
            if (!samedb)
            {
                DB = new TccDatabase(lang);
            }
            DB?.CheckVersion();
            if (DB?.IsUpToDate == false)
            {
                if (!App.Loading)
                {
                    Log.N("TCC", SR.UpdatingDatabase, NotificationType.Warning, 5000);
                    Log.Chat(SR.UpdatingDatabase);
                }

                DB.DownloadOutdatedDatabases();
                updated = true;
            }

            if (DB?.Exists == false)
            {
                var res = TccMessageBox.Show(SR.CannotLoadDbForLang(lang), MessageBoxType.ConfirmationWithYesNoCancel);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        WindowManager.SettingsWindow.ShowDialogAtPage(9);
                        InitDatabases(App.Settings.LastLanguage);
                        break;
                    case MessageBoxResult.No:
                        InitDatabases("EU-EN");
                        break;
                    case MessageBoxResult.Cancel:
                        App.Close();
                        break;
                }
            }
            else
            {
                if (!samedb || updated)
                {
                    DB?.Load();
                }
            }
        }

        private static void InstallHooks()
        {
            PacketAnalyzer.Sniffer.NewConnection += OnConnected;
            PacketAnalyzer.Sniffer.EndConnection += OnDisconnected;

            // db stuff
            PacketAnalyzer.Processor.Hook<C_LOGIN_ARBITER>(OnLoginArbiter);

            // player stuff
            PacketAnalyzer.Processor.Hook<S_LOGIN>(OnLogin);
            PacketAnalyzer.Processor.Hook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
            PacketAnalyzer.Processor.Hook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);

            // ep
            PacketAnalyzer.Processor.Hook<S_RESET_EP_PERK>(OnResetEpPerk);
            PacketAnalyzer.Processor.Hook<S_LEARN_EP_PERK>(OnLearnEpPerk);
            PacketAnalyzer.Processor.Hook<S_LOAD_EP_INFO>(OnLoadEpInfo);

            // guild
            PacketAnalyzer.Processor.Hook<S_GUILD_MEMBER_LIST>(OnGuildMemberList);
            PacketAnalyzer.Processor.Hook<S_CHANGE_GUILD_CHIEF>(OnChangeGuildChief);
            PacketAnalyzer.Processor.Hook<S_NOTIFY_GUILD_QUEST_URGENT>(OnNotifyGuildQuestUrgent);
            PacketAnalyzer.Processor.Hook<S_GET_USER_GUILD_LOGO>(OnGetUserGuildLogo);

            // abnormality
            PacketAnalyzer.Processor.Hook<S_ABNORMALITY_BEGIN>(OnAbnormalityBegin);
            PacketAnalyzer.Processor.Hook<S_ABNORMALITY_REFRESH>(OnAbnormalityRefresh);
            PacketAnalyzer.Processor.Hook<S_ABNORMALITY_END>(OnAbnormalityEnd);

            // guardian
            PacketAnalyzer.Processor.Hook<S_FIELD_EVENT_ON_ENTER>(OnFieldEventOnEnter);
            PacketAnalyzer.Processor.Hook<S_FIELD_EVENT_ON_LEAVE>(OnFieldEventOnLeave);
            PacketAnalyzer.Processor.Hook<S_FIELD_POINT_INFO>(OnFieldPointInfo);

            //
            PacketAnalyzer.Processor.Hook<S_USER_STATUS>(OnUserStatus);
            PacketAnalyzer.Processor.Hook<S_GET_USER_LIST>(OnGetUserList);
            PacketAnalyzer.Processor.Hook<S_LOAD_TOPO>(OnLoadTopo);
            PacketAnalyzer.Processor.Hook<S_ACCOUNT_PACKAGE_LIST>(OnAccountPackageList);
            PacketAnalyzer.Processor.Hook<S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA>(OnNotifyToFriendsWalkIntoSameArea);
            PacketAnalyzer.Processor.Hook<S_UPDATE_FRIEND_INFO>(OnUpdateFriendInfo);
            PacketAnalyzer.Processor.Hook<S_ACCOMPLISH_ACHIEVEMENT>(OnAccomplishAchievement);
            PacketAnalyzer.Processor.Hook<S_SYSTEM_MESSAGE_LOOT_ITEM>(OnSystemMessageLootItem);
            PacketAnalyzer.Processor.Hook<S_SYSTEM_MESSAGE>(OnSystemMessage);
            PacketAnalyzer.Processor.Hook<S_SPAWN_ME>(OnSpawnMe);
            PacketAnalyzer.Processor.Hook<S_SPAWN_USER>(OnSpawnUser);
            PacketAnalyzer.Processor.Hook<S_SPAWN_NPC>(OnSpawnNpc);
            PacketAnalyzer.Processor.Hook<S_DESPAWN_NPC>(OnDespawnNpc);
            PacketAnalyzer.Processor.Hook<S_DESPAWN_USER>(OnDespawnUser);
            PacketAnalyzer.Processor.Hook<S_START_COOLTIME_ITEM>(OnStartCooltimeItem);
            PacketAnalyzer.Processor.Hook<S_START_COOLTIME_SKILL>(OnStartCooltimeSkill);
            PacketAnalyzer.Processor.Hook<S_FRIEND_LIST>(OnFriendList);
            PacketAnalyzer.Processor.Hook<S_USER_BLOCK_LIST>(OnUserBlockList);
            PacketAnalyzer.Processor.Hook<S_CHAT>(OnChat);
            PacketAnalyzer.Processor.Hook<S_PRIVATE_CHAT>(OnPrivateChat);
            PacketAnalyzer.Processor.Hook<S_WHISPER>(OnWhisper);
            PacketAnalyzer.Processor.Hook<S_BOSS_GAGE_INFO>(OnBossGageInfo);
            PacketAnalyzer.Processor.Hook<S_CREATURE_CHANGE_HP>(OnCreatureChangeHp);

            PacketAnalyzer.Processor.Hook<S_FIN_INTER_PARTY_MATCH>(OnFinInterPartyMatch);
            PacketAnalyzer.Processor.Hook<S_BATTLE_FIELD_ENTRANCE_INFO>(OnBattleFieldEntranceInfo);
            PacketAnalyzer.Processor.Hook<S_BEGIN_THROUGH_ARBITER_CONTRACT>(OnRequestContract);
            PacketAnalyzer.Processor.Hook<S_TRADE_BROKER_DEAL_SUGGESTED>(OnTradeBrokerDealSuggested);

            PacketAnalyzer.Processor.Hook<S_PARTY_MEMBER_LIST>(OnPartyMemberList);
            PacketAnalyzer.Processor.Hook<S_LEAVE_PARTY>(OnLeaveParty);
            PacketAnalyzer.Processor.Hook<S_BAN_PARTY>(OnBanParty);
            PacketAnalyzer.Processor.Hook<S_CHANGE_PARTY_MANAGER>(OnChangePartyManager);
            PacketAnalyzer.Processor.Hook<S_LEAVE_PARTY_MEMBER>(OnLeavePartyMember);
            PacketAnalyzer.Processor.Hook<S_BAN_PARTY_MEMBER>(OnBanPartyMember);

            //PacketAnalyzer.Processor.Hook<S_FATIGABILITY_POINT>(OnFatigabilityPoint);
        }

        private static void OnConnected(Server server)
        {
            Server = server;
        }

        private static Laurel GetLaurel(uint pId)
        {
            var ch = Account.Characters.FirstOrDefault(x => x.Id == pId);
            return ch?.Laurel ?? Laurel.None;
        }

        public static void SetEncounter(float curHP, float maxHP)
        {
            if (maxHP > curHP)
            {
                Encounter = true;
            }
            else if (maxHP == curHP || curHP == 0)
            {
                Encounter = false;
            }
        }

        public static void SetSorcererElementsBoost(bool f, bool i, bool a)
        {
            Me.FireBoost = f;
            Me.IceBoost = i;
            Me.ArcaneBoost = a;
        }

        private static void OnReturnToLobbyHotkeyPressed()
        {
            if (!Logged
              || LoadingScreen
              || Combat
              || !StubInterface.Instance.IsStubAvailable) return;

            WindowManager.ViewModels.LfgVM.ForceStopPublicize();
            StubInterface.Instance.StubClient.ReturnToLobby();
        }

        private static void OnDisconnected()
        {
            Me.ClearAbnormalities();
            Logged = false;
            LoadingScreen = true;
            App.Settings.Save();
        }

        private static void SetAbnormalityTracker(Class c)
        {
            CurrentAbnormalityTracker = c switch
            {
                Class.Warrior => new WarriorAbnormalityTracker(),
                Class.Lancer => new LancerAbnormalityTracker(),
                Class.Slayer => new SlayerAbnormalityTracker(),
                Class.Berserker => new BerserkerAbnormalityTracker(),
                Class.Sorcerer => new SorcererAbnormalityTracker(),
                Class.Archer => new ArcherAbnormalityTracker(),
                Class.Priest => new PriestAbnormalityTracker(),
                Class.Mystic => new MysticAbnormalityTracker(),
                Class.Reaper => new ReaperAbnormalityTracker(),
                Class.Gunner => new GunnerAbnormalityTracker(),
                Class.Brawler => new BrawlerAbnormalityTracker(),
                Class.Ninja => new NinjaAbnormalityTracker(),
                Class.Valkyrie => new ValkyrieAbnormalityTracker(),
                _ => new AbnormalityTracker()
            };
        }
        private static void CheckChatMention(ParsedMessage m)
        {

            string author = "", txt = "", strCh = "";

            switch (m)
            {
                case S_WHISPER w:
                    txt = ChatUtils.GetPlainText(w.Message).UnescapeHtml();
                    if (!TccUtils.CheckMention(txt)) return;
                    author = w.Author;
                    strCh = TccUtils.ChatChannelToName(ChatChannel.ReceivedWhisper);
                    break;
                case S_CHAT c:
                    txt = ChatUtils.GetPlainText(c.Message).UnescapeHtml();
                    if (!TccUtils.CheckMention(txt)) return;
                    author = c.AuthorName;
                    strCh = TccUtils.ChatChannelToName((ChatChannel)c.Channel);
                    break;
                case S_PRIVATE_CHAT p:
                    txt = ChatUtils.GetPlainText(p.Message).UnescapeHtml();
                    if (!TccUtils.CheckMention(txt)) return;
                    author = p.AuthorName;
                    strCh = TccUtils.ChatChannelToName((ChatChannel)p.Channel);
                    break;

            }


            TccUtils.CheckWindowNotify(txt, $"{author} - {strCh}");
            TccUtils.CheckDiscordNotify($"`{strCh}` {txt}", author);
        }

        #region Hooks

        private static void OnPlayerStatUpdate(S_PLAYER_STAT_UPDATE m)
        {
            Me.MaxCoins = m.MaxCoins;
            Me.Coins = m.Coins;

            switch (Me.Class)
            {
                case Class.Sorcerer:
                    Me.Fire = m.Fire;
                    Me.Ice = m.Ice;
                    Me.Arcane = m.Arcane;
                    break;
                case Class.Warrior:
                    Me.StacksCounter.Val = m.Edge;
                    break;
            }
        }
        private static void OnTradeBrokerDealSuggested(S_TRADE_BROKER_DEAL_SUGGESTED m)
        {
            DB.ItemsDatabase.Items.TryGetValue((uint)m.Item, out var i);
            TccUtils.CheckWindowNotify($"New broker offer for {m.Amount} <{i?.Name ?? m.Item.ToString()}> from {m.Name}", "Broker offer");
            TccUtils.CheckDiscordNotify($"New broker offer for {m.Amount} **<{i?.Name ?? m.Item.ToString()}>**", m.Name);
        }
        private static void OnRequestContract(S_BEGIN_THROUGH_ARBITER_CONTRACT p)
        {
            if (p.Type != S_BEGIN_THROUGH_ARBITER_CONTRACT.RequestType.PartyInvite) return;
            TccUtils.CheckWindowNotify($"{p.Sender} invited you to join a party", "Party invite");
            TccUtils.CheckDiscordNotify($"**{p.Sender}** invited you to join a party", "TCC");
        }
        private static void OnBanPartyMember(S_BAN_PARTY_MEMBER obj)
        {
            Group.Remove(obj.PlayerId, obj.ServerId);
        }
        private static void OnLeavePartyMember(S_LEAVE_PARTY_MEMBER obj)
        {
            Group.Remove(obj.PlayerId, obj.ServerId);
        }
        private static void OnChangePartyManager(S_CHANGE_PARTY_MANAGER p)
        {
            Group.ChangeLeader(p.Name);
        }
        private static void OnBanParty(S_BAN_PARTY p)
        {
            Group.Disband();
        }
        private static void OnLeaveParty(S_LEAVE_PARTY p)
        {
            Group.Disband();
        }
        private static void OnPartyMemberList(S_PARTY_MEMBER_LIST p)
        {
            Group.SetGroup(p.Members, p.Raid);
        }

        private static void OnBattleFieldEntranceInfo(S_BATTLE_FIELD_ENTRANCE_INFO p)
        {
            // TODO: add discord notification after events revamp
            Log.N("Instance Matching", SR.BgMatchingComplete, NotificationType.Success);
            Log.F($"Zone: {p.Zone}\nId: {p.Id}\nData: {p.Data.Array.ToHexString()}", "S_BATTLE_FIELD_ENTRANCE_INFO.txt");
        }
        private static void OnFinInterPartyMatch(S_FIN_INTER_PARTY_MATCH p)
        {
            Log.N("Instance Matching", SR.DungMatchingComplete, NotificationType.Success);
            Log.F($"Zone: {p.Zone}\nData: {p.Data.Array.ToHexString()}", "S_FIN_INTER_PARTY_MATCH.txt");
        }
        private static void OnCreatureChangeHp(S_CREATURE_CHANGE_HP m)
        {
            if (IsMe(m.Target)) return;
            SetEncounter(m.CurrentHP, m.MaxHP);
        }
        private static void OnBossGageInfo(S_BOSS_GAGE_INFO m)
        {
            SetEncounter(m.CurrentHP, m.MaxHP);
        }
        private static void OnWhisper(S_WHISPER p)
        {
            if (p.Recipient != Me.Name) return;
            CheckChatMention(p);
        }

        private static void OnChat(S_CHAT m)
        {
            #region Greet meme
            if ((ChatChannel)m.Channel == ChatChannel.Greet
                && (m.AuthorName == "Foglio"
                    || m.AuthorName == "Folyemi"))
                Log.N("owo", SR.GreetMemeContent, NotificationType.Success, 3000);
            #endregion

            #region Global trade angery
            if (m.AuthorName == Me.Name)
            {
                if ((ChatChannel)m.Channel != ChatChannel.Global) return;

                if (!(m.Message.IndexOf("WTS", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                      m.Message.IndexOf("WTB", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                      m.Message.IndexOf("WTT", StringComparison.InvariantCultureIgnoreCase) >= 0)) return;
                Log.N("REEEEEEEEEEEEEEEEEEEEEE", SR.GlobalSellAngery, NotificationType.Error);

            }
            #endregion

            if (BlockList.Contains(m.AuthorName)) return;

            CheckChatMention(m);
        }

        private static void OnPrivateChat(S_PRIVATE_CHAT m)
        {
            if (BlockList.Contains(m.AuthorName)) return;
            CheckChatMention(m);
        }
        private static void OnSpawnNpc(S_SPAWN_NPC p)
        {
            if (!DB.MonsterDatabase.TryGetMonster(p.TemplateId, p.HuntingZoneId, out var m)) return;
            NearbyNPC[p.EntityId] = m.Name;
            FlyingGuardianDataProvider.InvokeProgressChanged();
        }
        private static void OnUpdateFriendInfo(S_UPDATE_FRIEND_INFO x)
        {
            if (!x.Online) return;
            SystemMessagesProcessor.AnalyzeMessage($"@0\vUserName\v{x.Name}", "SMT_FRIEND_IS_CONNECTED");
        }
        private static void OnAccomplishAchievement(S_ACCOMPLISH_ACHIEVEMENT x)
        {
            SystemMessagesProcessor.AnalyzeMessage($"@0\vAchievementName\v@achievement:{x.AchievementId}", "SMT_ACHIEVEMENT_GRADE0_CLEAR_MESSAGE");
        }
        private static void OnSystemMessageLootItem(S_SYSTEM_MESSAGE_LOOT_ITEM x)
        {
            App.BaseDispatcher.InvokeAsync(() =>
            {
                try
                {
                    SystemMessagesProcessor.AnalyzeMessage(x.SysMessage);
                }
                catch (Exception)
                {
                    Log.CW($"Failed to parse sysmsg: {x.SysMessage}");
                    Log.F($"Failed to parse sysmsg: {x.SysMessage}");
                }
            });
        }
        private static void OnSystemMessage(S_SYSTEM_MESSAGE x)
        {
            if (DB.SystemMessagesDatabase.IsHandledInternally(x.Message)) return;
            App.BaseDispatcher.InvokeAsync(() =>
            {
                try
                {
                    SystemMessagesProcessor.AnalyzeMessage(x.Message);
                }
                catch (Exception)
                {
                    Log.CW($"Failed to parse system message: {x.Message}");
                    Log.F($"Failed to parse system message: {x.Message}");
                }
            });
        }
        private static void OnDespawnNpc(S_DESPAWN_NPC p)
        {
            NearbyNPC.Remove(p.Target);
            FlyingGuardianDataProvider.InvokeProgressChanged();
            AbnormalityTracker.CheckMarkingOnDespawn(p.Target);
        }
        private static void OnDespawnUser(S_DESPAWN_USER p)
        {
            #region Aura meme
            if (p.EntityId == _foglioEid) Me.EndAbnormality(10241024);
            #endregion
            NearbyPlayers.Remove(p.EntityId);
        }
        private static void OnSpawnUser(S_SPAWN_USER p)
        {
            #region Aura meme
            switch (p.Name)
            {
                case "Foglio":
                case "Fogolio":
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
                    if (CivilUnrestZone) break;
                    _foglioEid = p.EntityId;
                    var ab = DB.AbnormalityDatabase.Abnormalities[10241024];
                    Me.UpdateAbnormality(ab, int.MaxValue, 1);
                    SystemMessagesProcessor.AnalyzeMessage($"@0\vAbnormalName\v{ab.Name}", "SMT_BATTLE_BUFF_DEBUFF");
                    break;
            }

            #endregion
            NearbyPlayers[p.EntityId] = new Tuple<string, Class>(p.Name, TccUtils.ClassFromModel(p.TemplateId));
        }
        private static void OnSpawnMe(S_SPAWN_ME p)
        {
            NearbyNPC.Clear();
            NearbyPlayers.Clear();
            AbnormalityTracker.ClearMarkedTargets();
            FlyingGuardianDataProvider.Stacks = 0;
            FlyingGuardianDataProvider.StackType = FlightStackType.None;
            FlyingGuardianDataProvider.InvokeProgressChanged();
            // was done with timer before, test it
            Task.Delay(2000).ContinueWith(t =>
            {
                LoadingScreen = false;
                WindowManager.VisibilityManager.RefreshDim();

                #region Fear Inoculum
                if (!App.FI) return;
                var ab = DB.AbnormalityDatabase.Abnormalities[30082019];
                Me.UpdateAbnormality(ab, int.MaxValue, 1);
                SystemMessagesProcessor.AnalyzeMessage($"@0\vAbnormalName\v{ab.Name}", "SMT_BATTLE_BUFF_DEBUFF");

                #endregion            
            });
        }
        private static void OnAccountPackageList(S_ACCOUNT_PACKAGE_LIST m)
        {
            Account.IsElite = m.IsElite;
        }
        private static void OnLoadTopo(S_LOAD_TOPO m)
        {
            LoadingScreen = true;
            Encounter = false;
            CurrentZoneId = m.Zone;
            Teleported?.Invoke();
            Me.ClearAbnormalities();

        }
        private static void OnGetUserList(S_GET_USER_LIST m)
        {
            if (PacketAnalyzer.Factory.ReleaseVersion == 0)
                Log.F("Warning: C_LOGIN_ARBITER not received.");

            Logged = false;
            Firebase.RegisterWebhook(App.Settings.WebhookUrlGuildBam, false);
            Firebase.RegisterWebhook(App.Settings.WebhookUrlFieldBoss, false);
            Me.ClearAbnormalities();

            foreach (var item in m.CharacterList)
            {
                var ch = Account.Characters.FirstOrDefault(x => x.Id == item.Id);
                if (ch != null)
                {
                    ch.Name = item.Name;
                    ch.Laurel = item.Laurel;
                    ch.Position = item.Position;
                    ch.GuildName = item.GuildName;
                    ch.Level = item.Level;
                    ch.LastLocation = new Location(item.LastWorldId, item.LastGuardId, item.LastSectionId);
                    ch.LastOnline = item.LastOnline;
                    ch.ServerName = Game.Server.Name;
                }
                else
                {
                    Account.Characters.Add(new Character(item));
                }
            }

        }
        private static void OnUserStatus(S_USER_STATUS m)
        {
            if (IsMe(m.EntityId)) Combat = m.IsInCombat;
        }
        private static void OnFieldEventOnLeave(S_FIELD_EVENT_ON_LEAVE p)
        {
            SystemMessagesProcessor.AnalyzeMessage("", "SMT_FIELD_EVENT_LEAVE");

            if (!StubInterface.Instance.IsStubAvailable
                || !StubInterface.Instance.IsFpsModAvailable
                || !App.Settings.FpsAtGuardian) return;
            StubInterface.Instance.StubClient.InvokeCommand("fps mode 1");
        }
        private static void OnFieldEventOnEnter(S_FIELD_EVENT_ON_ENTER p)
        {
            SystemMessagesProcessor.AnalyzeMessage("", "SMT_FIELD_EVENT_ENTER");

            if (!StubInterface.Instance.IsStubAvailable
                || !StubInterface.Instance.IsFpsModAvailable
                || !App.Settings.FpsAtGuardian) return;
            StubInterface.Instance.StubClient.InvokeCommand("fps mode 3");
        }
        private static void OnFieldPointInfo(S_FIELD_POINT_INFO p)
        {
            if (Account.CurrentCharacter == null) return;
            var old = Account.CurrentCharacter.GuardianInfo.Cleared;
            Account.CurrentCharacter.GuardianInfo.Claimed = p.Claimed;
            Account.CurrentCharacter.GuardianInfo.Cleared = p.Cleared;
            if (old == p.Cleared) return;
            SystemMessagesProcessor.AnalyzeMessage("@0", "SMT_FIELD_EVENT_REWARD_AVAILABLE");
        }
        private static void OnGetUserGuildLogo(S_GET_USER_GUILD_LOGO p)
        {
            if (p.GuildLogo == null) return;
            S_IMAGE_DATA.Database[p.GuildId] = p.GuildLogo;

            if (!Directory.Exists("resources/images/guilds")) Directory.CreateDirectory("resources/images/guilds");
            try
            {
                var clonebmp = (Bitmap)p.GuildLogo.Clone();
                clonebmp.Save(
                    Path.Combine(App.ResourcesPath, $"images/guilds/guildlogo_{Server.ServerId}_{p.GuildId}_{0}.bmp"),
                    ImageFormat.Bmp);
                clonebmp.Dispose();
            }
            catch (Exception e)
            {
                Log.F($"Error while saving guild logo: {e}");
            }
        }
        private static void OnGuildMemberList(S_GUILD_MEMBER_LIST m)
        {
            Task.Run(() => Guild.Set(m.Members, m.MasterId, m.MasterName));
        }
        private static void OnLoadEpInfo(S_LOAD_EP_INFO m)
        {
            if (!m.Perks.TryGetValue(851010, out var level)) return;
            EpDataProvider.SetManaBarrierPerkLevel(level);
        }
        private static void OnLearnEpPerk(S_LEARN_EP_PERK m)
        {
            if (!m.Perks.TryGetValue(851010, out var level)) return;
            EpDataProvider.SetManaBarrierPerkLevel(level);
        }
        private static void OnResetEpPerk(S_RESET_EP_PERK m)
        {
            if (!m.Success) return;
            EpDataProvider.SetManaBarrierPerkLevel(0);
        }
        private static void OnReturnToLobby(S_RETURN_TO_LOBBY m)
        {
            Logged = false;
            Me.ClearAbnormalities();

        }
        private static void OnLogin(S_LOGIN m)
        {
            Firebase.RegisterWebhook(App.Settings.WebhookUrlGuildBam, true);
            Firebase.RegisterWebhook(App.Settings.WebhookUrlFieldBoss, true);
            Firebase.SendUsageStatAsync();

            App.Settings.LastLanguage = Language;

            Logged = true;
            LoadingScreen = true;
            Encounter = false;
            Account.LoginCharacter(m.PlayerId);
            Guild.Clear();
            FriendList.Clear();
            BlockList.Clear();

            Server = DB.ServerDatabase.GetServer(m.ServerId) ?? Server;

            Me.Name = m.Name;
            Me.Class = m.CharacterClass;
            Me.EntityId = m.EntityId;
            Me.Level = m.Level;
            Me.PlayerId = m.PlayerId;
            Me.ServerId = m.ServerId;
            Me.Laurel = GetLaurel(Me.PlayerId);
            Me.ClearAbnormalities();
            Me.StacksCounter.SetClass(m.CharacterClass);

            WindowManager.ReloadPositions();
            GameEventManager.Instance.SetServerTimeZone(App.Settings.LastLanguage);
            InitDatabases(App.Settings.LastLanguage);
            SetAbnormalityTracker(m.CharacterClass);
        }
        private static void OnLoginArbiter(C_LOGIN_ARBITER m)
        {
            CurrentAccountNameHash = HashUtils.GenerateHash(m.AccountName);
            DB.ServerDatabase.Language = m.Language == LangEnum.EN && Server.Region == "RU" ? LangEnum.RU : LangEnum.EN;
            App.Settings.LastLanguage = DB.ServerDatabase.StringLanguage;
            App.Settings.LastAccountNameHash = CurrentAccountNameHash;
        }
        private static void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
        {
            CurrentAbnormalityTracker?.CheckAbnormality(p);
            if (!IsMe(p.TargetId)) return;
            if (!DB.AbnormalityDatabase.GetAbnormality(p.AbnormalityId, out var ab) || !ab.CanShow) return;
            ab.Infinity = p.Duration >= int.MaxValue / 2;
            Me.UpdateAbnormality(ab, p.Duration, p.Stacks);
            FlyingGuardianDataProvider.HandleAbnormal(p);
        }
        private static void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
        {
            CurrentAbnormalityTracker?.CheckAbnormality(p);
            if (!IsMe(p.TargetId)) return;
            if (!DB.AbnormalityDatabase.GetAbnormality(p.AbnormalityId, out var ab) || !ab.CanShow) return;
            ab.Infinity = p.Duration >= int.MaxValue / 2;
            Me.UpdateAbnormality(ab, p.Duration, p.Stacks);
            FlyingGuardianDataProvider.HandleAbnormal(p);

        }
        private static void OnAbnormalityEnd(S_ABNORMALITY_END p)
        {
            CurrentAbnormalityTracker?.CheckAbnormality(p);
            if (!IsMe(p.TargetId)) return;
            if (!DB.AbnormalityDatabase.GetAbnormality(p.AbnormalityId, out var ab) || !ab.CanShow) return;
            FlyingGuardianDataProvider.HandleAbnormal(p);
            Me.EndAbnormality(ab);

        }
        private static void OnStartCooltimeItem(S_START_COOLTIME_ITEM m)
        {
            App.BaseDispatcher.InvokeAsync(() => SkillStarted?.Invoke());
        }
        private static void OnStartCooltimeSkill(S_START_COOLTIME_SKILL m)
        {
            App.BaseDispatcher.InvokeAsync(() => SkillStarted?.Invoke());
        }
        private static void OnChangeGuildChief(S_CHANGE_GUILD_CHIEF m)
        {
            SystemMessagesProcessor.AnalyzeMessage($"@0\vName\v{Guild.NameOf(m.PlayerId)}", "SMT_GC_SYSMSG_GUILD_CHIEF_CHANGED");
            Guild.SetMaster(m.PlayerId, Guild.NameOf(m.PlayerId));
        }
        private static void OnNotifyGuildQuestUrgent(S_NOTIFY_GUILD_QUEST_URGENT p)
        {
            if (p.Type != S_NOTIFY_GUILD_QUEST_URGENT.GuildBamQuestType.Announce) return;

            var questName = DB.GuildQuestDatabase.GuildQuests.TryGetValue(p.QuestId, out var gq)
                ?
                 gq.Title
                    : "Defeat Guild BAM";
            var zone = DB.MonsterDatabase.GetZoneName(p.ZoneId);
            var name = DB.MonsterDatabase.GetName(p.TemplateId, p.ZoneId);
            SystemMessagesProcessor.AnalyzeMessage($"@0\vquestName\v{questName}\vnpcName\v{name}\vzoneName\v{zone}", "SMT_GQUEST_URGENT_NOTIFY");
        }
        private static void OnNotifyToFriendsWalkIntoSameArea(S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA x)
        {
            var friend = FriendList.FirstOrDefault(f => f.PlayerId == x.PlayerId);
            if (friend.Equals(default(FriendData))) return;
            var areaName = x.SectionId.ToString();
            try
            {
                areaName = DB.RegionsDatabase.GetZoneName(DB.MapDatabase.Worlds[x.WorldId].Guards[x.GuardId].Sections[x.SectionId].NameId);
            }
            catch (Exception)
            {
                // ignored
            }

            SystemMessagesProcessor.AnalyzeMessage($"@0\vUserName\v{friend.Name}\vAreaName\v{areaName}", "SMT_FRIEND_WALK_INTO_SAME_AREA");
        }
        private static void OnFriendList(S_FRIEND_LIST m)
        {
            FriendList = m.Friends;
        }
        private static void OnUserBlockList(S_USER_BLOCK_LIST m)
        {
            m.BlockedUsers.ForEach(u =>
            {
                if (BlockList.Contains(u)) return;
                BlockList.Add(u);
            });
        }
        //private static void OnFatigabilityPoint(S_FATIGABILITY_POINT p)
        //{
        //    var ppFactor = MathUtils.FactorCalc(p.CurrFatigability, p.MaxFatigability) * 100;

        //    Log.Chat(ChatUtils.Font("Production Points: ", R.Colors.MainColor.ToHex())
        //           + ChatUtils.Font($"{p.CurrFatigability}", R.Colors.GoldColor.ToHex())
        //           + ChatUtils.Font($"/{p.MaxFatigability} (", "cccccc")
        //           + ChatUtils.Font($"{ppFactor:F}%", R.Colors.MainColor.ToHex())
        //           + ChatUtils.Font($").", "cccccc")
        //        );
        //}

        #endregion
    }
}