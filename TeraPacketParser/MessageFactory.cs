using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using TeraPacketParser.Messages;

namespace TeraPacketParser
{
    public class MessageFactory
    {
        public static event Action<int> ReleaseVersionChanged;

        private static readonly Delegate? UnknownMessageDelegate = Constructor<Func<TeraMessageReader, UnknownMessage>>();
        private static readonly Dictionary<ushort, Delegate?> OpcodeNameToType = new Dictionary<ushort, Delegate?> { { 19900, Constructor<Func<TeraMessageReader, C_CHECK_VERSION>>() } };
        private static readonly Dictionary<string, Delegate?> TeraMessages = new Dictionary<string, Delegate?>
        {
            { nameof(C_CHECK_VERSION),                         Constructor<Func<TeraMessageReader, C_CHECK_VERSION>>()},
            { nameof(C_LOGIN_ARBITER),                         Constructor<Func<TeraMessageReader, C_LOGIN_ARBITER>>()},
            { nameof(S_LOGIN) ,                                Constructor<Func<TeraMessageReader, S_LOGIN>>()},
            { nameof(S_START_COOLTIME_SKILL) ,                 Constructor<Func<TeraMessageReader, S_START_COOLTIME_SKILL>>()},
            { nameof(S_DECREASE_COOLTIME_SKILL) ,              Constructor<Func<TeraMessageReader, S_DECREASE_COOLTIME_SKILL>>()},
            { nameof(S_START_COOLTIME_ITEM) ,                  Constructor<Func<TeraMessageReader, S_START_COOLTIME_ITEM>>()},
            { nameof(S_PLAYER_CHANGE_MP) ,                     Constructor<Func<TeraMessageReader, S_PLAYER_CHANGE_MP>>()},
            { nameof(S_CREATURE_CHANGE_HP) ,                   Constructor<Func<TeraMessageReader, S_CREATURE_CHANGE_HP>>()},
            { nameof(S_PLAYER_CHANGE_STAMINA) ,                Constructor<Func<TeraMessageReader, S_PLAYER_CHANGE_STAMINA>>()},
            { nameof(S_PLAYER_CHANGE_FLIGHT_ENERGY) ,          Constructor<Func<TeraMessageReader, S_PLAYER_CHANGE_FLIGHT_ENERGY>>()},
            { nameof(S_PLAYER_STAT_UPDATE) ,                   Constructor<Func<TeraMessageReader, S_PLAYER_STAT_UPDATE>>()},
            { nameof(S_USER_STATUS) ,                          Constructor<Func<TeraMessageReader, S_USER_STATUS>>()},
            { nameof(S_SPAWN_NPC) ,                            Constructor<Func<TeraMessageReader, S_SPAWN_NPC>>()},
            { nameof(S_DESPAWN_NPC) ,                          Constructor<Func<TeraMessageReader, S_DESPAWN_NPC>>()},
            { nameof(S_NPC_STATUS) ,                           Constructor<Func<TeraMessageReader, S_NPC_STATUS>>()},
            { nameof(S_BOSS_GAGE_INFO) ,                       Constructor<Func<TeraMessageReader, S_BOSS_GAGE_INFO>>()},
            { nameof(S_ABNORMALITY_BEGIN) ,                    Constructor<Func<TeraMessageReader, S_ABNORMALITY_BEGIN>>()},
            { nameof(S_ABNORMALITY_REFRESH),                   Constructor<Func<TeraMessageReader, S_ABNORMALITY_REFRESH>>()},
            { nameof(S_ABNORMALITY_END) ,                      Constructor<Func<TeraMessageReader, S_ABNORMALITY_END>>()},
            { nameof(S_GET_USER_LIST) ,                        Constructor<Func<TeraMessageReader, S_GET_USER_LIST>>()},
            { nameof(S_SPAWN_ME) ,                             Constructor<Func<TeraMessageReader, S_SPAWN_ME>>()},
            { nameof(S_RETURN_TO_LOBBY) ,                      Constructor<Func<TeraMessageReader, S_RETURN_TO_LOBBY>>()},
            { nameof(C_PLAYER_LOCATION) ,                      Constructor<Func<TeraMessageReader, C_PLAYER_LOCATION>>() },
            { nameof(S_USER_EFFECT) ,                          Constructor<Func<TeraMessageReader, S_USER_EFFECT>>() },
            { nameof(S_LOAD_TOPO) ,                            Constructor<Func<TeraMessageReader, S_LOAD_TOPO>>() },
            { nameof(S_DESPAWN_USER),                          Constructor<Func<TeraMessageReader, S_DESPAWN_USER>>() },
            { nameof(S_PARTY_MEMBER_LIST),                     Constructor<Func<TeraMessageReader, S_PARTY_MEMBER_LIST>>() },
            { nameof(S_LOGOUT_PARTY_MEMBER),                   Constructor<Func<TeraMessageReader, S_LOGOUT_PARTY_MEMBER>>() },
            { nameof(S_LEAVE_PARTY_MEMBER),                    Constructor<Func<TeraMessageReader, S_LEAVE_PARTY_MEMBER>>() },
            { nameof(S_LEAVE_PARTY),                           Constructor<Func<TeraMessageReader, S_LEAVE_PARTY>>() },
            { nameof(S_BAN_PARTY_MEMBER),                      Constructor<Func<TeraMessageReader, S_BAN_PARTY_MEMBER>>() },
            { nameof(S_BAN_PARTY),                             Constructor<Func<TeraMessageReader, S_BAN_PARTY>>() },
            { nameof(S_PARTY_MEMBER_CHANGE_HP),                Constructor<Func<TeraMessageReader, S_PARTY_MEMBER_CHANGE_HP>>() },
            { nameof(S_PARTY_MEMBER_CHANGE_MP),                Constructor<Func<TeraMessageReader, S_PARTY_MEMBER_CHANGE_MP>>() },
            { nameof(S_PARTY_MEMBER_CHANGE_STAMINA),           Constructor<Func<TeraMessageReader, S_PARTY_MEMBER_CHANGE_STAMINA>>() },
            { nameof(S_PARTY_MEMBER_STAT_UPDATE),              Constructor<Func<TeraMessageReader, S_PARTY_MEMBER_STAT_UPDATE>>() },
            { nameof(S_CHECK_TO_READY_PARTY),                  Constructor<Func<TeraMessageReader, S_CHECK_TO_READY_PARTY>>() },
            { nameof(S_CHECK_TO_READY_PARTY_FIN),              Constructor<Func<TeraMessageReader, S_CHECK_TO_READY_PARTY_FIN>>() },
            { nameof(S_ASK_BIDDING_RARE_ITEM),                 Constructor<Func<TeraMessageReader, S_ASK_BIDDING_RARE_ITEM>>() },
            { nameof(S_RESULT_ITEM_BIDDING),                   Constructor<Func<TeraMessageReader, S_RESULT_ITEM_BIDDING>>() },
            { nameof(S_RESULT_BIDDING_DICE_THROW),             Constructor<Func<TeraMessageReader, S_RESULT_BIDDING_DICE_THROW>>() },
            { nameof(S_PARTY_MEMBER_BUFF_UPDATE),              Constructor<Func<TeraMessageReader, S_PARTY_MEMBER_BUFF_UPDATE>>() },
            { nameof(S_PARTY_MEMBER_ABNORMAL_ADD),             Constructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_ADD>>() },
            { nameof(S_PARTY_MEMBER_ABNORMAL_REFRESH),         Constructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_REFRESH>>() },
            { nameof(S_PARTY_MEMBER_ABNORMAL_DEL),             Constructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_DEL>>() },
            { nameof(S_PARTY_MEMBER_ABNORMAL_CLEAR),           Constructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_CLEAR>>() },
            { nameof(S_CHANGE_PARTY_MANAGER),                  Constructor<Func<TeraMessageReader, S_CHANGE_PARTY_MANAGER>>() },
            { nameof(S_WEAK_POINT),                            Constructor<Func<TeraMessageReader, S_WEAK_POINT>>() },
            { nameof(S_CHAT),                                  Constructor<Func<TeraMessageReader, S_CHAT>>() },
            { nameof(S_WHISPER),                               Constructor<Func<TeraMessageReader, S_WHISPER>>() },
            { nameof(S_PRIVATE_CHAT),                          Constructor<Func<TeraMessageReader, S_PRIVATE_CHAT>>() },
            { nameof(S_JOIN_PRIVATE_CHANNEL),                  Constructor<Func<TeraMessageReader, S_JOIN_PRIVATE_CHANNEL>>() },
            { nameof(S_LEAVE_PRIVATE_CHANNEL),                 Constructor<Func<TeraMessageReader, S_LEAVE_PRIVATE_CHANNEL>>() },
            { nameof(S_SYSTEM_MESSAGE),                        Constructor<Func<TeraMessageReader, S_SYSTEM_MESSAGE>>() },
            { nameof(S_SYSTEM_MESSAGE_LOOT_ITEM),              Constructor<Func<TeraMessageReader, S_SYSTEM_MESSAGE_LOOT_ITEM>>() },
            { nameof(S_CREST_MESSAGE),                         Constructor<Func<TeraMessageReader, S_CREST_MESSAGE>>() },
            { nameof(S_ANSWER_INTERACTIVE),                    Constructor<Func<TeraMessageReader, S_ANSWER_INTERACTIVE>>() },
            { nameof(S_USER_BLOCK_LIST),                       Constructor<Func<TeraMessageReader, S_USER_BLOCK_LIST>>() },
            { nameof(S_FRIEND_LIST),                           Constructor<Func<TeraMessageReader, S_FRIEND_LIST>>() },
            { nameof(S_ACCOMPLISH_ACHIEVEMENT),                Constructor<Func<TeraMessageReader, S_ACCOMPLISH_ACHIEVEMENT>>() },
            { nameof(S_TRADE_BROKER_DEAL_SUGGESTED),           Constructor<Func<TeraMessageReader, S_TRADE_BROKER_DEAL_SUGGESTED>>() },
            { nameof(S_UPDATE_FRIEND_INFO),                    Constructor<Func<TeraMessageReader, S_UPDATE_FRIEND_INFO>>() },
            { nameof(S_PARTY_MATCH_LINK),                      Constructor<Func<TeraMessageReader, S_PARTY_MATCH_LINK>>() },
            { nameof(S_PARTY_MEMBER_INFO),                     Constructor<Func<TeraMessageReader, S_PARTY_MEMBER_INFO>>() },
            { nameof(S_OTHER_USER_APPLY_PARTY),                Constructor<Func<TeraMessageReader, S_OTHER_USER_APPLY_PARTY>>() },
            { nameof(S_DUNGEON_EVENT_MESSAGE),                 Constructor<Func<TeraMessageReader, S_DUNGEON_EVENT_MESSAGE>>() },
            { nameof(S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA), Constructor<Func<TeraMessageReader, S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA>>() },
            { nameof(S_AVAILABLE_EVENT_MATCHING_LIST),         Constructor<Func<TeraMessageReader, S_AVAILABLE_EVENT_MATCHING_LIST>>() },
            { nameof(S_DUNGEON_COOL_TIME_LIST),                Constructor<Func<TeraMessageReader, S_DUNGEON_COOL_TIME_LIST>>() },
            { nameof(S_ACCOUNT_PACKAGE_LIST),                  Constructor<Func<TeraMessageReader, S_ACCOUNT_PACKAGE_LIST>>() },
            { nameof(S_GUILD_TOWER_INFO),                      Constructor<Func<TeraMessageReader, S_GUILD_TOWER_INFO>>() },
            { nameof(S_ITEMLIST),                              Constructor<Func<TeraMessageReader, S_ITEMLIST>>() },
            { nameof(S_SPAWN_USER),                            Constructor<Func<TeraMessageReader, S_SPAWN_USER>>() },
            { nameof(S_PARTY_MEMBER_INTERVAL_POS_UPDATE),      Constructor<Func<TeraMessageReader, S_PARTY_MEMBER_INTERVAL_POS_UPDATE>>() },
            { nameof(S_ABNORMALITY_DAMAGE_ABSORB),             Constructor<Func<TeraMessageReader, S_ABNORMALITY_DAMAGE_ABSORB>>() },
            { nameof(S_IMAGE_DATA),                            Constructor<Func<TeraMessageReader, S_IMAGE_DATA>>() },
            { nameof(S_GET_USER_GUILD_LOGO),                   Constructor<Func<TeraMessageReader, S_GET_USER_GUILD_LOGO>>() },
            { nameof(S_FIELD_POINT_INFO),                      Constructor<Func<TeraMessageReader, S_FIELD_POINT_INFO>>() },
            { nameof(S_DUNGEON_CLEAR_COUNT_LIST),              Constructor<Func<TeraMessageReader, S_DUNGEON_CLEAR_COUNT_LIST>>() },
            { nameof(S_SHOW_PARTY_MATCH_INFO),                 Constructor<Func<TeraMessageReader, S_SHOW_PARTY_MATCH_INFO>>() },
            { nameof(S_SHOW_CANDIDATE_LIST),                   Constructor<Func<TeraMessageReader, S_SHOW_CANDIDATE_LIST>>() },
            { nameof(S_SHOW_HP),                               Constructor<Func<TeraMessageReader, S_SHOW_HP>>() },
            { nameof(S_REQUEST_CITY_WAR_MAP_INFO),             Constructor<Func<TeraMessageReader, S_REQUEST_CITY_WAR_MAP_INFO>>() },
            { nameof(S_REQUEST_CITY_WAR_MAP_INFO_DETAIL),      Constructor<Func<TeraMessageReader, S_REQUEST_CITY_WAR_MAP_INFO_DETAIL>>() },
            { nameof(S_DESTROY_GUILD_TOWER),                   Constructor<Func<TeraMessageReader, S_DESTROY_GUILD_TOWER>>() },
            { nameof(S_FIELD_EVENT_ON_ENTER),                  Constructor<Func<TeraMessageReader, S_FIELD_EVENT_ON_ENTER>>() },
            { nameof(S_FIELD_EVENT_ON_LEAVE),                  Constructor<Func<TeraMessageReader, S_FIELD_EVENT_ON_LEAVE>>() },
            { nameof(S_UPDATE_NPCGUILD),                       Constructor<Func<TeraMessageReader, S_UPDATE_NPCGUILD>>() },
            { nameof(S_NPCGUILD_LIST),                         Constructor<Func<TeraMessageReader, S_NPCGUILD_LIST>>() },
            { nameof(S_NOTIFY_GUILD_QUEST_URGENT),             Constructor<Func<TeraMessageReader, S_NOTIFY_GUILD_QUEST_URGENT>>() },
            { nameof(S_CHANGE_GUILD_CHIEF),                    Constructor<Func<TeraMessageReader, S_CHANGE_GUILD_CHIEF>>() },
            { nameof(S_GUILD_MEMBER_LIST),                     Constructor<Func<TeraMessageReader, S_GUILD_MEMBER_LIST>>() },
            { nameof(S_CREATURE_LIFE),                         Constructor<Func<TeraMessageReader, S_CREATURE_LIFE>>() },
            { nameof(S_PLAYER_CHANGE_EXP),                     Constructor<Func<TeraMessageReader, S_PLAYER_CHANGE_EXP>>() },
            { nameof(S_LOAD_EP_INFO),                          Constructor<Func<TeraMessageReader, S_LOAD_EP_INFO>>() },
            { nameof(S_LEARN_EP_PERK),                         Constructor<Func<TeraMessageReader, S_LEARN_EP_PERK>>() },
            { nameof(S_RESET_EP_PERK),                         Constructor<Func<TeraMessageReader, S_RESET_EP_PERK>>() },
            { nameof(S_REQUEST_SPAWN_SERVANT),                 Constructor<Func<TeraMessageReader, S_REQUEST_SPAWN_SERVANT>>() },
            { nameof(S_FIN_INTER_PARTY_MATCH),                 Constructor<Func<TeraMessageReader, S_FIN_INTER_PARTY_MATCH>>() },
            { nameof(S_BATTLE_FIELD_ENTRANCE_INFO),            Constructor<Func<TeraMessageReader, S_BATTLE_FIELD_ENTRANCE_INFO>>() },
            { nameof(S_LEAVE_GUILD),                           Constructor<Func<TeraMessageReader, S_LEAVE_GUILD>>() },
            { nameof(S_BEGIN_THROUGH_ARBITER_CONTRACT),        Constructor<Func<TeraMessageReader, S_BEGIN_THROUGH_ARBITER_CONTRACT>>() },
            { nameof(S_FATIGABILITY_POINT),                    Constructor<Func<TeraMessageReader, S_FATIGABILITY_POINT>>() },
          //{ nameof(S_VIEW_WARE_EX),                          Contructor<Func<TeraMessageReader, S_VIEW_WARE_EX>>() },
          //{ nameof(S_ACTION_STAGE),                          Contructor<Func<TeraMessageReader, S_ACTION_STAGE>>() }, //nvm
          //{ nameof(S_EACH_SKILL_RESULT),                     Contructor<Func<TeraMessageReader, S_EACH_SKILL_RESULT>>() },
        };
        public static readonly Dictionary<string, ushort> Extras = new Dictionary<string, ushort>();
        public uint Version { get; private set; }

