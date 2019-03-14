// Copyright (c) Gothos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TCC.TeraCommon.Game
{
    // NPCs and Mosters - Tera doesn't distinguish these
    public class NpcEntity : Entity, IHasOwner
    {
        public NpcEntity(EntityId id, EntityId ownerId, Entity owner, NpcInfo info, Vector3f position, Angle heading)
            : base(id, position, heading)
        {
            OwnerId = ownerId;
            Owner = owner;
            Info = info;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((NpcEntity)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public bool Equals(NpcEntity other)
        {
            return Id.Equals(other?.Id);
        }

        public static bool operator ==(NpcEntity a, NpcEntity b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(NpcEntity a, NpcEntity b)
        {
            return !(a == b);
        }


        public NpcInfo Info { get; }

        public EntityId OwnerId { get; set; }
        public Entity Owner { get; set; }

        public override string ToString()
        {
            return Info.Name + " : " + Info.Area;
        }
    }
}