using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName ="Food Item",menuName ="Inventory/Items/New Food Item")]
    public class FoodItem : ItemScriptableObject
    {
        public int healAmount;

        private void Start()
        {
            itemType = ItemType.Food;
        }
    }
}
