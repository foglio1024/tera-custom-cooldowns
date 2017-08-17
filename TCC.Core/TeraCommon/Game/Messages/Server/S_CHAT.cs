using System;

namespace Tera.Game.Messages
{
    public class S_CHAT : ParsedMessage
    {
        internal S_CHAT(TeraMessageReader reader) : base(reader)
        {
            reader.Skip(4);//offsets
            var channel = reader.ReadUInt32();
            Channel = (ChannelEnum)channel;
            reader.Skip(11);
            Username = reader.ReadTeraString();
            Text = reader.ReadTeraString();
        }

        public string Username { get; set; }

        public string Text { get; set; }

        public ChannelEnum Channel { get; set; }

        public enum ChannelEnum
        {
            Guild = 2,
            General = 27,
            Say = 0,
            Greetings = 9,
            Trading = 4,
            Emotes = 26,
            Alliance = 28,
            Area = 3,
            Group = 1,
            Raid = 32

        }
    }
}