using System;
using System.Collections.Generic;
using Inventory;
using Objects.Weapon.Fireball;
using Objects.Weapon.Bow;
using Photon.Pun;
using Photon.Realtime;
using ScriptableObjects.Weapons;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace Objects.Weapon
{
    public class WeaponManager : MonoBehaviourPun
    {
        public Weapon[] weapons;
        public int currentWeaponIndex = 0;

        [SerializeField] private GameObject weaponSlot1;
        [SerializeField] private GameObject weaponSlot2;

        public Image reloadImage;

        public GameObject posTarget;

        void Start()
        {
            if (!photonView.IsMine) return;

            weaponSlot1.GetComponent<InventorySlot>().OnChangeItems += OnChangeSlot1;//
            weaponSlot2.GetComponent<InventorySlot>().OnChangeItems += OnChangeSlot2;//
        }

        void Update()
        {
            if (!photonView.IsMine) return;

            int previousWeaponIndex = currentWeaponIndex;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (weapons[currentWeaponIndex] != null)
                {
                    if (!weapons[currentWeaponIndex].IsReloading)
                    {
                        currentWeaponIndex = 0;
                        posTarget.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                        photonView.RPC("Activator", RpcTarget.Others, 
                            posTarget.transform.GetChild(1).GetChild(0).gameObject.GetComponent<PhotonView>().ViewID, false);
                    }
                }
                else
                {
                    currentWeaponIndex = 0;
                }

                if (weapons[currentWeaponIndex] != null)
                {
                    posTarget.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    photonView.RPC("Activator", RpcTarget.Others,
                            posTarget.transform.GetChild(0).GetChild(0).gameObject.GetComponent<PhotonView>().ViewID, true);
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (weapons[currentWeaponIndex] != null)
                {
                    if (!weapons[currentWeaponIndex].IsReloading)
                    {
                        currentWeaponIndex = 1;
                        posTarget.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                        photonView.RPC("Activator", RpcTarget.Others,
                            posTarget.transform.GetChild(0).GetChild(0).gameObject.GetComponent<PhotonView>().ViewID, false);
                    }
                }
                else
                {
                    currentWeaponIndex = 1;
                }

                if (weapons[currentWeaponIndex] != null && !weapons[currentWeaponIndex].IsReloading)
                {
                    posTarget.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                    photonView.RPC("Activator", RpcTarget.Others,
                            posTarget.transform.GetChild(1).GetChild(0).gameObject.GetComponent<PhotonView>().ViewID, true);
                }
            }

            if (previousWeaponIndex != currentWeaponIndex)
            {
                SelectWeapon();
                UpdateAmmoUI();
            }

            if (Input.GetButtonDown("Fire1") && weapons[currentWeaponIndex] != null)
            {
                weapons[currentWeaponIndex].Use();
            }

            if (Input.GetKeyDown(KeyCode.R) && weapons[currentWeaponIndex] != null)
            {
                weapons[currentWeaponIndex].Reload();
            }
        }
        private void OnChangeSlot1()
        {
            RemoveWeapon(0);
            if (weaponSlot1.GetComponent<InventorySlot>().item != null)
            {
                GameObject usePref = null;
                if (weaponSlot1.GetComponent<InventorySlot>().item.itemID == "6")
                {
                    usePref = (weaponSlot1.GetComponent<InventorySlot>().item as BowItemData).usePrefab;
                    AddBow(0);
                }
                else if (weaponSlot1.GetComponent<InventorySlot>().item.itemID == "7")
                {
                    usePref = (weaponSlot1.GetComponent<InventorySlot>().item as FireBallItemData).usePrefab;
                    AddFireBall(0);
                }
                else if (weaponSlot1.GetComponent<InventorySlot>().item.itemID == "11")///
                {
                    usePref = (weaponSlot1.GetComponent<InventorySlot>().item as SwordItemData).usePrefab;
                    AddSword(0);
                }
                else if (weaponSlot1.GetComponent<InventorySlot>().item.itemID == "12")///
                {
                    usePref = (weaponSlot1.GetComponent<InventorySlot>().item as StaffItem).usePrefab;
                    AddStaff(0);
                }
                else if (weaponSlot1.GetComponent<InventorySlot>().item.itemID == "13")/////////////////////////////////////
                {
                    usePref = (weaponSlot1.GetComponent<InventorySlot>().item as CrucifixItem).usePrefab;
                    AddCrucifix(0);
                }
                SelectWeapon();

                if (posTarget.transform.GetChild(0).childCount > 0)
                {
                    PhotonNetwork.Destroy(posTarget.transform.GetChild(0).GetChild(0).gameObject);
                }

                SetObj(0, weaponSlot1, usePref);
            }
            else
            {
                PhotonNetwork.Destroy(posTarget.transform.GetChild(0).GetChild(0).gameObject);
            }

            UpdateAmmoUI();
        }
        private void OnChangeSlot2()
        {
            RemoveWeapon(1);
            if (weaponSlot2.GetComponent<InventorySlot>().item != null)
            {
                GameObject usePref = null;
                if (weaponSlot2.GetComponent<InventorySlot>().item.itemID == "6")
                {
                    usePref = (weaponSlot2.GetComponent<InventorySlot>().item as BowItemData).usePrefab;
                    AddBow(1);
                }
                else if (weaponSlot2.GetComponent<InventorySlot>().item.itemID == "7")
                {
                    usePref = (weaponSlot2.GetComponent<InventorySlot>().item as FireBallItemData).usePrefab;
                    AddFireBall(1);
                }
                else if (weaponSlot2.GetComponent<InventorySlot>().item.itemID == "11")///
                {
                    usePref = (weaponSlot2.GetComponent<InventorySlot>().item as SwordItemData).usePrefab;
                    AddSword(1);
                }
                else if (weaponSlot2.GetComponent<InventorySlot>().item.itemID == "12")///
                {
                    usePref = (weaponSlot2.GetComponent<InventorySlot>().item as StaffItem).usePrefab;
                    AddStaff(1);
                }
                else if (weaponSlot2.GetComponent<InventorySlot>().item.itemID == "13")/////////////////////////////////////
                {
                    usePref = (weaponSlot2.GetComponent<InventorySlot>().item as CrucifixItem).usePrefab;
                    AddCrucifix(1);
                }
                SelectWeapon();

                if (posTarget.transform.GetChild(1).childCount > 0)
                {
                    PhotonNetwork.Destroy(posTarget.transform.GetChild(1).GetChild(0).gameObject);
                }

                SetObj(1, weaponSlot2, usePref);
            }
            else
            {
                PhotonNetwork.Destroy(posTarget.transform.GetChild(1).GetChild(0).gameObject);
            }

            UpdateAmmoUI();
        }

        private void SetObj(int index, GameObject curSlot, GameObject usePrefab)
        {
            GameObject obj = PhotonNetwork.Instantiate(usePrefab.name,
                        posTarget.transform.GetChild(index).transform.position, Quaternion.identity);
            obj.transform.SetParent(posTarget.transform.GetChild(index));
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;

            bool activ;
            if (currentWeaponIndex == index)
            {
                activ = true;
                obj.SetActive(true);
            }
            else
            {
                activ = false;
                obj.SetActive(false);
            }

            photonView.RPC("SetForOthers", RpcTarget.Others, index, obj.GetComponent<PhotonView>().ViewID, activ);           
        }
        [PunRPC]
        private void SetForOthers(int index, int viewID, bool isActive)
        {
            GameObject obj = PhotonView.Find(viewID).gameObject;
            obj.transform.SetParent(posTarget.transform.GetChild(index));
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.SetActive(isActive);
        }
        [PunRPC]
        private void Activator(int viewID, bool isActive) 
        {
            GameObject obj = PhotonView.Find(viewID).gameObject;
            obj.SetActive(isActive);
        }
        public void AddBow(int numberOfSlot)
        {
            SimpleBow bow = gameObject.AddComponent<SimpleBow>();
            bow.Initialize(reloadImage, numberOfSlot == 0 ? (weaponSlot1.GetComponent<InventorySlot>().item as BowItemData).data.pathOfScObj
                : (weaponSlot2.GetComponent<InventorySlot>().item as BowItemData).data.pathOfScObj);
            weapons[numberOfSlot] = bow;
        }
        public void AddFireBall(int numberOfSlot)
        {
            PlayerShootingFireball fireball = gameObject.AddComponent<PlayerShootingFireball>();
            fireball.Initialize(numberOfSlot == 0 ? (weaponSlot1.GetComponent<InventorySlot>().item as FireBallItemData).data.pathOfScObj
                : (weaponSlot2.GetComponent<InventorySlot>().item as FireBallItemData).data.pathOfScObj);
            weapons[numberOfSlot] = fireball;
        }
        public void AddSword(int numberOfSlot)
        {
            SimpleSword sword = gameObject.AddComponent<SimpleSword>();
            sword.Initialize(numberOfSlot == 0 ? (weaponSlot1.GetComponent<InventorySlot>().item as SwordItemData).data.pathOfScObj 
                : (weaponSlot2.GetComponent<InventorySlot>().item as SwordItemData).data.pathOfScObj);
            weapons[numberOfSlot] = sword;
        }
        public void AddStaff(int numberOfSlot)
        {
            SummonerStaf staff = gameObject.AddComponent<SummonerStaf>();
            staff.Initialize(numberOfSlot == 0 ? (weaponSlot1.GetComponent<InventorySlot>().item as StaffItem).data.pathOfScObj
                : (weaponSlot2.GetComponent<InventorySlot>().item as StaffItem).data.pathOfScObj);
            weapons[numberOfSlot] = staff;
        }
        public void AddCrucifix(int numberOfSlot)
        {
            CrucifixWeapon crucifix = gameObject.AddComponent<CrucifixWeapon>();
            crucifix.Initialize(numberOfSlot == 0 ? (weaponSlot1.GetComponent<InventorySlot>().item as CrucifixItem).data.pathOfScObj
                : (weaponSlot2.GetComponent<InventorySlot>().item as CrucifixItem).data.pathOfScObj);
            weapons[numberOfSlot] = crucifix;
        }
        public void RemoveWeapon(int numberOfSlot) 
        {
            if (weapons[numberOfSlot] as SimpleBow) 
            {
                if ((weapons[numberOfSlot] as SimpleBow).CountOfBulletsInBackpack + (weapons[numberOfSlot] as SimpleBow).CountOfBulletsInWeapon > 0) 
                {                                   
                    int allBullets = (weapons[numberOfSlot] as SimpleBow).CountOfBulletsInBackpack + 
                        (weapons[numberOfSlot] as SimpleBow).CountOfBulletsInWeapon;
                    while (allBullets > 0)
                    {
                        GameObject itemObject = CreateBullet();
                        if (allBullets > (weapons[numberOfSlot] as SimpleBow).MaxBulletsInWeapon)
                        {
                            allBullets -= (weapons[numberOfSlot] as SimpleBow).MaxBulletsInWeapon;
                            itemObject.GetComponent<PhotonView>().RPC("RPC_Ammount", RpcTarget.All, 
                                (weapons[numberOfSlot] as SimpleBow).MaxBulletsInWeapon);
                            //itemObject.GetComponent<Item>().amount = (weapons[numberOfSlot] as SimplePistol).MaxBulletsInWeapon;
                            //If more than 12
                        }
                        else
                        {
                            itemObject.GetComponent<PhotonView>().RPC("RPC_Ammount", RpcTarget.All, allBullets);
                            //itemObject.GetComponent<Item>().amount = allBullets;
                            allBullets = 0;
                        }
                    }
                }                
            }

            Destroy(weapons[numberOfSlot]);

            if (weapons[currentWeaponIndex] != null) 
            {
                if (weapons[currentWeaponIndex] == weapons[numberOfSlot])
                {
                    weapons[currentWeaponIndex].ClearAmmo();
                }
            }
            
            weapons[numberOfSlot] = null;
        }
        GameObject CreateBullet() 
        {
            ItemScriptableObject item = gameObject.GetComponent<InventoryManager>().ItemReturner("14"); //pistol bullet ID
            GameObject itemObject = PhotonNetwork.Instantiate(item.itemPrefab.name,
            transform.position + Vector3.up + transform.forward, Quaternion.identity);
            itemObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
            itemObject.GetComponent<Item>().item = item;
            return itemObject;
        }
        public void NullifySlotData(int index)
        {
            InventorySlot slot = null;
            if (index == 0)
            {
                slot = weaponSlot1.GetComponent<InventorySlot>();
            }
            else if (index == 1)
            {
                slot = weaponSlot2.GetComponent<InventorySlot>();
            }
            else 
            {
                Debug.LogWarning("Unknown slot");
                return;
            }
            slot.item = null;
            slot.amount = 0;
            slot.isEmpty = true;
            slot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            slot.SetBasedIcon();
            slot.itemAmountText.text = "";
            slot.defenseID = 0;
            slot.OnSlotItemChanged();
        }
        void SelectWeapon()
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] != null)
                {
                    weapons[i].enabled = (i == currentWeaponIndex);
                }
            }
        }
        void UpdateAmmoUI()
        {
            if (weapons[currentWeaponIndex] is SimpleBow pistol)
            {
                pistol.UpdateAmmo(pistol.CountOfBulletsInWeapon, pistol.CountOfBulletsInBackpack);
            }
            else if (weapons[currentWeaponIndex] is PlayerShootingFireball fireball)
            {
                fireball.UpdateFireballAmmo("∞");
            }
            else if (weapons[currentWeaponIndex] is SimpleSword sword)
            {
                sword.UpdateSwordAmmo("∞");
            }
            else if (weapons[currentWeaponIndex] is SummonerStaf staff)
            {
                staff.UpdateSwordAmmo("∞");
            }
            else if (weapons[currentWeaponIndex] is CrucifixWeapon crucifix)
            {
                crucifix.UpdateSwordAmmo("1");
            }
            else if (weapons[currentWeaponIndex] == null)
            {
                WeaponEvents.OnClearAmmo.Invoke();
            }
        }
    }
}