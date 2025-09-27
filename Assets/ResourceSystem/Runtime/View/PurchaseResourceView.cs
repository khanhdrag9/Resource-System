using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ResourceSystem
{
    [RequireComponent(typeof(ResourceView))]
    public class PurchaseResourceView : MonoBehaviour
    {
        [SerializeField] private ResourceView _rewardResourceView;
        [SerializeField] private ResourceView _priceResourceView;

        [Header("Event")]
        [SerializeField] private UnityEvent<bool> _onPurchasableStateChangedEvent;
        [SerializeField] private UnityEvent<ResourceRewardData> _onPurchased;
        [SerializeField] private UnityEvent<ResourceRewardData> _onNotEnoughPurchased;

        private ResourceRewardData _rewardData;
        private RewardDataHandler _rewardDataHandler;
        private ResourceCostData _costData;
        private CostDataHandler _costDataHandler;

        public ResourceView RewardResourceView
        {
            get => _rewardResourceView;
            set => _rewardResourceView = value;
        }

        public ResourceView PriceResourceView
        {
            get => _priceResourceView;
            set => _priceResourceView = value;
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

            _rewardResourceView.OnDataChangedEvent.AddListener(OnRewardDataChanged);
            _priceResourceView.OnDataChangedEvent.AddListener(OnPriceDataChanged);
        }

        private void OnRewardDataChanged(ResourceData data, int amount)
        {
            if (data == null)
            {
                return;
            }

            _rewardData = new ResourceRewardData(data, amount);
            _rewardDataHandler = new RewardDataHandler(_rewardData);
        }

        private void OnPriceDataChanged(ResourceData data, int amount)
        {
            if (data == null)
            {
                return;
            }

            _costData = new ResourceCostData(data, amount);
            _costDataHandler = new CostDataHandler(_costData);
            _onPurchasableStateChangedEvent.Invoke(_costDataHandler.Enough());
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

            if (!_costDataHandler.Enough())
            {
                _onNotEnoughPurchased.Invoke(_rewardData);
                return;
            }

            _costDataHandler.Cost();
            _rewardDataHandler.Claim();
            _rewardDataHandler.Renew();
            _onPurchased.Invoke(_rewardData);
            _onPurchasableStateChangedEvent.Invoke(_costDataHandler.Enough());
        }
    }
}