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
        private OwnedCurrency _ownedResource;

        public UnityEvent<int> OnOwnedAmountChangedEvent => _onOwnedAmountChangedEvent;
        public ResourceView ResourceView => _resourceView;
        public OwnedCurrency OwnedResource => _ownedResource;
        public ResourceManager ResourceManager => ResourceManager.Instance;

        private void Start()
        {
            _resourceView = GetComponent<ResourceView>();
            _resourceView.OnDataChangedEvent.AddListener(OnDataChanged);
            OnDataChanged(_resourceView.Data, _resourceView.Amount);
        }

        private void OnDestroy()
        {
            RemoveOwnedResourceListener();
        }

        private void OnDataChanged(ResourceData data, int amount)
        {
            if (data == null)
            {
                _ownedAmountText.text = string.Empty;
                return;
            }

            RemoveOwnedResourceListener();
            NewOwnedResourceListener(ResourceManager.GetOwnedCurrency(data.Id));
            UpdateAmountText();
        }

        private void NewOwnedResourceListener(OwnedCurrency newOwnedResource)
        {
            _ownedResource = newOwnedResource;

            if (_ownedResource != null)
            {
                _ownedResource.OnAmountChanged += OnOwnedAmountChanged;
            }
        }

        private void RemoveOwnedResourceListener()
        {
            if (_ownedResource != null)
            {
                _ownedResource.OnAmountChanged -= OnOwnedAmountChanged;
                _ownedResource = null;
            }
        }

        private void OnOwnedAmountChanged(int amount)
        {
            UpdateAmountText();
            _onOwnedAmountChangedEvent?.Invoke(amount);
        }

        private void UpdateAmountText()
        {
            if (_ownedResource != null)
            {
                _ownedAmountText.text = string.Format(_ownedAmountTextFormat, _ownedResource.Amount);
            }
            else
            {
                _ownedAmountText.text = "";
            }
        }
    }
}