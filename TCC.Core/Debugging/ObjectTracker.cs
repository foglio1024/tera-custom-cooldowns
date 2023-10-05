using System;
using System.Collections.Concurrent;
using TCC.Utils;

namespace TCC.Debugging;

public static class ObjectTracker
{
    static ConcurrentDictionary<Type, long> Instances { get; } = new();
    static readonly object Lock = new();
    public static void Register(Type t)
    {
        lock (Lock)
        {
            if (!Instances.ContainsKey(t)) Instances.TryAdd(t, 0);
            Instances[t]++;
        }
    }

    public static void Unregister(Type t)
    {
        lock (Lock)
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