// Copyright (c) Gothos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using TCC.TeraCommon.PacketLog.Parsing;

namespace TCC.TeraCommon.PacketLog
{
    public class PacketLogReader
    {
        private readonly Stream _stream;

        private DateTime _time;

        internal PacketLogReader(Stream stream)
        {
            _stream = stream;
            Header = new LogHeader();
            ReadHeader();
        }

        public LogHeader Header { get; }

        private void ReadHeader()
        {
            BlockType blockType;
            byte[] data;

            {
                // magic bytes
                BlockHelper.ReadBlock(_stream, out blockType, out data);
                if (blockType != BlockType.MagicBytes)
                    throw new FormatException("First block must be a MagicBytes block");
                if (!data.SequenceEqual(LogHelper.Encoding.GetBytes(LogHelper.MagicBytes)))
                    throw new FormatException("Incorrect magic bytes");
            }

            do
            {
                BlockHelper.ReadBlock(_stream, out blockType, out data);
                switch (blockType)
                {
                    case BlockType.Start:
                        break;
                    case BlockType.Region:
                        Header.Region = LogHelper.Encoding.GetString(data);
                        break;
                    case BlockType.MagicBytes:
                    case BlockType.Timestamp:
                    case BlockType.Server:
                    case BlockType.Client:
                        throw new FormatException($"Unexpected block type in header '{blockType}'");
                }
            } while (blockType != BlockType.Start);
        }

        public Message ReadMessage()
        {
            while (_stream.Position != _stream.Length)
            {
                BlockType blockType;
                byte[] data;
                BlockHelper.ReadBlock(_stream, out blockType, out data);
                MessageDirection direction;
                switch (blockType)
                {
                    case BlockType.Timestamp:
                        _time = LogHelper.BytesToTimeSpan(data);
                        break;
                    case BlockType.Client:
                        direction = MessageDirection.ClientToServer;
                        return new Message(_time, direction, new ArraySegment<byte>(data));
                    case BlockType.Server:
                        direction = MessageDirection.ServerToClient;
                        return new Message(_time, direction, new ArraySegment<byte>(data));
                    default:
                        throw new FormatException($"Unexpected blocktype {blockType}");
                }
            }
            return null;
        }
    }
}