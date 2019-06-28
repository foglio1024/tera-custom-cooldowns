using Newtonsoft.Json;

namespace TCC.Data.Abnormalities
{
    public class AbnormalityData
    {
        public uint Id { get; set; }
        public int Stacks { get; set; }
        public ulong Duration { get; set; }
        [JsonIgnore]
        public Abnormality Abnormality => Session.DB.AbnormalityDatabase.Abnormalities[Id];
    }
}
