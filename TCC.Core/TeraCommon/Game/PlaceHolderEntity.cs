namespace TCC.TeraCommon.Game
{
    public class PlaceHolderEntity : Entity
    {
        public PlaceHolderEntity(EntityId id)
            : base(id)
        {



    }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((PlaceHolderEntity)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public bool Equals(PlaceHolderEntity other)
        {
            return Id.Equals(other?.Id);
        }

        public static bool operator ==(PlaceHolderEntity a, PlaceHolderEntity b)
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

        public static bool operator !=(PlaceHolderEntity a, PlaceHolderEntity b)
        {
            return !(a == b);
        }
    }
}