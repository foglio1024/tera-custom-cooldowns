
 using System;
 using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game.Messages.Server
{
    public class S_BEGIN_THROUGH_ARBITER_CONTRACT : ParsedMessage
    {
        internal S_BEGIN_THROUGH_ARBITER_CONTRACT(TeraMessageReader reader)
            : base(reader)
        {
            //reader.Skip(18);
            reader.Skip(8);
            var type = reader.ReadByte();
            if(type == 0x23)
            {
                Console.Beep();
                Console.WriteLine("ERROR TYPE 23:");
                PrintRaw();
            }
            reader.Skip(9);
            InviteName = reader.ReadTeraString();
            try
            {
                PlayerName = reader.ReadTeraString();
            }
            catch
            {
                PlayerName = "Error parsing S_BEGIN_THROUGH_ARBITER_CONTRACT: " + BitConverter.ToString(Raw);
            }

            //Debug.WriteLine("InviteName:" + InviteName + " PlayerName:" + PlayerName);
        }

        public string InviteName { get; set; }
        public string PlayerName { get; set; }
    }
}
