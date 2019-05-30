using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using TeraPacketParser.Messages;

namespace TeraPacketParser
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
            { nameof(S_PLAYER_CHANGE_EXP),                     Contructor<Func<TeraMessageReader, S_PLAYER_CHANGE_EXP>>() },
            { nameof(S_LOAD_EP_INFO),                          Contructor<Func<TeraMessageReader, S_LOAD_EP_INFO>>() },
            { nameof(S_LEARN_EP_PERK),                         Contructor<Func<TeraMessageReader, S_LEARN_EP_PERK>>() },
            { nameof(S_RESET_EP_PERK),                         Contructor<Func<TeraMessageReader, S_RESET_EP_PERK>>() },
          //{ nameof(S_VIEW_WARE_EX),                          Contructor<Func<TeraMessageReader, S_VIEW_WARE_EX>>() },
          //{ nameof(S_ACTION_STAGE),                          Contructor<Func<TeraMessageReader, S_ACTION_STAGE>>() }, //nvm
          //{ nameof(S_EACH_SKILL_RESULT),                     Contructor<Func<TeraMessageReader, S_EACH_SKILL_RESULT>>() },
        };

        public readonly uint Version;
        public int ReleaseVersion { get; set; }
        public OpCodeNamer OpCodeNamer { get; }
        public OpCodeNamer SystemMessageNamer { get; set; }
        public static bool NoGuildBamOpcode { get; private set; }    //by HQ 20190324
        public MessageFactory()
        {
            OpCodeNamer = new OpCodeNamer(new Dictionary<ushort, string> { { 19900, nameof(C_CHECK_VERSION) } });
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


        public async void ReloadSysMsg(string path)
        {
            if (SystemMessageNamer == null)
            {

                SystemMessageNamer = new OpCodeNamer(path);
            }
            SystemMessageNamer?.Reload(Version, ReleaseVersion);
        }
    }
}
