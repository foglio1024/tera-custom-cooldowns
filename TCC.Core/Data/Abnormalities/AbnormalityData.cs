using Newtonsoft.Json;

namespace TCC.Data.Abnormalities
{
    public class AbnormalityData
    {
        public uint Id { get; set; }
        public int Stacks { get; set; }
        public double Duration { get; set; }
        [JsonIgnore]
        public Abnormality Abnormality => Game.DB!.AbnormalityDatabase.Abnormalities[Id];
    }
}
