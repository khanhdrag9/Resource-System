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

    [Header("Owned Item")]
    public List<RewardData> InitialOwnedItems = new List<RewardData>();
    public ResourcesListView OwnedItemListView;


    [Header("Reward")]
    public List<RewardData> Rewards = new List<RewardData>();

    [Header("Shop")]
    public List<RewardData> ShopRewards = new List<RewardData>();
    public List<RewardData> ShopCost = new List<RewardData>();


    [Header("UI")]
    public Button _addButton;
    public Button _resetAllRewardClaimButton;
    public TMP_InputField _idInput;
    public TMP_InputField _amountInput;

    private List<ResourceUsageData> _ownedItemCache = new List<ResourceUsageData>();
    private bool _ownedItemDirty = true;

    public OwnedResourceManager OwnedResourceManager => OwnedResourceManager.Instance;
    public ResourceManager ResourceManager => ResourceManager.Instance;


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

        for (int i = 0; i < ShopRewards.Count; i++)
        {
            RewardData reward = ShopRewards[i];
            RewardData cost = ShopCost[i];
            reward.View.GetComponent<ResourceView>().UpdateInfo(ResourceManager.Instance.GetResourceData(reward.Id), reward.Amount);
            reward.View.transform.Find("Claim button").GetComponent<ResourceView>().UpdateInfo(ResourceManager.Instance.GetResourceData(cost.Id), cost.Amount);
        }

        _addButton.onClick.AddListener(OnAddButtonClicked);
        _resetAllRewardClaimButton.onClick.AddListener(ResetAllRewardClaim);

        foreach (var reward in InitialOwnedItems)
        {
            ReceiveResource(reward.Id, reward.Amount);
        }

        foreach (var ownedItem in OwnedResourceManager.GetAllOwnedItems())
        {
            _ownedItemCache.Add(new ResourceUsageData(ownedItem.Data, 1));
        }

        OwnedResourceManager.OnOwnedResourceChanged += (data, changedValue) =>
        {
            if (data is IItemType)
            {
                if (changedValue > 0)
                {
                    for (int i = 0; i < changedValue; i++)
                    {
                        _ownedItemCache.Add(new ResourceUsageData(data, 1));
                    }
                }
                else if(changedValue < 0)
                {
                    int removeCount = -changedValue;
                    for (int i = _ownedItemCache.Count - 1; i >= 0 && removeCount > 0; i--)
                    {
                        if (_ownedItemCache[i].ResourceData.Id == data.Id)
                        {
                            _ownedItemCache.RemoveAt(i);
                            removeCount--;
                        }
                    }
                }

                _ownedItemDirty = true;
            }
        };
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

        if(_ownedItemDirty)
        {
            _ownedItemDirty = false;
            OwnedItemListView.UpdateInfo(_ownedItemCache);
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
        OwnedResourceManager.AddOwnedResource(resourceData, amount);
    }

    public void ResetAllRewardClaim()
    {
        foreach (var data in Rewards)
        {
            data.View.GetComponent<ClaimResourceView>().Renew();
        }
    }
}
