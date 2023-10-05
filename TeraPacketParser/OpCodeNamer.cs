using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nostrum;

namespace TeraPacketParser;

// Maps between numeric OpCodes and OpCode names
// Since this mapping is version dependent, we can't use a sing global instance of this
public class OpCodeNamer
{
    Dictionary<string, ushort> _opCodeCodes;
    Dictionary<ushort, string> _opCodeNames;

    public OpCodeNamer(IEnumerable<KeyValuePair<ushort, string>> names)
    {
        var namesArray = names.ToArray();
        _opCodeNames = namesArray.ToDictionary(parts => parts.Key, parts => parts.Value);
        _opCodeCodes = namesArray.ToDictionary(parts => parts.Value, parts => parts.Key);
    }

    public OpCodeNamer(string filename) : this(ReadOpCodeFile(filename).Result)
    {
    }

    public string GetName(ushort opCode)
    {
        return _opCodeNames.TryGetValue(opCode, out var name) ? name : opCode.ToString("X4");
    }

    static async Task<IEnumerable<KeyValuePair<ushort, string>>> ReadOpCodeFile(string filename)
    {
        if (!File.Exists(filename))
        {
            filename = filename.Contains("smt_")
                ? filename.Replace("smt_", "sysmsg.").Replace(".txt", ".map")
                : Path.GetDirectoryName(filename) + "/protocol." + Path.GetFileName(filename).Replace(".txt", ".map");
        }

        if (!File.Exists(filename)) { return new List<KeyValuePair<ushort, string>>(); }

        await MiscUtils.WaitForFileUnlock(filename, FileAccess.Read);

        var names = File.ReadLines(filename)
            .Select(s => Regex.Replace(s.Replace("=", " "), @"\s+", " ").Split(' ').ToArray())
            .Select(parts => new KeyValuePair<ushort, string>(ushort.Parse(parts[1]), parts[0]));
        return names;
    }

    public ushort GetCode(string name)
    {
        ushort code;
        if (_opCodeCodes.TryGetValue(name, out code))
            return code;
        Debug.WriteLine("Missing opcode: " + name);
        return 0;
        //throw new ArgumentException($"Unknown name '{name}'");
    }

    public void Reload(uint version, int releaseVersion, string path)
    {
        var p = Path.GetDirectoryName(path) ?? "";
        var filename = p + "/sysmsg." + version + ".map";
        if (!File.Exists(filename)) filename = p + "/sysmsg." + releaseVersion / 100 + ".map";
        if (!File.Exists(filename)) return;
        var namesArray = ReadOpCodeFile(filename).Result.ToArray();
        _opCodeNames = namesArray.ToDictionary(parts => parts.Key, parts => parts.Value);
        _opCodeCodes = namesArray.ToDictionary(parts => parts.Value, parts => parts.Key);
    }

    public void Add(string name, ushort code)
    {
        _opCodeCodes[name] = code;
        _opCodeNames[code] = name;
    }
}