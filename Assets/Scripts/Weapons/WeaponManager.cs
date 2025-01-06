using System;
using System.Collections.Generic;
using Inventory;
using Objects.Weapon.Fireball;
using Objects.Weapon.Pistol;
using Photon.Pun;
using ScriptableObjects.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Objects.Weapon
{
    public class WeaponManager : MonoBehaviourPun
    {
        //public List<Weapon> weapons = new List<Weapon>();

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
        public void RemoveWeapon(int numberOfSlot) 
        {
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
        }
    }
}