using System.Collections.Generic;
using System.Linq;
using TCC.Data.Databases;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class ItemAmount
    {
        public uint Id { get; set; }
        public int Amount { get; set; }

        public ItemAmount(uint id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
    public class ParsedGearItem
    {
        public uint ItemId { get; }
        public int Enchant { get; }
        public long Exp { get; }
        public ParsedGearItem(uint id, int en, long exp)
        {
            ItemId = id;
            Enchant = en;
            Exp = exp;
        }
    }
    public class S_INVEN : ParsedMessage
    {
        public bool Failed { get; }
        public bool IsOpen { get; }
        public bool First { get; }
        public bool More { get; }

        public Dictionary<uint, ItemAmount> Items { get; private set; }
        public Dictionary<uint, ParsedGearItem> GearItems { get; private set; }

        public S_INVEN(TeraMessageReader reader) : base(reader)
        {
            Items = new Dictionary<uint, ItemAmount>();
            GearItems = new Dictionary<uint, ParsedGearItem>();
            // don't parse if standard noctenium is on
            if (SessionManager.CurrentPlayer.InfBuffs.Any(x => AbnormalityDatabase.NoctIds.Contains(x.Abnormality.Id))) return;
            try
            {
                var count = reader.ReadUInt16();
                var offset = reader.ReadUInt16();
                reader.Skip(16);
                IsOpen = reader.ReadBoolean();
                First = reader.ReadBoolean();
                More = reader.ReadBoolean();
                if (offset == 0) return;
                reader.BaseStream.Position = offset - 4;
                for (var i = 0; i < count; i++)
                {
                    reader.Skip(2); // var curr = reader.ReadUInt16();
                    var next = reader.ReadUInt16();
                    reader.Skip(2); // passivity array count
                    reader.Skip(2); // passivity array offset
                    if (reader.Factory.ReleaseVersion >= 8000)    // by HQ 20190216. Probably less than 8201  
                    {
                        reader.Skip(2);     // Unknown array count added.    by HQ 20190122
                        reader.Skip(2);     // Unknown array offset added.    by HQ 20190122
                    }
                    reader.Skip(2); // custom string offset
                    var itemId = reader.ReadUInt32();
                    reader.Skip(8); //dbid
                    reader.Skip(8); //ownerId (playerId)
                    var slot = reader.ReadUInt32();
                    reader.Skip(4); //storage type (0)
                    var amount = reader.ReadInt32();
                    var enchant = reader.ReadInt32();
                    reader.Skip(4); //durability
                    reader.Skip(1); //soulbound
                    reader.Skip(4 * 5); //crystals
                    reader.Skip(4); //passivity set
                    reader.Skip(4); //extra passivity set
                    reader.Skip(4); //remodel
                    reader.Skip(4); //dye
                    reader.Skip(4); //dye sec remaining
                    reader.Skip(8); //dye date
                    reader.Skip(8); //dye expire date
                    reader.Skip(1); //masterwork
                    reader.Skip(4); //enigma
                    reader.Skip(4); //times brokered
                    reader.Skip(4); //enchant advantage
                    reader.Skip(4); //enchant bonus
                    reader.Skip(4); //enchant bonus max plus
                    reader.Skip(8); //bound to player (playerId)
                    reader.Skip(1); //awakened
                    reader.Skip(4); //liberation count
                    reader.Skip(4); //feedstock
                    reader.Skip(4); //bound to item
                    reader.Skip(1); //has etching
                    reader.Skip(1); //pc bang
                    var exp = reader.ReadInt64();

                    if (slot > 39) Items[slot] = new ItemAmount(itemId, amount);
                    else GearItems[itemId] = new ParsedGearItem(itemId, enchant, exp);

                    if (next == 0) break;
                    reader.BaseStream.Position = next - 4;
                }
            }
            catch
            {
                Log.F("Failed to parse S_INVEN");
                Failed = true;
            }
        }
    }
    //public class S_INVEN : ParsedMessage
    //{
    //    public static List<Tuple<uint, int, uint>> GearItems { get; private set; }
    //    public static Dictionary<uint, ItemAmount> Items { get; private set; }
    //    public bool More { get; }
    //    public bool First { get; }
    //    public bool IsOpen { get; }
    //    public S_INVEN(TeraMessageReader reader) : base(reader)
    //    {
    //        //TODO
    //        if (SessionManager.CurrentPlayer.InfBuffs.Any(x => AbnormalityDatabase.NoctIds.Contains(x.Abnormality.Id))) return;
    //        var count = reader.ReadUInt16();
    //        var invOffset = reader.ReadUInt16();
    //        reader.Skip(8 + 8);
    //        IsOpen = reader.ReadBoolean();
    //        if (!IsOpen) return;
    //        First = reader.ReadBoolean();
    //        if (First)
    //        {
    //            Items = new Dictionary<uint, ItemAmount>();
    //            GearItems = new List<Tuple<uint, int, uint>>();
    //        }
    //        More = reader.ReadBoolean();
    //        if (invOffset == 0) return;
    //        reader.BaseStream.Position = invOffset - 4;
    //        try
    //        {
    //            for (var i = 0; i < count; i++)
    //            {

    //                reader.Skip(2); //var offset = reader.ReadUInt16();
    //                var next = reader.ReadUInt16();
    //                reader.Skip(6);
    //                var itemId = reader.ReadUInt32();
    //                reader.Skip(8 + 8);
    //                var slot = reader.ReadUInt32();
    //                if (slot > 39)
    //                {
    //                    reader.Skip(4);
    //                    var amount = reader.ReadInt32();
    //                    Items[slot] = new ItemAmount(itemId, amount);
    //                    //switch (itemId)
    //                    //{
    //                    //    case 151643:
    //                    //        ElleonMarks = reader.ReadUInt32();
    //                    //        break;
    //                    //    case 45474:
    //                    //        DragonwingScales = reader.ReadUInt32();
    //                    //        break;
    //                    //    case 45482:
    //                    //        PiecesOfDragonScroll = reader.ReadUInt32();
    //                    //        break;
    //                    //}

    //                    if (next == 0) break;
    //                    reader.BaseStream.Position = next - 4;
    //                    continue;
    //                }
    //                reader.Skip(4 + 4);
    //                var enchant = reader.ReadInt32();
    //                reader.Skip(4 + 1 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 8 + 1 + 4 + 4 + 4 + 4 + 4 + 4 + 1 + 4 + 4 + 4 + 4 + 1 + 1);
    //                var exp = reader.ReadUInt32();
    //                GearItems.Add(new Tuple<uint, int, uint>(itemId, enchant, exp));
    //                reader.BaseStream.Position = next - 4;
    //            }

    //        }
    //        catch
    //        {
    //            // ignored
    //        }

    //    }
    //}
}
