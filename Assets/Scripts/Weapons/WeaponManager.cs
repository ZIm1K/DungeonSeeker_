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
            //InitializeWeapons();

            if (!photonView.IsMine) return;

            //weapons[0] = null;
            //weapons[1] = null;
            
            //weapons.Add(null);
            //weapons.Add(null);

            //SelectWeapon();
            //UpdateAmmoUI();
            weaponSlot1.GetComponent<InventorySlot>().OnChangeItems += OnChange;//
            weaponSlot2.GetComponent<InventorySlot>().OnChangeItems += OnChange2;//
        }

        private void InitializeWeapons()
        {
            PlayerShootingFireball fireballWeapon = gameObject.AddComponent<PlayerShootingFireball>();
            fireballWeapon.Initialize();
            //weapons.Add(fireballWeapon);

            SimplePistol pistol = gameObject.AddComponent<SimplePistol>();
            //pistol.Initialize();
            //weapons.Add(pistol);
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
        private void OnChange()//
        {
            if (weaponSlot1.GetComponent<InventorySlot>().item != null)
            {
                //weapons.Remove(null);
                AddWeapon();
                SelectWeapon();
            }                       
            else
            {
                RemoveWeapon();
            }
            UpdateAmmoUI();
        }
        private void OnChange2()//
        {
            if (weaponSlot2.GetComponent<InventorySlot>().item != null)
            {
                //weapons.Remove(null);
                AddWeapon2();
                SelectWeapon();
            }
            else
            {
                RemoveWeapon2();
            }
            UpdateAmmoUI();
        }
        public void AddWeapon() //
        {
            SimplePistol pistol = gameObject.AddComponent<SimplePistol>();
            pistol.Initialize(reloadImage);
            //weapons.Add(pistol);
            weapons[0] = pistol;
        }
        public void AddWeapon2() //
        {
            SimplePistol pistol = gameObject.AddComponent<SimplePistol>();
            pistol.Initialize(reloadImage);
            //weapons.Add(pistol);
            weapons[1] = pistol;
        }
        public void RemoveWeapon() 
        {
            weapons[0] = null;
        }
        public void RemoveWeapon2()
        {
            weapons[1] = null;
        }
        void SelectWeapon()
        {
            for (int i = 0; i < weapons.Length; i++) //Count
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