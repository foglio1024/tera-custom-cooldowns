using System;
using System.Linq;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_NOTIFY_GUILD_QUEST_URGENT : ParsedMessage
    {
        public uint ZoneId { get; }
        public uint TemplateId { get; }
        public string Quest { get; }

        public uint QuestId
        {
            get
            {
                var strId = Quest.Substring(Quest.IndexOf(":")+1);
                return Convert.ToUInt32(strId);
            }
        }
        public S_NOTIFY_GUILD_QUEST_URGENT(TeraMessageReader reader) : base(reader)
        {
            reader.BaseStream.Position = 0;
            var questOffset = reader.ReadInt16();
            reader.Skip(4);
            ZoneId = reader.ReadUInt32();
            TemplateId = reader.ReadUInt32();
            reader.BaseStream.Position = questOffset - 4;
            Quest = reader.ReadTeraString();
        }
    }
}
