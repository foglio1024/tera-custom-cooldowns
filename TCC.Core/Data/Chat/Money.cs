﻿using System;
using Nostrum.WPF.ThreadSafe;
using TCC.ViewModels;

namespace TCC.Data.Chat;

public class Money : ThreadSafeObservableObject
{
    public long Gold { get; set; }
    public long Silver { get; set; }
    public long Copper { get; set; }

    private Money()
    {
        Dispatcher = ChatManager.Instance.Dispatcher;
    }

    public Money(long money) : this()
    {
        Gold = Convert.ToInt64(money / 10000);
        Silver = Convert.ToInt64(money / 100) - Gold * 100;
        Copper = Convert.ToInt64(money / 1) - Silver * 100 - Gold * 10000;
    }

    public Money(int g, int s, int c) : this()
    {
        Gold = g;
        Silver = s;
        Copper = c;
    }

    public Money(string money) : this() // this didn't set dispatcher, keep this()?
    {
        long gold = 0;
        long silver = 0;
        long copper;
        switch (money.Length)
        {
            case >= 5:
                copper = long.Parse(money[^2..]);
                silver = long.Parse(money.Substring(money.Length - 4, 2));
                gold = long.Parse(money[..^4]);
                break;

            case >= 3 and < 5:
                copper = long.Parse(money[^2..]);
                silver = long.Parse(money[..^2]);
                break;

            default:
                copper = long.Parse(money);
                break;
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