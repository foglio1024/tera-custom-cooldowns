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
        public static List<Tuple<uint, int, uint>> Items { get; private set; }
        public bool More { get; private set; }
        public bool First { get; private set; }
        public bool IsOpen { get; private set; }
        public static uint ElleonMarks { get; set; }
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
            if (First) Items = new List<Tuple<uint, int, uint>>();
            More = reader.ReadBoolean();
            if (invOffset == 0) return;
            reader.BaseStream.Position = invOffset - 4;
            try
            {
                for (var i = 0; i < count; i++)
                {

                    var offset = reader.ReadUInt16();
                    var next = reader.ReadUInt16();
                    reader.Skip(6);
                    var itemId = reader.ReadUInt32();
                    reader.Skip(8 + 8);
                    var slot = reader.ReadUInt32();
                    if (slot > 39)
                    {
                        if (itemId == 151643)
                        {
                            reader.Skip(4);
                            ElleonMarks = reader.ReadUInt32();
                        }
                        reader.BaseStream.Position = next - 4;
                        continue;
                    }
                    reader.Skip(4 + 4);
                    var enchant = reader.ReadInt32();
                    reader.Skip(4 + 1 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 8 + 1 + 4 + 4 + 4 + 4 + 4 + 4 + 1 + 4 + 4 + 4 + 4 + 1 + 1);
                    var exp = reader.ReadUInt32();
                    Items.Add(new Tuple<uint, int, uint>(itemId, enchant, exp));
                    reader.BaseStream.Position = next - 4;
                }

            }
            catch { }

        }
    }
}
