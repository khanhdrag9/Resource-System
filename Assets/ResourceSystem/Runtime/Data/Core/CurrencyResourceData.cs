using UnityEngine;

namespace ResourceSystem
{
    public class CurrencyResourceData : VisualResourceData, IHasDefaultAmount
    {
        public int Limit;
        public int DefaultAmount { get; set; }
        public int AutoRefillTimeInSecond;
        public int RefillAmount;

        public bool Refillable => RefillAmount > 0 && AutoRefillTimeInSecond > 0;

        public CurrencyResourceData(int id) : base(id) { }

        public CurrencyResourceData(int id, string name, string description, Sprite icon, int rarity, int limit, int defaultAmount, int autoRefillTimeInSecond, int refillAmount) : base(id, name, description, icon, rarity)
        {
            Limit = limit;
            DefaultAmount = defaultAmount;
            AutoRefillTimeInSecond = autoRefillTimeInSecond;
            RefillAmount = refillAmount;
        }
    }
}