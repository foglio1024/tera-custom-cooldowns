using System;
using Nostrum;
using Nostrum.WPF.ThreadSafe;
using TCC.ViewModels;

namespace TCC.Data.Chat
{
    public class Money : ThreadSafeObservableObject
    {
        public long Gold { get; set; }
        public long Silver { get; set; }
        public long Copper { get; set; }

        public Money(long money)
        {
            Dispatcher = ChatManager.Instance.Dispatcher;

            Gold = Convert.ToInt64(money / 10000);
            Silver = Convert.ToInt64(money / 100) - Gold * 100;
            Copper = Convert.ToInt64(money / 1) - Silver * 100 - Gold * 10000;
        }
        public Money(int g, int s, int c)
        {
            Dispatcher = ChatManager.Instance.Dispatcher;

            Gold = g;
            Silver = s;
            Copper = c;
        }
        public Money(string money)
        {
            long gold = 0;
            long silver = 0;
            long copper;
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
            var goldString = Gold != 0 ? Gold + "g " : "";
            var silverString = Silver != 0 ? Silver + "s " : "";
            var copperString = Copper != 0 ? Copper + "c" : "";

            return $"{goldString}{silverString}{copperString}";
        }
    }
}
