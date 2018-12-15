using System.IO;
using System.Net;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

namespace TCC.Parsing
{
    public static class OpcodeDownloader
    {
        public static void DownloadIfNotExist(uint version, string directory)
        {
            DownloadOpcode(version, directory);
            DownloadSysmsg(version, directory);
        }

        private static bool IsFileValid(string filename, uint version)
        {
            if (!File.Exists(filename)) return false;
            if (!Settings.SettingsHolder.CheckOpcodesHash) return true;
            var file = File.Open(filename, FileMode.Open);
            var fileBuffer = new byte[file.Length];
            file.Read(fileBuffer, 0, (int)file.Length);
            file.Close();
            var localHash = SHA256.Create().ComputeHash(fileBuffer);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var c = new WebClient())
            {
                c.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");

                try
                {
                    var st = c.OpenRead("https://raw.githubusercontent.com/caali-hackerman/tera-data/master/mappings.json");
                    if (st != null)
                    {
                        var sr = new StreamReader(st);
                        var sMappings = sr.ReadToEnd();
                        var jMappings = JObject.Parse(sMappings);
                        var reg = SessionManager.Server.Region;
                        var jReg = jMappings[reg];
                        var remoteHash = jReg["protocol_hash"].Value<string>();
                        if (StringUtils.ByteArrayToString(localHash) == remoteHash) return true;
                    }
                }
                catch 
                {
                    return false;
                }
            }

            return false;
        }
        private static void DownloadOpcode(uint version, string directory)
        {
            Directory.CreateDirectory(directory);

            var filename = directory + Path.DirectorySeparatorChar + version + ".txt";
            if (IsFileValid(filename, version)) return;
            filename = directory + Path.DirectorySeparatorChar + "protocol." + version + ".map";
            if (IsFileValid(filename, version)) return;
            try
            {
                Download("https://raw.githubusercontent.com/caali-hackerman/tera-data/master/map_base/protocol." + version + ".map", filename);
                return;
            }
            catch { /* ignored*/ }
            try
            {
                Download("https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/opcodes/protocol." + version + ".map", filename);
            }
            catch { /* ignored*/ }
        }

        public static bool DownloadSysmsg(uint version, string directory, int revision = 0)
        {
            Directory.CreateDirectory(directory);

            var filename = directory + Path.DirectorySeparatorChar + "smt_" + version + ".txt";
            if (File.Exists(filename)) return false;
            filename = directory + Path.DirectorySeparatorChar + "sysmsg." + revision/100 + ".map";
            if (File.Exists(filename)) return false;
            try
            {
                Download("https://raw.githubusercontent.com/caali-hackerman/tera-data/master/map_base/sysmsg." + revision / 100 + ".map", filename);
                return true;
            }
            catch { /* ignored*/ }
            try
            {
                Download("https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/opcodes/sysmsg." + revision / 100 + ".map", filename);
                return true;
            }
            catch { /* ignored*/ }
            return false;
        }

        private static void Download(string remote, string local)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
                client.DownloadFile(remote, local);
            }
        }
    }
}
