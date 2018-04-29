using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_USER_EFFECT : ParsedMessage
    {
        private int circle, action;
        public ulong User { get; private set; }
        public ulong Source { get; private set; }
        public AggroAction Action
        {
            get
            {
                return (AggroAction)action;
            }
        }
        public AggroCircle Circle
        {
            get
            {
                return (AggroCircle)circle;
            }
        }

        public S_USER_EFFECT(TeraMessageReader reader) : base(reader)
        {
            User = reader.ReadUInt64();
            Source = reader.ReadUInt64();
            circle = reader.ReadInt32();
            action = reader.ReadInt32();

            //string sourceName = Source.ToString();
            //if (EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == Source) != null)
            //{
            //    sourceName = EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == Source).Name;
            //}
            //string userName = User.ToString();
            //if (User == SessionManager.CurrentPlayer.EntityId) userName = SessionManager.CurrentPlayer.Name;

            //Console.WriteLine("[S_USER_EFFECT] {0} > {1} ({2} {3})", sourceName, userName, Action, Circle);
        }
    }
}
