using System;
using System.Collections.Concurrent;
using System.Threading;
using TCC.Utils;

namespace TCC.Debug
{
    public static class ObjectTracker
    {
        private static ConcurrentDictionary<Type, long> Instances { get; } = new ConcurrentDictionary<Type, long>();
        private static readonly object _lock = new object();
        public static void Register(Type t)
        {
            lock (_lock)
            {
                if (!Instances.ContainsKey(t)) Instances.TryAdd(t, 0);
                Instances[t]++;
            }
        }

        public static void Unregister(Type t)
        {
            lock (_lock)
            {
                if (!Instances.ContainsKey(t)) return;
                Instances[t]--;
            }
        }

        public static void DumpToFile()
        {
            var sb = "Instanced objects\n";
            foreach (var (type, count) in Instances)
            {
                var name = $"- {type.Name} ";
                while (name.Length < 35)
                {
                    name += "-";
                }

                name += $" {count}";
                sb += name;
                sb += "\n";
            }

            Log.F(sb, "objects.log");
        }
    }
}
