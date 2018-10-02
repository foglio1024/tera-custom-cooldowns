using System.Collections.Generic;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_VIEW_WARE_EX : ParsedMessage
    {
        public static List<BankPage> Pages = new List<BankPage>
        {
            {new BankPage(0)},
            {new BankPage(1)},
            {new BankPage(2)},
            {new BankPage(3)},
            {new BankPage(4)},
            {new BankPage(5)},
            {new BankPage(6)},
            {new BankPage(7)}
        };
        public S_VIEW_WARE_EX(TeraMessageReader reader) : base(reader)
        {
            var itemsArrayCount = reader.ReadUInt16();
            if (itemsArrayCount == 0) return;
            var itemsArrayOffset = reader.ReadUInt16();
            reader.Skip(8);
            var type = reader.ReadInt32();
            if (type > 1) return;
            reader.Skip(4);
            var index = reader.ReadInt32();
            Pages[index / 72].Items.Clear();
            reader.Skip(4);
            reader.Skip(8);
            reader.Skip(2); // var tabsCount = reader.ReadInt16();
            reader.BaseStream.Position = itemsArrayOffset - 4;
            for (int i = 0; i < itemsArrayCount; i++)
            {
                reader.Skip(2); // var current = reader.ReadUInt16();
                var next = reader.ReadUInt16();
                reader.Skip(4); //crystals array count and offset
                var id = reader.ReadInt32();
                reader.Skip(8);
                reader.Skip(8);
                var slot = reader.ReadUInt32();
                reader.Skip(4);
                reader.Skip(4); //amountTotal (????)
                var amount = reader.ReadInt32();
                Pages[index/72].Items.Add(new BankItem(id, slot, amount));
                if (next == 0) break;
                reader.BaseStream.Position = next - 4;
            }
        }
    }

    public struct BankPage
    {
        public List<BankItem> Items { get; }
        public uint Index { get; }
        public BankPage(uint i)
        {
            Items = new List<BankItem>();
            Index = i;
        }
    }

    public class BankItem
    {
        public int Id { get; }
        public uint Slot { get; }
        public int Amount { get; set; }

        public BankItem(int id, uint slot, int amount)
        {
            Id = id;
            Slot = slot;
            Amount = amount;
        }
    }
}
