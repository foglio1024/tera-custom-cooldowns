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
            //{"S_PARTY_MEMBER_INFO", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_INFO>>() },
            {"S_LOGOUT_PARTY_MEMBER", Contructor<Func<TeraMessageReader, S_LOGOUT_PARTY_MEMBER>>() },
            { "S_LEAVE_PARTY_MEMBER", Contructor<Func<TeraMessageReader, S_LEAVE_PARTY_MEMBER>>() },
            {"S_LEAVE_PARTY", Contructor<Func<TeraMessageReader, S_LEAVE_PARTY>>() },
            {"S_BAN_PARTY_MEMBER", Contructor<Func<TeraMessageReader, S_BAN_PARTY_MEMBER>>() },
            {"S_PARTY_MEMBER_CHANGE_HP", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_CHANGE_HP>>() },
            {"S_PARTY_MEMBER_CHANGE_MP", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_CHANGE_MP>>() },
            {"S_PARTY_MEMBER_STAT_UPDATE", Contructor<Func<TeraMessageReader, S_PARTY_MEMBER_STAT_UPDATE>>() },
        };

        private static Dictionary<Type, Delegate> MainProcessor = new Dictionary<Type, Delegate>();
        private static readonly Dictionary<Type, Delegate> MessageToProcessing = new Dictionary<Type, Delegate>
        {
            {typeof(S_LOGIN), new Action<S_LOGIN>(x => PacketProcessor.HandleCharLogin(x)) },
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
            //{typeof(S_PARTY_MEMBER_INFO), new Action<S_PARTY_MEMBER_INFO>(x => PacketProcessor.HandlePartyMemberInfo(x)) },

            {typeof(S_LOGOUT_PARTY_MEMBER), new Action<S_LOGOUT_PARTY_MEMBER>(x => PacketProcessor.HandlePartyMemberLogout(x)) },
            {typeof(S_LEAVE_PARTY_MEMBER), new Action<S_LEAVE_PARTY_MEMBER>(x => PacketProcessor.HandlePartyMemberLeave(x)) },
            {typeof(S_LEAVE_PARTY), new Action<S_LEAVE_PARTY>(x => PacketProcessor.HandleLeaveParty(x)) },
            {typeof(S_BAN_PARTY_MEMBER), new Action<S_BAN_PARTY_MEMBER>(x => PacketProcessor.HandlePartyMemberKick(x)) },

            {typeof(S_PARTY_MEMBER_CHANGE_HP), new Action<S_PARTY_MEMBER_CHANGE_HP>(x => PacketProcessor.HandlePartyMemberHP(x)) },
            {typeof(S_PARTY_MEMBER_CHANGE_MP), new Action<S_PARTY_MEMBER_CHANGE_MP>(x => PacketProcessor.HandlePartyMemberMP(x)) },

            {typeof(S_PARTY_MEMBER_STAT_UPDATE), new Action<S_PARTY_MEMBER_STAT_UPDATE>(x => PacketProcessor.HandlePartyMemberStats(x)) },

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
