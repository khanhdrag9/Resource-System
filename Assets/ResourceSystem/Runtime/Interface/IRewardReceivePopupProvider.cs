using System.Collections.Generic;

namespace ResourceSystem
{
    public interface IResourceListViewProvider
    {
        public void ShowList(IEnumerable<ResourceUsageData> datas);
    }
}