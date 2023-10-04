using System;
using System.Collections.Generic;

namespace TCC.Interop.Moongourd;

public interface IMoongourdManager
{
    event Action Started;

    event Action<List<IMoongourdEncounter>> Finished;

    event Action<string> Failed;

    void GetEncounters(string playerName, string region, string playerServer = "", int areaId = 0, int bossId = 0);
};