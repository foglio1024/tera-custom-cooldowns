using System.Linq;
using Tera.Game;
using Tera.Game.Messages;

namespace TCC.Parsing.Messages
{
    public class S_NPC_STATUS : ParsedMessage
    {
        ulong entityId, targetId;
        bool enraged;
        int unk1, unk2;

        public ulong EntityId { get => entityId; }
        public ulong Target { get => targetId; }
        public bool IsEnraged
        {
            get => enraged;
        }

        public S_NPC_STATUS(TeraMessageReader reader) : base(reader)
        {
            entityId = reader.ReadUInt64();
            enraged = reader.ReadBoolean();
            reader.Skip(4); //unk1 = reader.ReadInt32();
            targetId = reader.ReadUInt64();
            reader.Skip(4); //unk2 = reader.ReadInt32();

            //string npcName = entityId.ToString();
            //if (EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == entityId) != null) 
            //{
            //    npcName = EntitiesManager.CurrentBosses.FirstOrDefault(x => x.EntityId == entityId).Name;
            //}
            //string target = targetId.ToString();
            //if (targetId == SessionManager.CurrentPlayer.EntityId) target = SessionManager.CurrentPlayer.Name;
            //System.Console.WriteLine("[S_NPC_STATUS] {0} > {1} ({2}) -- unk1:{3} unk2:{4}", npcName, target, IsEnraged, unk1, unk2);
        }
    }
}