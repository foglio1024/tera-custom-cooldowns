using System.Collections.Generic;

namespace TCC.Data;

public static class EpDataProvider
{
    // just keep this here, since it's the only perk we need
    static readonly Dictionary<uint, float> ManaBarrierAmplification = new()
    {
        { 0,  1.00f },
        { 1,  1.11f },
        { 2,  1.14f },
        { 3,  1.17f },
        { 4,  1.20f },
        { 5,  1.23f },
        { 6,  1.25f },
        { 7,  1.28f },
        { 8,  1.31f },
        { 9,  1.34f },
        { 10, 1.37f },
        { 11, 1.39f },
        { 12, 1.42f },
        { 13, 1.45f },
        { 14, 1.48f },
        { 15, 1.51f },
        { 16, 1.53f },
        { 17, 1.56f },
        { 18, 1.59f },
        { 19, 1.62f },
        { 20, 1.65f },
    };
    public static void SetManaBarrierPerkLevel(uint level)
    {
        ManaBarrierMult = ManaBarrierAmplification[level];
    }

    public static float ManaBarrierMult = 1;
}