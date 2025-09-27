using System;
using System.Collections.Generic;
using UnityEngine;
using ResourceSystem;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-100)]
public class ResourceSystemExample : MonoBehaviour
{
    [Serializable]
    public class ExampleData
    {
        public int Id;
        public int Amount;
        public bool IsItemType;
        public string Name;
        public string Desc;
        public int Rarity;
        public Sprite Icon;
    }

    [Serializable]
    public class RewardData
    {
        [Tooltip("Resource ID, cannot change in play mode")]
        public int Id;
        public int Amount;
        public GameObject View;
    }

    public TextMeshProUGUI ResourceNameText;
    public List<ExampleData> DataList = new List<ExampleData>();

    [Header("Reward")]
    public List<RewardData> Rewards = new List<RewardData>();

    [Header("Shop")]
    public List<RewardData> ShopRewards = new List<RewardData>();
    public List<RewardData> ShopCost = new List<RewardData>();
    public List<ResourceView> ShopRewardViews = new List<ResourceView>();
    public List<Button> ShopButtons = new List<Button>();
    
    private List<ResourceRewardData> _allShopRewardDatas = new List<ResourceRewardData>();
    private List<ResourceCostData> _allShopCostDatas = new List<ResourceCostData>();
    private List<CostDataHandler> _allCostDataHandlers = new List<CostDataHandler>();

    [Header("UI")]
    public Button _addButton;
    public Button _resetAllRewardClaimButton;
    public TMP_InputField _idInput;
    public TMP_InputField _amountInput;


    void Start()
    {
        foreach (ExampleData data in DataList)
        {
            if (data.IsItemType)
            {
                ItemResourceData itemData = new ItemResourceData(data.Id, data.Name, data.Desc, data.Icon, data.Rarity);
                ResourceManager.Instance.AddResourceData(itemData);
            }
            else
            {
                CurrencyResourceData currencyData = new CurrencyResourceData(data.Id, data.Name, data.Desc, data.Icon, data.Rarity, 0, 0, 0, 0);
                ResourceManager.Instance.AddResourceData(currencyData);
            }

            ReceiveResource(data.Id, data.Amount);
        }

        for (int i = 0; i < Rewards.Count; i++)
        {
            RewardData reward = Rewards[i];
            reward.View.GetComponent<ResourceView>().UpdateInfo(ResourceManager.Instance.GetResourceData(reward.Id), reward.Amount);
        }

        for (int i = 0; i < ShopButtons.Count; i++)
        {
            Button button = ShopButtons[i];
            if (i >= ShopRewards.Count || i >= ShopCost.Count)
            {
                button.gameObject.SetActive(false);
                continue;
            }

            RewardData reward = ShopRewards[i];
            RewardData cost = ShopCost[i];
            ResourceData rewardResourceData = ResourceManager.Instance.GetResourceData(reward.Id);
            ResourceData costResourceData = ResourceManager.Instance.GetResourceData(cost.Id);

            ResourceRewardData rewardData = new ResourceRewardData(rewardResourceData, reward.Amount);
            ResourceCostData costData = new ResourceCostData(costResourceData, cost.Amount);

            CostDataHandler costDataHandler = new CostDataHandler(costData);
            RewardDataHandler rewardDataHandler = new RewardDataHandler(rewardData);

            _allShopRewardDatas.Add(rewardData);
            _allShopCostDatas.Add(costData);
            _allCostDataHandlers.Add(costDataHandler);

            button.onClick.AddListener(() =>
            {
                if (costDataHandler.Enough() && !rewardDataHandler.Claimed)
                {
                    costDataHandler.Cost();
                    rewardDataHandler.Claim();
                    rewardDataHandler.Renew();
                }
            });
        }

        _addButton.onClick.AddListener(OnAddButtonClicked);
        _resetAllRewardClaimButton.onClick.AddListener(ResetAllRewardClaim);
    }

    void Update()
    {
        // UpdateShopUi();

        if (!string.IsNullOrEmpty(_idInput.text) && int.TryParse(_idInput.text, out int id))
        {
            ResourceData resourceData = ResourceManager.Instance.GetResourceData(id);
            if (resourceData != null)
            {
                ResourceNameText.text = resourceData is IHasName hasName ? hasName.Name : "No Name";
            }
            else
            {
                ResourceNameText.text = "Not Found";
            }
        }
        else
        {
            ResourceNameText.text = "";
        }
    }


    private void UpdateShopUi()
    {
        for (int i = 0; i < ShopRewards.Count; i++)
        {
            ShopRewardViews[i].UpdateInfo(
                ResourceManager.Instance.GetResourceData(ShopRewards[i].Id),
                ShopRewards[i].Amount
            );

            _allShopRewardDatas[i].Amount = ShopRewards[i].Amount;
            _allShopCostDatas[i].Amount = ShopCost[i].Amount;

            Button button = ShopButtons[i];
            CostDataHandler costDataHandler = _allCostDataHandlers[i];
            button.image.color = costDataHandler.Enough() ? Color.yellow : Color.gray;

            button.GetComponent<ResourceView>().UpdateInfo(
                ResourceManager.Instance.GetResourceData(ShopCost[i].Id),
                ShopCost[i].Amount
            );
        }
    }

    public void OnAddButtonClicked()
    {
        if (int.TryParse(_amountInput.text, out int amount))
        {
            if (int.TryParse(_idInput.text, out int id))
            {
                ReceiveResource(id, amount);
            }
            else
            {
                Debug.LogError("Invalid ID input");
            }
        }
        else
        {
            Debug.LogError("Invalid amount input");
        }
    }

    public void ReceiveResource(int id, int amount)
    {
        ResourceData resourceData = ResourceManager.Instance.GetResourceData(id);
        if (resourceData is IItemType)
        {
            if (amount > 0)
            {
                for (int i = 0; i < amount; i++)
                {
                    OwnedItem ownedItem = new OwnedItem(resourceData);
                    ResourceManager.Instance.AddOwnedItem(ownedItem);
                }
            }
            else
            {
                List<OwnedItem> ownedItems = ResourceManager.Instance.GetOwnedItems(id, -amount);
                foreach (OwnedItem ownedItem in ownedItems)
                {
                    ResourceManager.Instance.RemoveOwnedItem(ownedItem);
                }
            }
        }
        else
        {
            ResourceManager.Instance.GetOwnedCurrency(id).Amount += amount;
        }
    }

    public void ResetAllRewardClaim()
    {
        foreach (var data in Rewards)
        {
            data.View.GetComponent<ClaimRewardView>().Renew();
        }
    }
}
