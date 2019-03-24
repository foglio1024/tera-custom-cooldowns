using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TCC.Data;
using TCC.Parsing.Messages;
using TCC.TeraCommon;
using TCC.TeraCommon.Game;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;
using C_PLAYER_LOCATION = TCC.Parsing.Messages.C_PLAYER_LOCATION;
using S_GET_USER_GUILD_LOGO = TCC.TeraCommon.Game.Messages.Server.S_GET_USER_GUILD_LOGO;
using ParsedMessage = TCC.TeraCommon.Game.Messages.ParsedMessage;
using TCC.Settings;
using System.IO;

namespace TCC.Parsing
{
    public class MessageFactory
    {
        private static readonly Delegate UnknownMessageDelegate = Contructor<Func<TeraMessageReader, UnknownMessage>>();
        private static readonly Dictionary<ushort, Delegate> OpcodeNameToType = new Dictionary<ushort, Delegate> { { 19900, Contructor<Func<TeraMessageReader, C_CHECK_VERSION>>() } };
        private static readonly Dictionary<string, Delegate> TeraMessages = new Dictionary<string, Delegate>
        {
            { nameof(C_CHECK_VERSION),                         Contructor<Func<TeraMessageReader, C_CHECK_VERSION>>()},
            { nameof(C_LOGIN_ARBITER),                         Contructor<Func<TeraMessageReader, C_LOGIN_ARBITER>>()},
            { nameof(S_LOGIN) ,                                Contructor<Func<TeraMessageReader, S_LOGIN>>()},
            { nameof(S_START_COOLTIME_SKILL) ,                 Contructor<Func<TeraMessageReader, S_START_COOLTIME_SKILL>>()},
            { nameof(S_DECREASE_COOLTIME_SKILL) ,              Contructor<Func<TeraMessageReader, S_DECREASE_COOLTIME_SKILL>>()},
            { nameof(S_START_COOLTIME_ITEM) ,                  Contructor<Func<TeraMessageReader, S_START_COOLTIME_ITEM>>()},
            { nameof(S_PLAYER_CHANGE_MP) ,                     Contructor<Func<TeraMessageReader, S_PLAYER_CHANGE_MP>>()},
            { nameof(S_CREATURE_CHANGE_HP) ,                   Contructor<Func<TeraMessageReader, S_CREATURE_CHANGE_HP>>()},
            { nameof(S_PLAYER_CHANGE_STAMINA) ,                Contructor<Func<TeraMessageReader, S_PLAYER_CHANGE_STAMINA>>()},
            { nameof(S_PLAYER_CHANGE_FLIGHT_ENERGY) ,          Contructor<Func<TeraMessageReader, S_PLAYER_CHANGE_FLIGHT_ENERGY>>()},
            { nameof(S_PLAYER_STAT_UPDATE) ,                   Contructor<Func<TeraMessageReader, S_PLAYER_STAT_UPDATE>>()},
            { nameof(S_USER_STATUS) ,                          Contructor<Func<TeraMessageReader, S_USER_STATUS>>()},
            { nameof(S_SPAWN_NPC) ,                            Contructor<Func<TeraMessageReader, S_SPAWN_NPC>>()},
            { nameof(S_DESPAWN_NPC) ,                          Contructor<Func<TeraMessageReader, S_DESPAWN_NPC>>()},
            { nameof(S_NPC_STATUS) ,                           Contructor<Func<TeraMessageReader, S_NPC_STATUS>>()},
            { nameof(S_BOSS_GAGE_INFO) ,                       Contructor<Func<TeraMessageReader, S_BOSS_GAGE_INFO>>()},
            { nameof(S_ABNORMALITY_BEGIN) ,                    Contructor<Func<TeraMessageReader, S_ABNORMALITY_BEGIN>>()},
            { nameof(S_ABNORMALITY_REFRESH),                   Contructor<Func<TeraMessageReader, S_ABNORMALITY_REFRESH>>()},
            { nameof(S_ABNORMALITY_END) ,                      Contructor<Func<TeraMessageReader, S_ABNORMALITY_END>>()},
            { nameof(S_GET_USER_LIST) ,                        Contructor<Func<TeraMessageReader, S_GET_USER_LIST>>()},
            { nameof(S_SPAWN_ME) ,                             Contructor<Func<TeraMessageReader, S_SPAWN_ME>>()},
            { nameof(S_RETURN_TO_LOBBY) ,                      Contructor<Func<TeraMessageReader, S_RETURN_TO_LOBBY>>()},
            { nameof(C_PLAYER_LOCATION) ,                      Contructor<Func<TeraMessageReader, C_PLAYER_LOCATION>>() },
            { nameof(S_USER_EFFECT) ,                          Contructor<Func<TeraMessageReader, S_USER_EFFECT>>() },
            { nameof(S_LOAD_TOPO) ,                            Contructor<Func<TeraMessageReader, S_LOAD_TOPO>>() },
            { nameof(S_DESPAWN_USER),                          Contructor<Func<TeraMessageReader, S_DESPAWN_USER>>() },
            { nameof(S_PARTY_MEMBER_LIST),                     Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_LIST>>() },
            { nameof(S_LOGOUT_PARTY_MEMBER),                   Contructor<Func<TeraMessageReader, S_LOGOUT_PARTY_MEMBER>>() },
            { nameof(S_LEAVE_PARTY_MEMBER),                    Contructor<Func<TeraMessageReader, S_LEAVE_PARTY_MEMBER>>() },
            { nameof(S_LEAVE_PARTY),                           Contructor<Func<TeraMessageReader, S_LEAVE_PARTY>>() },
            { nameof(S_BAN_PARTY_MEMBER),                      Contructor<Func<TeraMessageReader, S_BAN_PARTY_MEMBER>>() },
            { nameof(S_BAN_PARTY),                             Contructor<Func<TeraMessageReader, S_BAN_PARTY>>() },
            { nameof(S_PARTY_MEMBER_CHANGE_HP),                Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_CHANGE_HP>>() },
            { nameof(S_PARTY_MEMBER_CHANGE_MP),                Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_CHANGE_MP>>() },
            { nameof(S_PARTY_MEMBER_STAT_UPDATE),              Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_STAT_UPDATE>>() },
            { nameof(S_CHECK_TO_READY_PARTY),                  Contructor<Func<TeraMessageReader, S_CHECK_TO_READY_PARTY>>() },
            { nameof(S_CHECK_TO_READY_PARTY_FIN),              Contructor<Func<TeraMessageReader, S_CHECK_TO_READY_PARTY_FIN>>() },
            { nameof(S_ASK_BIDDING_RARE_ITEM),                 Contructor<Func<TeraMessageReader, S_ASK_BIDDING_RARE_ITEM>>() },
            { nameof(S_RESULT_ITEM_BIDDING),                   Contructor<Func<TeraMessageReader, S_RESULT_ITEM_BIDDING>>() },
            { nameof(S_RESULT_BIDDING_DICE_THROW),             Contructor<Func<TeraMessageReader, S_RESULT_BIDDING_DICE_THROW>>() },
            { nameof(S_PARTY_MEMBER_BUFF_UPDATE),              Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_BUFF_UPDATE>>() },
            { nameof(S_PARTY_MEMBER_ABNORMAL_ADD),             Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_ADD>>() },
            { nameof(S_PARTY_MEMBER_ABNORMAL_REFRESH),         Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_REFRESH>>() },
            { nameof(S_PARTY_MEMBER_ABNORMAL_DEL),             Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_DEL>>() },
            { nameof(S_PARTY_MEMBER_ABNORMAL_CLEAR),           Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_CLEAR>>() },
            { nameof(S_CHANGE_PARTY_MANAGER),                  Contructor<Func<TeraMessageReader, S_CHANGE_PARTY_MANAGER>>() },
            { nameof(S_WEAK_POINT),                            Contructor<Func<TeraMessageReader, S_WEAK_POINT>>() },
            { nameof(S_CHAT),                                  Contructor<Func<TeraMessageReader, S_CHAT>>() },
            { nameof(S_WHISPER),                               Contructor<Func<TeraMessageReader, S_WHISPER>>() },
            { nameof(S_PRIVATE_CHAT),                          Contructor<Func<TeraMessageReader, S_PRIVATE_CHAT>>() },
            { nameof(S_JOIN_PRIVATE_CHANNEL),                  Contructor<Func<TeraMessageReader, S_JOIN_PRIVATE_CHANNEL>>() },
            { nameof(S_LEAVE_PRIVATE_CHANNEL),                 Contructor<Func<TeraMessageReader, S_LEAVE_PRIVATE_CHANNEL>>() },
            { nameof(S_SYSTEM_MESSAGE),                        Contructor<Func<TeraMessageReader, S_SYSTEM_MESSAGE>>() },
            { nameof(S_SYSTEM_MESSAGE_LOOT_ITEM),              Contructor<Func<TeraMessageReader, S_SYSTEM_MESSAGE_LOOT_ITEM>>() },
            { nameof(S_CREST_MESSAGE),                         Contructor<Func<TeraMessageReader, S_CREST_MESSAGE>>() },
            { nameof(S_ANSWER_INTERACTIVE),                    Contructor<Func<TeraMessageReader, S_ANSWER_INTERACTIVE>>() },
            { nameof(S_USER_BLOCK_LIST),                       Contructor<Func<TeraMessageReader, S_USER_BLOCK_LIST>>() },
            { nameof(S_FRIEND_LIST),                           Contructor<Func<TeraMessageReader, S_FRIEND_LIST>>() },
            { nameof(S_ACCOMPLISH_ACHIEVEMENT),                Contructor<Func<TeraMessageReader, S_ACCOMPLISH_ACHIEVEMENT>>() },
            { nameof(S_TRADE_BROKER_DEAL_SUGGESTED),           Contructor<Func<TeraMessageReader, S_TRADE_BROKER_DEAL_SUGGESTED>>() },
            { nameof(S_UPDATE_FRIEND_INFO),                    Contructor<Func<TeraMessageReader, S_UPDATE_FRIEND_INFO>>() },
            { nameof(S_PARTY_MATCH_LINK),                      Contructor<Func<TeraMessageReader, S_PARTY_MATCH_LINK>>() },
            { nameof(S_PARTY_MEMBER_INFO),                     Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_INFO>>() },
            { nameof(S_OTHER_USER_APPLY_PARTY),                Contructor<Func<TeraMessageReader, S_OTHER_USER_APPLY_PARTY>>() },
            { nameof(S_DUNGEON_EVENT_MESSAGE),                 Contructor<Func<TeraMessageReader, S_DUNGEON_EVENT_MESSAGE>>() },
            { nameof(S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA), Contructor<Func<TeraMessageReader, S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA>>() },
            { nameof(S_AVAILABLE_EVENT_MATCHING_LIST),         Contructor<Func<TeraMessageReader, S_AVAILABLE_EVENT_MATCHING_LIST>>() },
            { nameof(S_DUNGEON_COOL_TIME_LIST),                Contructor<Func<TeraMessageReader, S_DUNGEON_COOL_TIME_LIST>>() },
            { nameof(S_ACCOUNT_PACKAGE_LIST),                  Contructor<Func<TeraMessageReader, S_ACCOUNT_PACKAGE_LIST>>() },
            { nameof(S_GUILD_TOWER_INFO),                      Contructor<Func<TeraMessageReader, S_GUILD_TOWER_INFO>>() },
            { nameof(S_INVEN),                                 Contructor<Func<TeraMessageReader, S_INVEN>>() },
            { nameof(S_SPAWN_USER),                            Contructor<Func<TeraMessageReader, S_SPAWN_USER>>() },
            { nameof(S_PARTY_MEMBER_INTERVAL_POS_UPDATE),      Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_INTERVAL_POS_UPDATE>>() },
            { nameof(S_ABNORMALITY_DAMAGE_ABSORB),             Contructor<Func<TeraMessageReader, S_ABNORMALITY_DAMAGE_ABSORB>>() },
            { nameof(S_IMAGE_DATA),                            Contructor<Func<TeraMessageReader, S_IMAGE_DATA>>() },
            { nameof(S_GET_USER_GUILD_LOGO),                   Contructor<Func<TeraMessageReader, S_GET_USER_GUILD_LOGO>>() },
            { nameof(S_FIELD_POINT_INFO),                      Contructor<Func<TeraMessageReader, S_FIELD_POINT_INFO>>() },
            { nameof(S_DUNGEON_CLEAR_COUNT_LIST),              Contructor<Func<TeraMessageReader, S_DUNGEON_CLEAR_COUNT_LIST>>() },
            { nameof(S_SHOW_PARTY_MATCH_INFO),                 Contructor<Func<TeraMessageReader, S_SHOW_PARTY_MATCH_INFO>>() },
            { nameof(S_SHOW_CANDIDATE_LIST),                   Contructor<Func<TeraMessageReader, S_SHOW_CANDIDATE_LIST>>() },
            { nameof(S_SHOW_HP),                               Contructor<Func<TeraMessageReader, S_SHOW_HP>>() },
            { nameof(S_REQUEST_CITY_WAR_MAP_INFO),             Contructor<Func<TeraMessageReader, S_REQUEST_CITY_WAR_MAP_INFO>>() },
            { nameof(S_REQUEST_CITY_WAR_MAP_INFO_DETAIL),      Contructor<Func<TeraMessageReader, S_REQUEST_CITY_WAR_MAP_INFO_DETAIL>>() },
            { nameof(S_DESTROY_GUILD_TOWER),                   Contructor<Func<TeraMessageReader, S_DESTROY_GUILD_TOWER>>() },
            { nameof(S_FIELD_EVENT_ON_ENTER),                  Contructor<Func<TeraMessageReader, S_FIELD_EVENT_ON_ENTER>>() },
            { nameof(S_FIELD_EVENT_ON_LEAVE),                  Contructor<Func<TeraMessageReader, S_FIELD_EVENT_ON_LEAVE>>() },
            { nameof(S_UPDATE_NPCGUILD),                       Contructor<Func<TeraMessageReader, S_UPDATE_NPCGUILD>>() },
            { nameof(S_NPCGUILD_LIST),                         Contructor<Func<TeraMessageReader, S_NPCGUILD_LIST>>() },
            { nameof(S_NOTIFY_GUILD_QUEST_URGENT),             Contructor<Func<TeraMessageReader, S_NOTIFY_GUILD_QUEST_URGENT>>() },
            { nameof(S_CHANGE_GUILD_CHIEF),                    Contructor<Func<TeraMessageReader, S_CHANGE_GUILD_CHIEF>>() },
            { nameof(S_GUILD_MEMBER_LIST),                     Contructor<Func<TeraMessageReader, S_GUILD_MEMBER_LIST>>() },
            { nameof(S_CREATURE_LIFE),                         Contructor<Func<TeraMessageReader, S_CREATURE_LIFE>>() },
          //{ nameof(S_VIEW_WARE_EX),                          Contructor<Func<TeraMessageReader, S_VIEW_WARE_EX>>() },
          //{ nameof(S_ACTION_STAGE),                          Contructor<Func<TeraMessageReader, S_ACTION_STAGE>>() }, //nvm
          //{ nameof(S_EACH_SKILL_RESULT),                     Contructor<Func<TeraMessageReader, S_EACH_SKILL_RESULT>>() },
        };
        private static readonly Dictionary<Type, Delegate> MainProcessor = new Dictionary<Type, Delegate>();

