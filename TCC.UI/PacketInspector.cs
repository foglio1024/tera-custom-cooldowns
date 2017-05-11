using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Parsing;
using Tera;
using Tera.Game;

namespace TCC
{
    public static class PacketInspector 
    {


        public static void InspectPacket(Message msg)
        {
            List<string> exclusionList = new List<string>()
            {
                "S_USER_LOCATION",
                "S_SOCIAL",
                "PROJECTILE",
                "S_PARTY_MATCH_LINK",
                "C_PLAYER_LOCATION",
                "S_NPC_LOCATION",
                "S_CHAT",
                "S_PLAYER_CHANGE_STAMINA",
                "S_SPAWN_NPC" ,
                "S_START_COOLTIME_SKILL",
                "S_ACTION_END" ,
                "S_ACTION_STAGE",
                "S_PLAYER_CHANGE_MP",
                "S_DESPAWN_NPC",
                "S_CREATURE_CHANGE_HP",
                "S_EACH_SKILL_RESULT",
                "S_SPAWN_COLLECTION",
                "S_F2P_PremiumUser_Permission",
                "S_ABNORMALITY_BEGIN",
                "C_REQUEST_INGAMESTORE_PRODUCT_LIST",
                "S_BATTLE_FIELD_SEASON_RANKER",
                "S_QUEST_VILLAGER_INFO",
                "S_DUNGEON_UI_HIGHLIGHT",
                "S_SKILL_PERIOD",
                "S_PARTY_MEMBER_INTERVAL_POS_UPDATE",
                "S_DESPAWN_USER",
                "C_UPDATE_CONTENTS_PLAYTIME",
                "GET_USER_GUILD_LOGO",
                "S_START_COOLTIME_ITEM",
                "S_INVEN",
                "S_ABNORMALITY_END",
                "S_SEND_USER_PLAY_TIME",
                "S_CLEAR_WORLD_QUEST_VILLAGER_INFO",
                "S_PCBANGINVENTORY_DATALIST",
                "C_TRADE_BROKER_HIGHEST_ITEM_LEVEL",
                "SERVER_TIME",
                "S_GUILD_MEMBER_LIST",
                "S_GUILD_INFO",
                "S_GUILD_APPLY_COUNT",
                "S_SYSTEM_MESSAGE",
                "S_GUILD_QUEST_LIST",
                "S_PARTY_MEMBER_QUEST_DATA",
                "S_CURRENT_CHANNEL",
                "S_SPAWN_USER",
                "S_LOAD_ACHIEVEMENT_LIST",
                "S_TOTAL_GUILD_WAR_DATA",
                "INGAMESHOP",
                "S_QUEST_BALLOON",
                "S_UPDATE_GUILD_MEMBER",
                "S_ITEM_CUSTOM_STRING",
                "S_USER_STATUS",
                "S_DIALOG_EVENT",
                "S_UPDATE_EVENT_SEED_STATE",
                "S_RESULT_EVENT_SEED",
                "S_SPAWN_DROPITEM",
                "ARROW",
                "S_HIDE_HP",
                "S_IMAGE_DATA",
                "S_GUILD_NAME",
                "S_USER_FLYING_LOCATION",
                "VEHICLE",
                "C_REQUEST_IMAGE_DATA",
                "S_NPC_STATUS",
                "S_CREATURE_ROTATE",
                "S_NPC_AI_EVENT",
                "S_SPAWN_EVENT_SEED",
                "WHISPER",
                "S_DESPAWN_EVENT_SEED",
                "S_INSTANT_MOVE",
                "S_USER_EXTERNAL_CHANGE",
                "S_ABNORMALITY_REFRESH",
                "S_CHANGE_RELATION",
                "S_DESPAWN_DROPITEM",
                "S_UPDATE_ACHIEVEMENT_PROGRESS",
                "S_WORLD_QUEST_VILLAGER_INFO",
                "S_INSTANT_DASH"





            };
            var opName = PacketProcessor.OpCodeNamer.GetName(msg.OpCode);
            TeraMessageReader tmr = new TeraMessageReader(msg, PacketProcessor.OpCodeNamer, PacketProcessor.Version, PacketProcessor.SystemMessageNamer);
            if (exclusionList.Any(opName.Contains)) return;
//            if(opName.Equals("S_LOAD_TOPO") || opName.Equals("C_LOAD_TOPO_FIN")|| opName.Equals("S_SPAWN_ME"))
                Console.WriteLine(opName);
        }
    }
}
