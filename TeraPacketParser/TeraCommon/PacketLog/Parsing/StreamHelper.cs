﻿// Copyright (c) Gothos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace TeraPacketParser.TeraCommon.PacketLog.Parsing;

internal static class StreamHelper
{
    private static void ReadBytes(Stream stream, byte[] buffer, int offset, int count)
    {
        while (count > 0)
        {
            var bytesRead = stream.Read(buffer, offset, count);
            if (bytesRead == 0)
                throw new IOException("Unexpected end of stream");
            count -= bytesRead;
            offset += bytesRead;
        }
    }

    public static byte[] ReadBytes(this Stream stream, int count)
    {
        var result = new byte[count];
        ReadBytes(stream, result, 0, result.Length);
        return result;
    }
}