        private int _releaseVersion;
        public int ReleaseVersion
        {
            get => _releaseVersion;
            set
            {
                if (_releaseVersion == value) return;
                _releaseVersion = value;
                ReleaseVersionChanged?.Invoke(value);
            }
        }

        public OpCodeNamer OpCodeNamer { get; private set; }
        public OpCodeNamer SystemMessageNamer { get; set; }
        public static bool NoGuildBamOpcode { get; private set; }    //by HQ 20190324
        public MessageFactory()
        {
            OpCodeNamer = new OpCodeNamer(new Dictionary<ushort, string> { { 19900, nameof(C_CHECK_VERSION) } });
            foreach (var (key, val) in Extras.ToList())
            {
                OpCodeNamer.Add(key, val);
            }
            SystemMessageNamer = new OpCodeNamer(new KeyValuePair<ushort, string>[0]);
            Version = 0;
        }

        public void Refresh()
        {
            TeraMessages.ToList().ForEach(x => OpcodeNameToType[OpCodeNamer.GetCode(x.Key)] = x.Value);
        }
        public void Set(uint version, OpCodeNamer opcNamer)
        {
            OpCodeNamer = opcNamer;
            OpcodeNameToType.Clear();
            Version = version;
            Refresh();
            // by HQ 20190324 ===================================
            NoGuildBamOpcode = OpCodeNamer.GetCode(nameof(S_NOTIFY_GUILD_QUEST_URGENT)) == 0;
            // ==================================================
        }
#pragma warning disable 8618
        public MessageFactory(uint version, OpCodeNamer opcNamer) // only for testing
#pragma warning restore 8618
        {
            OpCodeNamer = opcNamer;
            OpcodeNameToType.Clear();
            Version = version;
            Refresh();

            // by HQ 20190324 ===================================
            NoGuildBamOpcode = OpCodeNamer.GetCode(nameof(S_NOTIFY_GUILD_QUEST_URGENT)) == 0;
            // ==================================================
        }

        public void InjectExtra(string name, ushort code)
        {
            Extras[name] = code;
            OpCodeNamer.Add(name, code);
        }


        private ParsedMessage? Instantiate(ushort opCode, TeraMessageReader reader)
        {
            if (!OpcodeNameToType.TryGetValue(opCode, out var type)) type = UnknownMessageDelegate;
            return (ParsedMessage?)type?.DynamicInvoke(reader);
        }
        public ParsedMessage? Create(Message message)
        {
            var reader = new TeraMessageReader(message, OpCodeNamer, this, SystemMessageNamer);
            return Instantiate(message.OpCode, reader);
        }
        public static TDelegate? Constructor<TDelegate>() where TDelegate : class
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


        public void ReloadSysMsg(string path)
        {
            SystemMessageNamer ??= new OpCodeNamer(path);
            SystemMessageNamer.Reload(Version, ReleaseVersion, path);
        }

        public List<string> OpcodesList => TeraMessages.Keys.Where(o => !(o.Equals(nameof(C_CHECK_VERSION)) || o.Equals(nameof(C_LOGIN_ARBITER)))).ToList();
    }
}
