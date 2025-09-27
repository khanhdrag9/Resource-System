using System;
using UnityEngine;

namespace ResourceSystem
{
    public sealed class OwnedCurrency : EquatableData
    {
        public readonly ResourceData Data;
        public int Amount
        {
            get => _amount;
            set
            {
                if (_amount == value)
                {
                    return;
                }

                _amount = value;
                OnAmountChanged?.Invoke(value);
                OnAmountChangedWithDataNAmountChanged?.Invoke(Data, value);
            }
        }

        public Action<int> OnAmountChanged = delegate { };
        public Action<ResourceData, int> OnAmountChangedWithDataNAmountChanged = delegate { };

        private int _amount;

        public OwnedCurrency(ResourceData resourceData)
        {
            Data = resourceData;
            SetNew();
        }

        public OwnedCurrency(ResourceData resourceData, int amount) : this(resourceData)
        {
            _amount = amount;
        }

        public void SetNew()
        {
            if (Data is IHasDefaultAmount hasDefaultAmount)
            {
                _amount = hasDefaultAmount.DefaultAmount;
            }
            else
            {
                _amount = 0;
            }
        }

        protected override bool EqualInternal(object other)
        {
            return other is OwnedCurrency ownedCurrency && Data.Id == ownedCurrency.Data.Id;
        }
        
        protected override int GetHashCodeInternal()
        {
            return Data.Id.GetHashCode();
        }
    }

    [Serializable]
    public class OwnedResourceSaveData
    {
        public int Id;
        public int Amount;

        [NonSerialized]
        public OwnedCurrency OwnedResource;

        public OwnedCurrency LoadOwnedResource()
        {
            ResourceData resourceData = ResourceManager.Instance.GetResourceData(Id);
            if(resourceData == null)
            {
                Debug.LogWarning($"ResourceSystem: Owned resource with id {Id} not found");
                return null;
            }

            OwnedResource = new OwnedCurrency(resourceData, Amount);
            return OwnedResource;
        }

        public void SaveOwnedResource()
        {
            if(OwnedResource == null)
            {
                Debug.LogWarning($"ResourceSystem: Owned resource is null");
                return;
            }

            Id = OwnedResource.Data.Id;
            Amount = OwnedResource.Amount;
        }
    }
}