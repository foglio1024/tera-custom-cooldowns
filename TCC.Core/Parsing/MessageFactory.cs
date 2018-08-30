using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.TeraCommon;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Messages.Client;
using TCC.TeraCommon.Game.Services;
using C_PLAYER_LOCATION = TCC.Parsing.Messages.C_PLAYER_LOCATION;
//using Tera.Game.Messages;
using S_GET_USER_GUILD_LOGO = TCC.TeraCommon.Game.Messages.Server.S_GET_USER_GUILD_LOGO;
using ParsedMessage = TCC.TeraCommon.Game.Messages.ParsedMessage;

namespace TCC.Parsing
{
    public class MessageFactory
    {
        private static readonly Delegate UnknownMessageDelegate = Contructor<Func<TeraMessageReader, UnknownMessage>>();
        private static readonly Dictionary<ushort, Delegate> OpcodeNameToType = new Dictionary<ushort, Delegate> { { 19900, Contructor<Func<TeraMessageReader, C_CHECK_VERSION>>() } };
        private static readonly Dictionary<string, Delegate> TeraMessages = new Dictionary<string, Delegate>
        {
            {"C_LOGIN_ARBITER", Contructor<Func<TeraMessageReader,C_LOGIN_ARBITER>>()},
            { "S_LOGIN" , Contructor<Func<TeraMessageReader,S_LOGIN>>()},
            { "S_START_COOLTIME_SKILL" , Contructor<Func<TeraMessageReader,S_START_COOLTIME_SKILL>>()},
            { "S_DECREASE_COOLTIME_SKILL" , Contructor<Func<TeraMessageReader,S_DECREASE_COOLTIME_SKILL>>()},
            { "S_START_COOLTIME_ITEM" , Contructor<Func<TeraMessageReader,S_START_COOLTIME_ITEM>>()},
            { "S_PLAYER_CHANGE_MP" , Contructor<Func<TeraMessageReader,S_PLAYER_CHANGE_MP>>()},
            { "S_CREATURE_CHANGE_HP" , Contructor<Func<TeraMessageReader,S_CREATURE_CHANGE_HP>>()},
            { "S_PLAYER_CHANGE_STAMINA" , Contructor<Func<TeraMessageReader,S_PLAYER_CHANGE_STAMINA>>()},
            { "S_PLAYER_CHANGE_FLIGHT_ENERGY" , Contructor<Func<TeraMessageReader,S_PLAYER_CHANGE_FLIGHT_ENERGY>>()},
            { "S_PLAYER_STAT_UPDATE" , Contructor<Func<TeraMessageReader,S_PLAYER_STAT_UPDATE>>()},
            { "S_USER_STATUS" , Contructor<Func<TeraMessageReader,S_USER_STATUS>>()},
            { "S_SPAWN_NPC" , Contructor<Func<TeraMessageReader,S_SPAWN_NPC>>()},
            { "S_DESPAWN_NPC" , Contructor<Func<TeraMessageReader,S_DESPAWN_NPC>>()},
            { "S_NPC_STATUS" , Contructor<Func<TeraMessageReader,S_NPC_STATUS>>()},
            { "S_BOSS_GAGE_INFO" , Contructor<Func<TeraMessageReader,S_BOSS_GAGE_INFO>>()},
            { "S_ABNORMALITY_BEGIN" , Contructor<Func<TeraMessageReader,S_ABNORMALITY_BEGIN>>()},
            { "S_ABNORMALITY_REFRESH" , Contructor<Func<TeraMessageReader,S_ABNORMALITY_REFRESH>>()},
            { "S_ABNORMALITY_END" , Contructor<Func<TeraMessageReader,S_ABNORMALITY_END>>()},
            { "S_GET_USER_LIST" , Contructor<Func<TeraMessageReader,S_GET_USER_LIST>>()},
            { "S_SPAWN_ME" , Contructor<Func<TeraMessageReader,S_SPAWN_ME>>()},
            { "S_RETURN_TO_LOBBY" , Contructor<Func<TeraMessageReader,S_RETURN_TO_LOBBY>>()},
            { "C_PLAYER_LOCATION" , Contructor<Func<TeraMessageReader,C_PLAYER_LOCATION>>() },
            { "S_USER_EFFECT" , Contructor<Func<TeraMessageReader,S_USER_EFFECT>>() },
            { "S_LOAD_TOPO" , Contructor<Func<TeraMessageReader,S_LOAD_TOPO>>() },
            //{ "C_LOAD_TOPO_FIN" , Contructor<Func<TeraMessageReader,C_LOAD_TOPO_FIN>>() },
            { "S_DESPAWN_USER", Contructor<Func<TeraMessageReader, S_DESPAWN_USER>>() },
            { "S_PARTY_MEMBER_LIST", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_LIST>>() },
            { "S_LOGOUT_PARTY_MEMBER", Contructor<Func<TeraMessageReader, S_LOGOUT_PARTY_MEMBER>>() },
            { "S_LEAVE_PARTY_MEMBER", Contructor<Func<TeraMessageReader, S_LEAVE_PARTY_MEMBER>>() },
            { "S_LEAVE_PARTY", Contructor<Func<TeraMessageReader, S_LEAVE_PARTY>>() },
            { "S_BAN_PARTY_MEMBER", Contructor<Func<TeraMessageReader, S_BAN_PARTY_MEMBER>>() },
            { "S_BAN_PARTY", Contructor<Func<TeraMessageReader, S_BAN_PARTY>>() },
            { "S_PARTY_MEMBER_CHANGE_HP", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_CHANGE_HP>>() },
            { "S_PARTY_MEMBER_CHANGE_MP", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_CHANGE_MP>>() },
            { "S_PARTY_MEMBER_STAT_UPDATE", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_STAT_UPDATE>>() },
            { "S_CHECK_TO_READY_PARTY", Contructor<Func<TeraMessageReader, S_CHECK_TO_READY_PARTY>>() },
            { "S_CHECK_TO_READY_PARTY_FIN", Contructor<Func<TeraMessageReader, S_CHECK_TO_READY_PARTY_FIN>>() },
            { "S_ASK_BIDDING_RARE_ITEM", Contructor<Func<TeraMessageReader, S_ASK_BIDDING_RARE_ITEM>>() },
            { "S_RESULT_ITEM_BIDDING", Contructor<Func<TeraMessageReader, S_RESULT_ITEM_BIDDING>>() },
            { "S_RESULT_BIDDING_DICE_THROW", Contructor<Func<TeraMessageReader, S_RESULT_BIDDING_DICE_THROW>>() },

            { "S_PARTY_MEMBER_BUFF_UPDATE", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_BUFF_UPDATE>>() },
            { "S_PARTY_MEMBER_ABNORMAL_ADD", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_ADD>>() },
            { "S_PARTY_MEMBER_ABNORMAL_REFRESH", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_REFRESH>>() },
            { "S_PARTY_MEMBER_ABNORMAL_DEL", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_DEL>>() },
            { "S_PARTY_MEMBER_ABNORMAL_CLEAR", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_CLEAR>>() },
            { "S_CHANGE_PARTY_MANAGER", Contructor<Func<TeraMessageReader, S_CHANGE_PARTY_MANAGER>>() },
            { "S_WEAK_POINT", Contructor<Func<TeraMessageReader, S_WEAK_POINT>>() },
            { "S_CHAT", Contructor<Func<TeraMessageReader, S_CHAT>>() },
            { "S_WHISPER", Contructor<Func<TeraMessageReader, S_WHISPER>>() },
            { "S_PRIVATE_CHAT", Contructor<Func<TeraMessageReader, S_PRIVATE_CHAT>>() },
            { "S_JOIN_PRIVATE_CHANNEL", Contructor<Func<TeraMessageReader, S_JOIN_PRIVATE_CHANNEL>>() },
            { "S_LEAVE_PRIVATE_CHANNEL", Contructor<Func<TeraMessageReader, S_LEAVE_PRIVATE_CHANNEL>>() },
            { "S_SYSTEM_MESSAGE", Contructor<Func<TeraMessageReader, S_SYSTEM_MESSAGE>>() },
            { "S_SYSTEM_MESSAGE_LOOT_ITEM", Contructor<Func<TeraMessageReader, S_SYSTEM_MESSAGE_LOOT_ITEM>>() },
            { "S_CREST_MESSAGE", Contructor<Func<TeraMessageReader, S_CREST_MESSAGE>>() },
            { "S_ANSWER_INTERACTIVE", Contructor<Func<TeraMessageReader, S_ANSWER_INTERACTIVE>>() },
            { "S_USER_BLOCK_LIST", Contructor<Func<TeraMessageReader, S_USER_BLOCK_LIST>>() },
            { "S_FRIEND_LIST", Contructor<Func<TeraMessageReader, S_FRIEND_LIST>>() },
            { "S_ACCOMPLISH_ACHIEVEMENT", Contructor<Func<TeraMessageReader, S_ACCOMPLISH_ACHIEVEMENT>>() },
            { "S_TRADE_BROKER_DEAL_SUGGESTED", Contructor<Func<TeraMessageReader, S_TRADE_BROKER_DEAL_SUGGESTED>>() },
            { "S_UPDATE_FRIEND_INFO", Contructor<Func<TeraMessageReader, S_UPDATE_FRIEND_INFO>>() },
            { "S_PARTY_MATCH_LINK", Contructor<Func<TeraMessageReader, S_PARTY_MATCH_LINK>>() },
            { "S_PARTY_MEMBER_INFO", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_INFO>>() },
            { "S_OTHER_USER_APPLY_PARTY", Contructor<Func<TeraMessageReader, S_OTHER_USER_APPLY_PARTY>>() },
            { "S_DUNGEON_EVENT_MESSAGE", Contructor<Func<TeraMessageReader, S_DUNGEON_EVENT_MESSAGE>>() },
            { "S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA", Contructor<Func<TeraMessageReader, S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA>>() },
            { "S_AVAILABLE_EVENT_MATCHING_LIST", Contructor<Func<TeraMessageReader, S_AVAILABLE_EVENT_MATCHING_LIST>>() },
            { "S_DUNGEON_COOL_TIME_LIST", Contructor<Func<TeraMessageReader, S_DUNGEON_COOL_TIME_LIST>>() },
            { "S_ACCOUNT_PACKAGE_LIST", Contructor<Func<TeraMessageReader, S_ACCOUNT_PACKAGE_LIST>>() },
            { "S_GUILD_TOWER_INFO", Contructor<Func<TeraMessageReader, S_GUILD_TOWER_INFO>>() },
            { "S_INVEN", Contructor<Func<TeraMessageReader, S_INVEN>>() },
            { "S_SPAWN_USER", Contructor<Func<TeraMessageReader, S_SPAWN_USER>>() },
            { "S_PARTY_MEMBER_INTERVAL_POS_UPDATE", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_INTERVAL_POS_UPDATE>>() },
            { "S_ABNORMALITY_DAMAGE_ABSORB", Contructor<Func<TeraMessageReader, S_ABNORMALITY_DAMAGE_ABSORB>>() },
            { "S_IMAGE_DATA", Contructor<Func<TeraMessageReader, S_IMAGE_DATA>>() },
            { "S_GET_USER_GUILD_LOGO", Contructor<Func<TeraMessageReader, S_GET_USER_GUILD_LOGO>>() },
            { "S_FIELD_POINT_INFO", Contructor<Func<TeraMessageReader, S_FIELD_POINT_INFO>>() },
            { "S_DUNGEON_CLEAR_COUNT_LIST", Contructor<Func<TeraMessageReader, S_DUNGEON_CLEAR_COUNT_LIST>>() },
            { "S_SHOW_PARTY_MATCH_INFO", Contructor<Func<TeraMessageReader, S_SHOW_PARTY_MATCH_INFO>>() },
            { "S_SHOW_CANDIDATE_LIST", Contructor<Func<TeraMessageReader, S_SHOW_CANDIDATE_LIST>>() },
            //{"S_ACTION_STAGE", Contructor<Func<TeraMessageReader, S_ACTION_STAGE>>() }, //nvm
            //{ "S_EACH_SKILL_RESULT", Contructor<Func<TeraMessageReader, S_EACH_SKILL_RESULT>>() },

        };

