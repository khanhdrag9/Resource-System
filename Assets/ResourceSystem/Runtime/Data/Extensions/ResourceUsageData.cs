namespace ResourceSystem
{
    public class ResourceUsageData
    {
        public readonly ResourceData ResourceData;
        public int Amount;

        public ResourceUsageData(ResourceData resourceData, int amount)
        {
            ResourceData = resourceData;
            Amount = amount;
        }
    }
}