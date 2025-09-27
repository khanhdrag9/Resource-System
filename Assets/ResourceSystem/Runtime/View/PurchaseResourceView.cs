using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ResourceSystem
{
    public class PurchaseResourceView : MonoBehaviour
    {
        [SerializeField] private ResourceView _rewardResourceView;
        [SerializeField] private ResourceView _priceResourceView;

        [Header("Event")]
        [SerializeField] private UnityEvent<bool> _onPurchasableStateChangedEvent;
        [SerializeField] private UnityEvent<List<ResourceUsageData>> _onPurchased;
        [SerializeField] private UnityEvent _onNotEnoughPurchased;

        [Header("Runtime")]
        [SerializeField] private bool _isPurchasable;

        private ResourceRewardData _rewardData;
        private RewardDataHandler _rewardDataHandler;
        private ResourceCostData _costData;
        private CostDataHandler _costDataHandler;

        public ResourceManager ResourceManager => ResourceManager.Instance;
        public IResourceListViewProvider ListViewProvider;

        public ResourceView RewardResourceView
        {
            get => _rewardResourceView;
            set
            {
                if (_rewardResourceView != null)
                {
                    _rewardResourceView.OnDataChangedEvent.RemoveListener(OnRewardDataChanged);
                }

                _rewardResourceView = value;

                if (_rewardResourceView != null)
                {
                    _rewardResourceView.OnDataChangedEvent.AddListener(OnRewardDataChanged);
                    OnRewardDataChanged(_rewardResourceView.Data, _rewardResourceView.Amount);
                }
                else
                {
                    OnRewardDataChanged(null, 0);
                }
            }
        }

        public ResourceView PriceResourceView
        {
            get => _priceResourceView;
            set
            {
                if (_priceResourceView != null)
                {
                    _priceResourceView.OnDataChangedEvent.RemoveListener(OnPriceDataChanged);
                }

                _priceResourceView = value;

                if (_priceResourceView != null)
                {
                    _priceResourceView.OnDataChangedEvent.AddListener(OnPriceDataChanged);
                    OnPriceDataChanged(_priceResourceView.Data, _priceResourceView.Amount);
                }
                else
                {
                    OnPriceDataChanged(null, 0);
                }
            }
        }

        private void Start()
        {
            if (_rewardResourceView == null)
            {
                Debug.LogError("PurchaseResourceView: RewardResourceView is missing.");
                return;
            }

            if (_priceResourceView == null)
            {
                Debug.LogError("PurchaseResourceView: PriceResourceView is missing.");
                return;
            }

            // Use Setter to ensure event subscription and initial data handling
            RewardResourceView = _rewardResourceView;
            PriceResourceView = _priceResourceView;

            ResourceManager.OnResourceChanged += OnGlobalResourceChanged;
        }

        private void OnDestroy()
        {
            if (ResourceManager != null)
            {
                ResourceManager.OnResourceChanged -= OnGlobalResourceChanged;
            }
        }

        private void OnRewardDataChanged(ResourceData data, int amount)
        {
            if (data == null)
            {
                _rewardData = null;
                _rewardDataHandler = null;
                return;
            }

            _rewardData = new ResourceRewardData(data, amount);
            _rewardDataHandler = new RewardDataHandler(_rewardData);
        }

        private void OnPriceDataChanged(ResourceData data, int amount)
        {
            if (data == null)
            {
                _costData = null;
                _costDataHandler = null;
                _isPurchasable = false;
                _onPurchasableStateChangedEvent.Invoke(_isPurchasable);
                return;
            }

            _costData = new ResourceCostData(data, amount);
            _costDataHandler = new CostDataHandler(_costData);
            _isPurchasable = _costDataHandler.Enough();
            _onPurchasableStateChangedEvent.Invoke(_isPurchasable);
        }

        private void OnGlobalResourceChanged(ResourceData data, int amount)
        {
            if (_costData != null && _costDataHandler != null && _costData.ResourceData.Id == data.Id)
            {
                bool newPurchasableState = _costDataHandler.Enough();
                if (newPurchasableState != _isPurchasable)
                {
                    _isPurchasable = newPurchasableState;
                    _onPurchasableStateChangedEvent.Invoke(_isPurchasable);
                }
            }
        }

        public void TryPurchase()
        {
            if (_rewardDataHandler == null)
            {
                Debug.LogWarning("PurchaseResourceView: RewardDataHandler is null");
                return;
            }

            if (_costDataHandler == null)
            {
                Debug.LogWarning("PurchaseResourceView: CostDataHandler is null");
                return;
            }

            if(!_isPurchasable)
            {
                return;
            }

            if (!_costDataHandler.Enough())
            {
                _onNotEnoughPurchased.Invoke();
                return;
            }

            _costDataHandler.Cost();

            List<RewardDataHandler> rewardHandlers = new List<RewardDataHandler> { _rewardDataHandler };
            List<ResourceUsageData> claimedResources = rewardHandlers.Claim();
            rewardHandlers.Renew();

            _onPurchased.Invoke(claimedResources);
            _onPurchasableStateChangedEvent.Invoke(_costDataHandler.Enough());
        }
    }
}