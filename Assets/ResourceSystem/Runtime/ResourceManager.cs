using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceSystem
{
    public class ResourceManager
    {
        public static ResourceData EmptyResourceData => new ResourceData(0);

        public static ResourceManager Instance { get; private set; } = new();

        private Dictionary<int, ResourceData> _resourceDataMap = new();

        public void AddResourceData(ResourceData resourceData)
        {
            if (_resourceDataMap.ContainsKey(resourceData.Id))
            {
                Debug.LogWarning($"ResourceSystem: Resource data with id {resourceData.Id} already exists");
                return;
            }

            _resourceDataMap.Add(resourceData.Id, resourceData);
        }

        public ResourceData GetResourceData(int id)
        {
            if (!_resourceDataMap.TryGetValue(id, out var resourceData))
            {
                Debug.LogWarning($"ResourceSystem: Resource data with id {id} not found");
                return null;
            }

            return resourceData;
        }
    }
}