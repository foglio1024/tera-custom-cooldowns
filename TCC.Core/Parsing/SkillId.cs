using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing
{
    public struct SkillId
    {
        private ulong _raw;
        private int _version;

        public SkillId(TeraMessageReader reader) : this()
        {
            _version = reader.Factory.ReleaseVersion;
            _raw = _version >= 7401 ? reader.ReadUInt64() : reader.ReadUInt32();
        }

        public int Id => (int)(_version >= 7401 ? _raw & 0x00000000_0FFFFFFF : _raw & 0x03FFFFFF);
        public bool isAction => _version >= 7401 ? (_raw & 0x00000000_10000000) != 0 : (_raw & 0x04000000) != 0;
        public bool isReaction => _version >= 7401 ? (_raw & 0x00000000_20000000) != 0 : (_raw & 0x08000000) != 0;
        public bool hasHuntingZone => _version >= 7401 ? (_raw & 0x00000001_00000000) != 0 : (_raw & 0x40000000) != 0;
        public int HuntingZone => hasHuntingZone ? (int)((_version >= 7401 ? _raw & 0x00000000_0FFF0000 : _raw & 0x03FF0000) >> 16) : 0;

    }
}
