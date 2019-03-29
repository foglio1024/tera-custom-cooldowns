using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_PLAYER_CHANGE_EXP : ParsedMessage
    {
        public ulong NextLevelExp { get; set; }
        public ulong KilledMobEntityId { get; set; }
        public uint RestedExp { get; set; }
        public uint GainedRestedExp { get; set; }
        public ulong LevelExp { get; set; }
        public ulong TotalExp { get; set; }
        public ulong GainedTotalExp { get; set; }

        public S_PLAYER_CHANGE_EXP(TeraMessageReader reader) : base(reader)
        {
            GainedTotalExp = reader.ReadUInt64();
            TotalExp = reader.ReadUInt64();
            LevelExp = reader.ReadUInt64();
            NextLevelExp = reader.ReadUInt64();
            KilledMobEntityId = reader.ReadUInt64();
            reader.Skip(4);
            GainedRestedExp = reader.ReadUInt32();
            RestedExp = reader.ReadUInt32();
            // float, u32, u32 (unks)
            Log.CW($"{nameof(NextLevelExp)} {NextLevelExp}");
            Log.CW($"{nameof(RestedExp)} {RestedExp}");
            Log.CW($"{nameof(GainedRestedExp)} {GainedRestedExp}");
            Log.CW($"{nameof(LevelExp)} {LevelExp}");
            Log.CW($"{nameof(TotalExp)} {TotalExp}");
            Log.CW($"{nameof(GainedTotalExp)} {GainedTotalExp}");
        }

    }
}
