using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceSystem
{
    public class OwnedResourceManager
    {
        public static OwnedResourceManager Instance { get; } = new OwnedResourceManager();

        // ID - Value
        // Currency type is New Value
        // Item type is Mod Value
        public Action<ResourceData, int> OnOwnedResourceChanged = delegate { };
        public ResourceManager ResourceManager => ResourceManager.Instance;

        private Dictionary<int, OwnedCurrency> _ownedCurrencyMap = new();
        private List<OwnedItem> _ownedItems = new();

        public void AddOwnedResource(ResourceData resourceData, int amount)
        {
            if (resourceData is not IItemType)
            {
                GetOwnedCurrency(resourceData.Id).Amount += amount;
                return;
            }

            if (amount > 0)
            {
                for (int i = 0; i < amount; i++)
                {
                    OwnedItem ownedItem = new OwnedItem(resourceData);
                    _ownedItems.Add(ownedItem);
                }

                OnOwnedResourceChanged?.Invoke(resourceData, amount);
                return;
            }

            if (amount < 0)
            {
                List<OwnedItem> ownedItems = GetOwnedItems(resourceData.Id, amount);
                foreach (var ownedItem in ownedItems)
                {
                    _ownedItems.Remove(ownedItem);
                }

                OnOwnedResourceChanged?.Invoke(resourceData, -ownedItems.Count);
            }
        }

        public void AddOwnedItem(OwnedItem ownedItem)
        {
            _ownedItems.Add(ownedItem);
            OnOwnedResourceChanged?.Invoke(ownedItem.Data, 1);
        }

        public IReadOnlyList<OwnedItem> GetAllOwnedItems()
        {
            return _ownedItems;
        }

        public List<OwnedItem> GetOwnedItems(int id, int amount = 0)
        {
            List<OwnedItem> result = new();
            foreach (OwnedItem item in _ownedItems)
            {
                if (item.Data.Id == id)
                {
                    result.Add(item);
                }

                if (amount > 0 && result.Count >= amount)
                {
                    break;
                }
            }
            return result;
        }

        public void RemoveOwnedItem(OwnedItem ownedItem)
        {
            if (_ownedItems.Remove(ownedItem))
            {
                OnOwnedResourceChanged?.Invoke(ownedItem.Data, -1);
            }
        }

        public OwnedCurrency GetOwnedCurrency(int id)
        {
            if (!_ownedCurrencyMap.TryGetValue(id, out var ownedCurrency))
            {
                ResourceData resourceData = ResourceManager.GetResourceData(id);
                if (resourceData == null)
                {
                    Debug.LogWarning($"ResourceSystem: Owned resource with id {id} not found");
                    return null;
                }

                ownedCurrency = new OwnedCurrency(resourceData);
                ownedCurrency.OnAmountChangedWithDataNAmountChanged += OnCurrencyChanged;
                _ownedCurrencyMap.Add(id, ownedCurrency);
                return ownedCurrency;
            }

            return ownedCurrency;
        }

        private void OnCurrencyChanged(ResourceData data, int changedValue)
        {
            OnOwnedResourceChanged?.Invoke(data, changedValue);
        }

        public void Clear()
        {
            foreach (var ownedCurrency in _ownedCurrencyMap.Values)
            {
                ownedCurrency.OnAmountChangedWithDataNAmountChanged -= OnCurrencyChanged;
            }

            _ownedCurrencyMap.Clear();
            _ownedItems.Clear();
        }
    }
}