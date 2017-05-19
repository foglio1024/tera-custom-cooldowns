using Tera.Game;
using Tera.Game.Messages;

namespace TCC
{
    public class S_PLAYER_CHANGE_MP : ParsedMessage
    {
        int currentMP, maxMP, diff;
        uint type;
        ulong target, source;

        public int CurrentMP { get => currentMP; }
        public int MaxMP { get => maxMP; }
        public int Diff { get => diff; }
        public uint Type { get => type; }
        public ulong Target { get => target; }
        public ulong Source { get => source; }

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