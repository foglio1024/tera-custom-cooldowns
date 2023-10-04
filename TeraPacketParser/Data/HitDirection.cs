using System;

namespace TeraPacketParser.Data;

/*
 *  The enum value NEED to be set manually
 *  without that, converting the enum to int will cause massive weird bug, like:
 *  https://github.com/neowutran/ShinraMeter/issues/184
 * */
[Flags]
public enum HitDirection
{
    Back = 1,
    Left = 2,
    Right = 4,
    Side = 8,
    Front = 16,
    Dot = 32,
    Pet = 64
}