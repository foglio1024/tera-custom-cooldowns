using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Game.Messages
{
    public class S_TRADE_BROKER_DEAL_SUGGESTED : ParsedMessage
    {
        internal S_TRADE_BROKER_DEAL_SUGGESTED(TeraMessageReader reader) : base(reader)
        {
            //PrintRaw();

            Unknown = reader.ReadInt64();
            reader.Skip(14);

            SellerPrice = reader.ReadInt64();
            OfferedPrice = reader.ReadInt64();
            PlayerName = reader.ReadTeraString();
            
            //Console.WriteLine("PlayerName:" + PlayerName + ";SellerPrice:" + SellerPrice + ";OfferedPrice:" + OfferedPrice);
        }

        public long Unknown { get; set; }

        public string PlayerName { get; set; }
        public long SellerPrice { get; set; }
        public long OfferedPrice { get; set; }

        public static int Gold(long price)
        {
            return (int)(price/10000);
        }


        public static int Silver(long price)
        {
            return (int)(price % 10000 / 100);
        }

        public static int Bronze(long price)
        {
            return (int)(price % 100);
        }
    }
}
