using System.IO;
using Nostrum;

namespace TCC.Update
{
    public static class OpcodeDownloader
    {
        public static bool DownloadOpcodesIfNotExist(uint version, string directory)
        {
            return DownloadOpcode(version, directory);
        }
        public static bool DownloadSysmsgIfNotExist(uint version, string directory, int revision = 0)
        {
            return DownloadSysmsg(version, directory, revision);
        }
        private static bool IsFileValid(string filename)
        {
            return File.Exists(filename);
            //if (!File.Exists(filename)) return false;
            //if (!App.Settings.CheckOpcodesHash) return true;
            //var localHash = HashUtils.GenerateFileHash(filename);
            //if (localHash == "")
            //{
            //    WindowManager.ViewModels.NotificationArea.Enqueue("TCC", "Failed to check opcode file hash.\n Skipping download...", Data.NotificationType.Warning);
            //    return true;
            //}
            //using (var c = MiscUtils.GetDefaultWebClient())
            //{
            //    try
            //    {
            //        var st = c.OpenRead("https://raw.githubusercontent.com/caali-hackerman/tera-data/master/mappings.json");
            //        if (st != null)
            //        {
            //            var sr = new StreamReader(st);
            //            var sMappings = sr.ReadToEnd();
            //            var jMappings = JObject.Parse(sMappings);
            //            var reg = Game.Server.Region;
            //            var jReg = jMappings[reg];
            //            var remoteHash = jReg["protocol_hash"].Value<string>();
            //            if (localHash == remoteHash) return true;
            //        }
            //    }
            //    catch
            //    {
            //        return false;
            //    }
            //}

            //return false;
        }
        private static bool DownloadOpcode(uint version, string directory)
        {
            Directory.CreateDirectory(directory);
            var ret = false;
            var filename = Path.Combine(directory, $"protocol.{version}.map");
            if (IsFileValid(filename)) return true;
            try
            {
                Download($"https://raw.githubusercontent.com/tera-toolbox/tera-data/master/map_base/protocol.{version}.map", filename);
                ret = IsFileValid(filename);
            }
            catch { /* ignored*/ }

            if (!ret)
            {
                try
                {
                    Download($"https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/opcodes/protocol.{version}.map", filename);
                    ret = IsFileValid(filename);
                }
                catch { /* ignored*/ }
            }

            return ret;
        }

        public static bool DownloadSysmsg(uint version, string directory, int revision = 0)
        {
            Directory.CreateDirectory(directory);

            var filename = Path.Combine(directory, $"sysmsg.{revision / 100}.map");
            if (File.Exists(filename)) return true;
            else
            {
                try
                {
                    Download($"https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/opcodes/sysmsg.{revision / 100}.map", filename);
                    return true;
                }
                catch { /* ignored*/ }
            }
            filename = Path.Combine(directory, $"sysmsg.{version}.map");
            if (File.Exists(filename)) return true;
            else
            {
                try
                {
                    Download($"https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/opcodes/sysmsg.{version}.map", filename);
                    return true;
                }
                catch { /* ignored*/ }
            }
            return false;
        }

        private static void Download(string remote, string local)
        {
            using var client = MiscUtils.GetDefaultWebClient();
            client.DownloadFile(remote, local);
        }
    }
}
