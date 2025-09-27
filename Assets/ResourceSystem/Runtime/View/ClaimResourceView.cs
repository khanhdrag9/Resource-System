using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ResourceSystem
{
    public class ClaimResourceView : MonoBehaviour
    {
        [SerializeField] private ResourceView _rewardResourceView;

        [Header("Event")]
        [SerializeField] private UnityEvent<bool> _onClaimableStateChangedEvent;
        [SerializeField] private UnityEvent<List<ResourceUsageData>> _onClaimed;
        [SerializeField] private UnityEvent _onAlreadyClaimed;

        private ResourceRewardData _rewardData;
        private RewardDataHandler _rewardDataHandler;

        public ResourceView RewardResourceView
        {
            get => _rewardResourceView;
            set => _rewardResourceView = value;
        }

        private void Start()
        {
            if (_rewardResourceView == null)
            {
                _rewardResourceView = GetComponent<ResourceView>();
            }

            if (_rewardResourceView == null)
            {
                Debug.LogError("ClaimResourceView: RewardResourceView is missing.");
                return;
            }

            _rewardResourceView.OnDataChangedEvent.AddListener(OnDataChanged);
            OnDataChanged(_rewardResourceView.Data, _rewardResourceView.Amount);
        }

        private void OnDataChanged(ResourceData data, int amount)
        {
            if (data == null)
            {
                return;
            }

            _rewardData = new ResourceRewardData(data, amount);
            _rewardDataHandler = new RewardDataHandler(_rewardData);
            _onClaimableStateChangedEvent.Invoke(!_rewardDataHandler.AlreadyClaimed);
        }

        public void TryClaim()
        {
            if (_rewardDataHandler == null)
            {
                Debug.LogWarning("ClaimResourceView: RewardDataHandler is null");
                return;
            }

            if (_rewardDataHandler.AlreadyClaimed)
            {
                _onAlreadyClaimed.Invoke();
                return;
            }

            List<RewardDataHandler> rewardHandlers = new List<RewardDataHandler> { _rewardDataHandler };
            List<ResourceUsageData> claimedResources = rewardHandlers.Claim();

            _onClaimableStateChangedEvent.Invoke(!_rewardDataHandler.AlreadyClaimed);
            _onClaimed.Invoke(claimedResources);
        }

        public void MarkAsClaimed()
        {
            if (_rewardDataHandler == null)
            {
                Debug.LogWarning("ClaimResourceView: RewardDataHandler is null");
                return;
            }

            if (_rewardDataHandler.AlreadyClaimed)
            {
                return;
            }

            _rewardDataHandler.MarkClaimed();
            _onClaimableStateChangedEvent.Invoke(false);
        }

        public void Renew()
        {
            if (_rewardDataHandler == null)
            {
                Debug.LogWarning("ClaimResourceView: RewardDataHandler is null");
                return;
            }

            if (!_rewardDataHandler.AlreadyClaimed)
            {
                return;
            }

            _rewardDataHandler.Renew();
            _onClaimableStateChangedEvent.Invoke(true);
        }
    }
}