using System;
using System.Collections.Generic;
using System.Linq;
using Tera.Game.Abnormality;
using Tera.Game.Messages;

namespace Tera.Game
{
    public class AbnormalityTracker
    {
        private Dictionary<EntityId, List<Abnormality.Abnormality>> _abnormalities = new Dictionary<EntityId, List<Abnormality.Abnormality>>();

        internal AbnormalityStorage AbnormalityStorage;
        internal EntityTracker EntityTracker;
        internal HotDotDatabase HotDotDatabase;
        internal PlayerTracker PlayerTracker;
        public Action<SkillResult> UpdateDamageTracker;

        public AbnormalityTracker(EntityTracker entityTracker, PlayerTracker playerTracker,
            HotDotDatabase hotDotDatabase, AbnormalityStorage abnormalityStorage, Action<SkillResult> update = null)
        {
            EntityTracker = entityTracker;
            PlayerTracker = playerTracker;
            HotDotDatabase = hotDotDatabase;
            UpdateDamageTracker = update;
            AbnormalityStorage = abnormalityStorage;
        }

        public void Update(SNpcStatus npcStatus)
        {
            RegisterAggro(npcStatus);
            if (npcStatus.Enraged)
                AddAbnormality(npcStatus.Npc, npcStatus.Target, 0, 0, 8888888, npcStatus.Time.Ticks);
            else
                DeleteAbnormality(npcStatus);
        }

        public void RegisterSlaying(UserEntity user, bool slaying, long ticks)
        {
            if (user == null) return;
            if (slaying)
            {
                if (!AbnormalityStorage.Death(PlayerTracker.GetOrUpdate(user)).Dead)
                    AddAbnormality(user.Id, user.Id, 0, 0, 8888889, ticks);
            }
            else
                DeleteAbnormality(user.Id, 8888889, ticks);
        }

        public void RegisterDead(EntityId id, long ticks, bool dead)
        {
            var user = EntityTracker.GetOrNull(id) as UserEntity;
            if (user == null) return;
            var player = PlayerTracker.GetOrUpdate(user);
            if (dead)
            {
                AbnormalityStorage.Death(player).Start(ticks);
                DeleteAbnormality(user.Id, 8888889, ticks);
            }
            else
                AbnormalityStorage.Death(player).End(ticks);
        }

        public void Update(SCreatureLife message)
        {
            RegisterDead(message.User, message.Time.Ticks, message.Dead);
        }

        private void RegisterAggro(SNpcStatus aggro)
        {
            var time = aggro.Time.Ticks;
            var entity = EntityTracker.GetOrNull(aggro.Npc) as NpcEntity;
            if (entity == null) return; //not sure why, but sometimes it fails
            var user = EntityTracker.GetOrNull(aggro.Target) as UserEntity;
            if (user != null)
            {
                var player = PlayerTracker.GetOrUpdate(user);
                if (AbnormalityStorage.Last(entity) != player)
                {
                    if (AbnormalityStorage.Last(entity) != null)
                        AbnormalityStorage.AggroEnd(AbnormalityStorage.Last(entity), entity, time);
                    AbnormalityStorage.AggroStart(player, entity, time);
                    AbnormalityStorage.LastAggro[entity] = player;
                }
            }
            else
            {
                if (AbnormalityStorage.Last(entity) != null)
                {
                    AbnormalityStorage.AggroEnd(AbnormalityStorage.Last(entity), entity, time);
                    AbnormalityStorage.LastAggro[entity] = null;
                }
            }
        }

        public void StopAggro(SDespawnNpc aggro)
        {
            var time = aggro.Time.Ticks;
            var entity = EntityTracker.GetOrNull(aggro.Npc) as NpcEntity;
            if (entity == null) return; // Strange, but seems there are not only NPC or something wrong with trackers
            if (AbnormalityStorage.Last(entity) != null)
            {
                AbnormalityStorage.AggroEnd(AbnormalityStorage.Last(entity), entity, time);
                AbnormalityStorage.LastAggro[entity] = null;
            }
        }


        public event AbnormalityEvent AbnormalityAdded;
        public event AbnormalityEvent AbnormalityRemoved;

        public delegate void AbnormalityEvent(EntityId target, int abnormalityId, int stack=0);

        public void Update(SAbnormalityBegin message)
        {
            AddAbnormality(message.TargetId, message.SourceId, message.Duration, message.Stack, message.AbnormalityId,
                message.Time.Ticks);
        }