        private static readonly Dictionary<Type, Delegate> Init = new Dictionary<Type, Delegate>()
        {
            {typeof(C_CHECK_VERSION),                          new Action<C_CHECK_VERSION>(PacketHandler.HandleCheckVersion)}
        };

        private static readonly Dictionary<Type, Delegate> Base = new Dictionary<Type, Delegate>()
        {
            {typeof(C_LOGIN_ARBITER),                          new Action<C_LOGIN_ARBITER>(PacketHandler.HandleLoginArbiter)},
            {typeof(S_CHAT),                                   new Action<S_CHAT>(PacketHandler.HandleChat) },
            {typeof(S_LOGIN),                                  new Action<S_LOGIN>(PacketHandler.HandleLogin) },
            {typeof(S_LOAD_TOPO),                              new Action<S_LOAD_TOPO>(PacketHandler.HandleLoadTopo) },
            {typeof(S_GET_USER_LIST),                          new Action<S_GET_USER_LIST>(PacketHandler.HandleCharList) },
            {typeof(S_SPAWN_ME),                               new Action<S_SPAWN_ME>(PacketHandler.HandleSpawnMe) },
            {typeof(S_RETURN_TO_LOBBY),                        new Action<S_RETURN_TO_LOBBY>(PacketHandler.HandleReturnToLobby) },
            {typeof(S_SPAWN_NPC),                              new Action<S_SPAWN_NPC>(PacketHandler.HandleSpawnNpc) },
            {typeof(S_PLAYER_CHANGE_MP),                       new Action<S_PLAYER_CHANGE_MP>(PacketHandler.HandlePlayerChangeMp) },
            {typeof(S_CREATURE_CHANGE_HP),                     new Action<S_CREATURE_CHANGE_HP>(PacketHandler.HandleCreatureChangeHp) },
            {typeof(S_PLAYER_CHANGE_STAMINA),                  new Action<S_PLAYER_CHANGE_STAMINA>(PacketHandler.HandlePlayerChangeStamina) },
            {typeof(S_PLAYER_STAT_UPDATE),                     new Action<S_PLAYER_STAT_UPDATE>(PacketHandler.HandlePlayerStatUpdate) },
            {typeof(S_USER_STATUS),                            new Action<S_USER_STATUS>(PacketHandler.HandleUserStatusChanged) },
            {typeof(S_DESPAWN_NPC),                            new Action<S_DESPAWN_NPC>(PacketHandler.HandleDespawnNpc) },
            {typeof(S_ABNORMALITY_BEGIN),                      new Action<S_ABNORMALITY_BEGIN>(PacketHandler.HandleAbnormalityBegin) },
            {typeof(S_ABNORMALITY_REFRESH),                    new Action<S_ABNORMALITY_REFRESH>(PacketHandler.HandleAbnormalityRefresh) },
            {typeof(S_ABNORMALITY_END),                        new Action<S_ABNORMALITY_END>(PacketHandler.HandleAbnormalityEnd) },
            {typeof(S_USER_EFFECT),                            new Action<S_USER_EFFECT>(PacketHandler.HandleUserEffect) },
            {typeof(S_SYSTEM_MESSAGE),                         new Action<S_SYSTEM_MESSAGE>(PacketHandler.HandleSystemMessage) },
            {typeof(S_INVEN),                                  new Action<S_INVEN>(PacketHandler.HandleInventory) },
            {typeof(S_SPAWN_USER),                             new Action<S_SPAWN_USER>(PacketHandler.HandleSpawnUser) },
            {typeof(S_DESPAWN_USER),                           new Action<S_DESPAWN_USER>(PacketHandler.HandleDespawnUser) },
            {typeof(S_ABNORMALITY_DAMAGE_ABSORB),              new Action<S_ABNORMALITY_DAMAGE_ABSORB>(PacketHandler.HandleShieldDamageAbsorb) },
            {typeof(S_IMAGE_DATA),                             new Action<S_IMAGE_DATA>(PacketHandler.HandleImageData) },
            {typeof(S_GET_USER_GUILD_LOGO),                    new Action<S_GET_USER_GUILD_LOGO>(PacketHandler.HandleUserGuildLogo) },
            {typeof(S_PLAYER_CHANGE_FLIGHT_ENERGY),            new Action<S_PLAYER_CHANGE_FLIGHT_ENERGY>(PacketHandler.HandlePlayerChangeFlightEnergy) },
            {typeof(S_SHOW_PARTY_MATCH_INFO),                  new Action<S_SHOW_PARTY_MATCH_INFO>(PacketHandler.HandleLfgList) },
            {typeof(S_SHOW_CANDIDATE_LIST),                    new Action<S_SHOW_CANDIDATE_LIST>(PacketHandler.HandleApplicantsList) },
            {typeof(S_ANSWER_INTERACTIVE),                     new Action<S_ANSWER_INTERACTIVE>(PacketHandler.HandleAnswerInteractive) },
            {typeof(S_DESTROY_GUILD_TOWER),                    new Action<S_DESTROY_GUILD_TOWER>(PacketHandler.HandleDestroyGuildTower) },
            {typeof(S_REQUEST_CITY_WAR_MAP_INFO),              new Action<S_REQUEST_CITY_WAR_MAP_INFO>(PacketHandler.HandleCityWarMapInfo) },
            {typeof(S_REQUEST_CITY_WAR_MAP_INFO_DETAIL),       new Action<S_REQUEST_CITY_WAR_MAP_INFO_DETAIL>(PacketHandler.HandleCityWarMapInfoDetail) },
            {typeof(S_UPDATE_NPCGUILD),                        new Action<S_UPDATE_NPCGUILD>(PacketHandler.HandleUpdateNpcGuild) },
            {typeof(S_NPCGUILD_LIST),                          new Action<S_NPCGUILD_LIST>(PacketHandler.HandleNpcGuildList) },
            {typeof(S_NOTIFY_GUILD_QUEST_URGENT),              new Action<S_NOTIFY_GUILD_QUEST_URGENT>(PacketHandler.HandleNotifyGuildQuestUrgent) },
            {typeof(S_CHANGE_GUILD_CHIEF),                     new Action<S_CHANGE_GUILD_CHIEF>(PacketHandler.HandleChangeGuildChief) },
            {typeof(S_GUILD_MEMBER_LIST),                      new Action<S_GUILD_MEMBER_LIST>(PacketHandler.HandleGuildMembersList) },
            {typeof(S_CREATURE_LIFE),                          new Action<S_CREATURE_LIFE>(PacketHandler.HandleCreatureLife) },
          //{typeof(S_VIEW_WARE_EX),                           new Action<S_VIEW_WARE_EX>(PacketHandler.HandleViewWareEx) },
          //{typeof(S_ACTION_STAGE),                           new Action<S_ACTION_STAGE>(x => PacketHandler.HandleActionStage(x)) }, //nvm
          //{typeof(C_LOAD_TOPO_FIN),                          new Action<C_LOAD_TOPO_FIN>(x => PacketHandler.HandleLoadTopoFin(x)) },
        };

