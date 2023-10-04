namespace TeraDataLite;

public struct ItemAmount
{
    public uint Id { get; set; }
    public int Amount { get; set; }

    public ItemAmount(uint id, int amount)
    {
        Id = id;
        Amount = amount;
    }
}