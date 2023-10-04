namespace TeraDataLite;

public struct CityWarGuildData
{
    public bool Self { get; }
    public uint Id { get; }
    public int Kills { get; }
    public int Deaths { get; }
    public float TowerHp { get; }

    public CityWarGuildData(int self, uint id, int kills, int deaths, float towerHp)
    {
        Self = self == 0;
        Id = id;
        Kills = kills;
        Deaths = deaths;
        TowerHp = towerHp;
    }
}