using Newtonsoft.Json;

namespace TCC.Data.Abnormalities;

public class AbnormalityData
{
    public uint Id { get; init; }
    public int Stacks { get; set; }
    public double Duration { get; init; }
    [JsonIgnore]
    public Abnormality Abnormality => Game.DB!.AbnormalityDatabase.Abnormalities[Id];
}