using System;
using System.Collections.Generic;
using Nostrum.Extensions;
using TeraDataLite;

namespace TeraPacketParser.Messages;

public class S_GUILD_MEMBER_LIST : ParsedMessage
{
    public List<GuildMemberData> Members { get; } = new();
    public uint MasterId { get; }
    public string MasterName { get; }

    public S_GUILD_MEMBER_LIST(TeraMessageReader reader) : base(reader)
    {
        var membersCount = reader.ReadInt16();
        var membersOffset = reader.ReadInt16();
        reader.Skip(2); //gname offset
        var masterNameOffset = reader.ReadInt16();
        MasterId = reader.ReadUInt32();
        reader.RepositionAt(masterNameOffset);
        MasterName = reader.ReadTeraString();
        //GuildMembersList = new Dictionary<uint, string>();
        if (membersCount == 0) return;
        try
        {
            reader.RepositionAt(membersOffset);

            for (var i = 0; i < membersCount; i++)
            {
                var m = new GuildMemberData();
                reader.Skip(2); //curr
                var next = reader.ReadUInt16();
                var nameOffset = reader.ReadInt16();

                reader.Skip(2); //noteOffset
                m.PlayerId = reader.ReadUInt32();

                reader.RepositionAt(nameOffset);
                m.Name = reader.ReadTeraString();

                Members.Add(m);
                //GuildMembersList[playerId] = name;

                if (next == 0) return;
                reader.RepositionAt(next);
            }

        }
        catch (Exception e)
        {
            Console.WriteLine($"[{nameof(S_GUILD_MEMBER_LIST)}] Failed to parse packet. \nContent:\n{Payload.Array?.ToHexString()}\nException:\n{e.Message}\n{e.StackTrace}");
            //WindowManager.ViewModels.NotificationArea.Enqueue("Warning", "A non-fatal error occured. More detailed info has been written to error.log. Please report this to the developer on Discord or Github.", NotificationType.Warning, 10000);
        }
    }

}