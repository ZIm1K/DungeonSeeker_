using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.UI;
using System.Net;
using Objects.PlayerScripts;
using System;
using OpenCover.Framework.Model;

namespace Inventory
{
    public class InventoryManager : MonoBehaviourPun
    {
        [Header("Inventory")]
        [SerializeField] private GameObject UIPanel;
        [SerializeField] private GameObject CharacterTextsPanel;
        [SerializeField] private Transform inventoryPanel;
        [SerializeField] private float reachDistance = 3f;
        
        [SerializeField] private ItemDatabase itemDatabase;

        private List<InventorySlot> slots = new List<InventorySlot>();
        private Camera mainCamera;
        private bool isOpened;
        public bool isItemOnDrag = false;
        
        [SerializeField] private GameObject slotPrefab;

        [SerializeField] private float rangeOfOpen;

        [Header("Chest Inventory")]
        [SerializeField] private GameObject ChestInventoryPanel;
        
        [SerializeField] private Camera _camera;
        
        public PhotonView currentChest;
        
        [Header("Craft Inventory")]
        [SerializeField] private GameObject CraftInventoryPanel;

        public PhotonView currentCrafter;

        private bool isOpenedCrafter;

        [SerializeField] private InventorySlot[] craftSlots;

        private void Awake()
        {
            UIPanel.SetActive(photonView.IsMine);
            ChestInventoryPanel.SetActive(!photonView.IsMine);
            CharacterTextsPanel.SetActive(photonView.IsMine);
        }

        void Start()
        {
            if (!photonView.IsMine) return;           

            mainCamera = Camera.main;
            for (int i = 0; i < inventoryPanel.childCount; i++)
            {
                if (inventoryPanel.GetChild(i).GetComponent<InventorySlot>() != null)
                {
                    slots.Add(inventoryPanel.GetChild(i).GetComponent<InventorySlot>());
                }
            }
            UIPanel.SetActive(false);
        }

