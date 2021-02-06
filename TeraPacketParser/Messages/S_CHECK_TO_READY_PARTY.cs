using System.Collections.Generic;
using TeraDataLite;

namespace TeraPacketParser.Messages
{
    public class S_CHECK_TO_READY_PARTY : ParsedMessage
    {
        internal S_CHECK_TO_READY_PARTY(TeraMessageReader reader) : base(reader)
        {

            Count = reader.ReadUInt16();
            var offset = reader.ReadUInt16();
            for (var i = 1; i <= Count; i++)
            {
                reader.BaseStream.Position = offset - 4;
                //var pointer = reader.ReadUInt16();
                reader.Skip(2);
                var nextOffset = reader.ReadUInt16();
                var serverId = reader.ReadUInt32();
                var playerId = reader.ReadUInt32();
                var status = reader.ReadByte();
                Party.Add(new ReadyPartyMember
                {
                    ServerId = serverId,
                    PlayerId = playerId,
                    Status = status == 1? ReadyStatus.Ready : ReadyStatus.NotReady
                });
                offset = nextOffset;
            }
        }

        public ushort Count { get; }

        public List<ReadyPartyMember> Party { get; } = new();
    }
}
