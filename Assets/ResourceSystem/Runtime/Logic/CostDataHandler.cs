using System.Collections.Generic;

namespace ResourceSystem
{
    public class CostDataHandler
    {
        public bool Costed { get; private set; }
        public OwnedResourceManager OwnedResourceManager => OwnedResourceManager.Instance;

        private ResourceCostData _costData;

        public CostDataHandler(ResourceCostData data)
        {
            _costData = data;
        }

        public bool IsItemType()
        {
            return _costData.ResourceData is IItemType;
        }

        public bool Enough()
        {
            int id = _costData.ResourceData.Id;
            int amount = _costData.Amount;

            if (IsItemType())
            {
                List<OwnedItem> ownedItems = OwnedResourceManager.GetOwnedItems(id);
                return ownedItems.Count >= amount;
            }
            else
            {
                return OwnedResourceManager.GetOwnedCurrency(id).Amount >= amount;
            }
        }

        public void Cost()
        {
            if (Costed)
            {
                return;
            }

            int id = _costData.ResourceData.Id;
            int amount = _costData.Amount;

            if (IsItemType())
            {
                List<OwnedItem> ownedItems = OwnedResourceManager.GetOwnedItems(id, amount);
                foreach (OwnedItem ownedItem in ownedItems)
                {
                    OwnedResourceManager.RemoveOwnedItem(ownedItem);
                }
            }
            else
            {
                OwnedResourceManager.GetOwnedCurrency(id).Amount -= amount;
            }

            Costed = true;
        }

        public void Renew()
        {
            Costed = false;
        }

        public void View(ResourceView view)
        {
            view.UpdateInfo(_costData.ResourceData, _costData.Amount);
        }
    }
}