using UnityEngine;

namespace ResourceSystem
{
    public class VisualResourceData : ResourceData, IHasName, IHasIcon, IHasRarity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Sprite Icon { get; private set; }
        public int Rarity { get; private set; }

        public VisualResourceData(int id) : base(id) { }

        public VisualResourceData(int id, string name, string description, Sprite icon, int rarity) : base(id)
        {
            Name = name;
            Description = description;
            Icon = icon;
            Rarity = rarity;
        }
    }
}