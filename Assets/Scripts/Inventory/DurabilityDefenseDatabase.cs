using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;
using static UnityEditor.Progress;
using Photon.Pun;

public class DurabilityDefenseDatabase : MonoBehaviourPun
{   
    public List<ItemScriptableObject> allItems;

    public List<int> allValues;

    void Start()
    {
        for(int i = 0;i < allItems.Count;i++) 
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
    }
    [PunRPC]
    void UpdateValueInOnline(int i, int value) 
    {
        Debug.LogWarning("Workerdsadwasada");
        allValues[i] = value;
    }
    public int OnNewDefenseItemAdded(ItemScriptableObject item) 
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
        return allItems.Count;  //return defense ID
    }
}
