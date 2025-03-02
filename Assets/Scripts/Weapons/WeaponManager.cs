using System;
using System.Collections.Generic;
using Inventory;
using Objects.Weapon.Fireball;
using Objects.Weapon.Pistol;
using Photon.Pun;
using ScriptableObjects.Weapons;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace Objects.Weapon
{
    public class WeaponManager : MonoBehaviourPun
    {
        public Weapon[] weapons;
        [SerializeField] private int currentWeaponIndex = 0;

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
                        }
                    }
                    else
                    {
                        currentWeaponIndex = 0;
                    }                                      

                    if (weapons[currentWeaponIndex] != null) 
                    {
                        posTarget.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);                        
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
                        }
                    }
                    else
                    {
                        currentWeaponIndex = 1;
                    }
                    
                    if (weapons[currentWeaponIndex] != null)
                    {
                        posTarget.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);                     
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
                    usePref = (weaponSlot1.GetComponent<InventorySlot>().item as PistolItemData).usePrefab;
                    AddPistol(0);
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
                SelectWeapon();

                if (posTarget.transform.GetChild(0).childCount > 0) 
                {
                    Destroy(posTarget.transform.GetChild(0).GetChild(0).gameObject);
                }
                
                SetObj(0,weaponSlot1,usePref);
            }
            else
            {
                Destroy(posTarget.transform.GetChild(0).GetChild(0).gameObject);
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
                    usePref = (weaponSlot2.GetComponent<InventorySlot>().item as PistolItemData).usePrefab;
                    AddPistol(1);
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
                SelectWeapon();

                if (posTarget.transform.GetChild(1).childCount > 0)
                {
                    Destroy(posTarget.transform.GetChild(1).GetChild(0).gameObject);
                }

                SetObj(1, weaponSlot2, usePref);
            }
            else 
            {
                Destroy(posTarget.transform.GetChild(1).GetChild(0).gameObject);
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

            if (currentWeaponIndex == index)
            {
                obj.SetActive(true);
            }
            else 
            {
                obj.SetActive(false);
            }
        }
        public void AddPistol(int numberOfSlot)
        {
            SimplePistol pistol = gameObject.AddComponent<SimplePistol>();
            pistol.Initialize(reloadImage);
            weapons[numberOfSlot] = pistol;
        }
        public void AddFireBall(int numberOfSlot)
        {
            PlayerShootingFireball fireball = gameObject.AddComponent<PlayerShootingFireball>();
            fireball.Initialize();
            weapons[numberOfSlot] = fireball;
        }
        public void AddSword(int numberOfSlot)
        {
            SimpleSword sword = gameObject.AddComponent<SimpleSword>();
            sword.Initialize();
            weapons[numberOfSlot] = sword;
        }
        public void RemoveWeapon(int numberOfSlot) 
        {
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
            if (weapons[currentWeaponIndex] is SimplePistol pistol)
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
            else if (weapons[currentWeaponIndex] == null)
            {
                WeaponEvents.OnClearAmmo.Invoke();
            }
        }
    }
}