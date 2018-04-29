using System.Diagnostics;
namespace Tera.Game.Messages
{
    public class S_UPDATE_NPCGUILD : ParsedMessage
    {
        public enum NpcGuildType
        {
            Vanguard = 609,
            Bellicarium = 901,
            KillingSpree = 902,
            Bellicarium7 = 90101,
            KillingSpree7 = 90201
        } // todo: not sure, may be it worth adding StrSheet_NPCGuild.xml parser to use region-specific names

        internal S_UPDATE_NPCGUILD(TeraMessageReader reader) : base(reader)
        {
            User=reader.ReadEntityId();
            reader.Skip(8);
            var type = reader.ReadInt32();
            Type = (NpcGuildType)type;
            reader.Skip(8);
            Credits = reader.ReadInt32();
            Debug.WriteLine("type:"+type+";translated:"+Type + "; Credits:"+Credits);
        }

        public EntityId User { get; private set; }
        public int Credits { get; private set; }
        public NpcGuildType Type { get; private set; }
    }
}

