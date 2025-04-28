using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class ItemDatabase : MonoBehaviour
    {
        public List<ItemScriptableObject> allItems;

        private static Dictionary<string, ItemScriptableObject> itemDictionary;

        private void Awake()
        {
            itemDictionary = new Dictionary<string, ItemScriptableObject>();
            foreach (ItemScriptableObject item in allItems)
            {
                if (!itemDictionary.ContainsKey(item.itemID))
                {
                    itemDictionary.Add(item.itemID, item);
                }
            }
        }

        public static ItemScriptableObject GetItemByID(string id)
        {
            if (itemDictionary.TryGetValue(id, out ItemScriptableObject item))
            {
                return item;
            }
            Debug.Log($"Item with ID {id} not found!");
            return null;
        }
    }
}