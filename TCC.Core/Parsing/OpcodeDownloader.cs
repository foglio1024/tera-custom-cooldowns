using System.IO;

namespace TCC.Parsing
{
    public class OpcodeDownloader
    {
        public static void DownloadIfNotExist(uint version, string directory)
        {
            DownloadOpcode(version, directory);
            DownloadSysmsg(version, directory);
        }

        private static void DownloadOpcode(uint version, string directory)
        {
            Directory.CreateDirectory(directory);

            var filename = directory + Path.DirectorySeparatorChar + version + ".txt";
            if (File.Exists(filename))
            {
                return;
            }
            filename = directory + Path.DirectorySeparatorChar + "protocol." + version + ".map";
            if (File.Exists(filename))
            {
                return;
            }
            try
            {
                Download("https://raw.githubusercontent.com/hackerman-caali/tera-data/master/map_base/protocol." + version + ".map", filename);
                return;
            }
            catch { }
            try
            {
                Download("https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/opcodes/protocol." + version + ".map", filename);
                return;
            }
            catch { }
            try
            {
                Download("https://raw.githubusercontent.com/meishuu/tera-data/master/map/protocol." + version + ".map", filename);
                return;
            }
            catch { }
        }

        public static bool DownloadSysmsg(uint version, string directory, int revision = 0)
        {
            Directory.CreateDirectory(directory);

            var filename = directory + Path.DirectorySeparatorChar + "smt_" + version + ".txt";
            if (File.Exists(filename))
            {
                return false;
            }
            filename = directory + Path.DirectorySeparatorChar + "sysmsg." + version + ".map";
            if (File.Exists(filename))
            {
                return false;
            }
            try
            {
                Download("https://raw.githubusercontent.com/hackerman-caali/tera-data/master/map_base/sysmsg." + version + ".map", filename);
                return true;
            }
            catch { }
            try
            {
                Download("https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/opcodes/sysmsg." + version + ".map", filename);
                return true;
            }
            catch { }
            try
            {
                Download("https://raw.githubusercontent.com/meishuu/tera-data/master/map/sysmsg." + version + ".map", filename);
                return true;
            }
            catch { }
            try
            {
                Download("https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/opcodes/sysmsg." + revision/100 + ".map", filename);
                return true;
            }
            catch { }
            try
            {
                Download("https://raw.githubusercontent.com/hackerman-caali/tera-data/master/map_base/sysmsg." + revision / 100 + ".map", filename);
                return true;
            }
            catch { }
            filename = directory + Path.DirectorySeparatorChar + "sysmsg." + revision / 100 + ".map";
            if (File.Exists(filename))
            {
                return false;
            }
            try
            {
                Download("https://raw.githubusercontent.com/meishuu/tera-data/master/map/sysmsg." + revision / 100 + ".map", filename);
                return true;
            }
            catch { }

            return false;
        }

        private static void Download(string remote, string local)
        {
            using (var client = new System.Net.WebClient())
            {
                client.Headers.Add(System.Net.HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
                client.DownloadFile(remote, local);
            }
        }
    }
}