        private static readonly Dictionary<Type, Delegate> PartyMemberPosition = new Dictionary<Type, Delegate>
        {
            {typeof(S_PARTY_MEMBER_INTERVAL_POS_UPDATE),       new Action<S_PARTY_MEMBER_INTERVAL_POS_UPDATE>(PacketHandler.HandlePartyMemberIntervalPosUpdate)},
        };
        private static readonly Dictionary<Type, Delegate> AccurateHp = new Dictionary<Type, Delegate>
        {
          //{typeof(S_EACH_SKILL_RESULT),                      new Action<S_EACH_SKILL_RESULT>(PacketHandler.HandleSkillResult)},
            {typeof(S_SHOW_HP),                                new Action<S_SHOW_HP>(PacketHandler.HandleShowHp)}
        };
        private static readonly Dictionary<Type, Delegate> CooldownWindow = new Dictionary<Type, Delegate>
        {
            {typeof(S_START_COOLTIME_SKILL),                   new Action<S_START_COOLTIME_SKILL>(PacketHandler.HandleNewSkillCooldown) },
            {typeof(S_DECREASE_COOLTIME_SKILL),                new Action<S_DECREASE_COOLTIME_SKILL>(PacketHandler.HandleDecreaseSkillCooldown) },
            {typeof(S_START_COOLTIME_ITEM),                    new Action<S_START_COOLTIME_ITEM>(PacketHandler.HandleNewItemCooldown) },
        };
        private static readonly Dictionary<Type, Delegate> GroupWindow = new Dictionary<Type, Delegate>
        {
            {typeof(S_PARTY_MEMBER_LIST),                      new Action<S_PARTY_MEMBER_LIST>(PacketHandler.HandlePartyMemberList) },
            {typeof(S_LOGOUT_PARTY_MEMBER),                    new Action<S_LOGOUT_PARTY_MEMBER>(PacketHandler.HandlePartyMemberLogout) },
            {typeof(S_LEAVE_PARTY_MEMBER),                     new Action<S_LEAVE_PARTY_MEMBER>(PacketHandler.HandlePartyMemberLeave) },
            {typeof(S_LEAVE_PARTY),                            new Action<S_LEAVE_PARTY>(PacketHandler.HandleLeaveParty) },
            {typeof(S_BAN_PARTY_MEMBER),                       new Action<S_BAN_PARTY_MEMBER>(PacketHandler.HandlePartyMemberKick) },
            {typeof(S_BAN_PARTY),                              new Action<S_BAN_PARTY>(PacketHandler.HandleKicked) },
            {typeof(S_PARTY_MEMBER_STAT_UPDATE),               new Action<S_PARTY_MEMBER_STAT_UPDATE>(PacketHandler.HandlePartyMemberStats) },
            {typeof(S_CHECK_TO_READY_PARTY),                   new Action<S_CHECK_TO_READY_PARTY>(PacketHandler.HandleReadyCheck) },
            {typeof(S_CHECK_TO_READY_PARTY_FIN),               new Action<S_CHECK_TO_READY_PARTY_FIN>(PacketHandler.HandleReadyCheckFin) },
            {typeof(S_ASK_BIDDING_RARE_ITEM),                  new Action<S_ASK_BIDDING_RARE_ITEM>(PacketHandler.HandleStartRoll) },
            {typeof(S_RESULT_ITEM_BIDDING),                    new Action<S_RESULT_ITEM_BIDDING>(PacketHandler.HandleEndRoll) },
            {typeof(S_RESULT_BIDDING_DICE_THROW),              new Action<S_RESULT_BIDDING_DICE_THROW>(PacketHandler.HandleRollResult) },
            {typeof(S_CHANGE_PARTY_MANAGER),                   new Action<S_CHANGE_PARTY_MANAGER>(PacketHandler.HandleChangeLeader) },
        };
        private static readonly Dictionary<Type, Delegate> GroupWindowAbnormals = new Dictionary<Type, Delegate>
        {
            {typeof(S_PARTY_MEMBER_BUFF_UPDATE),               new Action<S_PARTY_MEMBER_BUFF_UPDATE>(PacketHandler.HandlePartyMemberBuffUpdate) },
            {typeof(S_PARTY_MEMBER_ABNORMAL_ADD),              new Action<S_PARTY_MEMBER_ABNORMAL_ADD>(PacketHandler.HandlePartyMemberAbnormalAdd) },
            {typeof(S_PARTY_MEMBER_ABNORMAL_REFRESH),          new Action<S_PARTY_MEMBER_ABNORMAL_REFRESH>(PacketHandler.HandlePartyMemberAbnormalRefresh) },
            {typeof(S_PARTY_MEMBER_ABNORMAL_DEL),              new Action<S_PARTY_MEMBER_ABNORMAL_DEL>(PacketHandler.HandlePartyMemberAbnormalDel) },
            {typeof(S_PARTY_MEMBER_ABNORMAL_CLEAR),            new Action<S_PARTY_MEMBER_ABNORMAL_CLEAR>(PacketHandler.HandlePartyMemberAbnormalClear) },
        };
        private static readonly Dictionary<Type, Delegate> GroupWindowMp = new Dictionary<Type, Delegate>
        {
            {typeof(S_PARTY_MEMBER_CHANGE_MP),                 new Action<S_PARTY_MEMBER_CHANGE_MP>(PacketHandler.HandlePartyMemberMp) },
        };
        private static readonly Dictionary<Type, Delegate> GroupWindowHp = new Dictionary<Type, Delegate>
        {
            {typeof(S_PARTY_MEMBER_CHANGE_HP),                 new Action<S_PARTY_MEMBER_CHANGE_HP>(PacketHandler.HandlePartyMemberHp) },
        };
        private static readonly Dictionary<Type, Delegate> Phase1Only = new Dictionary<Type, Delegate>
        {
            {typeof(C_PLAYER_LOCATION),                        new Action<C_PLAYER_LOCATION>(PacketHandler.HandlePlayerLocation) },
            {typeof(S_DUNGEON_EVENT_MESSAGE),                  new Action<S_DUNGEON_EVENT_MESSAGE>(PacketHandler.HandleDungeonMessage) },
        };
        private static readonly Dictionary<Type, Delegate> ChatWindow = new Dictionary<Type, Delegate>
        {
            {typeof(S_PRIVATE_CHAT),                           new Action<S_PRIVATE_CHAT>(PacketHandler.HandlePrivateChat) },
            {typeof(S_WHISPER),                                new Action<S_WHISPER>(PacketHandler.HandleWhisper) },
            {typeof(S_JOIN_PRIVATE_CHANNEL),                   new Action<S_JOIN_PRIVATE_CHANNEL>(PacketHandler.HandleJoinPrivateChat) },
            {typeof(S_LEAVE_PRIVATE_CHANNEL),                  new Action<S_LEAVE_PRIVATE_CHANNEL>(PacketHandler.HandleLeavePrivateChat) },
            {typeof(S_SYSTEM_MESSAGE_LOOT_ITEM),               new Action<S_SYSTEM_MESSAGE_LOOT_ITEM>(PacketHandler.HandleSystemMessageLoot) },
            {typeof(S_CREST_MESSAGE),                          new Action<S_CREST_MESSAGE>(PacketHandler.HandleCrestMessage) },
            {typeof(S_USER_BLOCK_LIST),                        new Action<S_USER_BLOCK_LIST>(PacketHandler.HandleBlockList) },
            {typeof(S_FRIEND_LIST),                            new Action<S_FRIEND_LIST>(PacketHandler.HandleFriendList) },
            {typeof(S_ACCOMPLISH_ACHIEVEMENT),                 new Action<S_ACCOMPLISH_ACHIEVEMENT>(PacketHandler.HandleAccomplishAchievement) },
            {typeof(S_TRADE_BROKER_DEAL_SUGGESTED),            new Action<S_TRADE_BROKER_DEAL_SUGGESTED>(PacketHandler.HandleBrokerOffer) },
            {typeof(S_UPDATE_FRIEND_INFO),                     new Action<S_UPDATE_FRIEND_INFO>(PacketHandler.HandleFriendStatus) },
            {typeof(S_PARTY_MEMBER_INFO),                      new Action<S_PARTY_MEMBER_INFO>(PacketHandler.HandlePartyMemberInfo) },
            {typeof(S_OTHER_USER_APPLY_PARTY),                 new Action<S_OTHER_USER_APPLY_PARTY>(PacketHandler.HandleUserApplyToParty) },
            {typeof(S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA),  new Action<S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA>(PacketHandler.HandleFriendIntoArea) },
            {typeof(S_FIELD_EVENT_ON_ENTER),                   new Action<S_FIELD_EVENT_ON_ENTER>(PacketHandler.HandleGuardianOnEnter) },
            {typeof(S_FIELD_EVENT_ON_LEAVE),                   new Action<S_FIELD_EVENT_ON_LEAVE>(PacketHandler.HandleGuardianOnLeave) },
        };
        private static readonly Dictionary<Type, Delegate> ChatWindowLfg = new Dictionary<Type, Delegate>
        {
            {typeof(S_PARTY_MATCH_LINK),                       new Action<S_PARTY_MATCH_LINK>(PacketHandler.HandleLfgSpam) },
        };
        private static readonly Dictionary<Type, Delegate> ValkyrieOnly = new Dictionary<Type, Delegate>
        {
            {typeof(S_WEAK_POINT),                             new Action<S_WEAK_POINT>(PacketHandler.HandleRunemark) },
        };
        private static readonly Dictionary<Type, Delegate> BossWindow = new Dictionary<Type, Delegate>
        {
            {typeof(S_BOSS_GAGE_INFO),                         new Action<S_BOSS_GAGE_INFO>(PacketHandler.HandleBossGageInfo) },
            {typeof(S_NPC_STATUS),                             new Action<S_NPC_STATUS>(PacketHandler.HandleNpcStatusChanged) },
            {typeof(S_GUILD_TOWER_INFO),                       new Action<S_GUILD_TOWER_INFO>(PacketHandler.HandleGuildTowerInfo) },
        };
        private static readonly Dictionary<Type, Delegate> InfoWindow = new Dictionary<Type, Delegate>
        {
            {typeof(S_AVAILABLE_EVENT_MATCHING_LIST),          new Action<S_AVAILABLE_EVENT_MATCHING_LIST>(PacketHandler.HandleVanguardReceived) },
            {typeof(S_DUNGEON_COOL_TIME_LIST),                 new Action<S_DUNGEON_COOL_TIME_LIST>(PacketHandler.HandleDungeonCooltimeList) },
            {typeof(S_ACCOUNT_PACKAGE_LIST),                   new Action<S_ACCOUNT_PACKAGE_LIST>(PacketHandler.HandleAccountPackageList) },
            {typeof(S_FIELD_POINT_INFO),                       new Action<S_FIELD_POINT_INFO>(PacketHandler.HandleGuardianInfo) },
            {typeof(S_DUNGEON_CLEAR_COUNT_LIST),               new Action<S_DUNGEON_CLEAR_COUNT_LIST>(PacketHandler.HandleDungeonClears) },
        };

