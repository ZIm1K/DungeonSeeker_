using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.ProBuilder.MeshOperations;

public class DurabilityDefenseDatabase : MonoBehaviourPun
{
    [SerializeField] private List<ItemScriptableObject> allItems = new List<ItemScriptableObject>();

    [SerializeField] private List<int> allValues = new List<int>();

    //public ItemDatabase itemDatabase;

    public Action OnChangeValues;

    public static DurabilityDefenseDatabase instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void Start()
    {      
        for (int i = 0;i < allItems.Count;i++) 
        {
            switch (allItems[i])
            {              
                case HelmetItem:
                    {
                        allValues.Add((allItems[i] as HelmetItem).defense);
                        break;
                    }
                case ArmorItem:
                    {
                        allValues.Add((allItems[i] as ArmorItem).defense);
                        break;
                    }
                case BootsItem:
                    {
                        allValues.Add((allItems[i] as BootsItem).defense);
                        break;
                    }                  
            }           
        }    
        
    }

    public int GetValueByID(int id)
    {
        //Debug.LogWarning("Id in get  " + id);
        for (int i = 0; i < allValues.Count; i++) 
        {
            if (i + 1 == id) 
            {
                return allValues[i];
            }
        }
        Debug.LogWarning($"Item with ID {id} not found!");
        return 0;
    }
    public void SubDurabilAmmount(int id,int damage) 
    {
        //Debug.LogWarning("Id in sub " + id);
        for (int i = 0; i < id; i++)
        {
            if (i + 1 == id)
            {
                if (allValues[i] > damage)
                {
                    allValues[i] -= damage;
                    photonView.RPC("UpdateValueInOnline", RpcTarget.Others, i, allValues[i]);
                }
                else
                {
                    allValues[i] = 0;
                    photonView.RPC("UpdateValueInOnline", RpcTarget.Others, i, 0);
                }
            }
        }
        OnChangeValues?.Invoke();
    }
    public void HealDefense(int ammount) 
    {
        for (int i = 0; i < allValues.Count; i++) 
        {
            int maxDefense = 0;
            switch (allItems[i])
            {
                case HelmetItem:
                    {
                        maxDefense = (allItems[i] as HelmetItem).defense;
                        break;
                    }
                case ArmorItem:
                    {
                        maxDefense = (allItems[i] as ArmorItem).defense;
                        break;
                    }
                case BootsItem:
                    {
                        maxDefense = (allItems[i] as BootsItem).defense;
                        break;
                    }
            }

            if (allValues[i] < maxDefense)
            {
                if (allValues[i] + ammount <= maxDefense)
                {
                    allValues[i] += ammount;
                }
                else 
                {
                    allValues[i] += maxDefense - allValues[i];
                }
            }
            photonView.RPC("UpdateValueInOnline", RpcTarget.Others, i, allValues[i]);
        }
    }
    [PunRPC]
    void UpdateValueInOnline(int i, int value) 
    {
        allValues[i] = value;
    }
    [PunRPC]
    void AddNewItemInOnline(string ID)
    {      
        ItemScriptableObject item = ItemDatabase.GetItemByID(ID);

        AddNewItemWithType(item);
    }
    [PunRPC]
    void AddInExistedItemInOnline(string ID, int defenseId)
    {        
        ItemScriptableObject item = ItemDatabase.GetItemByID(ID);

        AddinExistedItemWithType(item, defenseId);
    }
    public int OnNewDefenseItemAdded(string ID) 
    {
        ItemScriptableObject item = ItemDatabase.GetItemByID(ID);

        for (int i = 0; i < allItems.Count; i++) 
        {
            if (allItems[i] == null) 
            {
                AddinExistedItemWithType(item, i);
                photonView.RPC("AddInExistedItemInOnline", RpcTarget.Others, item.itemID, i);
                //Debug.LogWarning("Existed defense Id " + i);
                return i; //return defense ID
            }
        }
        AddNewItemWithType(item);

        photonView.RPC("AddNewItemInOnline", RpcTarget.Others,item.itemID);
        //Debug.LogWarning("New defense Id " + (allItems.Count));
        return allItems.Count;  //return defense ID
    }
    void AddNewItemWithType(ItemScriptableObject item) 
    {
        switch (item)
        {
            case HelmetItem:
                {
                    allValues.Add((item as HelmetItem).defense);
                    break;
                }
            case ArmorItem:
                {
                    allValues.Add((item as ArmorItem).defense);
                    break;
                }
            case BootsItem:
                {
                    allValues.Add((item as BootsItem).defense);
                    break;
                }
        }
        allItems.Add(item);
    }
    void AddinExistedItemWithType(ItemScriptableObject item, int defenseID)
    {
        switch (item)
        {
            case HelmetItem:
                {
                    allValues[defenseID] = (item as HelmetItem).defense;
                    allItems[defenseID] = item;
                    break;
                }
            case ArmorItem:
                {
                    allValues[defenseID] = (item as ArmorItem).defense;
                    break;
                }
            case BootsItem:
                {
                    allValues[defenseID] = (item as BootsItem).defense;
                    break;
                }
            default:
                Debug.LogWarning("Error unknown defense");
                break;
        }
        allItems[defenseID] = item;        
    }
    public void ClearNotNeededItems() 
    {
        for (int i = allItems.Count - 1; i > -1; i--) 
        {
            if (allItems[i] == null)
            {
                allItems.RemoveAt(i);
                allValues.RemoveAt(i);
            }
            else 
            {
                return;
            }
        }
    }

    public void DestroySelf() 
    {
        Destroy(gameObject);
    }
    public void RemoveItemFromList(int defenseID) 
    {
        allItems[defenseID] = null;
        allValues[defenseID] = -1;
    }
}