        void Update()
        {
            if (!photonView.IsMine) return;

            if (isItemOnDrag == false)
            {
                if (Input.GetKeyDown(KeyCode.I))
                {
                    if (ChestInventoryPanel.activeSelf)
                    {
                        isOpened = !isOpened;
                        UIPanel.SetActive(isOpened);
                        ChestInventoryPanel.SetActive(isOpened);
                        CharacterTextsPanel.SetActive(!isOpened);
                        Cursor.lockState = isOpened ? CursorLockMode.None : CursorLockMode.Locked;
                        Cursor.visible = isOpened;
                        for (int i = 0; i < ChestInventoryPanel.transform.GetChild(0).childCount; i++)
                        {
                            Destroy(ChestInventoryPanel.transform.GetChild(0).GetChild(i).gameObject);
                        }
                        currentChest = null;
                        gameObject.GetComponent<PlayerControllerWithCC>().isCanRotate = true;
                        return;
                    }
                    else if (CraftInventoryPanel.activeSelf)
                    {
                        isOpened = !isOpened;
                        UIPanel.SetActive(isOpened);
                        CraftInventoryPanel.SetActive(isOpened);
                        CharacterTextsPanel.SetActive(!isOpened);
                        Cursor.lockState = isOpened ? CursorLockMode.None : CursorLockMode.Locked;
                        Cursor.visible = isOpened;
                        for (int i = 0; i < CraftInventoryPanel.transform.GetChild(0).childCount; i++)
                        {
                            craftSlots[i].item = null;
                            craftSlots[i].defenseID = 0;
                            craftSlots[i].amount = 0;
                            craftSlots[i].isEmpty = true;
                            craftSlots[i].iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                            craftSlots[i].SetBasedIcon();
                            craftSlots[i].itemAmountText.text = " ";
                        }
                        currentCrafter = null;
                        gameObject.GetComponent<PlayerControllerWithCC>().isCanRotate = true;
                        return;
                    }

                    isOpened = !isOpened;
                    UIPanel.SetActive(isOpened);
                    CharacterTextsPanel.SetActive(!isOpened);
                    Cursor.lockState = isOpened ? CursorLockMode.None : CursorLockMode.Locked;
                    Cursor.visible = isOpened;
                    gameObject.GetComponent<PlayerControllerWithCC>().isCanRotate = !isOpened;
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (UIPanel.activeSelf)
                    {
                        if (CraftInventoryPanel.activeSelf)
                        {
                            isOpened = !isOpened;
                            UIPanel.SetActive(isOpened);
                            CraftInventoryPanel.SetActive(isOpened);
                            CharacterTextsPanel.SetActive(!isOpened);
                            Cursor.lockState = isOpened ? CursorLockMode.None : CursorLockMode.Locked;
                            Cursor.visible = isOpened;
                            currentCrafter = null;
                            for (int i = 0; i < CraftInventoryPanel.transform.GetChild(0).childCount; i++)
                            {
                                craftSlots[i].item = null;
                                craftSlots[i].defenseID = 0;
                                craftSlots[i].amount = 0;
                                craftSlots[i].isEmpty = true;
                                craftSlots[i].iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                                craftSlots[i].SetBasedIcon();
                                craftSlots[i].itemAmountText.text = " ";
                            }
                            gameObject.GetComponent<PlayerControllerWithCC>().isCanRotate = true;
                            return;
                        }
                        else if (ChestInventoryPanel.activeSelf)
                        {
                            isOpened = !isOpened;
                            UIPanel.SetActive(isOpened);
                            ChestInventoryPanel.SetActive(isOpened);
                            CharacterTextsPanel.SetActive(!isOpened);
                            Cursor.lockState = isOpened ? CursorLockMode.None : CursorLockMode.Locked;
                            Cursor.visible = isOpened;
                            currentChest = null;
                            for (int i = 0; i < ChestInventoryPanel.transform.GetChild(0).childCount; i++)
                            {
                                Destroy(ChestInventoryPanel.transform.GetChild(0).GetChild(i).gameObject);
                            }
                            gameObject.GetComponent<PlayerControllerWithCC>().isCanRotate = true;
                            return;
                        }
                    }
                    else 
                    {
                        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

                        if (Physics.Raycast(ray, out RaycastHit hit, rangeOfOpen))
                        {
                            if (hit.transform.CompareTag("Chest"))
                            {
                                isOpened = !isOpened;
                                UIPanel.SetActive(isOpened);
                                ChestInventoryPanel.SetActive(isOpened);
                                CharacterTextsPanel.SetActive(!isOpened);
                                Cursor.lockState = isOpened ? CursorLockMode.None : CursorLockMode.Locked;
                                Cursor.visible = isOpened;

                                currentChest = hit.collider.GetComponent<PhotonView>();

                                int countOfItems = hit.collider.GetComponent<Chest>().CountOfItems;
                                if (countOfItems > 0)
                                {
                                    for (int i = 0; i < countOfItems; i++)
                                    {
                                        InventorySlot slot = Instantiate(slotPrefab, ChestInventoryPanel.transform.
                                            GetChild(0).transform).GetComponent<InventorySlot>();

                                        if (hit.collider.GetComponent<Chest>().saveChestItems[i].isEmpty == false)
                                        {
                                            ItemScriptableObject item = itemDatabase.GetItemByID(hit.collider.GetComponent<Chest>().saveChestItems[i].ID);
                                            slot.item = item;
                                            slot.defenseID = hit.collider.GetComponent<Chest>().saveChestItems[i].defenseID;
                                            slot.amount = hit.collider.GetComponent<Chest>().saveChestItems[i].ammount;
                                            slot.isEmpty = item == null ? true : false;
                                            if (!slot.isEmpty)
                                            {
                                                slot.SetIcon(item.icon);
                                            }
                                            else 
                                            {
                                                slot.SetBasedIcon();
                                            }
                                            slot.itemAmountText.text = slot.amount.ToString();
                                        }
                                    }
                                }
                                gameObject.GetComponent<PlayerControllerWithCC>().isCanRotate = false;
                            }
                            else if (hit.transform.CompareTag("Crafter"))
                            {
                                isOpened = !isOpened;
                                UIPanel.SetActive(isOpened);
                                CraftInventoryPanel.SetActive(isOpened);
                                CharacterTextsPanel.SetActive(!isOpened);
                                Cursor.lockState = isOpened ? CursorLockMode.None : CursorLockMode.Locked;
                                Cursor.visible = isOpened;

                                currentCrafter = hit.collider.GetComponent<PhotonView>();

                                for (int i = 0; i < craftSlots.Length; i++)
                                {
                                    if (hit.collider.GetComponent<Crafter>().saveCraftItems[i].isEmpty == false)
                                    {
                                        ItemScriptableObject item = itemDatabase.GetItemByID(hit.collider.GetComponent<Crafter>().saveCraftItems[i].ID);
                                        craftSlots[i].item = item;
                                        craftSlots[i].defenseID = 0;
                                        craftSlots[i].amount = 1;
                                        craftSlots[i].isEmpty = false;
                                        craftSlots[i].SetIcon(item.icon);
                                        craftSlots[i].itemAmountText.text = "1";
                                    }
                                    else 
                                    {

                                    }
                                }
                                photonView.RPC("CheckForCraftButtonLocaly", photonView.Owner);
                                gameObject.GetComponent<PlayerControllerWithCC>().isCanRotate = false;
                            }
                        }
                    }                   
                }               
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!UIPanel.activeSelf)
                {
                    TryPickupItem();
                }
            }
        }

