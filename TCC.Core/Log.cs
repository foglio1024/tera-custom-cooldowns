using System;

namespace TCC
{
    public static class Log
    {
        public static void CW(string line)
        {
#if DEBUG
            Console.WriteLine(line);
#endif
        }
    }
}
