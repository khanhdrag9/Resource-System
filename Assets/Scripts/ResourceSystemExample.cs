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
        public int Id;
        public int Amount;
    }

    public TextMeshProUGUI ResourceNameText;
    public List<ExampleData> DataList = new List<ExampleData>();
    public List<RewardData> Rewards = new List<RewardData>();
    public List<ResourceView> RewardViews = new List<ResourceView>();
    public List<Button> RewardButtons = new List<Button>();

    [Header("UI")]
    public Button _addButton;
    public Button _resetAllRewardClaimButton;
    public TMP_InputField _idInput;
    public TMP_InputField _amountInput;

    private List<RewardDataHandler> _allRewardDataHandlers = new List<RewardDataHandler>();

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

        for (int i = 0; i < RewardButtons.Count; i++)
        {
            Button button = RewardButtons[i];
            if (i >= Rewards.Count)
            {
                button.gameObject.SetActive(false);
                continue;
            }

            RewardData reward = Rewards[i];
            ResourceData resourceData = ResourceManager.Instance.GetResourceData(reward.Id);
            ResourceRewardData rewardData = new ResourceRewardData(resourceData, reward.Amount);
            RewardDataHandler rewardDataHandler = new RewardDataHandler(rewardData);
            _allRewardDataHandlers.Add(rewardDataHandler);

            button.onClick.AddListener(() =>
            {
                if (!rewardDataHandler.Claimed)
                {
                    rewardDataHandler.Claim();
                    button.GetComponentInChildren<TextMeshProUGUI>().text = "Claimed";
                    button.image.color = Color.gray;
                }
            });
        }

        _addButton.onClick.AddListener(OnAddButtonClicked);
        _resetAllRewardClaimButton.onClick.AddListener(resetAllRewardClaim);
    }

    void Update()
    {
        for (int i = 0; i < Rewards.Count; i++)
        {
            RewardViews[i].UpdateInfo(
                ResourceManager.Instance.GetResourceData(Rewards[i].Id),
                Rewards[i].Amount
            );
        }

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

    public void resetAllRewardClaim()
    {
        foreach (var rewardDataHandler in _allRewardDataHandlers)
        {
            rewardDataHandler.Renew();
        }

        for (int i = 0; i < Mathf.Min(RewardButtons.Count, Rewards.Count); i++)
        {
            Button button = RewardButtons[i];
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Claim";
            button.image.color = Color.yellow;
        }
    }
}
