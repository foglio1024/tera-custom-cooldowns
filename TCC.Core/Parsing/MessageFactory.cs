using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tera;
using Tera.Game;
using TCC.Parsing.Messages;

namespace TCC.Parsing
{
    public static class MessageFactory
    {
        private static readonly Delegate UnknownMessageDelegate = Contructor<Func<TeraMessageReader, Tera.Game.Messages.UnknownMessage>>();
        private static readonly Dictionary<ushort, Delegate> OpcodeNameToType = new Dictionary<ushort, Delegate> { { 19900, Contructor<Func<TeraMessageReader, Tera.Game.Messages.C_CHECK_VERSION>>() } };
        private static readonly Dictionary<string, Delegate> TeraMessages = new Dictionary<string, Delegate>
        {
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
            { "C_LOAD_TOPO_FIN" , Contructor<Func<TeraMessageReader,C_LOAD_TOPO_FIN>>() },
            {"S_SPAWN_USER", Contructor<Func<TeraMessageReader, S_SPAWN_USER>>() },
            {"S_DESPAWN_USER", Contructor<Func<TeraMessageReader, S_DESPAWN_USER>>() },
            {"S_PARTY_MEMBER_LIST", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_LIST>>() },
            {"S_LOGOUT_PARTY_MEMBER", Contructor<Func<TeraMessageReader, S_LOGOUT_PARTY_MEMBER>>() },
            { "S_LEAVE_PARTY_MEMBER", Contructor<Func<TeraMessageReader, S_LEAVE_PARTY_MEMBER>>() },
            {"S_LEAVE_PARTY", Contructor<Func<TeraMessageReader, S_LEAVE_PARTY>>() },
            {"S_BAN_PARTY_MEMBER", Contructor<Func<TeraMessageReader, S_BAN_PARTY_MEMBER>>() },
            {"S_PARTY_MEMBER_CHANGE_HP", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_CHANGE_HP>>() },
            {"S_PARTY_MEMBER_CHANGE_MP", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_CHANGE_MP>>() },
            {"S_PARTY_MEMBER_STAT_UPDATE", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_STAT_UPDATE>>() },
            {"S_CHECK_TO_READY_PARTY", Contructor<Func<TeraMessageReader, S_CHECK_TO_READY_PARTY>>() },
            {"S_CHECK_TO_READY_PARTY_FIN", Contructor<Func<TeraMessageReader, S_CHECK_TO_READY_PARTY_FIN>>() },
            {"S_ASK_BIDDING_RARE_ITEM", Contructor<Func<TeraMessageReader, S_ASK_BIDDING_RARE_ITEM>>() },
            {"S_RESULT_ITEM_BIDDING", Contructor<Func<TeraMessageReader, S_RESULT_ITEM_BIDDING>>() },
            {"S_RESULT_BIDDING_DICE_THROW", Contructor<Func<TeraMessageReader, S_RESULT_BIDDING_DICE_THROW>>() },

            {"S_PARTY_MEMBER_BUFF_UPDATE", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_BUFF_UPDATE>>() },
            {"S_PARTY_MEMBER_ABNORMAL_ADD", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_ADD>>() },
            {"S_PARTY_MEMBER_ABNORMAL_REFRESH", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_REFRESH>>() },
            {"S_PARTY_MEMBER_ABNORMAL_DEL", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_DEL>>() },
            {"S_PARTY_MEMBER_ABNORMAL_CLEAR", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_ABNORMAL_CLEAR>>() },
            {"S_CHANGE_PARTY_MANAGER", Contructor<Func<TeraMessageReader, S_CHANGE_PARTY_MANAGER>>() },
            {"S_WEAK_POINT", Contructor<Func<TeraMessageReader, S_WEAK_POINT>>() },
            //{"S_EACH_SKILL_RESULT", Contructor<Func<TeraMessageReader, S_EACH_SKILL_RESULT>>() },
            {"S_CHAT", Contructor<Func<TeraMessageReader, S_CHAT>>() },
            {"C_SHOW_ITEM_TOOLTIP_EX", Contructor<Func<TeraMessageReader, C_SHOW_ITEM_TOOLTIP_EX>>() },
            {"C_REQUEST_NONDB_ITEM_INFO", Contructor<Func<TeraMessageReader, C_REQUEST_NONDB_ITEM_INFO>>() },
            {"S_WHISPER", Contructor<Func<TeraMessageReader, S_WHISPER>>() },
            {"S_PRIVATE_CHAT", Contructor<Func<TeraMessageReader, S_PRIVATE_CHAT>>() },
            {"S_JOIN_PRIVATE_CHANNEL", Contructor<Func<TeraMessageReader, S_JOIN_PRIVATE_CHANNEL>>() },
            {"S_LEAVE_PRIVATE_CHANNEL", Contructor<Func<TeraMessageReader, S_LEAVE_PRIVATE_CHANNEL>>() },
            {"S_SYSTEM_MESSAGE", Contructor<Func<TeraMessageReader, S_SYSTEM_MESSAGE>>() },
            {"S_SYSTEM_MESSAGE_LOOT_ITEM", Contructor<Func<TeraMessageReader, S_SYSTEM_MESSAGE_LOOT_ITEM>>() },
            {"S_CREST_MESSAGE", Contructor<Func<TeraMessageReader, S_CREST_MESSAGE>>() },
            {"S_ANSWER_INTERACTIVE", Contructor<Func<TeraMessageReader, S_ANSWER_INTERACTIVE>>() },
            {"S_USER_BLOCK_LIST", Contructor<Func<TeraMessageReader, S_USER_BLOCK_LIST>>() },
            {"S_FRIEND_LIST", Contructor<Func<TeraMessageReader, S_FRIEND_LIST>>() },
            {"S_ACCOMPLISH_ACHIEVEMENT", Contructor<Func<TeraMessageReader, S_ACCOMPLISH_ACHIEVEMENT>>() },
            {"S_TRADE_BROKER_DEAL_SUGGESTED", Contructor<Func<TeraMessageReader, S_TRADE_BROKER_DEAL_SUGGESTED>>() },
            {"S_UPDATE_FRIEND_INFO", Contructor<Func<TeraMessageReader, S_UPDATE_FRIEND_INFO>>() },
            {"S_PARTY_MATCH_LINK", Contructor<Func<TeraMessageReader, S_PARTY_MATCH_LINK>>() },
            {"S_PARTY_MEMBER_INFO", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_INFO>>() },
            {"S_OTHER_USER_APPLY_PARTY", Contructor<Func<TeraMessageReader, S_OTHER_USER_APPLY_PARTY>>() },
            {"S_DUNGEON_EVENT_MESSAGE", Contructor<Func<TeraMessageReader, S_DUNGEON_EVENT_MESSAGE>>() },

        };

