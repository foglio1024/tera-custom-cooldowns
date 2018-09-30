using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_NPC_STATUS : ParsedMessage
    {
        public ulong EntityId { get; }
        public ulong Target { get; }
        public bool IsEnraged { get; }

        public S_NPC_STATUS(TeraMessageReader reader) : base(reader)
        {
            EntityId = reader.ReadUInt64();
            IsEnraged = reader.ReadBoolean();
            reader.Skip(4); //unk1 = reader.ReadInt32();
            Target = reader.ReadUInt64();
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