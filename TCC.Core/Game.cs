using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Nostrum;
using Nostrum.Extensions;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Chat;
using TCC.Data.Databases;
using TCC.Data.Map;
using TCC.Data.Pc;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.UI;
using TCC.UI.Windows;
using TCC.Update;
using TCC.Utilities;
using TCC.Utils;
using TCC.ViewModels;
using TeraDataLite;
using TeraPacketParser;
using TeraPacketParser.Analysis;
using TeraPacketParser.Messages;
using TeraPacketParser.Sniffing;
using TeraPacketParser.TeraCommon.Game;

namespace TCC;

public static class Game
{
    static ulong _foglioEid;
    static bool _logged;
    static bool _loadingScreen = true;
    static bool _encounter;
    static bool _inGameChatOpen;
    static bool _inGameUiOn = true;

    public static readonly Dictionary<ulong, string> NearbyNPCs = new();
    public static readonly Dictionary<ulong, (string Name, Class Class)> NearbyPlayers = new();
    public static readonly GroupInfo Group = new();
    public static readonly GuildInfo Guild = new();
    public static readonly FriendList Friends = new();
    public static Server Server { get; private set; } = new("Unknown", "Unknown", "0.0.0.0", 0);
    public static Account Account { get; set; } = new();
    public static string Language => PacketAnalyzer.ServerDatabase.StringLanguage;

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
        get => Me.IsInCombat;
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
    public static List<string> BlockList { get; } = new();
    public static AbnormalityTracker CurrentAbnormalityTracker { get; private set; } = new();

    public static bool IsMe(ulong eid)
    {
        return eid == Me.EntityId;
    }

    public static bool IsMe(uint playerId, uint serverId)
    {
        return playerId == Me.PlayerId && serverId == Me.ServerId;
    }

    public static event Action? ChatModeChanged;

    public static event Action? GameUiModeChanged;

    public static event Action? EncounterChanged;

    public static event Action? CombatChanged;

    public static event Action? LoadingScreenChanged;

    public static event Action? LoggedChanged;

    public static event Action? DatabaseLoaded;

    public static event Action? Teleported;

    public static event Action? SkillStarted;

    public static event Action? LootDistributionWindowShowRequest;

    public static Player Me { get; } = new();
    public static TccDatabase? DB { get; private set; }

    public static bool CivilUnrestZone => CurrentZoneId == 152;
    public static bool IsInDungeon => DB!.MapDatabase.IsDungeon(CurrentZoneId);
    public static string CurrentAccountNameHash { get; private set; } = "";

    public static async Task InitAsync()
    {
        PacketAnalyzer.ProcessorReady += InstallHooks;

        await InitDatabasesAsync(string.IsNullOrEmpty(App.Settings.LastLanguage)
            ? "EU-EN"
            : App.Settings.LastLanguage);

        KeyboardHook.Instance.RegisterCallback(App.Settings.ReturnToLobbyHotkey, OnReturnToLobbyHotkeyPressed);

        StubMessageParser.SetUiModeEvent += OnSetUiMode;
        StubMessageParser.SetChatModeEvent += OnSetChatMode;
        StubMessageParser.HandleChatMessageEvent += OnStubChatMessage;
        StubMessageParser.HandleTranslatedMessageEvent += OnStubTranslatedMessage;
        StubMessageParser.HandleRawPacketEvent += OnRawPacket;
    }

    static void OnRawPacket(Message msg)
    {
        PacketAnalyzer.EnqueuePacket(msg);
    }

    static void OnStubChatMessage(string author, uint channel, string message)
    {
        if (!ChatManager.Instance.PrivateChannels.Any(x => x.Id == channel && x.Joined))
            ChatManager.Instance.CachePrivateMessage(channel, author, message);
        else
            ChatManager.Instance.AddChatMessage(
                ChatManager.Instance.Factory.CreateMessage((ChatChannel)ChatManager.Instance.PrivateChannels.FirstOrDefault(x =>
                    x.Id == channel && x.Joined).Index + 11, author, message));
    }
    static void OnStubTranslatedMessage(string author, uint channel, string message, bool gm)
    {
        ChatManager.Instance.HandleTranslation(author, channel, message, gm);
    }

