using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceSystem
{
    [CreateAssetMenu(fileName = "RarityConfig", menuName = "ResourceSystem/RarityConfig")]
    public class RarityConfig : ScriptableObject
    {
        [Serializable]
        public class Info
        {
            public string Name;
            public Sprite UI;
        }

        [SerializeField] private List<Info> _rarities = new();

        public Info Get(int rarity)
        {
            if(rarity < 0 || rarity >= _rarities.Count)
            {
                Debug.LogWarning($"ResourceSystem: Rarity {rarity} not found");
            }

            return _rarities[Mathf.Clamp(rarity, 0, _rarities.Count - 1)];
        }

        public Info Get(string rarityName)
        {
            foreach(Info rarity in _rarities)
            {
                if(rarity.Name.Equals(rarityName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return rarity;
                }
            }

            Debug.LogWarning($"ResourceSystem: Rarity name {rarityName} not found");
            return _rarities[0];
        }
    }
}