        void TryPickupItem()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, reachDistance))
            {
                PhotonView targetPhotonView = hit.collider.gameObject.GetComponent<PhotonView>();
                             
                if (targetPhotonView != null && targetPhotonView.GetComponent<Item>() != null)
                {
                    int defenseID = 0; 
                    if (targetPhotonView.gameObject.GetComponent<DefenseItem>() != null)
                    {
                        defenseID = hit.collider.gameObject.GetComponent<DefenseItem>().ID;
                    }
                    string itemID = hit.collider.gameObject.GetComponent<Item>().item.itemID;
                    int amount = hit.collider.gameObject.GetComponent<Item>().amount;

                    if (CheckEmptyInInventory() == true)
                    {
                        photonView.RPC("RPC_AddItemToInventory", RpcTarget.All, itemID, amount, defenseID);

                        if (!targetPhotonView.IsMine)
                        {
                            if (PhotonNetwork.IsMasterClient)
                            {
                                PhotonNetwork.Destroy(hit.collider.gameObject);
                            }
                            else
                            {
                                targetPhotonView.RPC("RPC_RequestDestroy", RpcTarget.MasterClient, targetPhotonView.ViewID);
                            }
                        }
                        else
                        {
                            PhotonNetwork.Destroy(hit.collider.gameObject);
                        }
                    }
                    else 
                    {
                        Debug.Log("You have full inventory!!!");
                    }
                }
            }
        }
        public ItemScriptableObject ItemReturner(string id)
        {
            return itemDatabase.GetItemByID(id);
        }
        public bool CheckEmptyInInventory() 
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.isEmpty)
                {
                    return true;                    
                }
            }
            return false;
        }
        [PunRPC]
        void RPC_AddItemToInventory(string itemID, int amount, int defenseID)
        {           
            AddItem(itemID, amount, defenseID);
        }
        void AddItem(string itemID, int amount, int defenseID)
        {
            ItemScriptableObject item = itemDatabase.GetItemByID(itemID);
            if (item != null)
            {
                foreach (InventorySlot slot in slots)
                {
                    if (slot.item == item)
                    {
                        if (slot.amount + amount <= item.maximumAmount)
                        {
                            slot.amount += amount;
                            slot.itemAmountText.text = slot.amount.ToString();
                            return;
                        }
                        break;
                    }
                }
                foreach (InventorySlot slot in slots)
                {
                    if (slot.isEmpty)
                    {
                        slot.item = item;
                        slot.amount = amount;
                        slot.isEmpty = false;
                        slot.SetIcon(item.icon);
                        slot.itemAmountText.text = amount.ToString();
                        slot.defenseID = defenseID;
                        break;
                    }
                }
            }
        }
        public void UpdateSlotInOnlineLocalySent(int slotID) 
        {
            List<PhotonView> playerPhotonViews = PlayerViewManager.Instance.playersPhotonViews;

            foreach (PhotonView playerPhotonView in playerPhotonViews) 
            {
                if (playerPhotonView.ViewID != photonView.ViewID)
                {
                    if (currentChest != null)
                    {
                        playerPhotonView.RPC("UpdateSlotInOnlineLocaly", RpcTarget.Others, currentChest.ViewID, 0, slotID);

                    }
                    else if (currentCrafter != null)
                    {
                        playerPhotonView.RPC("UpdateSlotInOnlineLocaly", RpcTarget.Others, 0, currentCrafter.ViewID, slotID);
                    }
                    else 
                    {
                        Debug.LogWarning("You are not opened something");
                    }
                }                
            }
        }

        [PunRPC]       
        private void UpdateSlotInOnlineLocaly(int chestID, int crafterID, int slotID) 
        {
            InventorySlot slot;
            ItemScriptableObject item;
            if (currentChest == null)
            {
                if (currentCrafter != null)
                {
                    if (currentCrafter.ViewID != crafterID)
                    {
                        return;
                    }
                    else 
                    {
                        slot = CraftInventoryPanel.transform.GetChild(0).GetChild(slotID).GetComponent<InventorySlot>();
                        
                        item = itemDatabase.GetItemByID(currentCrafter.GetComponent<Crafter>().saveCraftItems[slotID].ID);
                    }
                }
                else 
                {
                    return;
                }
            }            
            else if (currentChest.ViewID != chestID)
            {
                return;
            }
            else
            {
                slot = ChestInventoryPanel.transform.GetChild(0).GetChild(slotID).GetComponent<InventorySlot>();

                item = itemDatabase.GetItemByID(currentChest.GetComponent<Chest>().saveChestItems[slotID].ID);
            }
            if (item != null)
            {
                slot.item = item;
                if (currentChest != null)
                {
                    slot.defenseID = currentChest.GetComponent<Chest>().saveChestItems[slotID].defenseID;
                    slot.amount = currentChest.GetComponent<Chest>().saveChestItems[slotID].ammount;
                }
                else if (currentCrafter != null) 
                {
                    slot.defenseID = 0;
                    slot.amount = 1;
                }
                slot.isEmpty = false;
                slot.SetIcon(item.icon);
                slot.itemAmountText.text = slot.amount.ToString();
            }
            else 
            {
                slot.item = null;
                slot.amount = 0;
                slot.isEmpty = true;
                slot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                slot.SetBasedIcon();
                slot.itemAmountText.text = "";
                slot.defenseID = 0;
            }
            if (currentCrafter != null)
            { 
                photonView.RPC("CheckForCraftButtonLocaly", photonView.Owner);
            }
        }

        [PunRPC]
        private void PlayAudio(string audioClipPath)
        {
            AudioClip clip = Resources.Load<AudioClip>(audioClipPath);
            PlayAudioLocally(clip);
        }
        private void PlayAudioLocally(AudioClip attackSound)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = attackSound;
            source.maxDistance = 20f;
            source.spatialBlend = 1f;
            source.volume = 0.1f;
            source.Play();
            Destroy(source, attackSound.length);
        }
    }
}