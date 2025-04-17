using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet Item", menuName = "Inventory/Items/New Bulllet Item")]
public class BulletItem : ItemScriptableObject
{
    public int bulletAmmount;
    private void Start()
    {
        itemType = ItemType.Bullet;
    }
}