        public readonly uint Version;
        public int ReleaseVersion { get; set; }
        public OpCodeNamer OpCodeNamer { get; }
        public OpCodeNamer SystemMessageNamer { get; private set; }
        public static bool NoGuildBamOpcode { get; private set; }    //by HQ 20190324
        public MessageFactory()
        {
            OpCodeNamer = new OpCodeNamer(new Dictionary<ushort, string> { { 19900, nameof(C_CHECK_VERSION) } });
            Init.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            SessionManager.Server = new Server("", "", "", 0);
            Version = 0;
        }
        public MessageFactory(uint version, OpCodeNamer opcNamer)
        {
            OpCodeNamer = opcNamer;
            OpcodeNameToType.Clear();
            Version = version;
            TeraMessages.ToList().ForEach(x => OpcodeNameToType[OpCodeNamer.GetCode(x.Key)] = x.Value);

            // by HQ 20190324 ===================================
            NoGuildBamOpcode = OpCodeNamer.GetCode(nameof(S_NOTIFY_GUILD_QUEST_URGENT)) == 0;
            // ==================================================
            Update();
        }

        public static void Update()
        {
            MainProcessor.Clear();

            Base.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);

            InfoWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);

            if (SettingsHolder.ChatEnabled)
            {
                ChatWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
                ChatWindowLfg.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            }
            if (SettingsHolder.CooldownWindowSettings.Enabled || SettingsHolder.ClassWindowSettings.Enabled) CooldownWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            if (SettingsHolder.BossWindowSettings.Enabled || SettingsHolder.GroupWindowSettings.Enabled) BossWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            if (SettingsHolder.GroupWindowSettings.Enabled)
            {
                GroupWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
                if (!SettingsHolder.DisablePartyAbnormals) GroupWindowAbnormals.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
                if (!SettingsHolder.DisablePartyMP) GroupWindowMp.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
                if (!SettingsHolder.DisablePartyHP) GroupWindowHp.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            }
            if (SettingsHolder.ClassWindowSettings.Enabled && SessionManager.CurrentPlayer.Class == Class.Valkyrie) ValkyrieOnly.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            if (WindowManager.BossWindow.VM.CurrentHHphase == HarrowholdPhase.Phase1) Phase1Only.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            if (SettingsHolder.AccurateHp) AccurateHp.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            if (!SessionManager.CivilUnrestZone) PartyMemberPosition.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
        }
        private ParsedMessage Instantiate(ushort opCode, TeraMessageReader reader)
        {
            if (!OpcodeNameToType.TryGetValue(opCode, out var type))
                type = UnknownMessageDelegate;
            return (ParsedMessage)type.DynamicInvoke(reader);
        }
        public ParsedMessage Create(Message message)
        {
            var reader = new TeraMessageReader(message, OpCodeNamer, this, SystemMessageNamer);
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
            MainProcessor.TryGetValue(message.GetType(), out var type);
            if (type == null) return false;
            type.DynamicInvoke(message);
            return true;
        }

        public void ReloadSysMsg()
        {
            if (SystemMessageNamer == null)
            {
                SystemMessageNamer = new OpCodeNamer(Path.Combine(App.DataPath, $"opcodes/sysmsg.{ReleaseVersion}.map"));
            }
            SystemMessageNamer?.Reload(Version, ReleaseVersion);
        }
    }
}
