using TeraDataLite;

namespace TeraPacketParser.Messages;

public class S_USER_EFFECT : ParsedMessage
{
    int _circle, _action;
    public ulong User { get; private set; }
    public ulong Source { get; private set; }
    public AggroAction Action => (AggroAction)_action;

    public AggroCircle Circle => (AggroCircle)_circle;

    public S_USER_EFFECT(TeraMessageReader reader) : base(reader)
    {
        User = reader.ReadUInt64();
        Source = reader.ReadUInt64();
        _circle = reader.ReadInt32();
        _action = reader.ReadInt32();

        //string sourceName = Source.ToString();
        //if (EntityManager.CurrentBosses.FirstOrDefault(x => x.EntityId == Source) != null)
        //{
        //    sourceName = EntityManager.CurrentBosses.FirstOrDefault(x => x.EntityId == Source).Name;
        //}
        //string userName = User.ToString();
        //if (User == SessionManager.CurrentPlayer.EntityId) userName = SessionManager.CurrentPlayer.Name;

        //Console.WriteLine("[S_USER_EFFECT] {0} > {1} ({2} {3})", sourceName, userName, Action, Circle);
    }
}