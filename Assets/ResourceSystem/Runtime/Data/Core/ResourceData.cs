using System;

namespace ResourceSystem
{
    public class ResourceData : IEquatable<ResourceData>
    {
        public readonly int Id;

        public ResourceData(int id)
        {
            Id = id;
        }

        public override bool Equals(object obj) => Equals(obj as ResourceData);
        public bool Equals(ResourceData other)
        {
            return ReferenceEquals(this, other) || Id == other.Id;
        }

        public static bool operator ==(ResourceData a, ResourceData b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null))
                return false;
            if (ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ResourceData a, ResourceData b) => !(a == b);

        public override int GetHashCode()
        {
            unchecked
            {
                return Id.GetHashCode();
            }
        }
    }
}