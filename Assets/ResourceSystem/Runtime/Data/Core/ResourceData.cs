using System;

namespace ResourceSystem
{
    public class ResourceData : EquatableData
    {
        public readonly int Id;

        public ResourceData(int id)
        {
            Id = id;
        }

        protected override bool EqualInternal(object other)
        {
            return other is ResourceData resourceData && Id == resourceData.Id;
        }

        protected override int GetHashCodeInternal()
        {
            return Id.GetHashCode();
        }
    }
}