using System;
using TCC.Data;
using TCC.ViewModels;

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

        public static void C(string line)
        {
#if DEBUG
            ChatWindowManager.Instance.AddTccMessage(line);
#endif
        }

        public static void All(string s)
        {
            CW(s);
            C(s);
        }
    }
}
