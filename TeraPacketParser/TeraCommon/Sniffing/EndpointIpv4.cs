// Copyright (c) CodesInChaos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Contracts;
using System.Net;

namespace TeraPacketParser.TeraCommon.Sniffing;

internal struct EndpointIpv4 : IEquatable<EndpointIpv4>
{
    readonly uint _ip;
    readonly ushort _port;

    public static bool operator ==(EndpointIpv4 x, EndpointIpv4 y)
    {
        return x._ip == y._ip && x._port == y._port;
    }

    public static bool operator !=(EndpointIpv4 x, EndpointIpv4 y)
    {
        return !(x == y);
    }

    public bool Equals(EndpointIpv4 other)
    {
        return this == other;
    }

    public override bool Equals(object? obj)
    {
        return obj is EndpointIpv4 ipv4 && Equals(ipv4);
    }

    public override int GetHashCode()
    {
        return unchecked((int) (_ip + (uint) _port*1397));
    }

    public EndpointIpv4(uint ip, ushort port)
    {
        _ip = ip;
        _port = port;
    }

    static IPAddress ToIpAddress(uint ip)
    {
        var bytes = new byte[4];
        bytes[0] = (byte) (ip >> 24);
        bytes[1] = (byte) (ip >> 16);
        bytes[2] = (byte) (ip >> 8);
        // ReSharper disable once ShiftExpressionRealShiftCountIsZero
        bytes[3] = (byte) (ip >> 0);
        return new IPAddress(bytes);
    }

    [Pure]
    public IPEndPoint ToIpEndpoint()
    {
        return new IPEndPoint(ToIpAddress(_ip), _port);
    }

    public override string ToString()
    {
        return $"{ToIpAddress(_ip)}:{_port}";
    }
}