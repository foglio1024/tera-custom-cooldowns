using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tera.Game
{
    public class ServerDatabase
    {
        private readonly Dictionary<uint, Server> _servers;
        private List<Server> _serverlist;

        public ServerDatabase(string folder)
        {
            _serverlist = File.ReadAllLines(Path.Combine(folder, "servers.txt"))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Split(new[] {' '}, 4))
                .Select(
                    parts =>
                        new Server(parts[3], parts[1], parts[0],
                            !string.IsNullOrEmpty(parts[2]) ? uint.Parse(parts[2]) : uint.MaxValue)).ToList();
            _servers = _serverlist.Where(x => x.ServerId != uint.MaxValue).ToDictionary(x => x.ServerId);
            _serverlist.Add(new Server("VPN", "Unknown", "127.0.0.1"));
        }

        public string Region { get; set; }

        public string GetServerName(uint serverId, Server oldServer = null)
        {
            return _servers.ContainsKey(serverId) ? _servers[serverId].Name : oldServer?.Name ?? $"{serverId}";
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
            return _servers.ContainsKey(serverId) ? _servers[serverId] : oldServer;
        }
    }
}