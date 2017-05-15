using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tera.Game.Messages
{
    public struct ReadyPartyMembers
    {
        public uint ServerId;
        public uint PlayerId;
        public byte Status;
    }

    public class S_CHECK_TO_READY_PARTY : ParsedMessage
    {
        internal S_CHECK_TO_READY_PARTY(TeraMessageReader reader)
            : base(reader)
        {
            PrintRaw();

            Count = reader.ReadUInt16();
            var offset = reader.ReadUInt16();
            for (var i = 1; i <= Count; i++)
            {
                reader.BaseStream.Position = offset - 4;
                var pointer = reader.ReadUInt16();
                Debug.Assert(pointer == offset);//should be the same
                var nextOffset = reader.ReadUInt16();
                var serverId = reader.ReadUInt32();
                var playerId = reader.ReadUInt32();
                var status = reader.ReadByte();
                Party.Add(new ReadyPartyMembers
                {
                    ServerId = serverId,
                    PlayerId = playerId,
                    Status = status
                });
                offset = nextOffset;
            }

            Debug.WriteLine($"Count:{Count}");
            foreach(ReadyPartyMembers menber in Party)
            {
                Debug.WriteLine($"ServerId:{BitConverter.ToString(BitConverter.GetBytes(menber.ServerId))}, PlayerId:{BitConverter.ToString(BitConverter.GetBytes(menber.PlayerId))}, State:{menber.Status}");
            }
        }

        public UInt16 Count { get; set; }

        public List<ReadyPartyMembers> Party { get; } = new List<ReadyPartyMembers>();
    }
}
