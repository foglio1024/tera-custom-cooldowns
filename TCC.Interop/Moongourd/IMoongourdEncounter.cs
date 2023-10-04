namespace TCC.Interop.Moongourd;

public interface IMoongourdEncounter
{
    string PlayerName { get; set; }
    long LogId { get; set; }
    int PlayerDps { get; set; }
    int PlayerDeaths { get; set; }
    int AreaId { get; set; }
    int BossId { get; set; }
    //string DungeonName { get; }
    //string BossName { get; }
}