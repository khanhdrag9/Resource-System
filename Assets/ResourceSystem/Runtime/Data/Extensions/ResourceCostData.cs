namespace ResourceSystem
{
    public class ResourceCostData
    {
        public readonly ResourceData ResourceData;
        public int Amount;

        public ResourceCostData(ResourceData resourceData, int amount)
        {
            ResourceData = resourceData;
            Amount = amount;
        }
    }
}