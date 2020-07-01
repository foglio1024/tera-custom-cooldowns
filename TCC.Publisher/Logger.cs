using System;

namespace TCC.Publisher
{
    public static class Logger
    {
        public static event Action<string> NewLine = null!;
        public static event Action<string> AppendedLine = null!;

        public static void WriteLine(string msg)
        {
            Console.WriteLine(msg);
            NewLine?.Invoke(msg);
        }
        public static void Write(string msg)
        {
            Console.Write(msg);
            AppendedLine?.Invoke(msg);

        }
    }
}