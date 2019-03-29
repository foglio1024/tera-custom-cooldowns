namespace TCC.Data.Abnormalities
{
    public class AbnormalityData
    {
        public uint Id { get; set; }
        public int Stacks { get; set; }
        public ulong Duration { get; set; }

        public Abnormality Abnormality => SessionManager.DB.AbnormalityDatabase.Abnormalities[Id];
    }
}
