using System;
using System.Collections.Generic;
using TCC.TeraCommon.Game.Messages;
using TCC.TeraCommon.Game.Services;

namespace TCC.Parsing.Messages
{
    public class S_REQUEST_CITY_WAR_MAP_INFO_DETAIL : ParsedMessage
    {
        public List<Tuple<uint, string>> GuildDetails;

        public S_REQUEST_CITY_WAR_MAP_INFO_DETAIL(TeraMessageReader reader) : base(reader)
        {
            GuildDetails = new List<Tuple<uint, string>>();
            try
            {
                var count = reader.ReadUInt16();
                if (count == 0) return;
                var offset = reader.ReadUInt16();
                reader.BaseStream.Position = offset - 4;
                for (int i = 0; i < count; i++)
                {
                    var current = reader.ReadUInt16();
                    var next = reader.ReadUInt16();
                    reader.Skip(6);
                    var id = reader.ReadUInt32();
                    var name = reader.ReadTeraString();
                    var gm = reader.ReadTeraString();
                    var logo = reader.ReadTeraString();
                    //TODO: use gm and logo?
                    GuildDetails.Add(new Tuple<uint, string>(id, name));
                    if (next != 0) reader.BaseStream.Position = next - 4;
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}