    static void OnSetChatMode(bool b)
    {
        InGameChatOpen = b;
    }

    static void OnSetUiMode(bool b)
    {
        InGameUiOn = b;
    }

    static async Task InitDatabasesAsync(string lang)
    {
        await Task.Run(() => InitDatabases(lang));
        DatabaseLoaded?.Invoke();
    }

    static async Task InitDatabases(string lang)
    {
        await UpdateManager.CheckDatabaseHash();
        UpdateManager.CheckServersFile();
        var samedb = DB?.Language == lang;
        var updated = false;
        if (!samedb)
        {
            DB = new TccDatabase(lang);
        }
        DB!.CheckVersion();
        if (DB.IsUpToDate == false)
        {
            if (!App.Loading)
            {
                Log.N("TCC", SR.UpdatingDatabase, NotificationType.Warning, 5000);
                Log.Chat(SR.UpdatingDatabase);
            }

            DB.DownloadOutdatedDatabases();
            updated = true;
        }

        if (DB.Exists == false)
        {
            var res = TccMessageBox.Show(SR.CannotLoadDbForLang(lang), MessageBoxType.ConfirmationWithYesNoCancel);
            switch (res)
            {
                case MessageBoxResult.Yes:
                    WindowManager.SettingsWindow.ShowDialogAtPage(9);
                    await InitDatabases(App.Settings.LastLanguage);
                    break;

                case MessageBoxResult.No:
                    await InitDatabases("EU-EN");
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
                DB.Load();
            }
        }
    }

    static void InstallHooks()
    {
        PacketAnalyzer.Sniffer.NewConnection += OnConnected;
        PacketAnalyzer.Sniffer.EndConnection += OnDisconnected;

        PacketAnalyzer.Processor.Hook<C_CHECK_VERSION>(p => _ = OnCheckVersion(p));
        PacketAnalyzer.Processor.Hook<C_LOGIN_ARBITER>(p => _ = OnLoginArbiter(p));

        // player stuff
        PacketAnalyzer.Processor.Hook<S_LOGIN>(p => _ = OnLogin(p));
        PacketAnalyzer.Processor.Hook<S_RETURN_TO_LOBBY>(OnReturnToLobby);
        PacketAnalyzer.Processor.Hook<S_PLAYER_STAT_UPDATE>(OnPlayerStatUpdate);
        PacketAnalyzer.Processor.Hook<S_CREATURE_LIFE>(OnCreatureLife);
        PacketAnalyzer.Processor.Hook<S_ABNORMALITY_DAMAGE_ABSORB>(OnAbnormalityDamageAbsorb);
        PacketAnalyzer.Processor.Hook<S_CREATURE_CHANGE_HP>(OnCreatureChangeHp);
        PacketAnalyzer.Processor.Hook<S_PLAYER_CHANGE_MP>(OnPlayerChangeMp);
        PacketAnalyzer.Processor.Hook<S_PLAYER_CHANGE_STAMINA>(OnPlayerChangeStamina);

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
        PacketAnalyzer.Processor.Hook<S_CHANGE_FRIEND_STATE>(OnChangeFriendState);
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

    static async Task OnCheckVersion(C_CHECK_VERSION p)
    {
        var opcPath = Path.Combine(App.DataPath, $"opcodes/protocol.{p.Versions[0]}.map").Replace("\\", "/");
        if (!File.Exists(opcPath))
        {
            if (!Directory.Exists(Path.Combine(App.DataPath, "opcodes")))
                Directory.CreateDirectory(Path.Combine(App.DataPath, "opcodes"));

            if (PacketAnalyzer.Sniffer is ToolboxSniffer tbs && !await tbs.ControlConnection.DumpMap(opcPath, "protocol"))
            {
                if (!OpcodeDownloader.DownloadOpcodesIfNotExist(p.Versions[0], Path.Combine(App.DataPath, "opcodes/")))
                {
                    TccMessageBox.Show(SR.UnknownClientVersion(p.Versions[0]), MessageBoxType.Error);
                    App.Close();
                    return;
                }
            }
        }

        OpCodeNamer opcNamer;
        try
        {
            opcNamer = new OpCodeNamer(opcPath);
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case OverflowException:
                case ArgumentException:
                    TccMessageBox.Show(SR.InvalidOpcodeFile(ex.Message), MessageBoxType.Error);
                    Log.F(ex.ToString());
                    App.Close();
                    break;
            }
            return;
        }

        PacketAnalyzer.Factory!.Set(p.Versions[0], opcNamer);
        PacketAnalyzer.Sniffer.Connected = true;
    }

