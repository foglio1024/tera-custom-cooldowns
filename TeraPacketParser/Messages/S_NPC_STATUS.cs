


namespace TeraPacketParser.Messages
{
    public class S_NPC_STATUS : ParsedMessage
    {
        public ulong EntityId { get; }
        public ulong Target { get; }
        public bool IsEnraged { get; }
        public int RemainingEnrageTime { get; }

        public S_NPC_STATUS(TeraMessageReader reader) : base(reader)
        {
            EntityId = reader.ReadUInt64();
            IsEnraged = reader.ReadBoolean();

            var et = reader.ReadInt32();
            RemainingEnrageTime = et < 0 ? 0 : et;

            reader.Skip(4); // hpLevel
            Target = reader.ReadUInt64();
            reader.Skip(4); // status

            //string npcName = entityId.ToString();
            //if (EntityManager.CurrentBosses.FirstOrDefault(x => x.EntityId == entityId) != null) 
            //{
            //    npcName = EntityManager.CurrentBosses.FirstOrDefault(x => x.EntityId == entityId).Name;
            //}
            //string target = targetId.ToString();
            //if (targetId == SessionManager.CurrentPlayer.EntityId) target = SessionManager.CurrentPlayer.Name;
            //System.Console.WriteLine("[S_NPC_STATUS] {0} > {1} ({2}) -- unk1:{3} unk2:{4}", npcName, target, IsEnraged, unk1, unk2);
        }
    }
}