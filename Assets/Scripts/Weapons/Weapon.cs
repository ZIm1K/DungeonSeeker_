using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Objects.Weapon
{
    public abstract class Weapon : MonoBehaviourPun
    {
        private string weaponName;
        protected int damage;
        private bool needsReloading;
        private float reloadTime;
        private AudioClip shotSound;
        protected float shotTimeout;
        protected bool isReloading;
        
        public bool IsReloading => isReloading;

        public void Initialize(string weaponName, int damage, bool needsReloading, float reloadTime, AudioClip shotSound, float shotTimeout)
        {
            this.weaponName = weaponName;
            this.damage = damage;
            this.needsReloading = needsReloading;
            this.reloadTime = reloadTime;
            this.shotSound = shotSound;
            this.shotTimeout = shotTimeout;
        }

        public abstract void Use();

        public virtual void Reload()
        {
            if (needsReloading)
            {
                Debug.Log(weaponName + " is reloading...");
            }
        }

        public void UpdateAmmo(int currentAmmo, int ammoInBackpack)
        {
            WeaponEvents.OnAmmoChanged.Invoke(currentAmmo, ammoInBackpack);
        }

        public void UpdateFireballAmmo(string ammoText)
        {
            WeaponEvents.OnFireballAmmoChanged.Invoke(ammoText);
        }
    }
}