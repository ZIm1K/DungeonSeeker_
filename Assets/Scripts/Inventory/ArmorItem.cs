using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor Item", menuName = "Inventory/Items/New Armor Item")]
public class ArmorItem : ItemScriptableObject
{
    public int defense;
    public float nerfSpeed;
    private void Start()
    {
        itemType = ItemType.Armor;
    }
}
