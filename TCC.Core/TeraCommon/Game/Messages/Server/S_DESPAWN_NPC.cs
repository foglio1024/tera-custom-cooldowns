using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class SDespawnNpc : ParsedMessage
    {
        internal SDespawnNpc(TeraMessageReader reader) : base(reader)
        {
            Npc = reader.ReadEntityId();
            Position = reader.ReadVector3f();
            var status = reader.ReadByte();
            Dead = status == 5 || status == 3; // 1 = move out of view, 3 = retreated?, 5 = death
        }

        public EntityId Npc { get; }

        public EntityId NPC => Npc;
        //different case in different projects, need refactoring
        public Vector3f Position { get; private set; }
        public bool Dead { get; }
    }
}