        public void AddAbnormality(EntityId target, EntityId source, int duration, int stack, int abnormalityId,
            long ticks)
        {
            if (!_abnormalities.ContainsKey(target))
            {
                _abnormalities.Add(target, new List<Abnormality.Abnormality>());
            }
            var hotdot = HotDotDatabase.Get(abnormalityId);
            if (hotdot == null)
            {
                return;
            }

            if (_abnormalities[target].Count(x => x.HotDot.Id == abnormalityId) == 0)
            {
                //dont add existing abnormalities since we don't delete them all, that may cause many untrackable issues.
                _abnormalities[target].Add(new Abnormality.Abnormality(hotdot, source, target, duration, stack, ticks,
                    this));
                AbnormalityAdded?.Invoke(target, abnormalityId, stack);
            }
        }

        public void Update(SAbnormalityRefresh message)
        {
            if (!_abnormalities.ContainsKey(message.TargetId))
            {
                return;
            }
            var abnormalityUser = _abnormalities[message.TargetId];
            foreach (var abnormality in abnormalityUser)
            {
                if (abnormality.HotDot.Id != message.AbnormalityId) continue;
                if (abnormality.Stack<message.StackCounter) AbnormalityAdded?.Invoke(message.TargetId, message.AbnormalityId, message.StackCounter);
                abnormality.Refresh(message.StackCounter, message.Duration, message.Time.Ticks);
                return;
            }
        }

        public bool HaveAbnormalities(EntityId target) => _abnormalities.ContainsKey(target) && _abnormalities[target].Any();

        public bool AbnormalityExist(EntityId target, int abnormalityid) => _abnormalities.ContainsKey(target) && _abnormalities[target].Any(x=>x.HotDot.Id==abnormalityid);
        /**
         * Return time left for the abnormality. Or -1 if no abnormality found
         */
        public long AbnormalityTimeLeft(EntityId target, HotDot.Types dotype)
        {
            if (!_abnormalities.ContainsKey(target))
            {
                return -1;
            }
            var abnormalityTarget = _abnormalities[target];
            var abnormalities = abnormalityTarget.Where(t => t.HotDot.Effects.Any(x => x.Type == dotype));
            if(!abnormalities.Any())
            {
                return -1;
            }
            return abnormalities.Max(x => x.TimeBeforeEnd);
       }

        /**
         * Return time left for the abnormality. Or -1 if no abnormality found
         */
        public long AbnormalityTimeLeft(EntityId target, int abnormalityId, int stack=0)
        {
            if (!_abnormalities.ContainsKey(target))
            {
                return -1;
            }
            var abnormalityTarget = _abnormalities[target];
            var i = abnormalityTarget.FindIndex(t => t.HotDot.Id == abnormalityId && t.Stack>=stack);
            if (i == -1)
            {
                return -1;
            }
            return abnormalityTarget[i].TimeBeforeEnd;
        }

        /**
         * Return current stack count for the abnormality. Or -1 if no abnormality found
         */
        public int Stack(EntityId target, int abnormalityId)
        {
            if (!_abnormalities.ContainsKey(target))
            {
                return -1;
            }
            var abnormalityTarget = _abnormalities[target];
            var i = abnormalityTarget.FindIndex(t => t.HotDot.Id == abnormalityId);
            if (i==-1)
            {
                return -1;
            }
            return abnormalityTarget[i].Stack;

        }
        public void DeleteAbnormality(EntityId target, int abnormalityId, long ticks)
        {
            if (!_abnormalities.ContainsKey(target))
            {
                return;
            }

            var abnormalityUser = _abnormalities[target];

            for (var i = 0; i < abnormalityUser.Count; i++)
            {
                if (abnormalityUser[i].HotDot.Id != abnormalityId) continue;
                abnormalityUser[i].ApplyBuffDebuff(ticks);
                abnormalityUser.Remove(abnormalityUser[i]);
                AbnormalityRemoved?.Invoke(target, abnormalityId);
                break;
            }

            if (abnormalityUser.Count == 0)
            {
                _abnormalities.Remove(target);
                return;
            }
            _abnormalities[target] = abnormalityUser;
        }

        public void Update(SAbnormalityEnd message)
        {
            DeleteAbnormality(message.TargetId, message.AbnormalityId, message.Time.Ticks);
        }

        public void DeleteAbnormality(SDespawnNpc message)
        {
            DeleteAbnormality(message.Npc, message.Time.Ticks);
        }

        public void DeleteAbnormality(SNpcStatus message)
        {
            DeleteAbnormality(message.Npc, 8888888, message.Time.Ticks);
        }

        public void DeleteAbnormality(SDespawnUser message)
        {
            DeleteAbnormality(message.User, message.Time.Ticks);
            RegisterDead(message.User, message.Time.Ticks, false);
        }

