using System.IO;
using System.Text;
using TCC.TeraCommon;
using TCC.TeraCommon.Game;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing
{
    internal class CustomReader : BinaryReader
    {
        public CustomReader(Message message)
        : base(GetStream(message), Encoding.Unicode)
        {
            Message = message;
        }

        public Message Message { get; private set; }
        public string OpCodeName { get; private set; }
        public uint Version { get; private set; }
        internal OpCodeNamer SysMsgNamer { get; private set; }

        private static MemoryStream GetStream(Message message)
        {
            return new MemoryStream(message.Payload.Array, message.Payload.Offset, message.Payload.Count, false, true);
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
            return new Angle(ReadInt16());
        }

        public void Skip(int count)
        {
            ReadBytes(count);
        }

        // Tera uses null terminated litte endian UTF-16 strings
        public string ReadTeraString()
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

    }
}
