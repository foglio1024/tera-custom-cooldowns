


namespace TeraPacketParser.Messages;

public class S_DUNGEON_EVENT_MESSAGE : ParsedMessage
{
    public uint MessageId { get; private set; }
    public S_DUNGEON_EVENT_MESSAGE(TeraMessageReader reader) : base(reader)
    {
        var o = reader.ReadUInt16();
        reader.BaseStream.Position = o - 4;
        if (uint.TryParse(reader.ReadTeraString().Substring("@dungeon:".Length), out var msgId)) MessageId = msgId;
        else MessageId = 0;
            
    }
}