using System.Collections.Generic;
using System.Linq;

namespace ResourceSystem
{
    public static class CostDataHandlerExtensions
    {
        public static IEnumerable<CostDataHandler> ToCostDataHandlers(this IEnumerable<ResourceCostData> costData)
        {
            return costData.Select(cost => new CostDataHandler(cost));
        }

        public static bool Costed(this IEnumerable<CostDataHandler> costData)
        {
            return costData.All(cost => cost.Costed);
        }

        public static bool Enough(this IEnumerable<CostDataHandler> costData)
        {
            return costData.All(cost => cost.Enough());
        }

        public static void Cost(this IEnumerable<CostDataHandler> costData)
        {
            foreach (var cost in costData)
            {
                cost.Cost();
            }
        }

        public static void Renew(this IEnumerable<CostDataHandler> costData)
        {
            foreach (var cost in costData)
            {
                cost.Renew();
            }
        }
    }
}