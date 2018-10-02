//// Copyright (c) Gothos
//// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TCC.TeraCommon.Game.Messages;
//using TCC.TeraCommon.Game.Messages.Server;

//namespace TCC.TeraCommon.Game.Services
//{
//    // Tracks which entities we have seen so far and what their properties are
//    public class EntityTracker : IEnumerable<Entity>
//    {
//        private readonly Dictionary<EntityId, Entity> _entities = new Dictionary<EntityId, Entity>();

//        private readonly NpcDatabase _npcDatabase;

//        public EntityTracker(NpcDatabase npcDatabase)
//        {
//            _npcDatabase = npcDatabase;
//        }

//        public UserEntity MeterUser { get; private set; }

//        public IEnumerator<Entity> GetEnumerator()
//        {
//            return _entities.Values.GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }

//        public event Action<Entity> EntityUpdated;

//        protected virtual void OnEntityUpdated(Entity entity)
//        {
//            var handler = EntityUpdated;
//            handler?.Invoke(entity);
//        }

//        public void Update(LoginServerMessage message)
//        {
//            var newEntity = LoginMe(message);
//            Register(newEntity);
//        }

//        public void Update(SpawnUserServerMessage message)
//        {
//            var newEntity = new UserEntity(message);
//            Register(newEntity);
//        }

//        public void Update(SDespawnUser message)
//        {
//            var entity = (UserEntity)GetOrNull(message.User);
//            if (entity!=null) entity.OutOfRange = true;
//        }

//        internal void Register(Entity newEntity)
//        {
//            _entities[newEntity.Id] = newEntity;
//            OnEntityUpdated(newEntity);
//        }

//        public void Update(EachSkillResultServerMessage m)
//        {
//            var entity = GetOrNull(m.Target);
//            if (entity == null) return;
//            if (m.Position.X == 0 && m.Position.Y == 0 && m.Position.Z == 0) return;
//            entity.Position = m.Position;
//            entity.Finish = m.Position;
//            entity.Speed = 0;
//            entity.StartTime = 0;
//            entity.Heading = m.Heading;
//            entity.EndAngle = m.Heading;
//            entity.EndTime = 0;
//        }

//        public void Update(SCreatureLife m)
//        {
//            var entity = GetOrNull(m.User);
//            if (entity == null) return;
//            entity.Position = m.Position;
//            entity.Finish = entity.Position;
//            entity.Speed = 0;
//            entity.StartTime = 0;
//            entity.EndAngle = entity.Heading;
//            entity.EndTime = 0;
//        }


//        public void Update(SpawnNpcServerMessage m)
//        {
//            var newEntity = new NpcEntity(m.Id, m.OwnerId, GetOrPlaceholder(m.OwnerId),
//                _npcDatabase.GetOrPlaceholder(m.NpcArea, m.NpcId), m.Position, m.Heading);
//            Register(newEntity);
//        }

//        public void Update(SpawnProjectileServerMessage m)
//        {
//            var newEntity = new ProjectileEntity(m.Id, m.OwnerId, GetOrPlaceholder(m.OwnerId), m.Start,
//                            m.Start.GetHeading(m.Finish), m.Finish, (int)m.Speed, m.Time.Ticks);
//            Register(newEntity);

//        }

//        public void Update(StartUserProjectileServerMessage m)
//        {
//            var newEntity = new ProjectileEntity(m.Id, m.OwnerId, GetOrPlaceholder(m.OwnerId), m.Start,
//                            m.Start.GetHeading(m.Finish), m.Finish, (int)m.Speed, m.Time.Ticks);
//            Register(newEntity);
//        }

//        public void Update(S_MOUNT_VEHICLE_EX m)
//        {
//            var entity = GetOrNull(m.Id) as IHasOwner;
//            if (entity == null) return;
//            var player = GetOrNull(m.Owner) as UserEntity;
//            if (player == null) return;
//            entity.OwnerId = player.Id;
//            entity.Owner = player;
//        }

//        //public void Update(C_PLAYER_LOCATION m)
//        //{
//        //    if (MeterUser == null) return; //Don't know how, but sometimes this happens.
//        //    MeterUser.Position = m.Position;
//        //    MeterUser.Heading = m.Heading;
//        //    MeterUser.Finish = m.Position;
//        //    MeterUser.Speed = 0;
//        //    MeterUser.StartTime = m.Time.Ticks;
//        //    MeterUser.EndAngle = m.Heading;
//        //    MeterUser.EndTime = 0;
//        //}

//        public void Update(S_CHANGE_DESTPOS_PROJECTILE m)
//        {
//            var entity = GetOrNull(m.Id);
//            if (entity == null) return;
//            entity.Position = entity.Position.MoveForvard(entity.Finish, entity.Speed,
//                m.Time.Ticks - entity.StartTime);
//            entity.Finish = m.Finish;
//            entity.Heading = entity.Position.GetHeading(entity.Finish);
//            entity.StartTime = m.Time.Ticks;
//            entity.EndAngle = entity.Heading;
//            entity.EndTime = 0;
//        }

//        public void Update(S_ACTION_STAGE m)
//        {
//            var entity = GetOrNull(m.Entity);
//            if (entity == null) return;
//            entity.Position = m.Position;
//            entity.Finish = entity.Position;
//            entity.Heading = m.Heading;
//            entity.Speed = 0;
//            entity.StartTime = 0;
//            entity.EndAngle = entity.Heading;
//            entity.EndTime = 0;
//            //Debug.WriteLine($"{entity.Position} {entity.Heading}");
//        }