        private static readonly Dictionary<Type, Delegate> MainProcessor = new Dictionary<Type, Delegate>();
        private static readonly Dictionary<Type, Delegate> Base = new Dictionary<Type, Delegate>()
        {
            {typeof(S_CHAT), new Action<S_CHAT>(PacketProcessor.HandleChat) },
            {typeof(S_LOGIN), new Action<S_LOGIN>(PacketProcessor.HandleLogin) },
            {typeof(S_LOAD_TOPO), new Action<S_LOAD_TOPO>(PacketProcessor.HandleLoadTopo) },
            {typeof(S_GET_USER_LIST), new Action<S_GET_USER_LIST>(PacketProcessor.HandleCharList) },
            {typeof(S_SPAWN_ME), new Action<S_SPAWN_ME>(PacketProcessor.HandleSpawnMe) },
            {typeof(S_RETURN_TO_LOBBY), new Action<S_RETURN_TO_LOBBY>(PacketProcessor.HandleReturnToLobby) },

            {typeof(S_SPAWN_NPC), new Action<S_SPAWN_NPC>(PacketProcessor.HandleSpawnNpc) },
            {typeof(S_PLAYER_CHANGE_MP), new Action<S_PLAYER_CHANGE_MP>(PacketProcessor.HandlePlayerChangeMp) },
            {typeof(S_CREATURE_CHANGE_HP), new Action<S_CREATURE_CHANGE_HP>(PacketProcessor.HandleCreatureChangeHp) },
            {typeof(S_PLAYER_CHANGE_STAMINA), new Action<S_PLAYER_CHANGE_STAMINA>(PacketProcessor.HandlePlayerChangeStamina) },
            {typeof(S_PLAYER_STAT_UPDATE), new Action<S_PLAYER_STAT_UPDATE>(PacketProcessor.HandlePlayerStatUpdate) },
            {typeof(S_USER_STATUS), new Action<S_USER_STATUS>(PacketProcessor.HandleUserStatusChanged) },
            {typeof(S_DESPAWN_NPC), new Action<S_DESPAWN_NPC>(PacketProcessor.HandleDespawnNpc) },
            {typeof(S_ABNORMALITY_BEGIN), new Action<S_ABNORMALITY_BEGIN>(PacketProcessor.HandleAbnormalityBegin) },
            {typeof(S_ABNORMALITY_REFRESH), new Action<S_ABNORMALITY_REFRESH>(PacketProcessor.HandleAbnormalityRefresh) },
            {typeof(S_ABNORMALITY_END), new Action<S_ABNORMALITY_END>(PacketProcessor.HandleAbnormalityEnd) },
            {typeof(S_USER_EFFECT), new Action<S_USER_EFFECT>(PacketProcessor.HandleUserEffect) },
            {typeof(S_SYSTEM_MESSAGE), new Action<S_SYSTEM_MESSAGE>(PacketProcessor.HandleSystemMessage) },
            {typeof(S_INVEN), new Action<S_INVEN>(PacketProcessor.HandleInventory) },
            {typeof(S_SPAWN_USER), new Action<S_SPAWN_USER>(PacketProcessor.HandleSpawnUser) },
            {typeof(S_DESPAWN_USER), new Action<S_DESPAWN_USER>(PacketProcessor.HandleDespawnUser) },
            {typeof(S_PARTY_MEMBER_INTERVAL_POS_UPDATE), new Action<S_PARTY_MEMBER_INTERVAL_POS_UPDATE>(PacketProcessor.HandlePartyMemberIntervalPosUpdate) },
            {typeof(S_ABNORMALITY_DAMAGE_ABSORB), new Action<S_ABNORMALITY_DAMAGE_ABSORB>(PacketProcessor.HandleShieldDamageAbsorb) },
            {typeof(S_IMAGE_DATA), new Action<S_IMAGE_DATA>(PacketProcessor.HandleImageData) },
            {typeof(S_GET_USER_GUILD_LOGO), new Action<S_GET_USER_GUILD_LOGO>(PacketProcessor.HandleUserGuildLogo) },
            {typeof(S_PLAYER_CHANGE_FLIGHT_ENERGY), new Action<S_PLAYER_CHANGE_FLIGHT_ENERGY>(PacketProcessor.HandlePlayerChangeFlightEnergy) },
            {typeof(S_SHOW_PARTY_MATCH_INFO), new Action<S_SHOW_PARTY_MATCH_INFO>(PacketProcessor.HandleLfgList) },
            {typeof(S_SHOW_CANDIDATE_LIST), new Action<S_SHOW_CANDIDATE_LIST>(PacketProcessor.HandleApplicantsList) },
            {typeof(C_LOGIN_ARBITER), Helpers.Contructor<Func<C_LOGIN_ARBITER, LoginArbiterMessage>>()},
            {typeof(S_ANSWER_INTERACTIVE), new Action<S_ANSWER_INTERACTIVE>(PacketProcessor.HandleAnswerInteractive) },
            //{typeof(S_ACTION_STAGE), new Action<S_ACTION_STAGE>(x => PacketProcessor.HandleActionStage(x)) }, //nvm
            //{typeof(C_LOAD_TOPO_FIN), new Action<C_LOAD_TOPO_FIN>(x => PacketProcessor.HandleLoadTopoFin(x)) },

        };

