using TeraDataLite;

namespace TCC.Data;

public class GearItem
{
    public uint Id { get; }
    public GearTier Tier { get; }
    public GearPiece Piece { get; }
    public int Enchant { get; }
    public long Experience { get; }
    public int MaxExperience => Game.DB!.GetItemMaxExp(Id, Enchant);
    public double ExperienceFactor
    {
        get
        {
            return MaxExperience == 0
                ? 0
                    : Experience >= MaxExperience
                    ? 1
                : Experience / (double)MaxExperience;
        }
    }
    public int CorrectedEnchant
    {
        get
        {
            return !IsJewel
                ? Enchant
                    : Tier == 0
                    ? 6
                : 9;
        }
    }
    public bool IsJewel => (int)Piece >= 5;
    public int TotalLevel => CalculateLevel();
    public double LevelFactor => TotalLevel / (double)37;
    public string Name => Game.DB!.ItemsDatabase.GetItemName(Id);

    public GearItem(uint id, GearTier t, GearPiece p, int enchant, long exp)
    {
        Id = id;
        Tier = t;
        Piece = p;
        Enchant = enchant;
        Experience = exp;
    }

    public GearItem(GearItemData data)
    {
        Id = data.Id;
        Tier = data.Tier;
        Piece = data.Piece;
    }

    private int CalculateLevel()
    {
        var t = 0;
        if (Tier > GearTier.Low)
        {
            t = ((int)Tier - 1) * 9 + 6;
        }
        return t + CorrectedEnchant;
    }

    public override string ToString()
    {
        return $"[{Id}] {Piece} {Tier} +{Enchant}";
    }
}