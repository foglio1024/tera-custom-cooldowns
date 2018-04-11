using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_AVAILABLE_EVENT_MATCHING_LIST : ParsedMessage
    {
        public int VanguardCredits { get; set; }
        public int WeeklyDone { get; set; }
        public int DailyDone { get; set; }
        public int WeeklyMax { get; set; }
        public S_AVAILABLE_EVENT_MATCHING_LIST(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(4 + 4 + 4 + 4);
            DailyDone = reader.ReadInt32();
            reader.Skip(12);
            WeeklyDone = reader.ReadInt32();
            WeeklyMax= reader.ReadInt32();
            reader.Skip(4);
            reader.Skip(4);
            reader.Skip(4);
            reader.Skip(4);
            reader.Skip(4);
            reader.Skip(3);
            VanguardCredits = reader.ReadInt32();
        }
    }
}
