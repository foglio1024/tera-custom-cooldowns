using System;

namespace TeraPacketParser.Data
{
    // ReSharper disable once InconsistentNaming
    public struct Vector3f
    {
        public float X;
        public float Y;
        public float Z;

        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }

        public float DistanceTo(Vector3f target)
        {
            double a = target.X - X;
            double b = target.Y - Y;
            double c = target.Z - Z;
            return (float) Math.Sqrt(a*a + b*b + c*c);
        }

        public Vector3f MoveForvard(Vector3f target, int speed, long time)
        {
            if (time == 0 || speed == 0) return this;
            var toGo = (float) speed*time/TimeSpan.TicksPerSecond;
            var distance = DistanceTo(target);
            if (toGo >= distance || distance == 0) return target;
            Vector3f result;
            result.X = X + (target.X - X)*toGo/distance;
            result.Y = Y + (target.Y - Y)*toGo/distance;
            result.Z = Z + (target.Z - Z)*toGo/distance;
            return result;
        }

        public Angle GetHeading(Vector3f target)
        {
            return new((short) (Math.Atan2(target.Y - Y, target.X - X)*0x8000/Math.PI));
        }
    }
}