using UnityEngine;

namespace ResourceSystem
{
    public class ResourceInfomationPopup : MonoBehaviour
    {
        [SerializeField] private ResourceView _resourceView;
        [SerializeField] private RectTransform _popup;
        [SerializeField] private Vector2 _popupOffset = new Vector2(0, 1);

        [Header("Runtime")]
        [SerializeField] private Transform _anchorTransform;

        private void Start()
        {
            PositionPopup();
        }

        private void OnEnable()
        {
            PositionPopup();
        }

        private void PositionPopup()
        {
            if (_anchorTransform)
            {
                _popup.transform.position = _anchorTransform.position + (Vector3)_popupOffset;
            }
        }

        public void UpdateInfo(Transform anchor, ResourceData data, int amount, OwnedCurrency ownedResource = null)
        {
            _anchorTransform = anchor;
            _resourceView.UpdateInfo(data, amount, ownedResource);
        }
    }
}