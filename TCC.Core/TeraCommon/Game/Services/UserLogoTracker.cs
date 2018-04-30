using System.Collections.Generic;
using System.Drawing;
using TCC.TeraCommon.Game.Messages.Server;

namespace TCC.TeraCommon.Game.Services
{
    public class UserLogoTracker
    {
        private Dictionary<uint, uint> _playerGuilds = new Dictionary<uint, uint>();
        private readonly Dictionary<uint, Bitmap> _guildLogos = new Dictionary<uint, Bitmap>();
        public void SetUserList(S_GET_USER_LIST message)
        {
            _playerGuilds = message.PlayerGuilds;
        }

        public void AddLogo(S_GET_USER_GUILD_LOGO message)
        {
            _guildLogos[message.GuildId] = message.GuildLogo;
        }

        public Bitmap GetLogo(uint playerId)
        {
            uint guildId;
            _playerGuilds.TryGetValue(playerId, out guildId);
            Bitmap result;
            _guildLogos.TryGetValue(guildId, out result);
            return result;
        }
    }
}
