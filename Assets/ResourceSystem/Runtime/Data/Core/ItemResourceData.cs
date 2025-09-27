using UnityEngine;

namespace ResourceSystem
{
    public class ItemResourceData : VisualResourceData, IItemType
    {
        public ItemResourceData(int id) : base(id) { }

        public ItemResourceData(int id, string name, string desc, Sprite icon, int rarity) : base(id, name, desc, icon, rarity)
        {
        }
    }
}