        private void DeleteAbnormality(EntityId entity, long ticks)
        {
            if (!_abnormalities.ContainsKey(entity))
            {
                return;
            }
            foreach (var abno in _abnormalities[entity])
            {
                abno.ApplyBuffDebuff(ticks);
            }
            _abnormalities.Remove(entity);
        }


        public void Update(SPlayerChangeMp message)
        {
            Update(message.TargetId, message.SourceId, message.MpChange, message.Type, message.Critical == 1, false,
                message.Time.Ticks);
        }

        private void Update(EntityId target, EntityId source, int change, int type, bool critical, bool isHp, long time)
        {
            if (!_abnormalities.ContainsKey(target))
            {
                return;
            }

            var abnormalities = _abnormalities[target];
            abnormalities =
                abnormalities.Where(
                    x => x.Source == EntityTracker.MeterUser.Id || x.Target == EntityTracker.MeterUser.Id)
                    .OrderByDescending(o => o.TimeBeforeApply)
                    .ToList();

            foreach (var abnormality in abnormalities)
            {
                if (abnormality.Source != source && abnormality.Source != abnormality.Target)
                {
                    continue;
                }

                if (isHp)
                {
                    if ((!(abnormality.HotDot.Hp > 0) || change <= 0) &&
                        (!(abnormality.HotDot.Hp < 0) || change >= 0)
                        ) continue;
                }
                else
                {
                    if ((!(abnormality.HotDot.Mp > 0) || change <= 0) &&
                        (!(abnormality.HotDot.Mp < 0) || change >= 0)
                        ) continue;
                }

                if ((int) HotDotDatabase.HotOrDot.Dot != type && (int) HotDotDatabase.HotOrDot.Hot != type)
                {
                    continue;
                }

                abnormality.Apply(change, critical, isHp, time);
                return;
            }
        }

        public void Update(SCreatureChangeHp message)
        {
            Update(message.TargetId, message.SourceId, message.HpChange, message.Type, message.Critical == 1, true,
                message.Time.Ticks);
            var user = EntityTracker.GetOrPlaceholder(message.TargetId) as UserEntity;
            RegisterSlaying(user, message.Slaying, message.Time.Ticks);
        }

        public void Update(SDespawnUser message)
        {
            DeleteAbnormality(message);

        }
        public void Update(SDespawnNpc message)
        {
            StopAggro(message);
            DeleteAbnormality(message);
        }
        public void Update(SpawnUserServerMessage message)
        {
            RegisterDead(message.Id, message.Time.Ticks, message.Dead);
        }
        public void Update(SpawnMeServerMessage message)
        {
            AbnormalityStorage.EndAll(message.Time.Ticks);
            _abnormalities = new Dictionary<EntityId, List<Abnormality.Abnormality>>();
            RegisterDead(message.Id, message.Time.Ticks, message.Dead);
        }
        public void Update(S_PLAYER_STAT_UPDATE message)
        {
            RegisterSlaying(EntityTracker.MeterUser, message.Slaying, message.Time.Ticks);
        }
        public void Update(S_PARTY_MEMBER_STAT_UPDATE message)
        {
            var user = PlayerTracker.GetOrNull(message.ServerId, message.PlayerId);
            if (user == null) return;
            RegisterSlaying(user.User, message.Slaying, message.Time.Ticks);
        }
        public void Update(SPartyMemberChangeHp message)
        {
            var user = PlayerTracker.GetOrNull(message.ServerId, message.PlayerId);
            if (user == null) return;
            RegisterSlaying(user.User, message.Slaying, message.Time.Ticks);
        }

        public void Update(ParsedMessage message)
        {
            message.On<S_PLAYER_STAT_UPDATE>(x => Update(x));
            message.On<S_PARTY_MEMBER_STAT_UPDATE>(x => Update(x));
            message.On<SPartyMemberChangeHp>(x => Update(x));
            message.On<SCreatureChangeHp>(x => Update(x));
            message.On<SPlayerChangeMp>(x => Update(x));
            message.On<SDespawnUser>(x => Update(x));
            message.On<SAbnormalityEnd>(x => Update(x));
            message.On<SAbnormalityRefresh>(x => Update(x));
            message.On<SAbnormalityBegin>(x => Update(x));
            message.On<SpawnMeServerMessage>(x => Update(x));
            message.On<SDespawnNpc>(x => Update(x));
            message.On<SCreatureLife>(x => Update(x));
            message.On<SNpcStatus>(x => Update(x));
            message.On<SpawnUserServerMessage>(x => Update(x));
        }
    }
}