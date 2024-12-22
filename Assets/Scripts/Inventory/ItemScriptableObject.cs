using UnityEngine;

namespace Inventory
{
    public enum ItemType
    {
        Default,
        Food,
        Weapon,
        Instrument,
        Helmet,
        Armor,
        Boots,
        Charm
    }

    [System.Serializable]
    public class ItemScriptableObject : ScriptableObject
    {
        public string itemID; 
        public string itemName = "Default";
        public int maximumAmount = 64;
        public GameObject itemPrefab;
        public Sprite icon;
        public ItemType itemType = ItemType.Default;
        public string itemDescription = "None";
    }
}