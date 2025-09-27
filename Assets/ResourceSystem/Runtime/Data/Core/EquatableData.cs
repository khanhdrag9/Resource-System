using System;

namespace ResourceSystem
{
    public abstract class EquatableData : IEquatable<EquatableData>
    {
        public override bool Equals(object obj) => Equals(obj as EquatableData);
        public bool Equals(EquatableData other)
        {
            return ReferenceEquals(this, other) || EqualInternal(other);
        }

        public static bool operator ==(EquatableData a, EquatableData b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null))
                return false;
            if (ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(EquatableData a, EquatableData b) => !(a == b);
        public override int GetHashCode()
        {
            unchecked
            {
                return GetHashCodeInternal();
            }
        }


        protected abstract bool EqualInternal(object other);
        protected abstract int GetHashCodeInternal();
    }
}