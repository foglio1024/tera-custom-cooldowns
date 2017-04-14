// Copyright (c) Gothos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Tera.Game
{
    public class ProjectileEntity : Entity, IHasOwner
    {
        public ProjectileEntity(EntityId id, EntityId ownerId, Entity owner, Vector3f position, Angle heading,
            Vector3f finish, int speed, long time)
            : base(id, position, heading, finish, speed, time)
        {
            OwnerId = ownerId;
            Owner = owner;
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ProjectileEntity)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public bool Equals(ProjectileEntity other)
        {
            return Id.Equals(other?.Id);
        }

        public static bool operator ==(ProjectileEntity a, ProjectileEntity b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(ProjectileEntity a, ProjectileEntity b)
        {
            return !(a == b);
        }

        public EntityId OwnerId { get; set; }
        public Entity Owner { get; set; }
    }
}