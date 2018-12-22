namespace TCC.Data
{
    public class GuildQuest
    {
        public uint Id { get; }
        public string Title { get; }
        /*
                public uint ZoneId { get; }
        */

        public GuildQuest(uint id, string s)
        {
            Id = id;
            Title = s;
            //ZoneId = zId;
        }
    }

}
