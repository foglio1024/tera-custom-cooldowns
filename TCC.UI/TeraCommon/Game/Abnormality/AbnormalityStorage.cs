using System.Collections.Generic;
using System.Linq;

namespace Tera.Game.Abnormality
{
    public class AbnormalityStorage
    {
        internal Dictionary<NpcEntity, Player> LastAggro;
        internal Dictionary<NpcEntity, Dictionary<HotDot, AbnormalityDuration>> NpcAbnormalityTime;
        internal Dictionary<Player, Dictionary<HotDot, AbnormalityDuration>> PlayerAbnormalityTime;
        internal Dictionary<Player, Dictionary<NpcEntity, Death>> PlayerAggro;
        internal Dictionary<Player, Death> PlayerDeath;

        public AbnormalityStorage()
        {
            NpcAbnormalityTime = new Dictionary<NpcEntity, Dictionary<HotDot, AbnormalityDuration>>();
            PlayerAbnormalityTime = new Dictionary<Player, Dictionary<HotDot, AbnormalityDuration>>();
            PlayerDeath = new Dictionary<Player, Death>();
            PlayerAggro = new Dictionary<Player, Dictionary<NpcEntity, Death>>();
            LastAggro = new Dictionary<NpcEntity, Player>();
        }

        public AbnormalityStorage(Dictionary<NpcEntity, Dictionary<HotDot, AbnormalityDuration>> npcTimes,
            Dictionary<Player, Dictionary<HotDot, AbnormalityDuration>> playerTimes,
            Dictionary<Player, Death> playerDeath, Dictionary<Player, Dictionary<NpcEntity, Death>> playerAggro)
        {
            NpcAbnormalityTime = npcTimes;
            PlayerAbnormalityTime = playerTimes;
            PlayerDeath = playerDeath;
            PlayerAggro = playerAggro;
            LastAggro = new Dictionary<NpcEntity, Player>();
        }

        internal Dictionary<HotDot, AbnormalityDuration> AbnormalityTime(NpcEntity entity)
        {
            if (!NpcAbnormalityTime.ContainsKey(entity))
                NpcAbnormalityTime.Add(entity, new Dictionary<HotDot, AbnormalityDuration>());
            return NpcAbnormalityTime[entity];
        }

        internal Dictionary<HotDot, AbnormalityDuration> AbnormalityTime(Player player)
        {
            if (!PlayerAbnormalityTime.ContainsKey(player))
                PlayerAbnormalityTime.Add(player, new Dictionary<HotDot, AbnormalityDuration>());
            return PlayerAbnormalityTime[player];
        }

        internal Death Death(Player player)
        {
            if (!PlayerDeath.ContainsKey(player))
                PlayerDeath.Add(player, new Death());
            return PlayerDeath[player];
        }

        internal Dictionary<NpcEntity, Death> Aggro(Player player)
        {
            if (!PlayerAggro.ContainsKey(player))
                PlayerAggro.Add(player, new Dictionary<NpcEntity, Death>());
            return PlayerAggro[player];
        }

        internal Player Last(NpcEntity entity)
        {
            Player result;
            LastAggro.TryGetValue(entity, out result);
            return result;
        }

        public Dictionary<HotDot, AbnormalityDuration> Get(NpcEntity entity)
        {
            if (entity == null) return new Dictionary<HotDot, AbnormalityDuration>();
            if (!NpcAbnormalityTime.ContainsKey(entity))
                return new Dictionary<HotDot, AbnormalityDuration>();
            return NpcAbnormalityTime[entity];
        }

        public PlayerAbnormals Get(Player player)
        {
            if (player == null) return new PlayerAbnormals();
            return new PlayerAbnormals(
                !PlayerAbnormalityTime.ContainsKey(player)
                    ? new Dictionary<HotDot, AbnormalityDuration>()
                    : PlayerAbnormalityTime[player],
                !PlayerDeath.ContainsKey(player) ? new Death() : PlayerDeath[player],
                !PlayerAggro.ContainsKey(player) ? new Dictionary<NpcEntity, Death>() : PlayerAggro[player]
                );
        }

        public AbnormalityStorage Clone(NpcEntity boss, long begin = 0, long end = 0)
        {
            Dictionary<NpcEntity, Dictionary<HotDot, AbnormalityDuration>> npcTimes;
            if (boss != null)
                npcTimes = NpcAbnormalityTime.Where(x => x.Key == boss)
                    .ToDictionary(y => y.Key, y => y.Value.ToDictionary(x => x.Key, x => x.Value.Clone(begin, end)));
            else npcTimes = NpcAbnormalityTime
                    .ToDictionary(y => y.Key, y => y.Value.ToDictionary(x => x.Key, x => x.Value.Clone(begin, end)));
            var playerTimes = PlayerAbnormalityTime.ToDictionary(y => y.Key,
                y => y.Value.ToDictionary(x => x.Key, x => x.Value.Clone(begin, end)));
            var playerDeath = PlayerDeath.ToDictionary(x => x.Key, x => x.Value.Clone(begin, end));
            Dictionary<Player, Dictionary<NpcEntity, Death>> playerAggro;
            if (boss != null)
                playerAggro = PlayerAggro.Where(x => x.Value.Keys.Contains(boss))
                    .ToDictionary(y => y.Key,
                        y => y.Value.Where(x => x.Key == boss).ToDictionary(x => x.Key, x => x.Value.Clone(begin, end)));
            else
                playerAggro = PlayerAggro
                    .ToDictionary(y => y.Key,
                        y => y.Value.ToDictionary(x => x.Key, x => x.Value.Clone(begin, end)));

            return new AbnormalityStorage(npcTimes, playerTimes, playerDeath, playerAggro);
        }

