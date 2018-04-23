using System;
using TCC.ViewModels;

namespace TCC.Data
{
    public class Money : TSPropertyChanged
    {
        public long Gold { get; set; }
        public long Silver { get; set; }
        public long Copper { get; set; }

        public Money(long money)
        {
            _dispatcher = ChatWindowManager.Instance.GetDispatcher();

            Gold = Convert.ToInt64(money / 10000);
            Silver = Convert.ToInt64(money / 100) - Gold * 100;
            Copper = Convert.ToInt64(money / 1) - Silver * 100 - Gold * 10000;
        }
        public Money(int g, int s, int c)
        {
            _dispatcher = ChatWindowManager.Instance.GetDispatcher();

            Gold = g;
            Silver = s;
            Copper = c;
        }
        public Money(string money)
        {
            long gold = 0;
            long silver = 0;
            long copper = 0;
            if (money.Length >= 5)
            {
                copper = long.Parse(money.Substring(money.Length - 2));
                silver = long.Parse(money.Substring(money.Length - 4, 2));
                gold = long.Parse(money.Substring(0, money.Length - 4));
            }
            else if (money.Length >= 3 && money.Length < 5)
            {
                copper = long.Parse(money.Substring(money.Length - 2));
                silver = long.Parse(money.Substring(0, money.Length - 2));
            }
            else
            {
                copper = long.Parse(money);
            }
            Gold = gold;
            Silver = silver;
            Copper = copper;
        }

        public override string ToString()
        {
            var goldString = Gold != 0 ? Gold.ToString() + "g " : "";
            var silverString = Silver != 0 ? Silver.ToString() + "s " : "";
            var copperString = Copper != 0 ? Copper.ToString() + "c " : "";

            return String.Format("{0}{1}{2}", goldString, silverString, copperString);
        }
    }
}