    static void OnConnected(Server server)
    {
        Server = server;
        //if (App.Settings.DontShowFUBH == false) App.FUBH();

        WindowManager.TrayIcon.Connected = true;
        WindowManager.TrayIcon.Text = $"{App.AppVersion} - connected";

        //if (Game.Server.Region == "EU")
        //    TccMessageBox.Show("WARNING",
        //        "Official statement from Gameforge:\n\n don't combine partners or pets! It will lock you out of your character permanently.\n\n This message will keep showing until next release.");
    }

    static Laurel GetLaurel(uint pId)
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

    static void OnReturnToLobbyHotkeyPressed()
    {
        if (!Logged
          || LoadingScreen
          || Combat
          || !StubInterface.Instance.IsStubAvailable) return;

        WindowManager.ViewModels.LfgVM.ForceStopPublicize();
        StubInterface.Instance.StubClient.ReturnToLobby();
    }

    static void OnDisconnected()
    {
        WindowManager.TrayIcon.Connected = false;
        WindowManager.TrayIcon.Text = $"{App.AppVersion} - not connected";
        Firebase.Dispose();
        Me.ClearAbnormalities();
        Logged = false;
        LoadingScreen = true;
        App.Settings.Save();
        if (App.ToolboxMode && UpdateManager.UpdateAvailable) App.Close();
    }

    static void SetAbnormalityTracker(Class c)
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

    static void CheckChatMention(ParsedMessage m)
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
                author = c.Name;
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

    internal static void ShowLootDistributionWindow()
    {
        LootDistributionWindowShowRequest?.Invoke();
    }

    #region Hooks

