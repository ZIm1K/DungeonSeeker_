using ExitGames.Client.Photon;
using Inventory;
using Objects.PlayerScripts;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using System;
using System.Net;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

namespace Inventory
{
    public class DragAndDropItem : MonoBehaviourPun, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public InventorySlot oldSlot;
        private Transform player;
        private PhotonView playerView;

        private bool isItemFromChest;

        private bool isItemFromCrafter;

        public enum NameOfAddFunction
        {
            AddItemToChest,
            AddItemToCrafter,
        }
        public enum NameOfInventorySpace
        {
            ChestInventory,
            CraftInventory,
        }
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            oldSlot = transform.GetComponentInParent<InventorySlot>();
            playerView = player.GetComponent<PhotonView>();
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (oldSlot.isEmpty)
                return;
            GetComponent<RectTransform>().position += new Vector3(eventData.delta.x, eventData.delta.y);
        }

        int SlotIndexReturner(GameObject slots, GameObject slotToFind) 
        {
            for (int i = 0; i < slots.transform.childCount; i++)
            {
                if (slots.transform.GetChild(i).gameObject == slotToFind)
                {
                    return i;
                }
            }
            Debug.LogWarning("Error unknown slot");
            return 0;
        }       
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (oldSlot.isEmpty)
                return;

            player.GetComponent<InventoryManager>().isItemOnDrag = true;
            if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.parent.parent.name == NameOfInventorySpace.ChestInventory.ToString())
            {
                isItemFromChest = true;
                isItemFromCrafter = false;

                GameObject chestSlots = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.parent.gameObject;

                RemoveItemFromChest(chestSlots, oldSlot);             
            }
            else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.parent.parent.name == NameOfInventorySpace.CraftInventory.ToString())
            {
                isItemFromChest = false;
                isItemFromCrafter = true;

                GameObject craftSlots = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.parent.gameObject;

                RemoveItemFromCrafter(craftSlots, oldSlot);
            }
            else 
            {
                isItemFromCrafter = false;
                isItemFromChest = false;
            }
            GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.5f);
            GetComponentInChildren<Image>().raycastTarget = false;
            transform.SetParent(transform.parent.parent);
        }
        
        public async void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || oldSlot.isEmpty || eventData.pointerCurrentRaycast.gameObject == null)
                return;

            GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1f);
            GetComponentInChildren<Image>().raycastTarget = true;

            transform.SetParent(oldSlot.transform);
            transform.position = oldSlot.transform.position;

            if (oldSlot.gameObject == eventData.pointerCurrentRaycast.gameObject) 
            {
                if (isItemFromChest)
                {
                    GameObject chestSlots = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;

                    AddItemToChest(chestSlots, oldSlot);
                }
                else if (isItemFromCrafter)
                {
                    GameObject craftSlots = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;

                    AddItemToCrafter(craftSlots, oldSlot);                   
                }
                return;
            }
            
            player.GetComponent<InventoryManager>().isItemOnDrag = false;            
            if (eventData.pointerCurrentRaycast.gameObject.name == "UIPanel")
            {
                //Drop item
                GameObject itemObject = PhotonNetwork.Instantiate(oldSlot.item.itemPrefab.name,
                    player.position + Vector3.up + player.forward, Quaternion.identity);
                itemObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
                itemObject.GetComponent<PhotonView>().RPC("RPC_Ammount", RpcTarget.All, oldSlot.amount);
                if (itemObject.GetComponent<DefenseItem>() != null)
                {
                    itemObject.GetComponent<PhotonView>().RPC("RPC_DefenseID", RpcTarget.All, oldSlot.defenseID);                    
                }
                if (oldSlot.itemTypeToGet != ItemType.Default)
                {
                    if (oldSlot.itemTypeToGet == ItemType.Helmet ||
                        oldSlot.itemTypeToGet == ItemType.Armor ||
                        oldSlot.itemTypeToGet == ItemType.Boots ||
                        oldSlot.itemTypeToGet == ItemType.Charm)
                    {
                        OnUnWearItem(oldSlot.item);
                    }
                }
                NullifySlotData();
                oldSlot.OnSlotItemChanged();
            }
            else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>() != null)
            {
                //Wear item logic
                InventorySlot newSlot = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>();
                bool isChanged = false;
                if (newSlot.itemTypeToGet == oldSlot.itemTypeToGet)
                {
                    if (newSlot.itemTypeToGet == ItemType.Default && oldSlot.itemTypeToGet == ItemType.Default)
                    {
                        isChanged = ExchangeSlotData(newSlot);
                    }
                    else
                    {
                        await OnUnWearItem(newSlot.item);
                        await OnUnWearItem(oldSlot.item);
                        isChanged = ExchangeSlotData(newSlot);
                        await OnWearItem(newSlot.item, newSlot.defenseID);
                        await OnWearItem(oldSlot.item, oldSlot.defenseID);
                    }
                }
                else
                {
                    if (newSlot.itemTypeToGet == oldSlot.item.itemType && oldSlot.itemTypeToGet == ItemType.Default)
                    {
                        await OnUnWearItem(newSlot.item);
                        isChanged = ExchangeSlotData(newSlot);
                        await OnWearItem(newSlot.item, newSlot.defenseID);
                    }
                    else if (newSlot.itemTypeToGet == ItemType.Default)
                    {
                        if (newSlot.item != null)
                        {
                            if (oldSlot.itemTypeToGet == newSlot.item.itemType)
                            {
                                await OnUnWearItem(newSlot.item);
                                isChanged = ExchangeSlotData(newSlot);
                                await OnWearItem(newSlot.item, newSlot.defenseID);
                            }
                        }
                        else
                        {
                            await OnUnWearItem(oldSlot.item);
                            isChanged = ExchangeSlotData(newSlot);
                        }
                    }
                }

                if (isChanged)
                {
                    if (oldSlot.transform.parent.parent.name == NameOfInventorySpace.ChestInventory.ToString() 
                        || newSlot.transform.parent.parent.name == NameOfInventorySpace.ChestInventory.ToString())
                    {
                        ChangeItemsInSomething(NameOfInventorySpace.ChestInventory.ToString(), NameOfAddFunction.AddItemToChest, newSlot);
                    }
                    else if (oldSlot.transform.parent.parent.name == NameOfInventorySpace.CraftInventory.ToString() 
                        || newSlot.transform.parent.parent.name == NameOfInventorySpace.CraftInventory.ToString())
                    {
                        ChangeItemsInSomething(NameOfInventorySpace.CraftInventory.ToString(), NameOfAddFunction.AddItemToCrafter, newSlot);
                    }
                }
                else 
                {
                    if (isItemFromChest)
                    {
                        if (!oldSlot.isEmpty)
                        {
                            GameObject chestSlots = oldSlot.gameObject.transform.parent.gameObject;

                            AddItemToChest(chestSlots, oldSlot);;

                            isItemFromChest = false;
                            return;
                        }
                    }
                    else if (isItemFromCrafter) 
                    {
                        if (!oldSlot.isEmpty)
                        {
                            GameObject craftSlots = oldSlot.gameObject.transform.parent.gameObject;

                            AddItemToCrafter(craftSlots, oldSlot);

                            isItemFromCrafter = false;
                            return;
                        }                      
                    }
                }             
            }
            else 
            {
                if (isItemFromChest)
                {
                    GameObject slots = transform.parent.parent.gameObject;
                    AddItemToChest(slots, oldSlot);
                }
                else if(isItemFromCrafter)
                {
                    GameObject slots = transform.parent.parent.gameObject;
                    AddItemToCrafter(slots, oldSlot);
                }
            }            
        }


        void AddItemToChest(GameObject chestSlots,InventorySlot slotToFind)
        {
            int index = SlotIndexReturner(chestSlots, slotToFind.gameObject);  

            player.GetComponent<InventoryManager>().currentChest.RPC("AddItemToChest", RpcTarget.All, slotToFind.item.itemID,
                        slotToFind.defenseID, slotToFind.amount, slotToFind.isEmpty, index);
            player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(index);
        }       
        void AddItemToCrafter(GameObject craftSlots, InventorySlot slotToFind)
        {
            int index = SlotIndexReturner(craftSlots, slotToFind.gameObject);  
            
            player.GetComponent<InventoryManager>().currentCrafter.RPC("AddItemToCrafter", RpcTarget.All, slotToFind.item.itemID,
                             slotToFind.isEmpty, index);
            player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(index);
        }        

        void RemoveItemFromChest(GameObject chestSlots, InventorySlot slotToFind) 
        {
            int index = SlotIndexReturner(chestSlots, slotToFind.gameObject);

            player.GetComponent<InventoryManager>().currentChest.RPC("RemoveItemFromChest", RpcTarget.All, index);
            player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(index);
        }
        void RemoveItemFromCrafter(GameObject craftSlots, InventorySlot slotToFind)
        {
            int index = SlotIndexReturner(craftSlots, slotToFind.gameObject);

            player.GetComponent<InventoryManager>().currentCrafter.RPC("RemoveItemFromCrafter", RpcTarget.All, index);
            player.GetComponent<InventoryManager>().UpdateSlotInOnlineLocalySent(index);
        }


        void ChangeItemsInSomething(string nameOfSpace, NameOfAddFunction nameOfAddFunction, InventorySlot newSlot) 
        {
            if (oldSlot.transform.parent.parent.name == nameOfSpace && newSlot.transform.parent.parent.name != nameOfSpace) //Item from something to inv
            {
                if (!oldSlot.isEmpty)
                {
                    GameObject slots = oldSlot.transform.parent.gameObject;
                    FunctionChooser(nameOfAddFunction, slots, oldSlot);
                }
            }
            else if (oldSlot.transform.parent.parent.name == nameOfSpace && newSlot.transform.parent.parent.name == nameOfSpace) //Items transfer in something
            {
                GameObject slots = newSlot.transform.parent.gameObject;

                if (oldSlot.isEmpty == false)
                {
                    FunctionChooser(nameOfAddFunction, slots, oldSlot);                    
                }

                FunctionChooser(nameOfAddFunction, slots, newSlot);                
            }
            else if (oldSlot.transform.parent.parent.name != nameOfSpace && newSlot.transform.parent.parent.name == nameOfSpace) //Items from inventory to something
            {
                GameObject slots = newSlot.transform.parent.gameObject;

                FunctionChooser(nameOfAddFunction, slots, newSlot);               
            }
        }
        void FunctionChooser(NameOfAddFunction nameOfFunc, GameObject slots, InventorySlot slotToFind) 
        {
            switch (nameOfFunc) 
            {
                case NameOfAddFunction.AddItemToChest:
                    AddItemToChest(slots, slotToFind);
                    break;
                case NameOfAddFunction.AddItemToCrafter:
                    AddItemToCrafter(slots, slotToFind);
                    break;
                default:
                    Debug.LogWarning("Error unknown func");                    
                    break;
            }             
        }
        private async Task WaitForPlayerView()
        {
            while (playerView == null)
            {
                await Task.Yield();
            }
        }
        public async Task LoadWearItem(ItemScriptableObject item, int defenseID) 
        {
            if (playerView == null) 
            {
                await WaitForPlayerView();
            }
            await OnWearItem(item,defenseID);
            await Task.Yield();
        }
        private async Task OnWearItem(ItemScriptableObject item, int defenseID)
        {
            switch (item)
            {
                case HelmetItem:
                    {
                        HelmetItem itemHelmet = item as HelmetItem;
                        playerView.RPC("WearDefense", playerView.Owner, itemHelmet, defenseID);
                        break;
                    }
                case ArmorItem:
                    {
                        ArmorItem itemArmor = item as ArmorItem;
                        playerView.RPC("WearDefense", playerView.Owner, itemArmor, defenseID);
                        playerView.RPC("NerfSpeed", playerView.Owner, itemArmor.nerfSpeed);
                        break;
                    }
                case BootsItem:
                    {
                        BootsItem itemBoots = item as BootsItem;
                        playerView.RPC("WearDefense", playerView.Owner, itemBoots, defenseID);
                        playerView.RPC("BuffSpeed", playerView.Owner, itemBoots.buffSpeed);
                        break;
                    }
                case CharmItem:
                    {
                        CharmItem itemCharm = item as CharmItem;
                        playerView.RPC("BuffSpeed", playerView.Owner, itemCharm.buffSpeed);
                        playerView.RPC("BuffJumpForce", playerView.Owner, itemCharm.buffJumpForce);
                        break;
                    }
                default:
                    {
                        Debug.Log("This is not buffer");
                        break;
                    }
            }
            await Task.Yield();
        }
        private async Task OnUnWearItem(ItemScriptableObject item)
        {
            switch (item)
            {
                case HelmetItem:
                    {
                        HelmetItem itemHelmet = item as HelmetItem;
                        playerView.RPC("UnWearDefense", playerView.Owner, itemHelmet);
                        break;
                    }
                case ArmorItem:
                    {
                        ArmorItem itemArmor = item as ArmorItem;
                        playerView.RPC("UnWearDefense", playerView.Owner, itemArmor);
                        playerView.RPC("BuffSpeed", playerView.Owner, itemArmor.nerfSpeed);
                        break;
                    }
                case BootsItem:
                    {
                        BootsItem itemBoots = item as BootsItem;
                        playerView.RPC("UnWearDefense", playerView.Owner, itemBoots);
                        playerView.RPC("NerfSpeed", playerView.Owner, itemBoots.buffSpeed);
                        break;
                    }
                case CharmItem:
                    {
                        CharmItem itemCharm = item as CharmItem;
                        playerView.RPC("NerfSpeed", playerView.Owner, itemCharm.buffSpeed);
                        playerView.RPC("NerfJumpForce", playerView.Owner, itemCharm.buffJumpForce);
                        break;
                    }
                default:
                    {
                        Debug.Log("This is not buffer");
                        break;
                    }
            }
            await Task.Yield();
        }       
        void NullifySlotData()
        {
            oldSlot.item = null;
            oldSlot.amount = 0;
            oldSlot.isEmpty = true;
            oldSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            oldSlot.SetBasedIcon();
            oldSlot.itemAmountText.text = "";
            oldSlot.defenseID = 0;
        }
        bool ExchangeSlotData(InventorySlot newSlot)
        {
            InventorySlot savedSlot = new InventorySlot();
            Sprite savedIcon = newSlot.iconGO.GetComponent<Image>().sprite;
            
            SavedSlotSaver(savedSlot, newSlot);

            if (newSlot.item == oldSlot.item && newSlot.item.maximumAmount > 1) 
            {                
                return AddItemToSameSlot(newSlot);
            }           

            SlotExchange(newSlot, oldSlot, oldSlot.iconGO.GetComponent<Image>().sprite);           

            SlotExchange(oldSlot, savedSlot, savedIcon);            
            
            return true;
        }
        void SavedSlotSaver(InventorySlot savedSlot, InventorySlot newSlot) 
        {
            savedSlot.item = newSlot.item;
            savedSlot.amount = newSlot.amount;
            savedSlot.isEmpty = newSlot.isEmpty;

            savedSlot.itemAmountText = newSlot.itemAmountText;
            savedSlot.defenseID = newSlot.defenseID;
        }
        void SlotExchange(InventorySlot slotToSet, InventorySlot slotToGet, Sprite icon) 
        {
            slotToSet.item = slotToGet.item;
            slotToSet.amount = slotToGet.amount;

            slotToSet.defenseID = slotToGet.defenseID;
            slotToSet.isEmpty = slotToGet.isEmpty;            

            if (slotToGet.isEmpty == false)
            { 
                slotToSet.SetIcon(icon);
                slotToSet.itemAmountText.text = slotToGet.amount.ToString();               
            }
            else
            {
                slotToSet.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                slotToSet.SetBasedIcon();
                slotToSet.itemAmountText.text = "";
            }


            slotToSet.OnSlotItemChanged();
        }

        bool AddItemToSameSlot(InventorySlot newSlot)
        {
            if (newSlot.amount + oldSlot.amount <= newSlot.item.maximumAmount)
            {
                newSlot.amount += oldSlot.amount;
                newSlot.itemAmountText.text = newSlot.amount.ToString();
                NullifySlotData();
            }
            else
            {
                int changedAmmount = newSlot.item.maximumAmount - newSlot.amount;
                newSlot.amount += changedAmmount;
                newSlot.itemAmountText.text = newSlot.amount.ToString();
                oldSlot.amount -= changedAmmount;
                oldSlot.itemAmountText.text = oldSlot.amount.ToString();
            }
            return true;
        }
    }  
}