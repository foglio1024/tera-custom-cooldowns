using System;

namespace TCC.Exceptions
{
    public class PacketParseException : Exception
    {
        public string OpcodeName { get; }
        public byte[] RawData { get; }
        public PacketParseException(string msg, Exception inner, string opcodeName, byte[] data) : base(msg, inner)
        {
            OpcodeName = opcodeName;
            RawData = data;
        }
    }
}