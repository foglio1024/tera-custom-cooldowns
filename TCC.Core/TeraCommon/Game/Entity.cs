using System;

namespace TCC.TeraCommon.Game
{
    // An object with an Id that can be spawned or deswpawned in the game world
    public class Entity : IEquatable<object>
    {
        protected Entity(EntityId id)
        {
            Id = id;
        }

        protected Entity(EntityId id, Vector3f position, Angle heading, Vector3f? finish = null, int speed = 0,
            long time = 0)
        {
            Id = id;
            Position = position;
            Heading = heading;
            Finish = finish ?? Position;
            Speed = speed;
            StartTime = time;
        }

        public EntityId Id { get; }

        public Entity RootOwner
        {
            get
            {
                var entity = this;
                var ownedEntity = entity as IHasOwner;
                while (ownedEntity?.Owner != null)
                {
                    entity = ownedEntity.Owner;
                    ownedEntity = entity as IHasOwner;
                }
                return entity;
            }
        }


        public Vector3f Position { get; set; }
        public Angle Heading { get; set; }
        public Vector3f Finish { get; set; }
        public Angle EndAngle { get; set; }
        public Angle LastCastAngle { get; set; }
        public int Speed { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Entity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            var result = $"{GetType().Name} {Id}";
            if (RootOwner != this)
                result = $"{result} owned by {RootOwner}";
            return result;
        }


        public bool Equals(Entity other)
        {
            return Id.Equals(other?.Id);
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }
    }
}