        private static readonly Dictionary<Type, Delegate> AccurateHp = new Dictionary<Type, Delegate>
        {
            //{typeof(S_EACH_SKILL_RESULT), new Action<S_EACH_SKILL_RESULT>(PacketProcessor.HandleSkillResult)}
            {typeof(S_SHOW_HP), new Action<S_SHOW_HP>(PacketProcessor.HandleShowHp)}
        };
        private static readonly Dictionary<Type, Delegate> CooldownWindow = new Dictionary<Type, Delegate>
        {
            {typeof(S_START_COOLTIME_SKILL), new Action<S_START_COOLTIME_SKILL>(PacketProcessor.HandleNewSkillCooldown) },
            {typeof(S_DECREASE_COOLTIME_SKILL), new Action<S_DECREASE_COOLTIME_SKILL>(PacketProcessor.HandleDecreaseSkillCooldown) },
            {typeof(S_START_COOLTIME_ITEM), new Action<S_START_COOLTIME_ITEM>(PacketProcessor.HandleNewItemCooldown) },

        };
        private static readonly Dictionary<Type, Delegate> GroupWindow = new Dictionary<Type, Delegate>
        {
            {typeof(S_PARTY_MEMBER_LIST), new Action<S_PARTY_MEMBER_LIST>(PacketProcessor.HandlePartyMemberList) },
            {typeof(S_LOGOUT_PARTY_MEMBER), new Action<S_LOGOUT_PARTY_MEMBER>(PacketProcessor.HandlePartyMemberLogout) },
            {typeof(S_LEAVE_PARTY_MEMBER), new Action<S_LEAVE_PARTY_MEMBER>(PacketProcessor.HandlePartyMemberLeave) },
            {typeof(S_LEAVE_PARTY), new Action<S_LEAVE_PARTY>(PacketProcessor.HandleLeaveParty) },
            {typeof(S_BAN_PARTY_MEMBER), new Action<S_BAN_PARTY_MEMBER>(PacketProcessor.HandlePartyMemberKick) },
            {typeof(S_BAN_PARTY), new Action<S_BAN_PARTY>(PacketProcessor.HandleKicked) },


            {typeof(S_PARTY_MEMBER_STAT_UPDATE), new Action<S_PARTY_MEMBER_STAT_UPDATE>(PacketProcessor.HandlePartyMemberStats) },
            {typeof(S_CHECK_TO_READY_PARTY), new Action<S_CHECK_TO_READY_PARTY>(PacketProcessor.HandleReadyCheck) },
            {typeof(S_CHECK_TO_READY_PARTY_FIN), new Action<S_CHECK_TO_READY_PARTY_FIN>(PacketProcessor.HandleReadyCheckFin) },

            {typeof(S_ASK_BIDDING_RARE_ITEM), new Action<S_ASK_BIDDING_RARE_ITEM>(PacketProcessor.HandleStartRoll) },
            {typeof(S_RESULT_ITEM_BIDDING), new Action<S_RESULT_ITEM_BIDDING>(PacketProcessor.HandleEndRoll) },
            {typeof(S_RESULT_BIDDING_DICE_THROW), new Action<S_RESULT_BIDDING_DICE_THROW>(PacketProcessor.HandleRollResult) },

            {typeof(S_CHANGE_PARTY_MANAGER), new Action<S_CHANGE_PARTY_MANAGER>(PacketProcessor.HandleChangeLeader) },
        };
        private static readonly Dictionary<Type, Delegate> GroupWindowAbnormals = new Dictionary<Type, Delegate>
        {
            {typeof(S_PARTY_MEMBER_BUFF_UPDATE), new Action<S_PARTY_MEMBER_BUFF_UPDATE>(PacketProcessor.HandlePartyMemberBuffUpdate) },
            {typeof(S_PARTY_MEMBER_ABNORMAL_ADD), new Action<S_PARTY_MEMBER_ABNORMAL_ADD>(PacketProcessor.HandlePartyMemberAbnormalAdd) },
            {typeof(S_PARTY_MEMBER_ABNORMAL_REFRESH), new Action<S_PARTY_MEMBER_ABNORMAL_REFRESH>(PacketProcessor.HandlePartyMemberAbnormalRefresh) },
            {typeof(S_PARTY_MEMBER_ABNORMAL_DEL), new Action<S_PARTY_MEMBER_ABNORMAL_DEL>(PacketProcessor.HandlePartyMemberAbnormalDel) },
            {typeof(S_PARTY_MEMBER_ABNORMAL_CLEAR), new Action<S_PARTY_MEMBER_ABNORMAL_CLEAR>(PacketProcessor.HandlePartyMemberAbnormalClear) },

        };
        private static readonly Dictionary<Type, Delegate> GroupWindowMp = new Dictionary<Type, Delegate>
        {
            {typeof(S_PARTY_MEMBER_CHANGE_MP), new Action<S_PARTY_MEMBER_CHANGE_MP>(PacketProcessor.HandlePartyMemberMp) },
        };
        private static readonly Dictionary<Type, Delegate> GroupWindowHp = new Dictionary<Type, Delegate>
        {
            {typeof(S_PARTY_MEMBER_CHANGE_HP), new Action<S_PARTY_MEMBER_CHANGE_HP>(PacketProcessor.HandlePartyMemberHp) },
        };
        private static readonly Dictionary<Type, Delegate> Phase1Only = new Dictionary<Type, Delegate>
        {
            {typeof(C_PLAYER_LOCATION), new Action<C_PLAYER_LOCATION>(PacketProcessor.HandlePlayerLocation) },
            {typeof(S_DUNGEON_EVENT_MESSAGE), new Action<S_DUNGEON_EVENT_MESSAGE>(PacketProcessor.HandleDungeonMessage) },

        };
        private static readonly Dictionary<Type, Delegate> ChatWindow = new Dictionary<Type, Delegate>
        {
            {typeof(S_PRIVATE_CHAT), new Action<S_PRIVATE_CHAT>(PacketProcessor.HandlePrivateChat) },
            {typeof(S_WHISPER), new Action<S_WHISPER>(PacketProcessor.HandleWhisper) },
            {typeof(S_JOIN_PRIVATE_CHANNEL), new Action<S_JOIN_PRIVATE_CHANNEL>(PacketProcessor.HandleJoinPrivateChat) },
            {typeof(S_LEAVE_PRIVATE_CHANNEL), new Action<S_LEAVE_PRIVATE_CHANNEL>(PacketProcessor.HandleLeavePrivateChat) },
            {typeof(S_SYSTEM_MESSAGE_LOOT_ITEM), new Action<S_SYSTEM_MESSAGE_LOOT_ITEM>(PacketProcessor.HandleSystemMessageLoot) },
            {typeof(S_CREST_MESSAGE), new Action<S_CREST_MESSAGE>(PacketProcessor.HandleCrestMessage) },
            {typeof(S_USER_BLOCK_LIST), new Action<S_USER_BLOCK_LIST>(PacketProcessor.HandleBlockList) },
            {typeof(S_FRIEND_LIST), new Action<S_FRIEND_LIST>(PacketProcessor.HandleFriendList) },
            {typeof(S_ACCOMPLISH_ACHIEVEMENT), new Action<S_ACCOMPLISH_ACHIEVEMENT>(PacketProcessor.HandleAccomplishAchievement) },
            {typeof(S_TRADE_BROKER_DEAL_SUGGESTED), new Action<S_TRADE_BROKER_DEAL_SUGGESTED>(PacketProcessor.HandleBrokerOffer) },
            {typeof(S_UPDATE_FRIEND_INFO), new Action<S_UPDATE_FRIEND_INFO>(PacketProcessor.HandleFriendStatus) },
            {typeof(S_PARTY_MEMBER_INFO), new Action<S_PARTY_MEMBER_INFO>(PacketProcessor.HandlePartyMemberInfo) },
            {typeof(S_OTHER_USER_APPLY_PARTY), new Action<S_OTHER_USER_APPLY_PARTY>(PacketProcessor.HandleUserApplyToParty) },
            {typeof(S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA), new Action<S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA>(PacketProcessor.HandleFriendIntoArea) },
        };
        private static readonly Dictionary<Type, Delegate> ChatWindowLfg = new Dictionary<Type, Delegate>
        {
            {typeof(S_PARTY_MATCH_LINK), new Action<S_PARTY_MATCH_LINK>(PacketProcessor.HandleLfgSpam) },
        };
        private static readonly Dictionary<Type, Delegate> ValkyrieOnly = new Dictionary<Type, Delegate>
        {
            {typeof(S_WEAK_POINT), new Action<S_WEAK_POINT>(PacketProcessor.HandleRunemark) },
        };
        private static readonly Dictionary<Type, Delegate> CharacterWindow = new Dictionary<Type, Delegate>
        {

        };
        private static readonly Dictionary<Type, Delegate> BossWindow = new Dictionary<Type, Delegate>
        {
            {typeof(S_BOSS_GAGE_INFO), new Action<S_BOSS_GAGE_INFO>(PacketProcessor.HandleGageReceived) },
            {typeof(S_NPC_STATUS), new Action<S_NPC_STATUS>(PacketProcessor.HandleNpcStatusChanged) },
            {typeof(S_GUILD_TOWER_INFO), new Action<S_GUILD_TOWER_INFO>(PacketProcessor.HandleGuildTowerInfo) },
        };
        private static readonly Dictionary<Type, Delegate> InfoWindow = new Dictionary<Type, Delegate>
        {
            {typeof(S_AVAILABLE_EVENT_MATCHING_LIST), new Action<S_AVAILABLE_EVENT_MATCHING_LIST>(PacketProcessor.HandleVanguardReceived) },
            {typeof(S_DUNGEON_COOL_TIME_LIST), new Action<S_DUNGEON_COOL_TIME_LIST>(PacketProcessor.HandleDungeonCooltimeList) },
            {typeof(S_ACCOUNT_PACKAGE_LIST), new Action<S_ACCOUNT_PACKAGE_LIST>(PacketProcessor.HandleAccountPackageList) },
            {typeof(S_FIELD_POINT_INFO), new Action<S_FIELD_POINT_INFO>(PacketProcessor.HandleGuardianInfo) },
            {typeof(S_DUNGEON_CLEAR_COUNT_LIST), new Action<S_DUNGEON_CLEAR_COUNT_LIST>(PacketProcessor.HandleDungeonClears) },
        };

