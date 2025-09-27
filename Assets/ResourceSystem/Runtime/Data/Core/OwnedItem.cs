using System;
using UnityEngine;

namespace ResourceSystem
{
    public sealed class OwnedItem : EquatableData
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

        protected override bool EqualInternal(object other)
        {
            return other is OwnedItem ownedItem && Data.Id == ownedItem.Data.Id;
        }

        protected override int GetHashCodeInternal()
        {
            return UniqueId.GetHashCode();
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