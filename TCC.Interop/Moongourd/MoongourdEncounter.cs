namespace TCC.Interop.Moongourd;

public class MoongourdEncounter : IMoongourdEncounter
{
    public string PlayerName { get; set; } = "";
    public long LogId { get; set; }
    public int PlayerDps { get; set; }
    public int PlayerDeaths { get; set; }
    public int AreaId { get; set; }
    public int BossId { get; set; }
    //public string DungeonName => Game.DB!.RegionsDatabase.GetZoneName((uint)AreaId);
    //public string BossName => Game.DB!.MonsterDatabase.GetMonsterName((uint)BossId, (uint)AreaId);
    //public ICommand Browse { get; }
    //public Class PlayerClass { get; }
    //public string PlayerServer { get; } = "";
    //public int Timestamp { get; set; }
    //public int PartyDps { get; set; }

    //public MoongourdEncounter()
    //{
    //    //Browse = new RelayCommand(_ =>
    //    //{
    //    //    var reg = region;
    //    //    reg = reg.StartsWith("eu") ? "eu" : reg;
    //    //    reg = reg == "na" ? "" : reg + "/";
    //    //    Utils.Utilities.OpenUrl("https://" + $"moongourd.com/{reg}upload/{AreaId}/{BossId}/1/{LogId}");
    //    //});
    //}
}