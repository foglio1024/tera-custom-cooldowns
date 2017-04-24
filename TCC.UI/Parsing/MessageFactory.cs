using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tera;
using Tera.Game;
using TCC.Messages;
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
            { "C_PLAYER_LOCATION" , Contructor<Func<TeraMessageReader,C_PLAYER_LOCATION>>() }
        };

        private static Dictionary<Type, Delegate> MainProcessor = new Dictionary<Type, Delegate>();
        private static readonly Dictionary<Type, Delegate> MessageToProcessing = new Dictionary<Type, Delegate>
        {
            { typeof(S_LOGIN), new Action<S_LOGIN>(x => PacketRouter.HandleCharLogin(x)) },
            { typeof(S_START_COOLTIME_SKILL), new Action<S_START_COOLTIME_SKILL>(x => PacketRouter.HandleNewSkillCooldown(x)) },
            { typeof(S_DECREASE_COOLTIME_SKILL), new Action<S_DECREASE_COOLTIME_SKILL>(x => PacketRouter.HandleDecreaseSkillCooldown(x)) },
            { typeof(S_START_COOLTIME_ITEM), new Action<S_START_COOLTIME_ITEM>(x => PacketRouter.HandleNewItemCooldown(x)) },
            { typeof(S_PLAYER_CHANGE_MP), new Action<S_PLAYER_CHANGE_MP>(x => PacketRouter.HandlePlayerChangeMP(x)) },
            { typeof(S_CREATURE_CHANGE_HP), new Action<S_CREATURE_CHANGE_HP>(x => PacketRouter.HandleCreatureChangeHP(x)) },
            { typeof(S_PLAYER_CHANGE_STAMINA), new Action<S_PLAYER_CHANGE_STAMINA>(x => PacketRouter.HandlePlayerChangeStamina(x)) },
            { typeof(S_PLAYER_CHANGE_FLIGHT_ENERGY), new Action<S_PLAYER_CHANGE_FLIGHT_ENERGY>(x => PacketRouter.HandlePlayerChangeFlightEnergy(x)) },
            { typeof(S_PLAYER_STAT_UPDATE), new Action<S_PLAYER_STAT_UPDATE>(x => PacketRouter.HandlePlayerStatUpdate(x)) },
            { typeof(S_USER_STATUS), new Action<S_USER_STATUS>(x => PacketRouter.HandleUserStatusChanged(x)) },
            { typeof(S_SPAWN_NPC), new Action<S_SPAWN_NPC>(x => PacketRouter.HandleNpcSpawn(x)) },
            { typeof(S_DESPAWN_NPC), new Action<S_DESPAWN_NPC>(x => PacketRouter.HandleNpcDespawn(x)) },
            { typeof(S_NPC_STATUS), new Action<S_NPC_STATUS>(x => PacketRouter.HandleNpcStatusChanged(x)) },
            { typeof(S_ABNORMALITY_BEGIN), new Action<S_ABNORMALITY_BEGIN>(x => PacketRouter.HandleAbnormalityBegin(x)) },
            { typeof(S_ABNORMALITY_REFRESH), new Action<S_ABNORMALITY_REFRESH>(x => PacketRouter.HandleAbnormalityRefresh(x)) },
            { typeof(S_ABNORMALITY_END), new Action<S_ABNORMALITY_END>(x => PacketRouter.HandleAbnormalityEnd(x)) },
            { typeof(S_GET_USER_LIST), new Action<S_GET_USER_LIST>(x => PacketRouter.HandleCharList(x)) },
            { typeof(S_SPAWN_ME), new Action<S_SPAWN_ME>(x => PacketRouter.HandleSpawn(x)) },
            { typeof(S_RETURN_TO_LOBBY), new Action<S_RETURN_TO_LOBBY>(x => PacketRouter.HandleReturnToLobby(x)) },
            { typeof(S_BOSS_GAGE_INFO), new Action<S_BOSS_GAGE_INFO>(x => PacketRouter.HandleGageReceived(x)) },
            {typeof(C_PLAYER_LOCATION), new Action<C_PLAYER_LOCATION>(x => PacketRouter.HandlePlayerLocation(x)) }

        };

        public static void Init()
        {
            OpcodeNameToType.Clear();
            TeraMessages.ToList().ForEach(x => OpcodeNameToType[PacketRouter.OpCodeNamer.GetCode(x.Key)] = x.Value);
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
            var reader = new TeraMessageReader(message, PacketRouter.OpCodeNamer, PacketRouter.Version, PacketRouter.SystemMessageNamer);
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
