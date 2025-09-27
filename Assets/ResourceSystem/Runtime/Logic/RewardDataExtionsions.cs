using System.Collections.Generic;
using System.Linq;

namespace ResourceSystem
{
    public static class RewardDataExtensions
    {
        public static IResourceListViewProvider ListViewProvider;

        public static IEnumerable<RewardDataHandler> ToRewardDataHandlers(this IEnumerable<ResourceRewardData> rewardData)
        {
            return rewardData.Select(reward => new RewardDataHandler(reward));
        }

        public static bool AlreadyClaimed(this IEnumerable<RewardDataHandler> rewardData)
        {
            return rewardData.All(reward => reward.AlreadyClaimed);
        }

        public static List<ResourceUsageData> Claim(this IEnumerable<RewardDataHandler> rewardData, bool showList = true, IResourceListViewProvider overrideListViewProvider = null)
        {
            List<ResourceUsageData> result = new();
            foreach (var reward in rewardData)
            {
                List<ResourceUsageData> rewardClaims = reward.Claim();
                result.AddRange(rewardClaims);
            }

            if (showList)
            {
                IResourceListViewProvider listViewProvider = overrideListViewProvider != null ? overrideListViewProvider : ListViewProvider;
                if (listViewProvider != null)
                {
                    listViewProvider.ShowList(result);
                }
            }

            return result;
        }

        public static void Renew(this IEnumerable<RewardDataHandler> rewardData)
        {
            foreach (var reward in rewardData)
            {
                reward.Renew();
            }
        }
    }
}