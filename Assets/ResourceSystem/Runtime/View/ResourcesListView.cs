using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ResourceSystem
{
    public class ResourcesListView : MonoBehaviour
    {
        [SerializeField] private List<ResourceView> _resourceViewPools;

        public void UpdateInfo(IEnumerable<ResourceUsageData> resourceList)
        {
            if (resourceList == null || resourceList.Count() == 0)
            {
                DeactivePool(0);
                return;
            }

            int index = 0;
            foreach (ResourceUsageData reward in resourceList)
            {
                ResourceView resourceView = GetPooledResourceView(index);
                resourceView.UpdateInfo(reward.ResourceData, reward.Amount);
                resourceView.gameObject.SetActive(true);
                index++;
            }

            DeactivePool(index);
        }

        private ResourceView GetPooledResourceView(int index)
        {
            if (index < _resourceViewPools.Count)
            {
                return _resourceViewPools[index];
            }

            ResourceView template = _resourceViewPools[0];
            ResourceView resourceView = Instantiate(template, template.transform.parent);
            _resourceViewPools.Add(resourceView);

            return resourceView;
        }

        private void DeactivePool(int startIndex)
        {
            for (int i = startIndex; i < _resourceViewPools.Count; i++)
            {
                _resourceViewPools[i].gameObject.SetActive(false);
            }
        }
    }
}