        private readonly OpCodeNamer _opCodeNamer;
        private readonly OpCodeNamer _sysMsgNamer;
        public string Region;
        public uint Version;
        public int ReleaseVersion { get; set; }


        public MessageFactory(OpCodeNamer opCodeNamer, string region, uint version, bool chatEnabled = false, OpCodeNamer sysMsgNamer = null)
        {
            _opCodeNamer = opCodeNamer;
            _sysMsgNamer = sysMsgNamer;
            OpcodeNameToType.Clear();
            Version = version;
            Region = region;
            TeraMessages.ToList().ForEach(x => OpcodeNameToType[PacketProcessor.OpCodeNamer.GetCode(x.Key)] = x.Value);
            Update();

        }
        public static void Update()
        {
            MainProcessor.Clear();

            Base.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);

            InfoWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);

            if (SettingsManager.ChatEnabled)
            {
                ChatWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
                ChatWindowLfg.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            }
            if (SettingsManager.CooldownWindowSettings.Enabled || SettingsManager.ClassWindowSettings.Enabled) CooldownWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            if (SettingsManager.BossWindowSettings.Enabled || SettingsManager.GroupWindowSettings.Enabled) BossWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            if (SettingsManager.GroupWindowSettings.Enabled)
            {
                GroupWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
                if (!SettingsManager.DisablePartyAbnormals) GroupWindowAbnormals.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
                if (!SettingsManager.DisablePartyMP) GroupWindowMp.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
                if (!SettingsManager.DisablePartyHP) GroupWindowHp.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            }
            if (SettingsManager.ClassWindowSettings.Enabled && SessionManager.CurrentPlayer.Class == Class.Valkyrie) ValkyrieOnly.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            if (ViewModels.BossGageWindowViewModel.Instance.CurrentHHphase == HarrowholdPhase.Phase1) Phase1Only.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            if (SettingsManager.AccurateHp) AccurateHp.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
        }
        private ParsedMessage Instantiate(ushort opCode, TeraMessageReader reader)
        {
            Delegate type;
            if (!OpcodeNameToType.TryGetValue(opCode, out type))
                type = UnknownMessageDelegate;
            return (ParsedMessage)type.DynamicInvoke(reader);
        }
        public ParsedMessage Create(Message message)
        {
            var reader = new TeraMessageReader(message, PacketProcessor.OpCodeNamer, this, PacketProcessor.SystemMessageNamer);
            return Instantiate(message.OpCode, reader);
        }
        public static TDelegate Contructor<TDelegate>() where TDelegate : class
        {
            var source = typeof(TDelegate).GetGenericArguments().Where(t => !t.IsGenericParameter).ToArray().Last();
            var ctrArgs = typeof(TDelegate).GetGenericArguments().Where(t => !t.IsGenericParameter).ToArray().Reverse().Skip(1).Reverse().ToArray();
            var constructorInfo = (source.GetConstructor(BindingFlags.Public, null, ctrArgs, null) ??
                                   source.GetConstructor(BindingFlags.NonPublic, null, ctrArgs, null)) ??
                                   source.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, ctrArgs, null);
            if (constructorInfo == null)
            {
                return null;
            }
            var parameters = ctrArgs.Select(Expression.Parameter).ToList();
            return Expression.Lambda(Expression.New(constructorInfo, parameters), parameters).Compile() as TDelegate;
        }
        public bool Process(ParsedMessage message)
        {
            Delegate type;
            MainProcessor.TryGetValue(message.GetType(), out type);
            if (type == null) return false;
            type.DynamicInvoke(message);
            return true;
        }

        public void ReloadSysMsg() { _sysMsgNamer?.Reload(Version, ReleaseVersion); }
    }
}
