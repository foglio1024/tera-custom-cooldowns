using System;

namespace TCC.Data
{
    public class Money : TSPropertyChanged
    {
        public int Gold { get; set; }
        public int Silver { get; set; }
        public int Copper { get; set; }

        public Money(int g, int s, int c)
        {
            _dispatcher = WindowManager.ChatWindow.Dispatcher;

            Gold = g;
            Silver = s;
            Copper = c;
        }
        public Money(string money)
        {
            int gold = 0;
            int silver = 0;
            int copper = 0;
            if (money.Length >= 5)
            {
                copper = int.Parse(money.Substring(money.Length - 2));
                silver = int.Parse(money.Substring(money.Length - 4, 2));
                gold = int.Parse(money.Substring(0, money.Length - 4));
            }
            else if (money.Length >= 3 && money.Length < 5)
            {
                copper = int.Parse(money.Substring(money.Length - 2));
                silver = int.Parse(money.Substring(0, money.Length - 2));
            }
            else
            {
                copper = int.Parse(money);
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
