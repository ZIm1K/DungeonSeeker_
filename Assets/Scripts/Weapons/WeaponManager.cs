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
        private int currentWeaponIndex = 0;

        [SerializeField] private GameObject weaponSlot1;
        [SerializeField] private GameObject weaponSlot2;

        public Image reloadImage;

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
                    }
                }
                else
                {
                    currentWeaponIndex = 0;
                }               
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (weapons[currentWeaponIndex] != null)
                {
                    if (!weapons[currentWeaponIndex].IsReloading)
                    {
                        currentWeaponIndex = 1;
                    }
                }
                else
                {
                    currentWeaponIndex = 1;
                   
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
            if (weaponSlot1.GetComponent<InventorySlot>().item != null)
            {
                if (weaponSlot1.GetComponent<InventorySlot>().item.itemID == "6")
                {
                    AddPistol(0);
                }
                else if (weaponSlot1.GetComponent<InventorySlot>().item.itemID == "7") 
                {
                    AddFireBall(0);
                }
                else if (weaponSlot1.GetComponent<InventorySlot>().item.itemID == "11")///
                {
                    AddSword(0);
                }
                SelectWeapon();
            }                       
            else
            {
                RemoveWeapon(0);
            }            
            UpdateAmmoUI();
        }
        private void OnChangeSlot2()
        {
            if (weaponSlot2.GetComponent<InventorySlot>().item != null)
            {
                if (weaponSlot2.GetComponent<InventorySlot>().item.itemID == "6")
                {
                    AddPistol(1);
                }
                else if (weaponSlot2.GetComponent<InventorySlot>().item.itemID == "7")
                {
                    AddFireBall(1);
                }
                else if (weaponSlot2.GetComponent<InventorySlot>().item.itemID == "11")///
                {
                    AddSword(1);
                }
                SelectWeapon();
            }
            else
            {
                RemoveWeapon(1);
            }
            UpdateAmmoUI();
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

            if (weapons[currentWeaponIndex] == weapons[numberOfSlot])
            {
                weapons[currentWeaponIndex].ClearAmmo();
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