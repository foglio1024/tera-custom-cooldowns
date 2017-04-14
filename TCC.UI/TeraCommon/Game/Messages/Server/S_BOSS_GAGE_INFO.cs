namespace Tera.Game.Messages
{
    public class S_BOSS_GAGE_INFO : ParsedMessage
    {
        internal S_BOSS_GAGE_INFO(TeraMessageReader reader) : base(reader)
        {
            EntityId = reader.ReadEntityId();
            Type = reader.ReadInt32();
            NpcId = reader.ReadInt32();
            TargetId = reader.ReadEntityId();
            Unk1 = reader.ReadInt32();
            HpChange = reader.ReadSingle();
            Unk2 = reader.ReadByte(); //enrage?
            HpRemaining = reader.ReadSingle();
            TotalHp = reader.ReadSingle();
        }

        public byte Unk2 { get; }
        public int Unk1 { get; }
        public int NpcId { get; }
        public float HpChange { get; }
        public int Type { get; }
        public float HpRemaining { get; }
        public float TotalHp { get; }
        public EntityId TargetId { get; }
        public EntityId EntityId { get; }
    }
}