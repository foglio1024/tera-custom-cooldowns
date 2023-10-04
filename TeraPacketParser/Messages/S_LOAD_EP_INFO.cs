using System.Collections.Generic;

namespace TeraPacketParser.Messages;

public class S_LOAD_EP_INFO : ParsedMessage
{
    public Dictionary<uint, uint> Perks { get; }

    public S_LOAD_EP_INFO(TeraMessageReader r) : base(r)
    {
        Perks = new Dictionary<uint, uint>();

        var perksCount = r.ReadInt16();
        if (perksCount == 0) return;

        var perksOffset = r.ReadInt16();
        r.RepositionAt(perksOffset);

        for (var i = 0; i < perksCount; i++)
        {
            r.Skip(2);    //var curr = r.ReadUInt16();
            var next = r.ReadUInt16();
            var perkId = r.ReadUInt32();
            var perkLevel = r.ReadUInt32();
            Perks[perkId] = perkLevel;
            if (next == 0) break;
            r.RepositionAt(next);
        }
    }
}