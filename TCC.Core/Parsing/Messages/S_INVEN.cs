using System;
using System.Collections.Generic;
using System.Linq;
using TCC.Data.Databases;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{

    public class S_INVEN : ParsedMessage
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
        public static List<Tuple<uint, int, uint>> GearItems { get; private set; }
        public static Dictionary<uint, ItemAmount> Items { get; private set; }
        public bool More { get; }
        public bool First { get; }
        public bool IsOpen { get; }
        public S_INVEN(TeraMessageReader reader) : base(reader)
        {
            //TODO
            if (SessionManager.CurrentPlayer.InfBuffs.Any(x => AbnormalityDatabase.NoctIds.Contains(x.Abnormality.Id))) return;
            var count = reader.ReadUInt16();
            var invOffset = reader.ReadUInt16();
            reader.Skip(8 + 8);
            IsOpen = reader.ReadBoolean();
            if (!IsOpen) return;
            First = reader.ReadBoolean();
            if (First)
            {
                Items = new Dictionary<uint, ItemAmount>();
                GearItems = new List<Tuple<uint, int, uint>>();
            }
            More = reader.ReadBoolean();
            if (invOffset == 0) return;
            reader.BaseStream.Position = invOffset - 4;
            try
            {
                for (var i = 0; i < count; i++)
                {

                    reader.Skip(2); //var offset = reader.ReadUInt16();
                    var next = reader.ReadUInt16();
                    reader.Skip(6);
                    var itemId = reader.ReadUInt32();
                    reader.Skip(8 + 8);
                    var slot = reader.ReadUInt32();
                    if (slot > 39)
                    {
                        reader.Skip(4);
                        var amount = reader.ReadInt32();
                        Items[slot] = new ItemAmount(itemId, amount);
                        //switch (itemId)
                        //{
                        //    case 151643:
                        //        ElleonMarks = reader.ReadUInt32();
                        //        break;
                        //    case 45474:
                        //        DragonwingScales = reader.ReadUInt32();
                        //        break;
                        //    case 45482:
                        //        PiecesOfDragonScroll = reader.ReadUInt32();
                        //        break;
                        //}

                        if (next == 0) break;
                        reader.BaseStream.Position = next - 4;
                        continue;
                    }
                    reader.Skip(4 + 4);
                    var enchant = reader.ReadInt32();
                    reader.Skip(4 + 1 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 8 + 1 + 4 + 4 + 4 + 4 + 4 + 4 + 1 + 4 + 4 + 4 + 4 + 1 + 1);
                    var exp = reader.ReadUInt32();
                    GearItems.Add(new Tuple<uint, int, uint>(itemId, enchant, exp));
                    reader.BaseStream.Position = next - 4;
                }

            }
            catch
            {
                // ignored
            }

        }
    }
}
