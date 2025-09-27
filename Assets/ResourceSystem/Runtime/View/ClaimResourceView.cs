using UnityEngine;
using UnityEngine.Events;

namespace ResourceSystem
{
    public class ClaimResourceView : MonoBehaviour
    {
        [SerializeField] private ResourceView _rewardResourceView;

        [Header("Event")]
        [SerializeField] private UnityEvent<bool> _onClaimableStateChangedEvent;
        [SerializeField] private UnityEvent<ResourceRewardData> _onClaimed;
        [SerializeField] private UnityEvent<ResourceRewardData> _onAlreadyClaimed;

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
            _onClaimableStateChangedEvent.Invoke(!_rewardDataHandler.Claimed);
        }

        public void TryClaim()
        {
            if (_rewardDataHandler == null)
            {
                Debug.LogWarning("ClaimResourceView: RewardDataHandler is null");
                return;
            }

            if (_rewardDataHandler.Claimed)
            {
                _onAlreadyClaimed.Invoke(_rewardData);
                return;
            }

            _rewardDataHandler.Claim();
            _onClaimableStateChangedEvent.Invoke(!_rewardDataHandler.Claimed);
            _onClaimed.Invoke(_rewardData);
        }

        public void MarkAsClaimed()
        {
            if (_rewardDataHandler == null)
            {
                Debug.LogWarning("ClaimResourceView: RewardDataHandler is null");
                return;
            }

            if (_rewardDataHandler.Claimed)
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

            if (!_rewardDataHandler.Claimed)
            {
                return;
            }

            _rewardDataHandler.Renew();
            _onClaimableStateChangedEvent.Invoke(true);
        }
    }
}