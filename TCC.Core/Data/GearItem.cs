namespace TCC.Data
{
    public class GearItem
    {
        public uint Id { get; private set; }
        public GearTier Tier { get; private set; }
        public GearPiece Piece { get; private set; }

        public int Enchant { get; private set; }
        public long Experience { get; private set; }
        public int MaxExperience => SessionManager.CurrentDatabase.GetItemMaxExp(Id, Enchant);

        public double ExperienceFactor
        {
            get
            {
                if (MaxExperience == 0) return 0;
                if (Experience >= MaxExperience) return 1;
                return Experience / (double)MaxExperience;
            }
        }

        public int CorrectedEnchant
        {
            get
            {
                if (!IsJewel) return Enchant;
                return Tier == 0 ? 6 : 9;
            }
        }

        public bool IsJewel => (int)Piece >= 5;
        public int TotalLevel => CalculateLevel();
        public double LevelFactor => TotalLevel / (double)37;
        private int CalculateLevel()
        {
            var t = 0;
            if (Tier > GearTier.Low)
            {
                t = ((int)Tier - 1) * 9 + 6;
            }
            return t + CorrectedEnchant;
        }

        public string Name => SessionManager.CurrentDatabase.ItemsDatabase.GetItemName(Id);
        public GearItem(uint id, GearTier t, GearPiece p, int enchant, long exp)
        {
            Id = id;
            Tier = t;
            Piece = p;
            Enchant = enchant;
            Experience = exp;
        }

        public override string ToString()
        {
            return $"[{Id}] {Piece} {Tier} +{Enchant}";
        }
    }
}
