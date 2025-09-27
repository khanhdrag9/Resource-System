namespace ResourceSystem
{
    public class ResourceRewardData
    {
        public readonly ResourceData ResourceData;
        public int Amount;

        public ResourceRewardData(ResourceData resourceData, int amount)
        {
            ResourceData = resourceData;
            Amount = amount;
        }
    }
}