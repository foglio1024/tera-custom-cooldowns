using System;
using System.Linq;


namespace TeraPacketParser.Messages
{
    public class S_NOTIFY_GUILD_QUEST_URGENT : ParsedMessage
    {
        public uint ZoneId { get; }
        public uint TemplateId { get; }
        public string Quest { get; }
        public GuildBamQuestType Type { get; }
        public enum GuildBamQuestType
        {
            Announce = 0,
            Spawn = 1,
            Death = 3
        }
        public uint QuestId
        {
            get
            {
                switch (Type)
                {
                    case GuildBamQuestType.Announce:
                        try
                        {
                            var param = Quest.Split('@');
                            var gqStr = param.FirstOrDefault(x => x.StartsWith("GuildQuest"));
                            if (gqStr != null)
                            {
                                var keyVal = gqStr.Split(':');
                                var strId = keyVal[1];
                                return Convert.ToUInt32(strId);
                            }
                        }
                        catch
                        {
                            //Log.F($"[{nameof(S_NOTIFY_GUILD_QUEST_URGENT)}] Failed to parse guild quest id. \nContent:\n{Payload.Array.ToStringEx()}\nQuest string:\n{Quest}");
                            //Log.N("Warning", "A non-fatal error occured. More detailed info have been written to error.log. Please report this to the developer on Discord or Github.", NotificationType.Warning, 10000);
                        }
                        break;
                }
                return 0;
                //var param = Quest.Split('@');
                //var gqStr = param.FirstOrDefault(x => x.StartsWith("GuildQuest"));
                //if (gqStr == null)
                //{
                //    Log.F($"[{nameof(S_NOTIFY_GUILD_QUEST_URGENT)}] Failed to parse guild quest id. \nContent:\n{StringUtils.ByteArrayToString(Payload.Array)}\nQuest string:\n{Quest}");
                //    WindowManager.ViewModels.NotificationArea.Enqueue("Warning", "A non-fatal error occured. More detailed info has been written to error.log. Please report this to the developer on Discord or Github.", NotificationType.Warning, 10000);
                //    return 0;
                //}
                //var keyVal = gqStr.Split(':');
                //if (keyVal.Length < 2)
                //{
                //    Log.F($"[{nameof(S_NOTIFY_GUILD_QUEST_URGENT)}] Failed to parse guild quest id. \nContent:\n{StringUtils.ByteArrayToString(Payload.Array)}\nQuest string:\n{Quest}");
                //    WindowManager.ViewModels.NotificationArea.Enqueue("Warning", "A non-fatal error occured. More detailed info has been written to error.log. Please report this to the developer on Discord or Github.", NotificationType.Warning, 10000);
                //    return 0;
                //}
                //var strId = keyVal[1];
                //try
                //{
                //    return Convert.ToUInt32(strId);
                //}
                //catch
                //{
                //    Log.F($"[{nameof(S_NOTIFY_GUILD_QUEST_URGENT)}] Failed to parse guild quest id. \nContent:\n{StringUtils.ByteArrayToString(Payload.Array)}\nQuest string:\n{Quest}");
                //    WindowManager.ViewModels.NotificationArea.Enqueue("Warning", "A non-fatal error occured. More detailed info has been written to error.log. Please report this to the developer on Discord or Github.", NotificationType.Warning, 10000);
                //    return 0;
                //}
            }
        }
        public S_NOTIFY_GUILD_QUEST_URGENT(TeraMessageReader reader) : base(reader)
        {
            /*
             * offset quest
             *
             * short type # 0 = announce, 1 = spawn, 3 = death
             * int templateId
             * int zoneId
             * string quest # "@GuildQuest:questId" for StrSheet_GuildQuest lookup (if type == announce), "\0" for spawn and death types
             */

            //announce
            //1200      # questOffset
            //00000000  # type
            //9800      # zone
            //00005a1b  # template
            //0000      # quest
            //...
            //0000

            //spawn
            //1200     # questOffset
            //01000000  # type
            //9800      # zone
            //00005a1b  # template
            //00000000 # quest
            //0000

            //death
            //1200     # questOffset
            //03000000  # type
            //9800      # zone
            //00005a1b  # template
            //00000000 # quest
            //0000

            try
            {
                var questOffset = reader.ReadInt16();
                var t = reader.ReadInt32();
                Type = (GuildBamQuestType)t;
                ZoneId = reader.ReadUInt32();
                TemplateId = reader.ReadUInt32();
                reader.BaseStream.Position = questOffset - 4;
                Quest = reader.ReadTeraString();
            }
            catch
            {
                //Log.F($"[{nameof(S_NOTIFY_GUILD_QUEST_URGENT)}] Error while reading packet: {e}\nData: {Payload.Array.ToStringEx()}");
                //Log.N("Warning", "A non-fatal error occured while parsing guild bam notification. More detailed info have been written to error.log. Please report this to the developer on Discord or Github.", NotificationType.Warning, 10000);

            }
        }
    }
}
