using System;
using FoglioUtils.Extensions;

namespace TCC.TeraCommon
{
    public class Message
    {
        public Message(DateTime time, MessageDirection direction, ArraySegment<byte> data)
        {
            Time = time;
            Direction = direction;
            Data = data;
        }
        public Message(DateTime time, MessageDirection dir, string hex) : this(time, dir, new ArraySegment<byte>(hex.ToByteArrayHex())) { }

        public DateTime Time { get; private set; }
        public MessageDirection Direction { get; private set; }
        public ArraySegment<byte> Data { get; }

        // ReSharper disable once PossibleNullReferenceException
        public ushort OpCode => (ushort) (Data.Array[Data.Offset] | Data.Array[Data.Offset + 1] << 8);
        // ReSharper disable once AssignNullToNotNullAttribute
        public ArraySegment<byte> Payload => new ArraySegment<byte>(Data.Array, Data.Offset + 2, Data.Count - 2);
    }
}