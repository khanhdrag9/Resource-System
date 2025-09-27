using System.Collections.Generic;

namespace ResourceSystem
{
    public class RewardDataHandler
    {
        public bool AlreadyClaimed { get; private set; }
        public IReadOnlyList<ResourceUsageData> LastClaimedResources { get; private set; }

        private ResourceRewardData _rewardData;

        public RewardDataHandler(ResourceRewardData data)
        {
            _rewardData = data;
        }

        public bool IsItemType()
        {
            return _rewardData.ResourceData is IItemType;
        }

        public List<ResourceUsageData> Claim()
        {
            List<ResourceUsageData> result = new();

            if (AlreadyClaimed)
            {
                return result;
            }

            int id = _rewardData.ResourceData.Id;
            int amount = _rewardData.Amount;

            if (IsItemType())
            {
                for (int i = 0; i < amount; i++)
                {
                    OwnedItem ownedItem = new OwnedItem(_rewardData.ResourceData);
                    ResourceManager.Instance.AddOwnedItem(ownedItem);
                    result.Add(new ResourceUsageData(ownedItem.Data, 1));
                }
            }
            else
            {
                OwnedCurrency ownedCurrency = ResourceManager.Instance.GetOwnedCurrency(id);
                ownedCurrency.Amount += amount;
                result.Add(new ResourceUsageData(ownedCurrency.Data, amount));
            }

            AlreadyClaimed = true;
            LastClaimedResources = result;
            return result;
        }

        public void Renew()
        {
            AlreadyClaimed = false;
        }

        public void MarkClaimed()
        {
            AlreadyClaimed = true;
        }

        public void View(ResourceView view)
        {
            view.UpdateInfo(_rewardData.ResourceData, _rewardData.Amount);
        }
    }
}