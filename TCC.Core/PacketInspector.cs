using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TCC.Parsing;
using Tera;
using Tera.Game;

namespace TCC
{
    public static class PacketInspector
    {
        private static bool running;
        private static Dictionary<string, MessageStats> Stats;
        public static void NewStat()
        {
            if (running) return;
            Stats = new Dictionary<string, MessageStats>();
            running = true;
        }
        public static void Stop()
        {
            running = false;
            Dump();
        }
        public static void Dump()
        {
            var lines = new List<string>();
            foreach (var item in Stats)
            {
                var sb = new StringBuilder();
                sb.Append(item.Key);
                sb.Append("\t");
                sb.Append(item.Value.Count);
                sb.Append("\t");
                sb.Append(item.Value.TotalSize);

                lines.Add(sb.ToString());
            }
            var now = DateTime.Now.ToString();
            now = now.Replace("/", "-");
            now = now.Replace(":", "-");
            var path = "packets-log_" + now + ".log";
            File.WriteAllLines(path, lines);
        }
        public static void InspectPacket(Message msg)
        {
            var exclusionList = new List<string>()
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
            var tmr = new TeraMessageReader(msg, PacketProcessor.OpCodeNamer, PacketProcessor.Factory, PacketProcessor.SystemMessageNamer);
            if (exclusionList.Any(opName.Contains)) return;
            //            if(opName.Equals("S_LOAD_TOPO") || opName.Equals("C_LOAD_TOPO_FIN")|| opName.Equals("S_SPAWN_ME"))
            Debug.WriteLine(opName + " " + msg.OpCode);
        }
        public static void AddToStats(Message msg)
        {
            if (!running) return;
            var opName = PacketProcessor.OpCodeNamer.GetName(msg.OpCode);

            if (Stats.ContainsKey(opName))
            {
                Stats[opName].TotalSize += msg.Data.Count;
                Stats[opName].Count++;
            }
            else
            {
                Stats.Add(opName, new MessageStats(opName, msg.Data.Count));
            }
        }

        //public static void Analyze(Message msg)
        //{
        //    return;
        //    if (msg.Direction == MessageDirection.ClientToServer) return;
        //    var r = new TeraMessageReader(msg, PacketProcessor.OpCodeNamer, PacketProcessor.Version, PacketProcessor.SystemMessageNamer);
        //    Debug.WriteLine("OpCode: " + msg.OpCode + " [" + msg.Data.Count + "]");
        //    Debug.WriteLine(r.ReadUInt32());
        //    Debug.WriteLine(r.ReadUInt32());
        //    Debug.WriteLine(r.ReadUInt32());
        //    Debug.WriteLine(r.ReadUInt32());

        //    //try
        //    //{
        //    //    Debug.WriteLine("id: " + r.ReadUInt64());
        //    //    Debug.WriteLine("unk1: " + r.ReadInt32());
        //    //    Debug.WriteLine("skill: " + r.ReadUInt32());
        //    //    Debug.WriteLine("x1: " + r.ReadSingle());
        //    //    Debug.WriteLine("y1: " + r.ReadSingle());
        //    //    Debug.WriteLine("z1: " + r.ReadSingle());
        //    //    Debug.WriteLine("x2: " + r.ReadSingle());
        //    //    Debug.WriteLine("y2: " + r.ReadSingle());
        //    //    Debug.WriteLine("z2: " + r.ReadSingle());
        //    //    Debug.WriteLine("unk2: " + r.ReadByte());
        //    //    Debug.WriteLine("spd: " + r.ReadSingle());
        //    //    Debug.WriteLine("src: " + r.ReadUInt64());
        //    //    Debug.WriteLine("model: " + r.ReadUInt32());
        //    //    Debug.WriteLine("unk4: " + r.ReadUInt32());
        //    //    Debug.WriteLine("unk5: " + r.ReadUInt32());
        //    //    Debug.WriteLine("");
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    //ignore
        //    //}
        //}
        private static Dictionary<string, int> FixedSizePackets = new Dictionary<string, int>
        {
            { "S_RETURN_TO_LOBBY", 4 },
            { "S_USER_EFFECT",4+8+8+4+4 },
            { "S_PARTY_MEMBER_ABNORMAL_ADD", 4+4+4+4+4+4+4 },
            { "S_PARTY_MEMBER_ABNORMAL_CLEAR", 4+4+4 },
            { "S_PARTY_MEMBER_ABNORMAL_DEL",4+4+4+4 },
            { "S_PARTY_MEMBER_ABNORMAL_REFRESH",4+4+4+4+4+4+4},
            { "S_PARTY_MEMBER_CHANGE_HP",4+4+4+4+4 },
            { "S_PARTY_MEMBER_CHANGE_MP",4+4+4+4+4 },
            { "S_PARTY_MEMBER_STAT_UPDATE",4+4+4+4+4+4+4+2+2+2+1+4+4+4+4 },
            { "S_DUNGEON_EVENT_MESSAGE",4+2+4+1+4 },
        };
    }

    public class MessageStats
    {
        public string Name;
        public int TotalSize;
        public int Count;
        public MessageStats(string n, int t)
        {
            Name = n;
            TotalSize = t;
            Count = 1;
        }
    }
}
