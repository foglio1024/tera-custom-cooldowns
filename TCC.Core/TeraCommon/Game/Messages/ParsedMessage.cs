using System;
using System.Diagnostics;
using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages
{
    // Base class for parsed messages
    public abstract class ParsedMessage : Message
    {
        public ParsedMessage(TeraMessageReader reader)
            : base(reader.Message.Time, reader.Message.Direction, reader.Message.Data)
        {
            Raw = reader.Message.Payload.Array;
            OpCodeName = reader.OpCodeName;
        }

        public byte[] Raw { get; protected set; }

        public string OpCodeName { get; }

        public void PrintRaw()
        {
            Debug.WriteLine(OpCodeName + ": ");
            Debug.WriteLine(BitConverter.ToString(Raw));
        }
    }
}