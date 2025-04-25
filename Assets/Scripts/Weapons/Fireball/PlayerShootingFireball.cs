using System;
using Objects.PlayerScripts;
using Photon.Pun;
using ScriptableObjects.Weapons;
using UnityEngine;

namespace Objects.Weapon.Fireball
{
    public class PlayerShootingFireball : Weapon
    {
        [SerializeField] private FireBallItemData data;
        
        [SerializeField] private GameObject fireballPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireballSpeed;
        [SerializeField] private int fireballDamage;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private GameObject decalPrefab;
        [SerializeField] private AudioClip shotSound;
        [SerializeField] private int manaCost;

        private CharacterModel model;

        private float lastShotTime;

        public void Initialize(string path)
        {
            data = Resources.Load<FireBallItemData>(path);
            
            model = GetComponent<CharacterModel>();
            
            manaCost = data.data.manaCost;
            fireballPrefab = data.data.fireballPrefab;
            firePoint = GameObject.Find("FireballFirePoint").gameObject.transform;
            fireballSpeed = data.data.fireballSpeed;
            fireballDamage = data.data.fireballDamage;
            explosionPrefab = data.data.explosionPrefab;
            decalPrefab = data.data.decalPrefab;
            shotSound = data.data.shotSound;
            shotTimeout = data.data.shotTimeout;

            base.Initialize("Fireball", fireballDamage, false, 0f, shotSound, shotTimeout);
            lastShotTime = -shotTimeout;
        }
        public override void InitializeAnimation(Animation animation)
        {
            animationClip = data.data.animationClip;
            animation_ = animation;
            animation_.clip = animationClip;
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
                    animation_.Play();
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