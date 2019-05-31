


namespace TeraPacketParser.Messages
{
    public class S_PLAYER_CHANGE_MP : ParsedMessage
    {
        private int _currentMP, _maxMP, _diff;
        private uint _type;
        private ulong _target, _source;

        public int CurrentMP => _currentMP;
        public int MaxMP => _maxMP;
        public int Diff => _diff;
        public uint Type => _type;
        public ulong Target => _target;
        public ulong Source => _source;

        public S_PLAYER_CHANGE_MP(TeraMessageReader reader) : base(reader)
        {
            _currentMP = reader.ReadInt32();
            _maxMP = reader.ReadInt32();
            _diff = reader.ReadInt32();
            _type = reader.ReadUInt32();
            _target = reader.ReadUInt64();
            _source = reader.ReadUInt64();
            //System.Console.WriteLine(this);

        }
        public override string ToString()
        {
            return $"Current:{_currentMP} Max:{_maxMP} Diff:{_diff} Type:{_type} Target:{_target} Source:{_source}";
        }
    }
}