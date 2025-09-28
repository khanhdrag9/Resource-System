using UnityEngine;
using UnityEngine.UI;

namespace ResourceSystem.Example
{
    public class ShowInfoPopupOnClick : MonoBehaviour
    {
        public GameObject InfoPopupPrefab;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                ResourceView resourceView = GetComponent<ResourceView>();

                // Instantiate the popup
                GameObject instance = Instantiate(InfoPopupPrefab, transform.position, Quaternion.identity);
                Canvas canvas = GetComponentInParent<Canvas>();
                instance.transform.SetParent(canvas.transform, false);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.SetAsLastSibling();

                ResourceData resourceData = resourceView.Data;
                OwnedCurrency ownedResourceSaveData = OwnedResourceManager.Instance.GetOwnedCurrency(resourceData.Id);
                instance.GetComponent<ResourceInfomationPopup>().UpdateInfo(
                    transform, resourceView.Data, resourceView.Amount, ownedResourceSaveData
                );
            });
        }
    }
}