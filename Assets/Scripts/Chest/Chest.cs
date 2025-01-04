using Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

[Serializable]
public class SaveChestItem 
{
    public string ID;
    public int defenseID;
    public int ammount;
    public bool isEmpty;

    public SaveChestItem(string ID, int defenseID, int ammount, bool isEmpty) 
    {
        this.ID = ID;
        this.defenseID = defenseID;
        this.ammount = ammount;
        this.isEmpty = isEmpty;
    }
}

public class Chest : MonoBehaviourPun
{
    public int CountOfItems;

    public SaveChestItem[] saveChestItems;

    private void Start()
    {
        saveChestItems = new SaveChestItem[CountOfItems];
        for (int i = 0; i < CountOfItems; i++)
        {
            saveChestItems[i] = new SaveChestItem(" ", 0, 0, true);
        }
    }

    [PunRPC]
    void AddItemToChest(string ID, int defenseID, int ammount, bool isEmpty, int slotID)
    {
        saveChestItems[slotID] = new SaveChestItem(ID, defenseID, ammount, isEmpty);        
    }
    
    [PunRPC]
    void RemoveItemFromChest(int slotID)
    {
        saveChestItems[slotID] = new SaveChestItem(" ", 0, 0, true);
    }
}
