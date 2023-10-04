namespace TeraPacketParser.Messages;

public class S_BATTLE_FIELD_ENTRANCE_INFO : ParsedMessage
{
    public int Id { get; }
    public int Zone { get; }

    public S_BATTLE_FIELD_ENTRANCE_INFO(TeraMessageReader reader) : base(reader)
    {
        Id = reader.ReadInt32();
        Zone = reader.ReadInt32();
    }
}