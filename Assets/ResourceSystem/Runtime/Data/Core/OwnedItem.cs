using System;
using UnityEngine;

namespace ResourceSystem
{
    public sealed class OwnedItem : IEquatable<OwnedItem>
    {
        public Guid UniqueId { get; private set; }
        public ResourceData Data { get; private set; }

        public OwnedItem(ResourceData data)
        {
            UniqueId = Guid.NewGuid();
            Data = data;
        }

        public OwnedItem(Guid uniqueId, ResourceData data)
        {
            UniqueId = uniqueId;
            Data = data;
        }

        public override bool Equals(object obj) => Equals(obj as OwnedItem);
        public bool Equals(OwnedItem other)
        {
            return ReferenceEquals(this, other) || UniqueId == other.UniqueId;
        }

        public static bool operator ==(OwnedItem a, OwnedItem b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null))
                return false;
            if (ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(OwnedItem a, OwnedItem b) => !(a == b);

        public override int GetHashCode()
        {
            unchecked
            {
                return UniqueId.GetHashCode();
            }
        }
    }

    [Serializable]
    public class OwnedItemSaveData
    {
        public Guid UniqueId;
        public int Id;

        [NonSerialized]
        public OwnedItem OwnedItem;

        public OwnedItem LoadOwnedItem()
        {
            ResourceData resourceData = ResourceManager.Instance.GetResourceData(Id);
            if(resourceData == null)
            {
                Debug.LogWarning($"ResourceSystem: Owned item with id {Id} not found");
                return null;
            }

            OwnedItem = new OwnedItem(UniqueId, resourceData);
            return OwnedItem;
        }

        public void SaveOwnedItem()
        {
            if(OwnedItem == null)
            {
                Debug.LogWarning($"ResourceSystem: Owned item is null");
                return;
            }

            UniqueId = OwnedItem.UniqueId;
            Id = OwnedItem.Data.Id;
        }
    }
}