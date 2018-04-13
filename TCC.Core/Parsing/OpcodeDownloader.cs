using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Parsing
{
    public class OpcodeDownloader
    {
        public static void DownloadIfNotExist(uint version, String directory)
        {
            DownloadOpcode(version, directory);
            DownloadSysmsg(version, directory);
        }

        private static void DownloadOpcode(uint version, String directory)
        {
            Directory.CreateDirectory(directory);

            String filename = directory + Path.DirectorySeparatorChar + version + ".txt";
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
                Download("https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/opcodes/protocol." + version + ".map", filename);
                return;
            }
            catch { }
            try
            {
                Download("https://raw.githubusercontent.com/hackerman-caali/tera-data/master/map_base/protocol." + version + ".map", filename);
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

        private static void DownloadSysmsg(uint version, String directory)
        {
            Directory.CreateDirectory(directory);

            String filename = directory + Path.DirectorySeparatorChar + "smt_" + version + ".txt";
            if (File.Exists(filename))
            {
                return;
            }
            filename = directory + Path.DirectorySeparatorChar + "sysmsg." + version + ".map";
            if (File.Exists(filename))
            {
                return;
            }
            try
            {
                Download("https://raw.githubusercontent.com/neowutran/TeraDpsMeterData/master/opcodes/sysmsg." + version + ".map", filename);
                return;
            }
            catch { }
            try
            {
                Download("https://raw.githubusercontent.com/hackerman-caali/tera-data/master/map_base/sysmsg." + version + ".map", filename);
                return;
            }
            catch { }
            try
            {
                Download("https://raw.githubusercontent.com/meishuu/tera-data/master/map/sysmsg." + version + ".map", filename);
                return;
            }
            catch { }
        }

        private static void Download(String remote, String local)
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.Headers.Add(System.Net.HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
                client.DownloadFile(remote, local);
            }
        }
    }
}
