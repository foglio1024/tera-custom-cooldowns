﻿using System.Collections.Generic;

namespace TeraPacketParser.Messages;

public class S_NPCGUILD_LIST : ParsedMessage
{
    public Dictionary<int, int> NpcGuildList { get; }
    public ulong UserId { get; }
    public S_NPCGUILD_LIST(TeraMessageReader reader) : base(reader)
    {
        NpcGuildList = new Dictionary<int, int>();
        try
        {
            var count = reader.ReadUInt16();
            if (count == 0) return;
            var offset = reader.ReadUInt16();
            UserId = reader.ReadUInt64();
            reader.BaseStream.Position = offset - 4;
            for (var i = 0; i < count; i++)
            {
                reader.Skip(2); //var curr = reader.ReadUInt16();
                var next = reader.ReadUInt16();
                reader.Skip(4); //var region = reader.ReadInt32();
                var faction = reader.ReadInt32();
                reader.Skip(4); //var rank = reader.ReadInt32();
                reader.Skip(4); //var reputation = reader.ReadInt32();
                var credits = reader.ReadInt32();
                NpcGuildList[faction] = credits;
                if (next == 0) return;
                reader.BaseStream.Position = next - 4;
            }
        }
        catch
        {
            //Log.F($"[{nameof(S_NPCGUILD_LIST)}] Failed to parse packet. \nContent:\n{StringUtils.ByteArrayToString(Payload.Array)}\nException:\n{e.Message}\n{e.StackTrace}");
            //WindowManager.ViewModels.NotificationArea.Enqueue("Warning", "A non-fatal error occured. More detailed info has been written to error.log. Please report this to the developer on Discord or Github.", NotificationType.Warning, 10000);
        }
    }
}