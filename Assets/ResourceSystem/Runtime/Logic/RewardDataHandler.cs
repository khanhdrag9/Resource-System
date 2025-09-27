namespace ResourceSystem
{
    public class RewardDataHandler
    {
        public bool Claimed { get; private set; }

        private ResourceRewardData _rewardData;

        public RewardDataHandler(ResourceRewardData data)
        {
            _rewardData = data;
        }

        public bool IsItemType()
        {
            return _rewardData.ResourceData is IItemType;
        }

        public void Claim()
        {
            if (Claimed)
            {
                return;
            }

            int id = _rewardData.ResourceData.Id;
            int amount = _rewardData.Amount;

            if (IsItemType())
            {
                for(int i = 0; i < amount; i++)
                {
                    OwnedItem ownedItem = new OwnedItem(_rewardData.ResourceData);
                    ResourceManager.Instance.AddOwnedItem(ownedItem);
                }
            }
            else
            {
                ResourceManager.Instance.GetOwnedCurrency(id).Amount += amount;
            }

            Claimed = true;
        }

        public void Renew()
        {
            Claimed = false;
        }

        public void View(ResourceView view)
        {
            view.UpdateInfo(_rewardData.ResourceData, _rewardData.Amount);
        }
    }
}