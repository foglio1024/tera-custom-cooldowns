


namespace TeraPacketParser.Messages;

public class S_PLAYER_CHANGE_FLIGHT_ENERGY : ParsedMessage
{
    public float Energy { get; private set; }
    public S_PLAYER_CHANGE_FLIGHT_ENERGY(TeraMessageReader reader) : base(reader)
    {
        Energy = reader.ReadSingle();
    }
}