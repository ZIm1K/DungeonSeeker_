using System;
using Objects.PlayerScripts;
using Photon.Pun;
using ScriptableObjects.Weapons;
using UnityEngine;

namespace Objects.Weapon.Fireball
{
    public class PlayerShootingFireball : Weapon
    {
        [SerializeField] private FireballData data;
        
        [SerializeField] private GameObject fireballPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireballSpeed;
        [SerializeField] private int fireballDamage;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private GameObject decalPrefab;
        [SerializeField] private AudioClip shotSound;
        [SerializeField] private float shotTimeout;
        [SerializeField] private int manaCost;

        private CharacterModel model;

        private float lastShotTime;

        public void Initialize()
        {
            data = Resources.Load<FireballData>("Data/Fireball");
            
            model = GetComponent<CharacterModel>();
            
            manaCost = data.manaCost;
            fireballPrefab = data.fireballPrefab;
            firePoint = GameObject.Find("FireballFirePoint").gameObject.transform;
            fireballSpeed = data.fireballSpeed;
            fireballDamage = data.fireballDamage;
            explosionPrefab = data.explosionPrefab;
            decalPrefab = data.decalPrefab;
            shotSound = data.shotSound;
            shotTimeout = data.shotTimeout;

            base.Initialize("Fireball", fireballDamage, false, 0f, shotSound, shotTimeout);
            lastShotTime = -shotTimeout;
        }

        public override void Use()
        {
            if (Time.time >= lastShotTime + shotTimeout && model.Mana >= manaCost)
            {
                if (Cursor.lockState == CursorLockMode.Locked) 
                { 
                    lastShotTime = Time.time;

                    GameObject fireball = PhotonNetwork.Instantiate(fireballPrefab.name, firePoint.position, firePoint.rotation);
                    fireball.GetComponent<Fireball>().Initialize(explosionPrefab, decalPrefab, damage);
                    Rigidbody rb = fireball.GetComponent<Rigidbody>();
                    rb.velocity = firePoint.forward * fireballSpeed;
                    model.SpendMana(manaCost);
                }              
            }
        }

        public override void Reload()
        {
            base.Reload();
            // Additional reload logic specific to Fireball, if any
        }
    }
}