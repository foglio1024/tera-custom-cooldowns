using System;

namespace TCC.TeraCommon.Game.Abnormality
{
    public class Duration : ICloneable
    {
        public Duration(long begin, long end, int stack)
        {
            End = end;
            Begin = begin;
            Stack = stack;
        }

        public long Begin { get; }
        public long End { get; set; }
        public int Stack { get; }

        public object Clone()
        {
            return new Duration(Begin, End, Stack);
        }

        public Duration Clone(long begin, long end)
        {
            return new Duration(Begin > begin ? Begin : begin, End < end ? End : end, Stack);
        }
    }
}