        public AbnormalityStorage Clone()
        {
            var npcTimes = NpcAbnormalityTime.ToDictionary(y => y.Key,
                y => y.Value.ToDictionary(x => x.Key, x => (AbnormalityDuration) x.Value.Clone()));
            var playerTimes = PlayerAbnormalityTime.ToDictionary(y => y.Key,
                y => y.Value.ToDictionary(x => x.Key, x => (AbnormalityDuration) x.Value.Clone()));
            var playerDeath = PlayerDeath.ToDictionary(x => x.Key, x => x.Value.Clone());
            var playerAggro = PlayerAggro.ToDictionary(y => y.Key,
                y => y.Value.ToDictionary(x => x.Key, x => x.Value.Clone()));
            return new AbnormalityStorage(npcTimes, playerTimes, playerDeath, playerAggro);
        }

        public void ClearEnded()
        {
            var npcTimes = new Dictionary<NpcEntity, Dictionary<HotDot, AbnormalityDuration>>();
            foreach (var i in NpcAbnormalityTime)
            {
                var j = i.Value.Where(x => !x.Value.Ended())
                    .ToDictionary(x => x.Key,
                        x => new AbnormalityDuration(x.Value.InitialPlayerClass, x.Value.LastStart(), x.Value.LastStack()));
                if (j.Count > 0)
                    npcTimes.Add(i.Key, j);
            }
            var playerTimes = new Dictionary<Player, Dictionary<HotDot, AbnormalityDuration>>();
            foreach (var i in PlayerAbnormalityTime)
            {
                var j = i.Value.Where(x => !x.Value.Ended())
                    .ToDictionary(x => x.Key,
                        x => new AbnormalityDuration(x.Value.InitialPlayerClass, x.Value.LastStart(), x.Value.LastStack()));
                if (j.Count > 0)
                    playerTimes.Add(i.Key, j);
            }
            var death = new Dictionary<Player, Death>();
            foreach (var i in PlayerDeath)
            {
                if (i.Value.Dead)
                    death.Add(i.Key, i.Value.Clear());
            }
            var aggro = new Dictionary<Player, Dictionary<NpcEntity, Death>>();
            foreach (var i in PlayerAggro)
            {
                var j = i.Value.Where(x => x.Value.Dead).ToDictionary(x => x.Key, x => x.Value.Clear());
                if (j.Count > 0)
                    aggro.Add(i.Key, j);
            }
            var last = LastAggro.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
            NpcAbnormalityTime = npcTimes;
            PlayerAbnormalityTime = playerTimes;
            PlayerDeath = death;
            PlayerAggro = aggro;
            LastAggro = last;
        }

        public void EndAll(long ticks)
        {
            foreach (var i in NpcAbnormalityTime)
            {
                foreach (var j in i.Value.Where(x => !x.Value.Ended()))
                {
                    j.Value.End(ticks);
                }
            }
            foreach (var i in PlayerAbnormalityTime)
            {
                foreach (var j in i.Value.Where(x => !x.Value.Ended()))
                {
                    j.Value.End(ticks);
                }
            }
            foreach (var j in PlayerDeath.Where(x => !x.Value.Dead))
            {
                j.Value.End(ticks);
            }
            foreach (var i in PlayerAggro)
            {
                foreach (var j in i.Value.Where(x => !x.Value.Dead))
                {
                    j.Value.End(ticks);
                }
            }
            LastAggro = new Dictionary<NpcEntity, Player>();
        }

        public void AggroStart(Player player, NpcEntity target, long start)
        {
            if (!PlayerAggro.ContainsKey(player))
                PlayerAggro.Add(player, new Dictionary<NpcEntity, Death>());
            if (!PlayerAggro[player].ContainsKey(target))
                PlayerAggro[player][target] = new Death();
            PlayerAggro[player][target].Start(start);
        }

        public void AggroEnd(Player player, NpcEntity target, long end)
        {
            if (PlayerAggro.ContainsKey(player))
                if (PlayerAggro[player].ContainsKey(target))
                    PlayerAggro[player][target].End(end);
        }

        public bool DeadOrJustResurrected(Player player)
        {
            if (!PlayerDeath.ContainsKey(player)) return false;
            return PlayerDeath[player].DeadOrJustResurrected;
        }

        public bool Dead(Player player)
        {
            if (!PlayerDeath.ContainsKey(player))return false;
            return PlayerDeath[player].Dead;
        }
    }
}