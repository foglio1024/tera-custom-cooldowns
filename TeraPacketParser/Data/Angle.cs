using System;

namespace TeraPacketParser.Data
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
            var side = diff > 180 ? Data.HitDirection.Left : Data.HitDirection.Right;
            if (diff > 180) diff = 360 - diff;
            if (diff <= 55) return Data.HitDirection.Back;
            if (diff >= 125) return Data.HitDirection.Front;
            return Data.HitDirection.Side|side;
        }
    }
}