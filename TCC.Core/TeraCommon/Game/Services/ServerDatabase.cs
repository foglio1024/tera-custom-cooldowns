using System.Collections.Generic;
using System.IO;
using System.Linq;
using TCC.Data;

namespace TCC.TeraCommon.Game.Services
{
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
        private LangEnum _language;
        public LangEnum Language
        {
            get => _language;
            set
            {
                _language = value;
                switch (_language)
                {
                    case LangEnum.EN:
                    case LangEnum.GER:
                    case LangEnum.FR:
                        Region = "EU"; break;
                    case LangEnum.THA:
                    case LangEnum.SE:
                        Region = "THA"; break;
                    default:
                        Region = _language.ToString(); break;
                }
            }
        }
        public string GetServerName(uint serverId, Server oldServer = null)
        {
            var servers = _servers.Where(x => x.ServerId == serverId).ToList();
            if (!servers.Any()) return oldServer?.Name ?? $"{serverId}";
            return servers.FirstOrDefault(x => x.Region == Region)?.Name ?? servers.First().Name;
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
            return servers.FirstOrDefault(x => x.Region == Region) ?? servers.First();
        }
    }
}