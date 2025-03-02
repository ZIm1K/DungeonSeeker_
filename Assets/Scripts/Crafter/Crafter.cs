using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SaveCraftItem
{
    public string ID;
    public bool isEmpty;

    public SaveCraftItem(string ID, bool isEmpty)
    {
        this.ID = ID;
        this.isEmpty = isEmpty;
    }
}
public class Crafter : MonoBehaviour
{ 
    public SaveCraftItem[] saveCraftItems;

    private void Start()
    {
        saveCraftItems = new SaveCraftItem[4];
        for (int i = 0; i < 4; i++)
        {
            saveCraftItems[i] = new SaveCraftItem(" ", true);
        }
    }

    [PunRPC]
    void AddItemToCrafter(string ID, bool isEmpty, int slotID)
    {
        saveCraftItems[slotID] = new SaveCraftItem(ID, isEmpty);
    }

    [PunRPC]
    void RemoveItemFromCrafter(int slotID)
    {
        saveCraftItems[slotID] = new SaveCraftItem(" ", true);
    }
}
