﻿using System.Collections.Generic;
using TeraDataLite;

namespace TeraPacketParser.Messages;

public class S_SHOW_CANDIDATE_LIST : ParsedMessage
{
    public List<GroupMemberData> Candidates { get; set; }

    public S_SHOW_CANDIDATE_LIST(TeraMessageReader reader) : base(reader)
    {
        var count = reader.ReadUInt16();
        var offset = reader.ReadUInt16();

        Candidates = new List<GroupMemberData>();
        if (count == 0) return;
        reader.BaseStream.Position = offset - 4;
        for (var i = 0; i < count; i++)
        {
            reader.Skip(2); //var current = reader.ReadUInt16();
            var next = reader.ReadUInt16();

            var nameOffset = reader.ReadUInt16();

            var playerId = reader.ReadUInt32();
            var cls = (Class)reader.ReadUInt16();
            reader.Skip(2 + 2);
            var level = reader.ReadUInt16();
            reader.Skip(4); // var worldId = reader.ReadUInt32();
            var guardId = reader.ReadUInt32();
            var sectionId = reader.ReadUInt32();

            reader.BaseStream.Position = nameOffset - 4;

            var name = reader.ReadTeraString();

            Candidates.Add(new GroupMemberData//User(WindowManager.LfgListWindow.Dispatcher)
            {
                PlayerId = playerId,
                Class = cls,
                Level = level,
                GuardId = guardId,
                SectionId = sectionId,
                //Location = SessionManager.DB.GetSectionName(guardId, sectionId),
                Online = true,
                Name = name
            });
            if (next != 0) reader.BaseStream.Position = next - 4;
        }
    }
}