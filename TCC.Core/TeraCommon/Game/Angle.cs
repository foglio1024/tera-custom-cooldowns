using System;

namespace TCC.TeraCommon.Game
{
    public struct Angle
    {
        private readonly short _raw;

        public Angle(short raw)
            : this()
        {
            _raw = raw;
        }

        public double Radians => _raw*(2*Math.PI/0x10000);
        public int Gradus => _raw*360/0x10000;

        public override string ToString()
        {
            return $"{Gradus}";
        }

        public HitDirection HitDirection(Angle target)
        {
            var diff = (target.Gradus - Gradus + 720)%360;
            var side = diff > 180 ? Game.HitDirection.Left : Game.HitDirection.Right;
            if (diff > 180) diff = 360 - diff;
            if (diff <= 55) return Game.HitDirection.Back;
            if (diff >= 125) return Game.HitDirection.Front;
            return Game.HitDirection.Side|side;
        }
    }

    /*
        *  The enum value NEED to be set manually
        *  without that, converting the enum to int will cause massive weird bug, like:
        *  https://github.com/neowutran/ShinraMeter/issues/184
        * */
    [Flags]
    public enum HitDirection
    {
        Back = 1,
        Left = 2,
        Right = 4,
        Side = 8,
        Front = 16,
        Dot = 32,
        Pet = 64
    }
}