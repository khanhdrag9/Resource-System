using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ResourceSystem
{
    public class ResourceView : MonoBehaviour
    {
        [Header("Default")]
        [SerializeField] private int _defaultResourceId;
        [SerializeField] private int _defaultAmount;
        [SerializeField] private bool _includeOwned;

        [Header("UI")]
        [SerializeField] private Image _rarityImage;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private string _amountTextFormat = "x{0}";
        [SerializeField] private TextMeshProUGUI _ownedAmountText;
        [SerializeField] private string _ownedAmountTextFormat = "Owned: {0}";

        [Header("Rarity")]
        [SerializeField] private RarityConfig _rarityConfig;
        [SerializeField] private string _rarityConfigPath = "ResourceSystem/RarityConfig";

        [Header("Event")]
        [SerializeField] private UnityEvent<int> _onOwnedAmountChangedEvent;

        public ResourceManager ResourceManager => ResourceManager.Instance;
        public ResourceData Data { get; private set; }
        public OwnedCurrency OwnedResource { get; private set; }
        public int Amount { get; private set; }
        public UnityEvent<int> OnOwnedAmountChangedEvent => _onOwnedAmountChangedEvent;

        private void Start()
        {
            LoadDefault();
        }

        private void OnDestroy()
        {
            RemoveOwnedResourceListener();
        }

        private void LoadDefault()
        {
            if (Data != null) return;

            if (_includeOwned)
            {
                OwnedCurrency ownedResource = ResourceManager.GetOwnedResource(_defaultResourceId);
                UpdateInfo(ownedResource.Data, _defaultAmount, ownedResource);
            }
            else
            {
                ResourceData resourceData = ResourceManager.GetResourceData(_defaultResourceId);
                UpdateInfo(resourceData, _defaultAmount);
            }
        }

        public void UpdateInfo(ResourceData data, int amount, OwnedCurrency ownedResource = null)
        {
            if (data == null)
            {
                Debug.LogError("CurrencyView: Data is null");
                return;
            }

            Data = data;
            Amount = amount;

            RemoveOwnedResourceListener();
            NewOwnedResourceListener(ownedResource);
            UpdateUI();
        }

        private void NewOwnedResourceListener(OwnedCurrency newOwnedResource)
        {
            OwnedResource = newOwnedResource;

            if (OwnedResource != null)
            {
                OwnedResource.OnAmountChanged += OnOwnedAmountChanged;
            }
        }

        private void RemoveOwnedResourceListener()
        {
            if (OwnedResource != null)
            {
                OwnedResource.OnAmountChanged -= OnOwnedAmountChanged;
                OwnedResource = null;
            }
        }

        private void OnOwnedAmountChanged(int amount)
        {
            UpdateAmountText();
            _onOwnedAmountChangedEvent?.Invoke(amount);
        }

        public void UpdateUI()
        {
            if (_iconImage && Data is IHasIcon hasIcon)
            {
                _iconImage.sprite = hasIcon.Icon;
            }

            if (_nameText && Data is IHasName hasName)
            {
                _nameText.text = hasName.Name;
                _descriptionText.text = hasName.Description;
            }

            UpdateRarityUI();
            UpdateAmountText();
        }

        private void UpdateRarityUI()
        {
            if (_rarityImage == null)
            {
                return;
            }

            if (_rarityConfig == null)
            {
                _rarityConfig = Resources.Load<RarityConfig>(_rarityConfigPath);

                if (_rarityConfig == null)
                {
                    return;
                }
            }

            _rarityImage.sprite = _rarityConfig.Get(Data is IHasRarity hasRarity ? hasRarity.Rarity : 0).UI;
        }

        private void UpdateAmountText()
        {
            if (_amountText != null)
            {
                _amountText.text = string.Format(_amountTextFormat, Amount.ToString());
            }

            if (OwnedResource != null && _ownedAmountText != null)
            {
                _ownedAmountText.text = string.Format(_ownedAmountTextFormat, OwnedResource.Amount.ToString());
            }
        }


    }
}