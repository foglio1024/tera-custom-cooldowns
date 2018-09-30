using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_PLAYER_CHANGE_MP : ParsedMessage
    {
        private int currentMP, maxMP, diff;
        private uint type;
        private ulong target, source;

        public int CurrentMP => currentMP;
        public int MaxMP => maxMP;
        public int Diff => diff;
        public uint Type => type;
        public ulong Target => target;
        public ulong Source => source;

        public S_PLAYER_CHANGE_MP(TeraMessageReader reader) : base(reader)
        {
            currentMP = reader.ReadInt32();
            maxMP = reader.ReadInt32();
            diff = reader.ReadInt32();
            type = reader.ReadUInt32();
            target = reader.ReadUInt64();
            source = reader.ReadUInt64();
            //System.Console.WriteLine(this);

        }
        public override string ToString()
        {
            return string.Format("Current:{0} Max:{1} Diff:{2} Type:{3} Target:{4} Source:{5}", currentMP, maxMP, diff, type, target, source);
        }
    }
}