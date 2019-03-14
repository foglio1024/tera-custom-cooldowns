using System;
using TCC.TeraCommon.Game.Services;

namespace TCC.TeraCommon.Game
{
    public class Player : IEquatable<object>
    {
        private UserEntity _user;

        public Player(UserEntity user, ServerDatabase serverdatabase)
        {
            _user = user;
            FullName = $"{serverdatabase?.GetServerName(user.ServerId) ?? user.ServerId.ToString()} : {user.Name}";
        }

        public uint PlayerId => User.PlayerId;
        public uint ServerId => User.ServerId;

        public string Name => User.Name;
        public string FullName { get; private set; }
        public bool IsHealer => Class == PlayerClass.Priest || Class == PlayerClass.Mystic;
        public int Level => User.Level;
        public string GuildName => User.GuildName;

        public RaceGenderClass RaceGenderClass => User.RaceGenderClass;

        public PlayerClass Class => RaceGenderClass.Class;

        public UserEntity User
        {
            get => _user;
            set
            {
                if (_user.ServerId != value.ServerId || _user.PlayerId != value.PlayerId)
                    throw new ArgumentException("Users must represent the same Player");
                _user = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Player) obj);
        }

        public bool Equals(Player other)
        {
            return ServerId.Equals(other.ServerId) && PlayerId.Equals(other.PlayerId);
        }

        public static bool operator ==(Player a, Player b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if ((object) a == null || (object) b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Player a, Player b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return ServerId.GetHashCode() ^ PlayerId.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Class} {Name} [{GuildName}]";
        }
    }
}