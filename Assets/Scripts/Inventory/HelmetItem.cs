using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Helmet Item", menuName = "Inventory/Items/New Helmet Item")]
public class HelmetItem : ItemScriptableObject 
{
    public int defense;
    private void Start()
    {
        itemType = ItemType.Helmet;
    }
}
