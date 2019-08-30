using System;
using System.Collections.Generic;
using System.Linq;
using TCC.Data;
using TeraDataLite;
using TeraPacketParser;
using TeraPacketParser.Messages;

namespace TCC.Parsing
{
    public class MessageProcessor
    {

        private static readonly Dictionary<Type, Delegate> InitMessages = new Dictionary<Type, Delegate>()
        {
            {typeof(C_CHECK_VERSION),                          new Action<C_CHECK_VERSION>(PacketHandler.HandleCheckVersion)}
        };

        private static readonly Dictionary<Type, Delegate> Base = new Dictionary<Type, Delegate>()
        {
            {typeof(C_LOGIN_ARBITER),                          new Action<C_LOGIN_ARBITER>(PacketHandler.HandleLoginArbiter)},
            {typeof(S_CHAT),                                   new Action<S_CHAT>(PacketHandler.HandleChat) },
            {typeof(S_LOGIN),                                  new Action<S_LOGIN>(PacketHandler.HandleLogin) },
            {typeof(S_LOAD_TOPO),                              new Action<S_LOAD_TOPO>(PacketHandler.HandleLoadTopo) },
            {typeof(S_GET_USER_LIST),                          new Action<S_GET_USER_LIST>(PacketHandler.HandleGetUserList) },
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
            {typeof(S_PARTY_MEMBER_INFO),                      new Action<S_PARTY_MEMBER_INFO>(PacketHandler.HandlePartyMemberInfo) },
            {typeof(S_OTHER_USER_APPLY_PARTY),                 new Action<S_OTHER_USER_APPLY_PARTY>(PacketHandler.HandleUserApplyToParty) },
            {typeof(S_PLAYER_CHANGE_EXP),                      new Action<S_PLAYER_CHANGE_EXP>(PacketHandler.HandlePlayerChangeExp) },
            {typeof(S_LOAD_EP_INFO),                           new Action<S_LOAD_EP_INFO>(PacketHandler.HandleLoadEpInfo) },
            {typeof(S_LEARN_EP_PERK),                           new Action<S_LEARN_EP_PERK>(PacketHandler.HandleLearnEpPerk) },
            {typeof(S_RESET_EP_PERK),                           new Action<S_RESET_EP_PERK>(PacketHandler.HandleResetEpPerk) },
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

        private static readonly Dictionary<Type, Delegate> MainProcessor = new Dictionary<Type, Delegate>();

        public MessageProcessor()
        {
            Init();

        }

        private void Init()
        {
            InitMessages.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
        }

        public void Update()
        {
            if (App.Loading) return;
            MainProcessor.Clear();

            Base.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);

            InfoWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);

            if (App.Settings.ChatEnabled)
            {
                ChatWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
                ChatWindowLfg.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            }
            if (App.Settings.CooldownWindowSettings.Enabled || App.Settings.ClassWindowSettings.Enabled) CooldownWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            if (App.Settings.NpcWindowSettings.Enabled || App.Settings.GroupWindowSettings.Enabled) BossWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            if (App.Settings.GroupWindowSettings.Enabled)
            {
                GroupWindow.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
                if (WindowManager.ViewModels.Group.Size <= App.Settings.GroupWindowSettings.DisableAbnormalitiesThreshold) GroupWindowAbnormals.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
                if (WindowManager.ViewModels.Group.Size <= App.Settings.GroupWindowSettings.HideHpThreshold) GroupWindowHp.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
                if (WindowManager.ViewModels.Group.Size <= App.Settings.GroupWindowSettings.HideMpThreshold) GroupWindowMp.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            }
            if (App.Settings.ClassWindowSettings.Enabled && Game.Me.Class == Class.Valkyrie) ValkyrieOnly.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            if (WindowManager.ViewModels.NPC.CurrentHHphase == HarrowholdPhase.Phase1) Phase1Only.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            if (App.Settings.NpcWindowSettings.AccurateHp) AccurateHp.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
            if (!Game.CivilUnrestZone) PartyMemberPosition.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
        }

        public bool Process(ParsedMessage message)
        {
            MainProcessor.TryGetValue(message.GetType(), out var type);
            if (type == null) return false;
            type.DynamicInvoke(message);
            return true;
        }
    }
}