    static void OnPlayerStatUpdate(S_PLAYER_STAT_UPDATE m)
    {
        Me.MaxCoins = m.MaxCoins;
        Me.Coins = m.Coins;
        Me.ItemLevel = m.Ilvl;
        Me.Level = m.Level;
        Me.CritFactor = m.TotalCritFactor;
        Me.MaxHP = m.MaxHP;
        Me.MaxMP = m.MaxMP;
        Me.MaxST = m.MaxST + m.BonusST;
        Me.CurrentHP = m.CurrentHP;
        Me.CurrentMP = m.CurrentMP;
        Me.CurrentST = m.CurrentST;
        Me.MagicalResistance = m.TotalMagicalResistance;

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

    static void OnCreatureLife(S_CREATURE_LIFE m)
    {
        if (!IsMe(m.Target)) return;
        Me.IsAlive = m.Alive;
    }

    static void OnTradeBrokerDealSuggested(S_TRADE_BROKER_DEAL_SUGGESTED m)
    {
        DB!.ItemsDatabase.Items.TryGetValue((uint)m.Item, out var i);
        TccUtils.CheckWindowNotify($"New broker offer for {m.Amount} <{i?.Name ?? m.Item.ToString()}> from {m.Name}", "Broker offer");
        TccUtils.CheckDiscordNotify($"New broker offer for {m.Amount} **<{i?.Name ?? m.Item.ToString()}>**", m.Name);
    }

    static void OnRequestContract(S_BEGIN_THROUGH_ARBITER_CONTRACT p)
    {
        if (p.Type != S_BEGIN_THROUGH_ARBITER_CONTRACT.RequestType.PartyInvite) return;
        TccUtils.CheckWindowNotify($"{p.Sender} invited you to join a party", "Party invite");
        TccUtils.CheckDiscordNotify($"**{p.Sender}** invited you to join a party", "TCC");
    }

    static void OnBanPartyMember(S_BAN_PARTY_MEMBER obj)
    {
        Group.RemoveMember(obj.PlayerId, obj.ServerId);
    }

    static void OnLeavePartyMember(S_LEAVE_PARTY_MEMBER obj)
    {
        Group.RemoveMember(obj.PlayerId, obj.ServerId);
    }

    static void OnChangePartyManager(S_CHANGE_PARTY_MANAGER p)
    {
        Group.ChangeLeader(p.Name);
    }

    static void OnBanParty(S_BAN_PARTY p)
    {
        Group.Disband();
    }

    static void OnLeaveParty(S_LEAVE_PARTY p)
    {
        Group.Disband();
    }

    static void OnPartyMemberList(S_PARTY_MEMBER_LIST p)
    {
        Group.UpdateComposition(p.Members, p.Raid);
    }

    static void OnBattleFieldEntranceInfo(S_BATTLE_FIELD_ENTRANCE_INFO p)
    {
        // TODO: add discord notification after events revamp
        Log.N("Instance Matching", SR.BgMatchingComplete, NotificationType.Success);
        Log.F($"Zone: {p.Zone}\nId: {p.Id}\nData: {p.Data.Array?.ToHexString()}", "S_BATTLE_FIELD_ENTRANCE_INFO.txt");
    }

    static void OnFinInterPartyMatch(S_FIN_INTER_PARTY_MATCH p)
    {
        Log.N("Instance Matching", SR.DungMatchingComplete, NotificationType.Success);
        Log.F($"Zone: {p.Zone}\nData: {p.Data.Array?.ToHexString()}", "S_FIN_INTER_PARTY_MATCH.txt");
    }

    static void OnCreatureChangeHp(S_CREATURE_CHANGE_HP m)
    {
        if (IsMe(m.Target))
        {
            Me.MaxHP = m.MaxHP;
            Me.CurrentHP = m.CurrentHP;
        }
        else
        {
            SetEncounter(m.CurrentHP, m.MaxHP);
        }
    }

    static void OnPlayerChangeMp(S_PLAYER_CHANGE_MP m)
    {
        if (!IsMe(m.Target)) return;
        Me.MaxMP = m.MaxMP;
        Me.CurrentMP = m.CurrentMP;
    }

    static void OnPlayerChangeStamina(S_PLAYER_CHANGE_STAMINA m)
    {
        Me.MaxST = m.MaxST + m.BonusST;
        Me.CurrentST = m.CurrentST;
    }

    static void OnAbnormalityDamageAbsorb(S_ABNORMALITY_DAMAGE_ABSORB p)
    {
        // todo: add chat message too
        if (!IsMe(p.Target) || Me.CurrentShield < 0) return;
        Me.DamageShield(p.Damage);
    }

    static void OnBossGageInfo(S_BOSS_GAGE_INFO m)
    {
        SetEncounter(m.CurrentHP, m.MaxHP);
    }

    static void OnWhisper(S_WHISPER p)
    {
        if (p.Recipient != Me.Name) return;
        CheckChatMention(p);
    }

    static void OnChat(S_CHAT m)
    {
        #region Greet meme

        if ((ChatChannel)m.Channel == ChatChannel.Greet
            && m.Name is "Foglio" or "Folyemi")
            Log.N("owo", SR.GreetMemeContent, NotificationType.Success, 3000);

        #endregion Greet meme

        #region Global trade angery

        if (m.Name == Me.Name)
        {
            if ((ChatChannel)m.Channel != ChatChannel.Global) return;

            if (!(m.Message.IndexOf("WTS", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                  m.Message.IndexOf("WTB", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                  m.Message.IndexOf("WTT", StringComparison.InvariantCultureIgnoreCase) >= 0)) return;
            Log.N("REEEEEEEEEEEEEEEEEEEEEE", SR.GlobalSellAngery, NotificationType.Error);
        }

        #endregion Global trade angery

        if (BlockList.Contains(m.Name)) return;

        CheckChatMention(m);
    }

    static void OnPrivateChat(S_PRIVATE_CHAT m)
    {
        if (BlockList.Contains(m.AuthorName)) return;
        CheckChatMention(m);
    }

    static void OnSpawnNpc(S_SPAWN_NPC p)
    {
        if (!DB!.MonsterDatabase.TryGetMonster(p.TemplateId, p.HuntingZoneId, out var m)) return;
        NearbyNPCs[p.EntityId] = m.Name;
        FlyingGuardianDataProvider.InvokeProgressChanged();
    }

    static void OnChangeFriendState(S_CHANGE_FRIEND_STATE p)
    {
        Log.Chat($"Changed friend state: {p.PlayerId} {p.FriendStatus}");
    }

    static void OnUpdateFriendInfo(S_UPDATE_FRIEND_INFO p)
    {
        Friends.UpdateFriendInfo(p.FriendUpdates);
    }

    static void OnAccomplishAchievement(S_ACCOMPLISH_ACHIEVEMENT x)
    {
        SystemMessagesProcessor.AnalyzeMessage($"@0\vAchievementName\v@achievement:{x.AchievementId}", "SMT_ACHIEVEMENT_GRADE0_CLEAR_MESSAGE");
    }

    static void OnSystemMessageLootItem(S_SYSTEM_MESSAGE_LOOT_ITEM x)
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

    static void OnSystemMessage(S_SYSTEM_MESSAGE x)
    {
        if (DB!.SystemMessagesDatabase.IsHandledInternally(x.Message)) return;
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

    static void OnDespawnNpc(S_DESPAWN_NPC p)
    {
        NearbyNPCs.Remove(p.Target);
        FlyingGuardianDataProvider.InvokeProgressChanged();
        AbnormalityTracker.CheckMarkingOnDespawn(p.Target);
    }

    static void OnDespawnUser(S_DESPAWN_USER p)
    {
        #region Aura meme

        if (p.EntityId == _foglioEid) Me.EndAbnormality(10241024);

        #endregion Aura meme

        NearbyPlayers.Remove(p.EntityId);
    }

    static void OnSpawnUser(S_SPAWN_USER p)
    {
        #region Aura meme

        switch (p.Name)
        {
            case "Foglio":
                //case "Fogolio":
                //case "Foglietto":
                //case "Foglia":
                //case "Myvia":
                //case "Foglietta.Blu":
                //case "Foglia.Trancer":
                //case "Folyria":
                //case "Folyvia":
                //case "Fogliolina":
                //case "Folyemi":
                //case "Foiya":
                //case "Fogliarya":
                if (p.ServerId != 2800) break;
                if (CivilUnrestZone) break;
                _foglioEid = p.EntityId;
                var ab = DB!.AbnormalityDatabase.Abnormalities[10241024];
                Me.UpdateAbnormality(ab, int.MaxValue, 1);
                SystemMessagesProcessor.AnalyzeMessage($"@0\vAbnormalName\v{ab.Name}", "SMT_BATTLE_BUFF_DEBUFF");
                break;
        }

        #endregion Aura meme

        NearbyPlayers[p.EntityId] = new ValueTuple<string, Class>(p.Name, TccUtils.ClassFromModel(p.TemplateId));
    }

    static void OnSpawnMe(S_SPAWN_ME p)
    {
        NearbyNPCs.Clear();
        NearbyPlayers.Clear();
        AbnormalityTracker.ClearMarkedTargets();
        FlyingGuardianDataProvider.Stacks = 0;
        FlyingGuardianDataProvider.StackType = FlightStackType.None;
        FlyingGuardianDataProvider.InvokeProgressChanged();

        Task.Delay(2000).ContinueWith(_ =>
        {
            LoadingScreen = false;
            WindowManager.VisibilityManager.RefreshDim();

            #region Fear Inoculum

            if (App.FI)
            {
                var ab = DB!.AbnormalityDatabase.Abnormalities[30082019];
                Me.UpdateAbnormality(ab, int.MaxValue, 1);
                SystemMessagesProcessor.AnalyzeMessage($"@0\vAbnormalName\v{ab.Name}", "SMT_BATTLE_BUFF_DEBUFF");
            }

            #endregion Fear Inoculum

            #region Lockdown
#if false
            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower() == "it")
            {
                var zg = DB!.AbnormalityDatabase.Abnormalities[10240001];
                var za = DB.AbnormalityDatabase.Abnormalities[10240002];
                var zr = DB.AbnormalityDatabase.Abnormalities[10240003];

                var zone = DateTime.Now.Month switch
                {
                    12 when DateTime.Now.Year == 2020 => DateTime.Now.Day switch
                    {
                        >= 24 and <= 27 => zr,
                        >= 28 and <= 30 => za,
                        31 => zr,
                        _ => zg
                    },
                    1 when DateTime.Now.Year == 2021 && DateTime.Now.Day < 7 => DateTime.Now.Day == 4 ? za : zr,
                    _ => null
                };

                if (zone == null) return;
                Me.UpdateAbnormality(zone, int.MaxValue, 1);
                SystemMessagesProcessor.AnalyzeMessage($"@0\vAbnormalName\v{zone.Name}", "SMT_BATTLE_BUFF_DEBUFF");
            }
#endif
            #endregion
        });
    }

    static void OnAccountPackageList(S_ACCOUNT_PACKAGE_LIST m)
    {
        Account.IsElite = m.IsElite;
    }

    static void OnLoadTopo(S_LOAD_TOPO m)
    {
        LoadingScreen = true;
        Encounter = false;
        CurrentZoneId = m.Zone;
        Teleported?.Invoke();
        Me.ClearAbnormalities();
    }

    static void OnGetUserList(S_GET_USER_LIST m)
    {
        if (PacketAnalyzer.Factory!.ReleaseVersion == 0)
            Log.F("Warning: C_LOGIN_ARBITER not received.");

        if (!StubInterface.Instance.IsConnected)
        {
            StubInterface.Instance.Disconnect();
            _ = StubInterface.Instance.InitAsync(App.Settings.LfgWindowSettings.Enabled,
                                 App.Settings.EnablePlayerMenu,
                                 App.Settings.EnableProxy,
                                 App.Settings.ShowIngameChat,
                                 App.Settings.ChatEnabled);
        }

        Logged = false;
        Firebase.RegisterWebhook(App.Settings.WebhookUrlGuildBam, false, CurrentAccountNameHash);
        Firebase.RegisterWebhook(App.Settings.WebhookUrlFieldBoss, false, CurrentAccountNameHash);
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
                ch.ServerName = Server.Name;
            }
            else
            {
                Account.Characters.Add(new Character(item));
            }
        }
    }

    static void OnUserStatus(S_USER_STATUS m)
    {
        if (IsMe(m.GameId)) Combat = m.Status == S_USER_STATUS.UserStatus.InCombat;
    }

    static void OnFieldEventOnLeave(S_FIELD_EVENT_ON_LEAVE p)
    {
        SystemMessagesProcessor.AnalyzeMessage("", "SMT_FIELD_EVENT_LEAVE");

        if (!StubInterface.Instance.IsStubAvailable
            || !StubInterface.Instance.IsFpsModAvailable
            || !App.Settings.FpsAtGuardian) return;
        StubInterface.Instance.StubClient.InvokeCommand("fps mode 1");
    }

    static void OnFieldEventOnEnter(S_FIELD_EVENT_ON_ENTER p)
    {
        SystemMessagesProcessor.AnalyzeMessage("", "SMT_FIELD_EVENT_ENTER");

        if (!StubInterface.Instance.IsStubAvailable
            || !StubInterface.Instance.IsFpsModAvailable
            || !App.Settings.FpsAtGuardian) return;
        StubInterface.Instance.StubClient.InvokeCommand("fps mode 3");
    }

    static void OnFieldPointInfo(S_FIELD_POINT_INFO p)
    {
        if (Account.CurrentCharacter == null) return;
        var old = Account.CurrentCharacter.GuardianInfo.Cleared;
        Account.CurrentCharacter.GuardianInfo.Claimed = p.Claimed;
        Account.CurrentCharacter.GuardianInfo.Cleared = p.Cleared;
        if (old == p.Cleared) return;
        SystemMessagesProcessor.AnalyzeMessage("@0", "SMT_FIELD_EVENT_REWARD_AVAILABLE");
    }

    static void OnGetUserGuildLogo(S_GET_USER_GUILD_LOGO p)
    {
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

    static void OnGuildMemberList(S_GUILD_MEMBER_LIST m)
    {
        Task.Run(() => Guild.Set(m.Members, m.MasterId, m.MasterName));
    }

    static void OnLoadEpInfo(S_LOAD_EP_INFO m)
    {
        if (!m.Perks.TryGetValue(851010, out var level)) return;
        EpDataProvider.SetManaBarrierPerkLevel(level);
    }

    static void OnLearnEpPerk(S_LEARN_EP_PERK m)
    {
        if (!m.Perks.TryGetValue(851010, out var level)) return;
        EpDataProvider.SetManaBarrierPerkLevel(level);
    }

    static void OnResetEpPerk(S_RESET_EP_PERK m)
    {
        if (!m.Success) return;
        EpDataProvider.SetManaBarrierPerkLevel(0);
    }

    static void OnReturnToLobby(S_RETURN_TO_LOBBY m)
    {
        Logged = false;
        Me.ClearAbnormalities();
    }

    static async Task OnLogin(S_LOGIN m)
    {
        var account = string.IsNullOrEmpty(CurrentAccountNameHash)
            ? string.IsNullOrEmpty(App.Settings.LastAccountNameHash)
                ? null
                : App.Settings.LastAccountNameHash
            : CurrentAccountNameHash;

        if (!string.IsNullOrEmpty(account))
        {
            Firebase.RegisterWebhook(App.Settings.WebhookUrlGuildBam, true, account);
            Firebase.RegisterWebhook(App.Settings.WebhookUrlFieldBoss, true, account);
        }

        if (!string.IsNullOrEmpty(account)
            // && (App.Settings.StatSentVersion != App.AppVersion ||
            // App.Settings.StatSentTime.Month != DateTime.UtcNow.Month ||
            // App.Settings.StatSentTime.Day != DateTime.UtcNow.Day)
            )
        {

            bool isDailyFirst = App.Settings.StatSentTime.Month == DateTime.Now.Month &&
                                App.Settings.StatSentTime.Day == DateTime.Now.Day &&
                                App.Settings.StatSentVersion != App.AppVersion;

            if (await Cloud.SendUsageStatAsync(Server.Region,
                                               Server.ServerId,
                                               Server.Ip,
                                               Server.Name,
                                               account,
                                               App.AppVersion,
                                               isDailyFirst))
            {
                App.Settings.StatSentTime = DateTime.UtcNow;
                App.Settings.StatSentVersion = App.AppVersion;
                App.Settings.Save();
            }
        }

        App.Settings.LastLanguage = Language;

        Logged = true;
        LoadingScreen = true;
        Encounter = false;
        Account.LoginCharacter(m.PlayerId);
        Guild.Clear();
        Friends.Clear();
        BlockList.Clear();

        Server = PacketAnalyzer.ServerDatabase.GetServer(m.ServerId);

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
        await InitDatabasesAsync(App.Settings.LastLanguage);
        SetAbnormalityTracker(m.CharacterClass);
    }

    static async Task OnLoginArbiter(C_LOGIN_ARBITER m)
    {
        CurrentAccountNameHash = HashUtils.GenerateHash(m.AccountName);
        App.Settings.LastAccountNameHash = CurrentAccountNameHash;
        PacketAnalyzer.ServerDatabase.Language = m.Language == LangEnum.EN && Server.Region == "RU" ? LangEnum.RU : LangEnum.EN;
        App.Settings.LastLanguage = PacketAnalyzer.ServerDatabase.StringLanguage;

        var rvSysMsgPath = Path.Combine(App.DataPath, $"opcodes/sysmsg.{PacketAnalyzer.Factory!.ReleaseVersion / 100}.map");
        var pvSysMsgPath = Path.Combine(App.DataPath, $"opcodes/sysmsg.{PacketAnalyzer.Factory.Version}.map");

        var path = File.Exists(rvSysMsgPath)
            ? rvSysMsgPath
            : File.Exists(pvSysMsgPath)
                ? pvSysMsgPath
                : "";

        var destPath = pvSysMsgPath.Replace("\\", "/");

        if (PacketAnalyzer.Sniffer.Connected && PacketAnalyzer.Sniffer is ToolboxSniffer tbs)
        {
            if (await tbs.ControlConnection.DumpMap(destPath, "sysmsg"))
            {
                PacketAnalyzer.Factory.SystemMessageNamer = new OpCodeNamer(destPath);
                return;
            }
        }
        else
        {
            if (OpcodeDownloader.DownloadSysmsgIfNotExist(PacketAnalyzer.Factory.Version, Path.Combine(App.DataPath, "opcodes/"), PacketAnalyzer.Factory.ReleaseVersion))
            {
                PacketAnalyzer.Factory.SystemMessageNamer = new OpCodeNamer(destPath);
                return;
            }
        }
        if (path == "")
        {
            TccMessageBox.Show(SR.InvalidSysMsgFile(PacketAnalyzer.Factory.ReleaseVersion / 100, PacketAnalyzer.Factory.Version), MessageBoxType.Error);
            App.Close();
            return;
        }
        PacketAnalyzer.Factory.ReloadSysMsg(path);
    }

    static void OnAbnormalityBegin(S_ABNORMALITY_BEGIN p)
    {
        CurrentAbnormalityTracker.OnAbnormalityBegin(p);
        if (!IsMe(p.TargetId)) return;
        if (!DB!.AbnormalityDatabase.GetAbnormality(p.AbnormalityId, out var ab) || !ab.CanShow) return;
        ab.Infinity = p.Duration >= int.MaxValue / 2;
        Me.UpdateAbnormality(ab, p.Duration, p.Stacks);
        FlyingGuardianDataProvider.HandleAbnormal(p);
    }

    static void OnAbnormalityRefresh(S_ABNORMALITY_REFRESH p)
    {
        CurrentAbnormalityTracker.OnAbnormalityRefresh(p);
        if (!IsMe(p.TargetId)) return;
        if (!DB!.AbnormalityDatabase.GetAbnormality(p.AbnormalityId, out var ab) || !ab.CanShow) return;
        ab.Infinity = p.Duration >= int.MaxValue / 2;
        Me.UpdateAbnormality(ab, p.Duration, p.Stacks);
        FlyingGuardianDataProvider.HandleAbnormal(p);
    }

    static void OnAbnormalityEnd(S_ABNORMALITY_END p)
    {
        CurrentAbnormalityTracker.OnAbnormalityEnd(p);
        if (!IsMe(p.TargetId)) return;
        if (!DB!.AbnormalityDatabase.GetAbnormality(p.AbnormalityId, out var ab) || !ab.CanShow) return;
        FlyingGuardianDataProvider.HandleAbnormal(p);
        Me.EndAbnormality(ab);
    }

    static void OnStartCooltimeItem(S_START_COOLTIME_ITEM m)
    {
        App.BaseDispatcher.InvokeAsync(() => SkillStarted?.Invoke());
    }

    static void OnStartCooltimeSkill(S_START_COOLTIME_SKILL m)
    {
        App.BaseDispatcher.InvokeAsync(() => SkillStarted?.Invoke());
    }

    static void OnChangeGuildChief(S_CHANGE_GUILD_CHIEF m)
    {
        SystemMessagesProcessor.AnalyzeMessage($"@0\vName\v{Guild.NameOf(m.PlayerId)}", "SMT_GC_SYSMSG_GUILD_CHIEF_CHANGED");
        Guild.SetMaster(m.PlayerId, Guild.NameOf(m.PlayerId));
    }

    static void OnNotifyGuildQuestUrgent(S_NOTIFY_GUILD_QUEST_URGENT p)
    {
        if (p.Type != S_NOTIFY_GUILD_QUEST_URGENT.GuildBamQuestType.Announce) return;

        var questName = DB!.GuildQuestDatabase.GuildQuests.TryGetValue(p.QuestId, out var gq)
            ?
             gq.Title
                : "Defeat Guild BAM";
        var zone = DB.MonsterDatabase.GetZoneName(p.ZoneId);
        var name = DB.MonsterDatabase.GetMonsterName(p.TemplateId, p.ZoneId);
        SystemMessagesProcessor.AnalyzeMessage($"@0\vquestName\v{questName}\vnpcName\v{name}\vzoneName\v{zone}", "SMT_GQUEST_URGENT_NOTIFY");
    }

    static void OnNotifyToFriendsWalkIntoSameArea(S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA x)
    {
        Friends.NotifyWalkInSameArea(x.PlayerId, x.WorldId, x.GuardId, x.SectionId);
    }

    static void OnFriendList(S_FRIEND_LIST m)
    {
        Friends.SetFrom(m.Friends);
    }

    static void OnUserBlockList(S_USER_BLOCK_LIST m)
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

    #endregion Hooks
}