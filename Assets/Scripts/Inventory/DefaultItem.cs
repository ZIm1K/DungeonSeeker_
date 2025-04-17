using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Item", menuName = "Inventory/Items/New Default Item")]
public class DefaultItem : ItemScriptableObject
{
    private void Start()
    {
        itemType = ItemType.Default;
    }
}