        private static Dictionary<Type, Delegate> MainProcessor = new Dictionary<Type, Delegate>();
        private static readonly Dictionary<Type, Delegate> MessageToProcessing = new Dictionary<Type, Delegate>
        {
            {typeof(S_LOGIN), new Action<S_LOGIN>(x => PacketProcessor.HandleLogin(x)) },
            {typeof(S_START_COOLTIME_SKILL), new Action<S_START_COOLTIME_SKILL>(x => PacketProcessor.HandleNewSkillCooldown(x)) },
            {typeof(S_DECREASE_COOLTIME_SKILL), new Action<S_DECREASE_COOLTIME_SKILL>(x => PacketProcessor.HandleDecreaseSkillCooldown(x)) },
            {typeof(S_START_COOLTIME_ITEM), new Action<S_START_COOLTIME_ITEM>(x => PacketProcessor.HandleNewItemCooldown(x)) },
            {typeof(S_PLAYER_CHANGE_MP), new Action<S_PLAYER_CHANGE_MP>(x => PacketProcessor.HandlePlayerChangeMP(x)) },
            {typeof(S_CREATURE_CHANGE_HP), new Action<S_CREATURE_CHANGE_HP>(x => PacketProcessor.HandleCreatureChangeHP(x)) },
            {typeof(S_PLAYER_CHANGE_STAMINA), new Action<S_PLAYER_CHANGE_STAMINA>(x => PacketProcessor.HandlePlayerChangeStamina(x)) },
            {typeof(S_PLAYER_CHANGE_FLIGHT_ENERGY), new Action<S_PLAYER_CHANGE_FLIGHT_ENERGY>(x => PacketProcessor.HandlePlayerChangeFlightEnergy(x)) },
            {typeof(S_PLAYER_STAT_UPDATE), new Action<S_PLAYER_STAT_UPDATE>(x => PacketProcessor.HandlePlayerStatUpdate(x)) },
            {typeof(S_USER_STATUS), new Action<S_USER_STATUS>(x => PacketProcessor.HandleUserStatusChanged(x)) },
            {typeof(S_SPAWN_NPC), new Action<S_SPAWN_NPC>(x => PacketProcessor.HandleSpawnNpc(x)) },
            {typeof(S_DESPAWN_NPC), new Action<S_DESPAWN_NPC>(x => PacketProcessor.HandleDespawnNpc(x)) },
            {typeof(S_NPC_STATUS), new Action<S_NPC_STATUS>(x => PacketProcessor.HandleNpcStatusChanged(x)) },
            {typeof(S_ABNORMALITY_BEGIN), new Action<S_ABNORMALITY_BEGIN>(x => PacketProcessor.HandleAbnormalityBegin(x)) },
            {typeof(S_ABNORMALITY_REFRESH), new Action<S_ABNORMALITY_REFRESH>(x => PacketProcessor.HandleAbnormalityRefresh(x)) },
            {typeof(S_ABNORMALITY_END), new Action<S_ABNORMALITY_END>(x => PacketProcessor.HandleAbnormalityEnd(x)) },
            {typeof(S_GET_USER_LIST), new Action<S_GET_USER_LIST>(x => PacketProcessor.HandleCharList(x)) },
            {typeof(S_SPAWN_ME), new Action<S_SPAWN_ME>(x => PacketProcessor.HandleSpawnMe(x)) },
            {typeof(S_RETURN_TO_LOBBY), new Action<S_RETURN_TO_LOBBY>(x => PacketProcessor.HandleReturnToLobby(x)) },
            {typeof(S_BOSS_GAGE_INFO), new Action<S_BOSS_GAGE_INFO>(x => PacketProcessor.HandleGageReceived(x)) },
            {typeof(C_PLAYER_LOCATION), new Action<C_PLAYER_LOCATION>(x => PacketProcessor.HandlePlayerLocation(x)) },
            {typeof(S_USER_EFFECT), new Action<S_USER_EFFECT>(x => PacketProcessor.HandleUserEffect(x)) },
            {typeof(S_LOAD_TOPO), new Action<S_LOAD_TOPO>(x => PacketProcessor.HandleLoadTopo(x)) },
            {typeof(C_LOAD_TOPO_FIN), new Action<C_LOAD_TOPO_FIN>(x => PacketProcessor.HandleLoadTopoFin(x)) },
            {typeof(S_SPAWN_USER), new Action<S_SPAWN_USER>(x => PacketProcessor.HandleSpawnUser(x)) },
            {typeof(S_DESPAWN_USER), new Action<S_DESPAWN_USER>(x => PacketProcessor.HandleDespawnUser(x)) },
            {typeof(S_PARTY_MEMBER_LIST), new Action<S_PARTY_MEMBER_LIST>(x => PacketProcessor.HandlePartyMemberList(x)) },

            {typeof(S_LOGOUT_PARTY_MEMBER), new Action<S_LOGOUT_PARTY_MEMBER>(x => PacketProcessor.HandlePartyMemberLogout(x)) },
            {typeof(S_LEAVE_PARTY_MEMBER), new Action<S_LEAVE_PARTY_MEMBER>(x => PacketProcessor.HandlePartyMemberLeave(x)) },
            {typeof(S_LEAVE_PARTY), new Action<S_LEAVE_PARTY>(x => PacketProcessor.HandleLeaveParty(x)) },
            {typeof(S_BAN_PARTY_MEMBER), new Action<S_BAN_PARTY_MEMBER>(x => PacketProcessor.HandlePartyMemberKick(x)) },

            {typeof(S_PARTY_MEMBER_CHANGE_HP), new Action<S_PARTY_MEMBER_CHANGE_HP>(x => PacketProcessor.HandlePartyMemberHP(x)) },
            {typeof(S_PARTY_MEMBER_CHANGE_MP), new Action<S_PARTY_MEMBER_CHANGE_MP>(x => PacketProcessor.HandlePartyMemberMP(x)) },

            {typeof(S_PARTY_MEMBER_STAT_UPDATE), new Action<S_PARTY_MEMBER_STAT_UPDATE>(x => PacketProcessor.HandlePartyMemberStats(x)) },
            {typeof(S_CHECK_TO_READY_PARTY), new Action<S_CHECK_TO_READY_PARTY>(x => PacketProcessor.HandleReadyCheck(x)) },
            {typeof(S_CHECK_TO_READY_PARTY_FIN), new Action<S_CHECK_TO_READY_PARTY_FIN>(x => PacketProcessor.HandleReadyCheckFin(x)) },

            {typeof(S_ASK_BIDDING_RARE_ITEM), new Action<S_ASK_BIDDING_RARE_ITEM>(x => PacketProcessor.HandleStartRoll(x)) },
            {typeof(S_RESULT_ITEM_BIDDING), new Action<S_RESULT_ITEM_BIDDING>(x => PacketProcessor.HandleEndRoll(x)) },
            {typeof(S_RESULT_BIDDING_DICE_THROW), new Action<S_RESULT_BIDDING_DICE_THROW>(x => PacketProcessor.HandleRollResult(x)) },

            {typeof(S_PARTY_MEMBER_BUFF_UPDATE), new Action<S_PARTY_MEMBER_BUFF_UPDATE>(x => PacketProcessor.HandlePartyMemberBuffUpdate(x)) },
            {typeof(S_PARTY_MEMBER_ABNORMAL_ADD), new Action<S_PARTY_MEMBER_ABNORMAL_ADD>(x => PacketProcessor.HandlePartyMemberAbnormalAdd(x)) },
            {typeof(S_PARTY_MEMBER_ABNORMAL_REFRESH), new Action<S_PARTY_MEMBER_ABNORMAL_REFRESH>(x => PacketProcessor.HandlePartyMemberAbnormalRefresh(x)) },
            {typeof(S_PARTY_MEMBER_ABNORMAL_DEL), new Action<S_PARTY_MEMBER_ABNORMAL_DEL>(x => PacketProcessor.HandlePartyMemberAbnormalDel(x)) },
            {typeof(S_PARTY_MEMBER_ABNORMAL_CLEAR), new Action<S_PARTY_MEMBER_ABNORMAL_CLEAR>(x => PacketProcessor.HandlePartyMemberAbnormalClear(x)) },
            {typeof(S_CHANGE_PARTY_MANAGER), new Action<S_CHANGE_PARTY_MANAGER>(x => PacketProcessor.HandleChangeLeader(x)) },
            {typeof(S_WEAK_POINT), new Action<S_WEAK_POINT>(x => PacketProcessor.HandleRunemark(x)) },
            //{typeof(S_EACH_SKILL_RESULT), new Action<S_EACH_SKILL_RESULT>(x => PacketProcessor.HandleSkillResult(x)) },
            {typeof(S_CHAT), new Action<S_CHAT>(x => PacketProcessor.HandleChat(x)) },
            {typeof(C_SHOW_ITEM_TOOLTIP_EX), new Action<C_SHOW_ITEM_TOOLTIP_EX>(x => PacketProcessor.HandleShowItemTooltipEx(x)) },
            {typeof(C_REQUEST_NONDB_ITEM_INFO), new Action<C_REQUEST_NONDB_ITEM_INFO>(x => PacketProcessor.HandleRequestNondbItemInfo(x)) },
            {typeof(S_PRIVATE_CHAT), new Action<S_PRIVATE_CHAT>(x => PacketProcessor.HandlePrivateChat(x)) },
            {typeof(S_WHISPER), new Action<S_WHISPER>(x => PacketProcessor.HandleWhisper(x)) },
            {typeof(S_JOIN_PRIVATE_CHANNEL), new Action<S_JOIN_PRIVATE_CHANNEL>(x => PacketProcessor.HandleJoinPrivateChat(x)) },
            {typeof(S_LEAVE_PRIVATE_CHANNEL), new Action<S_LEAVE_PRIVATE_CHANNEL>(x => PacketProcessor.HandleLeavePrivateChat(x)) },
            {typeof(S_SYSTEM_MESSAGE), new Action<S_SYSTEM_MESSAGE>(x => PacketProcessor.HandleSystemMessage(x)) },
            {typeof(S_SYSTEM_MESSAGE_LOOT_ITEM), new Action<S_SYSTEM_MESSAGE_LOOT_ITEM>(x => PacketProcessor.HandleSystemMessageLoot(x)) },
            {typeof(S_CREST_MESSAGE), new Action<S_CREST_MESSAGE>(x => PacketProcessor.HandleCrestMessage(x)) },
            {typeof(S_ANSWER_INTERACTIVE), new Action<S_ANSWER_INTERACTIVE>(x => PacketProcessor.HandleAnswerInteractive(x)) },
            {typeof(S_USER_BLOCK_LIST), new Action<S_USER_BLOCK_LIST>(x => PacketProcessor.HandleBlockList(x)) },
            {typeof(S_FRIEND_LIST), new Action<S_FRIEND_LIST>(x => PacketProcessor.HandleFriendList(x)) },
            {typeof(S_ACCOMPLISH_ACHIEVEMENT), new Action<S_ACCOMPLISH_ACHIEVEMENT>(x => PacketProcessor.HandleAccomplishAchievement(x)) },
            {typeof(S_TRADE_BROKER_DEAL_SUGGESTED), new Action<S_TRADE_BROKER_DEAL_SUGGESTED>(x => PacketProcessor.HandleBrokerOffer(x)) },
            {typeof(S_UPDATE_FRIEND_INFO), new Action<S_UPDATE_FRIEND_INFO>(x => PacketProcessor.HandleFriendStatus(x)) },
            {typeof(S_PARTY_MATCH_LINK), new Action<S_PARTY_MATCH_LINK>(x => PacketProcessor.HandleLfgSpam(x)) },
            {typeof(S_PARTY_MEMBER_INFO), new Action<S_PARTY_MEMBER_INFO>(x => PacketProcessor.HandlePartyMemberInfo(x)) },
            {typeof(S_OTHER_USER_APPLY_PARTY), new Action<S_OTHER_USER_APPLY_PARTY>(x => PacketProcessor.HandleUserApplyToParty(x)) },
            {typeof(S_DUNGEON_EVENT_MESSAGE), new Action<S_DUNGEON_EVENT_MESSAGE>(x => PacketProcessor.HandleDungeonMessage(x)) },

        };

        public static void Init()
        {
            OpcodeNameToType.Clear();
            TeraMessages.ToList().ForEach(x => OpcodeNameToType[PacketProcessor.OpCodeNamer.GetCode(x.Key)] = x.Value);
            MessageToProcessing.ToList().ForEach(x => MainProcessor[x.Key] = x.Value);
        }
        private static Tera.Game.Messages.ParsedMessage Instantiate(ushort opCode, TeraMessageReader reader)
        {
            Delegate type;
            if (!OpcodeNameToType.TryGetValue(opCode, out type))
                type = UnknownMessageDelegate;
            return (Tera.Game.Messages.ParsedMessage)type.DynamicInvoke(reader);
        }
        public static Tera.Game.Messages.ParsedMessage Create(Message message)
        {
            var reader = new TeraMessageReader(message, PacketProcessor.OpCodeNamer, PacketProcessor.Version, PacketProcessor.SystemMessageNamer);
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

        public static bool Process(Tera.Game.Messages.ParsedMessage message)
        {
            Delegate type;
            MainProcessor.TryGetValue(message.GetType(), out type);
            if (type == null) return false;
            type.DynamicInvoke(message);
            return true;
        }


    }
}
