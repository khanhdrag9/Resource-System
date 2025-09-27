using System.Collections.Generic;
using UnityEngine;

namespace ResourceSystem
{
    public class ResourceManager
    {
        public static ResourceManager Instance { get; private set; } = new();

        private Dictionary<int, ResourceData> _resourceDataMap = new();
        private Dictionary<int, OwnedCurrency> _ownedCurrencyMap = new();
        private List<OwnedItem> _ownedItems = new();

        public void AddResourceData(ResourceData resourceData)
        {
            if (_resourceDataMap.ContainsKey(resourceData.Id))
            {
                Debug.LogWarning($"ResourceSystem: Resource data with id {resourceData.Id} already exists");
                return;
            }

            _resourceDataMap.Add(resourceData.Id, resourceData);
        }

        public void AddOwnedItem(OwnedItem ownedItem)
        {
            _ownedItems.Add(ownedItem);
        }

        public ResourceData GetResourceData(int id)
        {
            if (!_resourceDataMap.TryGetValue(id, out var resourceData))
            {
                Debug.LogWarning($"ResourceSystem: Resource data with id {id} not found");
                return null;
            }

            return resourceData;
        }
        
        public OwnedCurrency GetOwnedCurrency(int id)
        {
            if (!_ownedCurrencyMap.TryGetValue(id, out var ownedCurrency))
            {
                ResourceData resourceData = GetResourceData(id);
                if (resourceData == null)
                {
                    Debug.LogWarning($"ResourceSystem: Owned resource with id {id} not found");
                }

                ownedCurrency = new OwnedCurrency(resourceData);
                _ownedCurrencyMap.Add(id, ownedCurrency);
                return ownedCurrency;
            }

            return ownedCurrency;
        }

        public IReadOnlyList<OwnedItem> GetOwnedItems()
        {
            return _ownedItems;
        }

        public List<OwnedItem> GetOwnedItems(int id, int amount = 0)
        {
            List<OwnedItem> result = new();
            foreach (OwnedItem item in _ownedItems)
            {
                if(item.Data.Id == id)
                {
                    result.Add(item);
                }

                if(amount > 0 && result.Count >= amount)
                {
                    break;
                }
            }
            return result;
        }

        public void RemoveOwnedItem(OwnedItem ownedItem)
        {
            _ownedItems.Remove(ownedItem);
        }

        public void Clear()
        {
            _resourceDataMap.Clear();
            _ownedCurrencyMap.Clear();
            _ownedItems.Clear();
        }
    }
}