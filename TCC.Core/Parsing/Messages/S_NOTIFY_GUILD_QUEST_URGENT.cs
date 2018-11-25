using System;
using System.Linq;
using TCC.Data;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;
using TCC.Windows.Widgets;

namespace TCC.Parsing.Messages
{
    public class S_NOTIFY_GUILD_QUEST_URGENT : ParsedMessage
    {
        public uint ZoneId { get; }
        public uint TemplateId { get; }
        public string Quest { get; }

        public uint QuestId
        {
            get
            {
                var param = Quest.Split('@');
                var gqStr = param.FirstOrDefault(x => x.StartsWith("GuildQuest"));
                if (gqStr == null)
                {
                    Log.F($"[{nameof(S_NOTIFY_GUILD_QUEST_URGENT)}] Failed to parse guild quest id. \nContent:\n{StringUtils.ByteArrayToString(Payload.Array)}\nQuest string:\n{Quest}");
                    WindowManager.FloatingButton.NotifyExtended("Warning", "A non-fatal error occured. More detailed info has been written to error.log. Please report this to the developer on Discord or Github.", NotificationType.Warning, 10000);
                    return 0;
                }
                var keyVal = gqStr.Split(':');
                if (keyVal.Length < 2)
                {
                    Log.F($"[{nameof(S_NOTIFY_GUILD_QUEST_URGENT)}] Failed to parse guild quest id. \nContent:\n{StringUtils.ByteArrayToString(Payload.Array)}\nQuest string:\n{Quest}");
                    WindowManager.FloatingButton.NotifyExtended("Warning", "A non-fatal error occured. More detailed info has been written to error.log. Please report this to the developer on Discord or Github.", NotificationType.Warning, 10000);
                    return 0;
                }
                var strId = keyVal[1];
                try
                {
                    return Convert.ToUInt32(strId);
                }
                catch
                {
                    Log.F($"[{nameof(S_NOTIFY_GUILD_QUEST_URGENT)}] Failed to parse guild quest id. \nContent:\n{StringUtils.ByteArrayToString(Payload.Array)}\nQuest string:\n{Quest}");
                    WindowManager.FloatingButton.NotifyExtended("Warning", "A non-fatal error occured. More detailed info has been written to error.log. Please report this to the developer on Discord or Github.", NotificationType.Warning, 10000);
                    return 0;
                }
            }
        }
        public S_NOTIFY_GUILD_QUEST_URGENT(TeraMessageReader reader) : base(reader)
        {
            reader.BaseStream.Position = 0;
            var questOffset = reader.ReadInt16();
            reader.Skip(4);
            ZoneId = reader.ReadUInt32();
            TemplateId = reader.ReadUInt32();
            reader.BaseStream.Position = questOffset - 4;
            Quest = reader.ReadTeraString();
        }
    }
}
