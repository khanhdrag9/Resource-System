using System.Collections.Generic;
using System.Linq;

namespace ResourceSystem
{
    public static class RewardDataExtensions
    {
        public static IEnumerable<RewardDataHandler> ToRewardDataHandlers(this IEnumerable<ResourceRewardData> rewardData)
        {
            return rewardData.Select(reward => new RewardDataHandler(reward));
        }

        public static bool Claimed(this IEnumerable<RewardDataHandler> rewardData)
        {
            return rewardData.All(reward => reward.Claimed);
        }

        public static void Claim(this IEnumerable<RewardDataHandler> rewardData)
        {
            foreach (var reward in rewardData)
            {
                reward.Claim();
            }
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