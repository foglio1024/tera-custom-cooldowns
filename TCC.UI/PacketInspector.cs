using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.Parsing;
using Tera;

namespace TCC
{
    public static class PacketInspector 
    {


        public static void InspectPacket(Message msg)
        {
            List<string> exclusionList = new List<string>()
            {
                "C_", "LOCATION", "ABNORMALITY", "S_CHAT",
                "S_INVEN", "SPAWN", "S_EACH_SKILL_RESULT",
                "ACTION", "S_DIALOG_EVENT", "S_PARTY_MATCH_LINK",
                "S_SOCIAL", "S_QUEST_BALLOON", "S_BATTLE_FIELD_SEASON_RANKER",
                "S_VISIT_NEW_SECTION", "S_QUEST_VILLAGER_INFO", "S_IMAGE_DATA",
                "S_ITEM_CUSTOM_STRING","S_USER_STATUS", "S_PLAYER_CHANGE_MP",
                "S_MOUNT_VEHICLE", "S_SYSTEM_MESSAGE", "S_HIDE_HP",
                "S_CHANGE_RELATION", "S_USER_EXTERNAL_CHANGE",
                "S_PLAYER_CHANGE_STAMINA", "S_SKILL_CATEGORY", "S_GUILD_NAME",
                "S_UPDATE_EVENT_SEED_STATE", "S_START_COOLTIME_SKILL",
                "S_ENABLE_DISABLE_SELLABLE_ITEM_LIST", "S_REIGN_INFO",
                "S_INSTANT_DASH", "S_CREATURE_CHANGE_HP", "S_UNMOUNT_VEHICLE",
                "S_PING", "S_INSTANCE_ARROW", "S_CURRENT_CHANNEL", "S_PARTY_INFO",
                "S_VIEW_PARTY_INVITE", "S_READY_PULL", "S_INSTANT_MOVE", "S_DUEL_TAG",
                "S_RESULT_EVENT_SEED", "S_LOCKON_ARROW", "S_SKILL_LIST", "S_SHORTCUT_CHANGE",
                "S_PLAYER_STAT_UPDATE", "S_PLAYER_CHANGE_ALL_PROF", "S_F2P_PremiumUser_Permission",
                "S_UPDATE_ACHIEVEMENT_PROGRESS", "S_PLAYER_CHANGE_FLIGHT_ENERGY", "S_UPDATE_FRIEND_INFO",
                "S_CANT_FLY_ANYMORE", "S_UPDATE_GUILD_MEMBER", "S_GUILD_MEMBER_LIST",
                "S_START_CLIENT_CUSTOM_SKILL", "S_PCBANGINVENTORY_DATALIST", "S_CREATURE_ROTATE",
                "S_SHOW_PARTY_MATCH_INFO", "S_MY_PARTY_MATCH_INFO", "S_PARTY_MEMBER_INFO",
                "S_NOTIFY_TO_FRIENDS_WALK_INTO_SAME_AREA", "S_REPLY_REQUEST_CONTRACT",
                "S_REQUEST_CONTRACT", "S_CANCEL_CONTRACT", "S_PARTY_MEMBER_QUEST_DATA",
                "S_PARTY_MEMBER_INTERVAL_POS_UPDATE"

            };
            var opName = PacketRouter.OpCodeNamer.GetName(msg.OpCode);
            //if (exclusionList.Any(opName.Contains)) return;

            Console.WriteLine(opName);
        }
    }
}
