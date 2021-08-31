using Nostrum;
using Newtonsoft.Json;
using Nostrum.WPF.ThreadSafe;

namespace TCC.Data.Pc
{
    public class InventoryItem : ThreadSafePropertyChanged
    {
        private int _amount;
        public uint Id { get; }

        [JsonIgnore]
        public Item Item => Game.DB!.ItemsDatabase.Items.TryGetValue(Id, out var item)
            ? item
            : new Item(0, "", RareGrade.Common, 0, 0, "");
        public uint Slot { get; }
        public int Amount
        {
            get => _amount;
            set
            {
                if (_amount == value) return;
                _amount = value;
                N();
            }
        }

        public InventoryItem(uint slot, uint id, int amount)
        {
            Id = id;
            Amount = amount;
            Slot = slot;
        }
    }
}