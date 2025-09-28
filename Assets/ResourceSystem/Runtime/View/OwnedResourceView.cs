using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ResourceSystem
{
    [RequireComponent(typeof(ResourceView))]
    public class OwnedResourceView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _ownedAmountText;
        [SerializeField] private string _ownedAmountTextFormat = "Owned: {0}";
        [SerializeField] private UnityEvent<int> _onOwnedAmountChangedEvent;

        private ResourceView _resourceView;

        public UnityEvent<int> OnOwnedAmountChangedEvent => _onOwnedAmountChangedEvent;
        public ResourceView ResourceView => _resourceView;
        public OwnedResourceManager OwnedResourceManager => OwnedResourceManager.Instance;

        private void Start()
        {
            _resourceView = GetComponent<ResourceView>();
            _resourceView.OnDataChangedEvent.AddListener(OnDataChanged);
            OnDataChanged(_resourceView.Data, _resourceView.Amount);
            OwnedResourceManager.OnOwnedResourceChanged += OnResourceChanged;
        }

        private void OnDestroy()
        {
            OwnedResourceManager.OnOwnedResourceChanged -= OnResourceChanged;
        }

        private void OnResourceChanged(ResourceData data, int newValue)
        {
            if (_resourceView.Data == null || data == null || _resourceView.Data.Id != data.Id)
            {
                return;
            }

            UpdateAmountText();
        }

        private void OnDataChanged(ResourceData data, int amount)
        {
            if (data == null)
            {
                _ownedAmountText.text = string.Empty;
                return;
            }

            UpdateAmountText();
            _onOwnedAmountChangedEvent?.Invoke(amount);
        }

        private void UpdateAmountText()
        {
            int ownedAmount;

            if (_resourceView.Data is IItemType)
            {
                ownedAmount = OwnedResourceManager.GetOwnedItems(_resourceView.Data.Id).Count;
            }
            else
            {
                ownedAmount = OwnedResourceManager.GetOwnedCurrency(_resourceView.Data.Id).Amount;
            }

            _ownedAmountText.text = string.Format(_ownedAmountTextFormat, ownedAmount);
        }
    }
}