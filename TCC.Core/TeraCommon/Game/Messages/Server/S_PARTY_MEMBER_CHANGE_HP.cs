using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class SPartyMemberChangeHp : ParsedMessage
    {
        internal SPartyMemberChangeHp(TeraMessageReader reader) : base(reader)
        {
            ServerId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt32();
            HpRemaining = reader.ReadInt32();
            TotalHp = reader.ReadInt32();
            Unknow3 = reader.ReadInt16();
            // Debug.WriteLine("target = " + TargetId + ";Hp left:" + HpRemaining + ";Max HP:" + TotalHp + ";Unknow3:" + Unknow3);
        }

        public int Unknow3 { get; }

        public int HpRemaining { get; }

        public int TotalHp { get; }

        public uint ServerId { get; }
        public uint PlayerId { get; }
        public bool Slaying => TotalHp > HpRemaining*2 && HpRemaining > 0;
    }
}