using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_SYSTEM_MESSAGE_LOOT_ITEM : ParsedMessage
    {
        public int ItemId { get; }
        public int Amount { get;}
        public string SysMessage { get;}


        public S_SYSTEM_MESSAGE_LOOT_ITEM(TeraMessageReader reader) : base(reader)
        {
            var msgOffset = reader.ReadUInt16();

            ItemId = reader.ReadInt32();
            reader.Skip(4); //unk1 = reader.ReadInt32();
            Amount = reader.ReadInt32();
            reader.Skip(4); //unk2 = reader.ReadInt32();
            reader.Skip(4); //unk3 = reader.ReadInt32();
            reader.Skip(4); //unk4 = reader.ReadInt32();
            reader.Skip(1); //unk5 = reader.ReadByte();
            reader.Skip(1); //unk6 = reader.ReadByte();
            reader.Skip(1); //unk7 = reader.ReadByte();
            reader.BaseStream.Position = msgOffset - 4;
            SysMessage = reader.ReadTeraString();
        }
    }
}
