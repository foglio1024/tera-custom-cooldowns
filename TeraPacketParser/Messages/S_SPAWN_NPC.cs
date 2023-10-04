


namespace TeraPacketParser.Messages;

public class S_SPAWN_NPC : ParsedMessage
{
    public GameId GameId { get; }
    public ulong EntityId { get; }
    public uint TemplateId { get; }
    public ushort HuntingZoneId { get; }
    public bool Villager { get; }
    public int RemainingEnrageTime { get; }
    public short Status { get; }
    public long MaxHP { get; set; }
    public long EnrageThreshold { get; set; }
    public int Level { get; set; }
    public bool IsEnraged { get; set; }

    public S_SPAWN_NPC(TeraMessageReader reader) : base(reader)
    {
        reader.Skip(10); // seatsRef [4] + partsRef[4] + npcNameRef [2]
        EntityId = reader.ReadUInt64();
        GameId = GameId.Parse(EntityId);
        reader.Skip(8); // target
        if (reader.Factory.ReleaseVersion / 100 >= 101)
        {
            Level = reader.ReadInt32();
            MaxHP = reader.ReadInt64();
            EnrageThreshold = reader.ReadInt64();
        }
        reader.Skip(12); //var loc = reader.ReadVector3f();
        reader.Skip(2); //var angle = reader.ReadInt16();
        reader.Skip(4); //var relation = reader.ReadInt32();
        TemplateId = reader.ReadUInt32();
        HuntingZoneId = reader.ReadUInt16();
        reader.Skip(4  // shapeID
                    + 2 + 2  // walkSpeed + runSpeed
        );
        Status = reader.ReadInt16(); // status
        IsEnraged = reader.ReadInt16() == 1; //var enrage = reader.ReadUInt16(); // 0/1
        RemainingEnrageTime = reader.ReadInt32();
        reader.Skip(2  // hpLevel  
                    + 2  // questInfo
                    + 1);  // visible
        Villager = reader.ReadBoolean();
        //reader.Skip(4);
        //reader.Skip(4+8+4+4);
        //var aggressive = reader.ReadBoolean();

        //Console.WriteLine("[S_SPAWN NPC] id:{0} tId:{1} hzId:{2}", id, templateId, huntingZoneId);
    }

}