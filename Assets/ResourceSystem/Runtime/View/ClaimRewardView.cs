using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ResourceSystem
{
    [RequireComponent(typeof(ResourceView))]
    public class ClaimRewardView : MonoBehaviour
    {
        [SerializeField] private Button _claimButton;

        [Header("Event")]
        [SerializeField] private UnityEvent<bool> _onClaimableStateChangedEvent;
        [SerializeField] private UnityEvent<ResourceRewardData> _onClaimed;
        [SerializeField] private UnityEvent<ResourceRewardData> _onAlreadyClaimed;

        private ResourceView _resourceView;
        private ResourceRewardData _rewardData;
        private RewardDataHandler _rewardDataHandler;

        private void Start()
        {
            _resourceView = GetComponent<ResourceView>();
            _resourceView.OnDataChangedEvent.AddListener(OnDataChanged);
            OnDataChanged(_resourceView.Data, _resourceView.Amount);
        }

        private void OnEnable()
        {
            if (_claimButton != null)
            {
                _claimButton.onClick.AddListener(OnClaimButtonClicked);
            }
        }

        private void OnDisable()
        {
            if (_claimButton != null)
            {
                _claimButton.onClick.RemoveListener(OnClaimButtonClicked);
            }
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

        private void OnClaimButtonClicked()
        {
            if (_rewardDataHandler == null)
            {
                Debug.LogWarning("ClaimRewardView: RewardDataHandler is null");
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
                Debug.LogWarning("ClaimRewardView: RewardDataHandler is null");
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
                Debug.LogWarning("ClaimRewardView: RewardDataHandler is null");
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