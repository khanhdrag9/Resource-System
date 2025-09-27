using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ResourceSystem
{
    public class ResourceView : MonoBehaviour
    {
        [Header("Default")]
        public int DefaultResourceId;
        [SerializeField] protected int _defaultAmount;
        [SerializeField] protected bool _includeOwned;

        [Header("UI")]
        [SerializeField] protected Image _rarityImage;
        [SerializeField] protected Image _iconImage;
        [SerializeField] protected TextMeshProUGUI _amountText;
        [SerializeField] protected string _amountTextFormat = "x{0}";
        [SerializeField][Tooltip("0: OwnedResource.Amount, 1: Amount")] protected string _amountTextWithOwnedFormat = "{0}/{1}";

        [Header("Rarity")]
        [SerializeField] protected RarityConfig _rarityConfig;
        [SerializeField] protected string _rarityConfigPath = "ResourceSystem/RarityConfig";

        [Header("Event")]
        [SerializeField] protected UnityEvent<int> _onOwnedAmountChangedEvent;

        public ResourceData Data { get; private set; }
        public OwnedCurrency OwnedResource { get; private set; }
        public int Amount { get; private set; }
        public UnityEvent<int> OnOwnedAmountChangedEvent => _onOwnedAmountChangedEvent;

        private void Start()
        {
            if (Data == null)
            {
                if (_includeOwned)
                {
                    UpdateInfo(ResourceManager.Instance.GetOwnedResource(DefaultResourceId), _defaultAmount);
                }
                else
                {
                    UpdateInfo(DefaultResourceId, _defaultAmount);
                }
            }
        }

        public void UpdateInfo(int id, int amount)
        {
            UpdateInfo(ResourceManager.Instance.GetResourceData(id), amount);
        }

        public void UpdateInfo(OwnedCurrency ownedResource, int amount)
        {
            if (OwnedResource != null)
            {
                OwnedResource.OnAmountChanged -= OnOwnedAmountChanged;
            }

            OwnedResource = ownedResource;
            if (ownedResource == null)
            {
                return;
            }

            ownedResource.OnAmountChanged += OnOwnedAmountChanged;
            UpdateInfo(ownedResource.Data, amount);
        }

        public void UpdateInfo(ResourceData data, int amount)
        {
            if (data == null)
            {
                Debug.LogError("CurrencyView: Data is null");
                return;
            }

            Data = data;
            Amount = amount;
            UpdateUI();
        }

        public virtual void UpdateUI()
        {
            if (_rarityImage)
            {
                _rarityImage.sprite = GetRaritySprite();
            }

            if (_iconImage)
            {
                _iconImage.sprite = GetResourceIcon();
            }

            if (_amountText)
            {
                _amountText.text = GetAmountText();
            }
        }

        protected virtual Sprite GetRaritySprite()
        {
            if (_rarityConfig == null)
            {
                _rarityConfig = Resources.Load<RarityConfig>(_rarityConfigPath);

                if (_rarityConfig == null)
                {
                    return null;
                }
            }

            return _rarityConfig.Get(Data is IHasRarity hasRarity ? hasRarity.Rarity : 0).UI;
        }

        protected virtual Sprite GetResourceIcon()
        {
            return Data is IHasIcon hasIcon ? hasIcon.Icon : null;
        }

        protected virtual string GetAmountText()
        {
            if (Amount <= 1)
            {
                return "";
            }

            if (OwnedResource != null)
            {
                return string.Format(_amountTextWithOwnedFormat, OwnedResource.Amount.ToString(), Amount.ToString());
            }

            return string.Format(_amountTextFormat, Amount.ToString());
        }

        private void OnOwnedAmountChanged(int amount)
        {
            if (_amountText)
            {
                _amountText.text = GetAmountText();
            }

            _onOwnedAmountChangedEvent?.Invoke(amount);
        }
    }
}