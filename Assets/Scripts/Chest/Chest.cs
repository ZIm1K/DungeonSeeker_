using Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using Photon.Realtime;
using Random = UnityEngine.Random;
using Objects.Weapon;
using System.Linq;

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
    public List<ItemScriptableObject> allItems;

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
    public void GenerateItems(List<ItemScriptableObject> allItems, DurabilityDefenseDatabase durabilDatabase) 
    {
        int generatedItems = 0;
        int countOfGenerItems = Random.Range(2, 6);

        while (generatedItems < countOfGenerItems)
        {
            float maxChanses = 0;
            foreach (var item in allItems)
                maxChanses += item.dropChanse;

            float randomChanses = Random.Range(0, maxChanses);

            float current = 0f;

            for (int i = 0; i < allItems.Count; i++)
            {
                current += allItems[i].dropChanse;
                if (randomChanses <= current)
                {                  
                    for (int slot = 0; slot < saveChestItems.Length; slot++)
                    {
                        if (saveChestItems[slot].isEmpty)
                        {
                            int amount = Random.Range(1, allItems[i].maximumAmount + 1);
                            int defenseID = 0;
                            if (allItems[i].itemType == ItemType.Helmet ||
                                allItems[i].itemType == ItemType.Armor ||
                                allItems[i].itemType == ItemType.Boots)
                            {
                                //if (durabilDatabase.allItems == null) 
                                //{
                                //    durabilDatabase.allItems = allItems;
                                //}
                                defenseID = durabilDatabase.OnNewDefenseItemAdded(allItems[i].itemID);
                            }
                            
                            photonView.RPC("AddItemToChest", RpcTarget.All, allItems[i].itemID, defenseID, amount, false, slot);
                            generatedItems++;
                            break;
                        }
                    }
                    break;
                }
            }
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
