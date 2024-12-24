using Inventory;
using Objects.PlayerScripts;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace Inventory
{
    public class DragAndDropItem : MonoBehaviourPun, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public InventorySlot oldSlot;
        private Transform player;
        private PhotonView playerView;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            oldSlot = transform.GetComponentInParent<InventorySlot>();
            playerView = player.GetComponent<PhotonView>();
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (oldSlot.isEmpty)
                return;
            GetComponent<RectTransform>().position += new Vector3(eventData.delta.x, eventData.delta.y);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (oldSlot.isEmpty)
                return;
            GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.5f);
            GetComponentInChildren<Image>().raycastTarget = false;
            transform.SetParent(transform.parent.parent);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (oldSlot.isEmpty)
                return;
            GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1f);
            GetComponentInChildren<Image>().raycastTarget = true;

            transform.SetParent(oldSlot.transform);
            transform.position = oldSlot.transform.position;

            if (eventData.pointerCurrentRaycast.gameObject == null) return;
            if (eventData.pointerCurrentRaycast.gameObject.name == "UIPanel")
            {
                GameObject itemObject = PhotonNetwork.Instantiate(oldSlot.item.itemPrefab.name,
                    player.position + Vector3.up + player.forward, Quaternion.identity);
                itemObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
                itemObject.GetComponent<Item>().amount = oldSlot.amount;
                if (itemObject.GetComponent<DefenseItem>() != null)
                {
                    OnUnWearItem(oldSlot.item);
                    itemObject.GetComponent<DefenseItem>().ID = oldSlot.defenseID;
                }
                else if (itemObject.GetComponent<Item>().item.itemType == ItemType.Charm) 
                {
                    OnUnWearItem(oldSlot.item);
                }
                NullifySlotData();
                oldSlot.OnSlotItemChanged();
            }
            else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>() != null)
            {
                InventorySlot newSlot = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>();
                if (newSlot.itemTypeToGet == oldSlot.itemTypeToGet)
                {
                    if (newSlot.itemTypeToGet == ItemType.Default && oldSlot.itemTypeToGet == ItemType.Default)
                    {
                        ExchangeSlotData(newSlot);
                    }
                    else
                    {
                        OnUnWearItem(newSlot.item);
                        OnUnWearItem(oldSlot.item);
                        ExchangeSlotData(newSlot);
                        OnWearItem(newSlot.item, oldSlot.defenseID);
                        OnWearItem(oldSlot.item, newSlot.defenseID);
                    }
                }
                else
                {
                    if (newSlot.itemTypeToGet == oldSlot.item.itemType && oldSlot.itemTypeToGet == ItemType.Default)
                    {
                        OnUnWearItem(newSlot.item);
                        ExchangeSlotData(newSlot);
                        OnWearItem(newSlot.item, oldSlot.defenseID);
                    }
                    else if (newSlot.itemTypeToGet == ItemType.Default)
                    {
                        if (newSlot.item != null)
                        {
                            if (oldSlot.itemTypeToGet == newSlot.item.itemType)
                            {
                                OnUnWearItem(newSlot.item);
                                ExchangeSlotData(newSlot);
                                OnWearItem(newSlot.item, oldSlot.defenseID);
                            }
                        }
                        else
                        {
                            OnUnWearItem(oldSlot.item);
                            ExchangeSlotData(newSlot);
                        }
                    }
                }
            }            
        }
        private void OnWearItem(ItemScriptableObject item, int defenseID) 
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
        }
        private void OnUnWearItem(ItemScriptableObject item) 
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
        }       
        void NullifySlotData()
        {
            oldSlot.item = null;
            oldSlot.amount = 0;
            oldSlot.isEmpty = true;
            oldSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            oldSlot.iconGO.GetComponent<Image>().sprite = null;
            oldSlot.itemAmountText.text = "";
            oldSlot.defenseID = 0;
        }
        void ExchangeSlotData(InventorySlot newSlot)
        {
            ItemScriptableObject item = newSlot.item;
            int amount = newSlot.amount;
            bool isEmpty = newSlot.isEmpty;
            Sprite iconGO = newSlot.iconGO.GetComponent<Image>().sprite;
            TMP_Text itemAmountText = newSlot.itemAmountText;
            int defenseID = newSlot.defenseID;

            newSlot.item = oldSlot.item;
            newSlot.amount = oldSlot.amount;
            if (oldSlot.isEmpty == false)
            {
                newSlot.SetIcon(oldSlot.iconGO.GetComponent<Image>().sprite);
                newSlot.itemAmountText.text = oldSlot.amount.ToString();
                newSlot.defenseID = oldSlot.defenseID;
            }
            else
            {
                newSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                newSlot.iconGO.GetComponent<Image>().sprite = null;
                newSlot.itemAmountText.text = "";
            }

            newSlot.isEmpty = oldSlot.isEmpty;
            oldSlot.item = item;
            oldSlot.amount = amount;
            if (isEmpty == false)
            {
                oldSlot.SetIcon(iconGO);
                oldSlot.itemAmountText.text = amount.ToString();
                oldSlot.defenseID = defenseID;
            }
            else
            {
                oldSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                oldSlot.iconGO.GetComponent<Image>().sprite = null;
                oldSlot.itemAmountText.text = "";
            }

            oldSlot.isEmpty = isEmpty;

            newSlot.OnSlotItemChanged();
            oldSlot.OnSlotItemChanged();
        }
    }
}