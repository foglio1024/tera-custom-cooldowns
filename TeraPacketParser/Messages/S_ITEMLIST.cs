using System.Collections.Generic;
using TeraDataLite;

namespace TeraPacketParser.Messages
{
    public class S_ITEMLIST : ParsedMessage
    {
        public bool Failed { get; }
        public bool IsOpen { get; }
        public bool First { get; }
        public bool More { get; }
        public bool LastInBatch { get; }
        public int Pocket { get; set; }
        public int NumPockets { get; set; }
        public int Size { get; set; }
        public int Container { get; set; }

        public Dictionary<uint, ItemAmount> Items { get; private set; }
        public Dictionary<uint, ParsedGearItem> GearItems { get; private set; }

        public S_ITEMLIST(TeraMessageReader r) : base(r)
        {
            Items = new Dictionary<uint, ItemAmount>();
            GearItems = new Dictionary<uint, ParsedGearItem>();

            try
            {
                var count = r.ReadUInt16();
                var offset = r.ReadUInt16();
                r.Skip(8); // gameId
                Container = r.ReadInt32();
                Pocket = r.ReadInt32();
                NumPockets = r.ReadInt32();
                Size = r.ReadInt32();
                r.Skip(8); // var money = r.ReadInt64();
                r.Skip(4); // var lootPriority = r.ReadInt32();
                IsOpen = r.ReadBoolean();
                r.Skip(1); // var requested = r.ReadBoolean();
                First = r.ReadBoolean();
                More = r.ReadBoolean();
                LastInBatch= r.ReadBoolean();
                if (offset == 0) return;
                r.RepositionAt(offset);
                for (var i = 0; i < count; i++)
                {
                    r.Skip(2); // var curr = reader.ReadUInt16();
                    var next = r.ReadUInt16();
                    r.Skip(4); // crystals ref
                    r.Skip(4); // passivity ref
                    r.Skip(4); // merged passivity ref
                    r.Skip(2); // custom string offset
                    var itemId = r.ReadUInt32();
                    r.Skip(8); // dbid
                    r.Skip(8); // ownerId (playerId)
                    var slot = r.ReadUInt32();
                    //if(slot <= 39) continue;
                    var amount = r.ReadInt32();
                    r.Skip(4); // enchant
                    r.Skip(4); // durability
                    r.Skip(4); // soulbound
                    r.Skip(4); // passivity sets
                    r.Skip(4); // extra passivity sets
                    r.Skip(4); // remodel
                    r.Skip(4); // dye
                    r.Skip(4); // dye sec remaining
                    r.Skip(8); // dye date
                    r.Skip(8); // dye expire date
                    r.Skip(1); // masterwork
                    r.Skip(4); // enigma
                    r.Skip(4); // times brokered
                    r.Skip(4); // enchant advantage
                    r.Skip(4); // enchant bonus
                    r.Skip(4); // enchant bonus max plus
                    r.Skip(8); // bound to player (playerId)
                    r.Skip(1); // awakened
                    r.Skip(4); // liberation count
                    r.Skip(4); // feedstock
                    r.Skip(4); // bound to item
                    r.Skip(1); // has etching
                    r.Skip(1); // pc bang
                    r.Skip(8); // exp

                    Items[slot] = new ItemAmount(itemId, amount);
                    //if (slot > 39) Items[slot] = new ItemAmount(itemId, amount);
                    //else GearItems[itemId] = new ParsedGearItem(itemId, enchant, exp);

                    if (next == 0) break;
                    r.RepositionAt(next);
                }
            }
            catch
            {
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
