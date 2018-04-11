namespace Tera.Game
{
    public class GuildQuestItem
    {
        public uint ItemId { get; private set; }
        public ulong Amount { get; private set; }
        public GuildQuestItem(uint itemId, ulong amount)
        {
            ItemId = itemId;
            Amount = amount;
        }


        public override string ToString()
        {
            return "ItemId:"+ItemId+";Amount:"+Amount;
        }

    }
}
