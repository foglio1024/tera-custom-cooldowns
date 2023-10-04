using System.IO;
using System.Text;
using TeraPacketParser.Data;

namespace TeraPacketParser;

// Used by `ParsedMessage`s to parse themselves
public class TeraMessageReader : BinaryReader
{
    public TeraMessageReader(Message message, OpCodeNamer opCodeNamer, MessageFactory factory, OpCodeNamer sysMsgNamer)
        : base(GetStream(message), Encoding.Unicode)
    {
        Message = message;
        OpCodeName = opCodeNamer.GetName(message.OpCode);
        SysMsgNamer = sysMsgNamer;
        Factory = factory;
    }

    public void RepositionAt(int position)
    {
        BaseStream.Position = position - 4;
    }

    public Message Message { get; private set; }
    public string OpCodeName { get; private set; }
    internal OpCodeNamer SysMsgNamer { get; private set; }
    public MessageFactory Factory { get; set; }

    static MemoryStream GetStream(Message message)
    {
        return message.Payload.Array != null 
            ? new MemoryStream(message.Payload.Array, message.Payload.Offset, message.Payload.Count, false, true) 
            : new MemoryStream();
    }

    public EntityId ReadEntityId()
    {
        var id = ReadUInt64();
        return new EntityId(id);
    }

    public Vector3f ReadVector3f()
    {
        Vector3f result;
        result.X = ReadSingle();
        result.Y = ReadSingle();
        result.Z = ReadSingle();
        return result;
    }

    public Angle ReadAngle()
    {
        return new(ReadInt16());
    }

    public void Skip(int count)
    {
        ReadBytes(count);
    }

    // Tera uses null terminated litte endian UTF-16 strings
    public string ReadTeraString()
    {
        try
        {
            var builder = new StringBuilder();
            while (true)
            {
                var c = ReadChar();
                if (c == 0)
                    return builder.ToString();
                builder.Append(c);
            }
        }
        catch (System.Exception)
        {
            return "";
        }
    }
}