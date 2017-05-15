using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC
{
    public enum Class
    {
        Warrior = 0,
        Lancer = 1,
        Slayer = 2,
        Berserker = 3,
        Sorcerer = 4,
        Archer = 5,
        Priest = 6,
        Elementalist = 7,
        Soulless = 8,
        Engineer = 9,
        Fighter = 10,
        Assassin = 11,
        Glaiver = 12,
        Common = 255,
        None = 256
    }
    public enum Laurel
    {
        None = 0,
        Bronze = 1,
        Silver = 2,
        Gold = 3,
        Diamond = 4,
        Champion = 5
    }
    public enum CooldownType
    {
        Skill,
        Item
    }

    public enum AbnormalityType
    {
        WeakeningEffect = 1,
        DamageOverTime = 2,
        Stun = 3,
        Buff = 4
    }

    public enum Dragon
    {
        None,
        Aquadrax,
        Ignidrax,
        Umbradrax,
        Terradrax
    }

    public enum AggroCircle
    {
        Main = 2,
        Secondary = 3,
        None = 255 //arbitrary
    }
    public enum AggroAction
    {
        Add = 1,
        Remove = 2
    }

    public enum ReadyStatus
    {
        NotReady = 0,
        Ready = 1,
        Undefined = 255 //arbitrary
    }
}
