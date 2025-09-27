using System.Collections.Generic;
using ResourceSystem;
using UnityEngine;

public class ShowResourceListProviderExample : MonoBehaviour, IResourceListViewProvider
{
    public GameObject Prefab;
    public Canvas Canvas;

    private void Start()
    {
        RewardDataExtensions.ListViewProvider = this;
    }

    public void ShowList(IEnumerable<ResourceUsageData> datas)
    {
        GameObject instance = Instantiate(Prefab, Canvas.transform);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.SetAsLastSibling();
        instance.GetComponent<ResourcesListView>().UpdateInfo(datas);
    }
}
