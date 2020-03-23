using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TeraPacketParser
{
    // Maps between numeric OpCodes and OpCode names
    // Since this mapping is version dependent, we can't use a sing global instance of this
    public class OpCodeNamer
    {
        private Dictionary<string, ushort> _opCodeCodes;
        private Dictionary<ushort, string> _opCodeNames;
        private readonly string _path;

        public OpCodeNamer(IEnumerable<KeyValuePair<ushort, string>> names)
        {
            var namesArray = names.ToArray();
            _opCodeNames = namesArray.ToDictionary(parts => parts.Key, parts => parts.Value);
            _opCodeCodes = namesArray.ToDictionary(parts => parts.Value, parts => parts.Key);
        }

        public OpCodeNamer(string filename)
            : this(ReadOpCodeFile(filename).Result)
        {
            _path = Path.GetDirectoryName(filename);
        }

        public string GetName(ushort opCode)
        {
            string name;
            if (_opCodeNames.TryGetValue(opCode, out name))
                return name;
            return opCode.ToString("X4");
        }

        private static async Task<IEnumerable<KeyValuePair<ushort, string>>> ReadOpCodeFile(string filename)
        {
            if (!File.Exists(filename))
            {
                filename = filename.Contains("smt_")
                    ? filename.Replace("smt_", "sysmsg.").Replace(".txt", ".map")
                    : Path.GetDirectoryName(filename) + "/protocol." + Path.GetFileName(filename).Replace(".txt", ".map");
            }

            if (!File.Exists(filename)) { return new List<KeyValuePair<ushort, string>>(); }

            await Nostrum.MiscUtils.WaitForFileUnlock(filename, FileAccess.Read);

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

        public void Reload(uint version, int releaseVersion)
        {
            var filename = _path + "/sysmsg." + version + ".map";
            if (!File.Exists(filename)) filename = _path + "/sysmsg." + releaseVersion / 100 + ".map";
            if (!File.Exists(filename)) return;
            var namesArray = ReadOpCodeFile(filename).Result.ToArray();
            _opCodeNames = namesArray.ToDictionary(parts => parts.Key, parts => parts.Value);
            _opCodeCodes = namesArray.ToDictionary(parts => parts.Value, parts => parts.Key);
        }
    }
}