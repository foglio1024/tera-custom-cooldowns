using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tera.Game.Messages;

namespace Tera.Game
{
    public enum LangEnum
    {
        INT = 0,
        KR = 1,
        USA = 2,
        JPN = 3,
        GER = 4,
        FR = 5,
        EN = 6,
        TW = 7,
        RUS = 8,
        CHN = 9,
        THA = 10
    }
    public class ServerDatabase
    {
        private readonly List<Server> _servers;
        private List<Server> _serverlist;

        public ServerDatabase(string folder)
        {
            _serverlist = File.ReadAllLines(Path.Combine(folder, "servers.txt"))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Split(new[] { ' ' }, 4))
                .Select(
                    parts =>
                        new Server(parts[3], parts[1], parts[0],
                            !string.IsNullOrEmpty(parts[2]) ? uint.Parse(parts[2]) : uint.MaxValue)).ToList();
            _servers = _serverlist.Where(x => x.ServerId != uint.MaxValue).ToList();
            _serverlist.Add(new Server("VPN", "Unknown", "127.0.0.1"));
        }

        public string Region { get; set; }
        public LangEnum Language { get; set; }

        public string GetServerName(uint serverId, Server oldServer = null)
        {
            var servers = _servers.Where(x => x.ServerId == serverId).ToList();
            if (!servers.Any()) return oldServer?.Name ?? $"{serverId}";
            return servers.FirstOrDefault(x => x.Region == Language.ToString())?.Name ?? servers.First().Name;
        }

        public Dictionary<string, Server> GetServersByIp()
        {
            return _serverlist.GroupBy(x => x.Ip).ToDictionary(x => x.Key, x => x.First());
        }

        public void AddOverrides(IEnumerable<Server> newServers)
        {
            _serverlist = _serverlist.Concat(newServers.Where(sl => _serverlist.All(os => os.Ip != sl.Ip))).ToList();
        }

        public Server GetServer(uint serverId, Server oldServer = null)
        {
            var servers = _servers.Where(x => x.ServerId == serverId).ToList();
            if (!servers.Any()) return oldServer;
            return servers.FirstOrDefault(x => x.Region == Language.ToString()) ?? servers.First();
        }
    }
}