//        public void Update(S_ACTION_END m)
//        {
//            var entity = GetOrNull(m.Entity);
//            if (entity == null) return;
//            entity.Position = m.Position;
//            entity.Finish = entity.Position;
//            entity.Heading = m.Heading;
//            entity.Speed = 0;
//            entity.StartTime = 0;
//            entity.LastCastAngle = entity.Heading;
//            entity.EndAngle = entity.Heading;
//            entity.EndTime = 0;
//            //Debug.WriteLine($"{entity.Position} {entity.Heading}");
//        }

//        public void Update(S_INSTANT_MOVE m)
//        {
//            var entity = GetOrNull(m.Entity);
//            if (entity == null) return;
//            entity.Position = m.Position;
//            entity.Finish = m.Position;
//            entity.Heading = m.Heading;
//            entity.Speed = 0;
//            entity.StartTime = 0;
//            entity.EndAngle = entity.Heading;
//            entity.EndTime = 0;
//            //Debug.WriteLine($"{entity.Position} {entity.Heading}");
//        }

//        public void Update(S_CREATURE_ROTATE m)
//        {
//            var entity = GetOrNull(m.Entity);
//            if (entity == null) return;
//            entity.Position = entity.Position.MoveForvard(entity.Finish, entity.Speed,
//                m.Time.Ticks - entity.StartTime);
//            entity.Finish = entity.Position;
//            entity.Speed = 0;
//            entity.StartTime = m.Time.Ticks;
//            if (entity.EndTime > 0 && entity.EndTime <= entity.StartTime)
//            {
//                entity.Heading = entity.EndAngle;
//            }
//            //else if (entity.EndTime > 0)
//            //{
//            //    Debug.WriteLine("New rotate started before old ended!");
//            //}
//            entity.EndAngle = m.Heading;
//            entity.EndTime = entity.StartTime + (m.NeedTime == 0 ? 0 : TimeSpan.TicksPerMillisecond * m.NeedTime);
//            //Debug.WriteLine($"{entity.Position} {entity.Heading} {entity.EndAngle} {m.NeedTime}");
//        }

//        public void Update(SNpcLocation m)
//        {
//            var entity = GetOrNull(m.Entity);
//            if (entity == null) return;
//            entity.Position = m.Start;
//            entity.Finish = m.Finish;
//            entity.Speed = m.Speed;
//            entity.StartTime = m.Time.Ticks;
//            entity.Heading = m.Heading;
//            entity.EndAngle = m.Heading;
//            entity.EndTime = 0;
//            //Debug.WriteLine($"{entity.Position} {entity.Heading} {entity.Finish} {entity.Speed}");
//        }

//        //public void Update(S_USER_LOCATION m)
//        //{
//        //    var entity = GetOrNull(m.Entity);
//        //    if (entity == null) return;
//        //    entity.Position = m.Start;
//        //    entity.Finish = m.Finish;
//        //    entity.Speed = m.Speed;
//        //    entity.StartTime = m.Time.Ticks;
//        //    entity.Heading = m.Heading;
//        //    entity.EndAngle = m.Heading;
//        //    entity.EndTime = 0;
//        //    //Debug.WriteLine($"{entity.Position} {entity.Heading} {entity.Finish} {entity.Speed}");
//        //}

//        public void Update(S_BOSS_GAGE_INFO m)
//        {
//            var entity = GetOrNull(m.EntityId) as NpcEntity;
//            if (entity == null) return;
//            _npcDatabase.AddDetectedBoss(entity.Info.HuntingZoneId, entity.Info.TemplateId);
//            entity.Info.Boss = true;
//            entity.Info.HP = (long) m.TotalHp;
//        }

//        /** Easy integrate style - compatible */
//        public void Update(ParsedMessage message)
//        {
//            message.On<SpawnUserServerMessage>(x => Update(x));
//            message.On<LoginServerMessage>(x => Update(x));
//            message.On<SpawnNpcServerMessage>(x => Update(x));
//            message.On<SpawnProjectileServerMessage>(x => Update(x));
//            message.On<StartUserProjectileServerMessage>(x => Update(x));
//            message.On<S_MOUNT_VEHICLE_EX>(x => Update(x));
//            //message.On<C_PLAYER_LOCATION>(x => Update(x));
//            message.On<S_CHANGE_DESTPOS_PROJECTILE>(x => Update(x));
//            message.On<S_ACTION_STAGE>(x => Update(x));
//            message.On<S_ACTION_END>(x => Update(x));
//            message.On<SCreatureLife>(x => Update(x));
//            message.On<S_INSTANT_MOVE>(x => Update(x));
//            message.On<S_CREATURE_ROTATE>(x => Update(x));
//            message.On<EachSkillResultServerMessage>(x => Update(x));
//            message.On<SNpcLocation>(x => Update(x));
//            //message.On<S_USER_LOCATION>(x => Update(x));
//            message.On<S_BOSS_GAGE_INFO>(x => Update(x));
//        }

//        private Entity LoginMe(LoginServerMessage m)
//        {
//            MeterUser = new UserEntity(m);
//            return MeterUser;
//        }

//        public Entity GetOrNull(EntityId id)
//        {
//            Entity entity;
//            _entities.TryGetValue(id, out entity);
//            return entity;
//        }

//        public Entity GetOrPlaceholder(EntityId id)
//        {
//            if (id == EntityId.Empty)
//                return null;
//            var entity = GetOrNull(id);
//            if (entity != null)
//                return entity;
//            return new PlaceHolderEntity(id);
//        }
//    }
//}