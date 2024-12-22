using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boots Item", menuName = "Inventory/Items/New Boots Item")]
public class BootsItem : ItemScriptableObject
{
    public int defense;
    public float buffSpeed;
    private void Start()
    {
        itemType = ItemType.Boots;
    }
}
