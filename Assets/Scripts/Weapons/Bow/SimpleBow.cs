using Inventory;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Objects.Weapon.Bow
{
    public class SimpleBow : Weapon
    {
        [SerializeField] private BowItemData data;

        [SerializeField] private GameObject hitObjectPrefab;
        [SerializeField] private GameObject decalPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private AudioClip shotSound;
        [SerializeField] private string shotSoundPath;
        [SerializeField] private float reloadTime;
        [SerializeField] private Image reloadProgressImage;

        [Header("Arrow")]
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private GameObject arrowDecal;
        [SerializeField] private float arrowSpeed;

        public int CountOfBulletsInWeapon => countOfBulletsInWeapon;
        public int CountOfBulletsInBackpack => countOfBulletsInBackpack;

        public int MaxBulletsInWeapon => maxBulletsInWeapon;


        private int countOfBulletsInWeapon;
        private int countOfBulletsInBackpack;
        private int maxBulletsInWeapon;
        private float lastShotTime;

        public void Initialize(Image reloadImage, string path)
        {
            data = Resources.Load<BowItemData>(path);

            damage = data.data.bowDamage;
            arrowPrefab = data.data.arrowPrefab;
            arrowSpeed = data.data.arrowSpeed;
            arrowDecal = data.data.decalArrow;
            firePoint = GameObject.Find("BowFirePoint").gameObject.transform;

            reloadProgressImage = reloadImage;

            shotSoundPath = data.data.shotSoundPath;
            shotSound = Resources.Load<AudioClip>(shotSoundPath);            

            shotTimeout = data.data.shotTimeout;
            reloadTime = data.data.reloadTime;

            maxBulletsInWeapon = data.data.maxBulletsInWeapon;

            base.Initialize("Bow", damage, true, reloadTime, shotSound, shotTimeout);
            lastShotTime = -shotTimeout;

            if (reloadProgressImage != null)
            {
                reloadProgressImage.fillAmount = 0;
            }
        }

        public override void InitializeAnimation(Animation animation)
        {
            animationClip = data.data.animationClip;
            animation_ = animation;
            animation_.clip = animationClip;
        }
        public override void Use()
        {
            if (!photonView.IsMine || isReloading) return;

            if (Time.time >= lastShotTime + shotTimeout) 
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    if (countOfBulletsInWeapon > 0)
                    {
                        lastShotTime = Time.time;

                        GameObject arrow = PhotonNetwork.Instantiate(arrowPrefab.name, firePoint.position + firePoint.forward * 2, firePoint.rotation);
                        arrow.GetComponent<Arrow>().Initialize(arrowDecal, damage);
                        Rigidbody rb = arrow.GetComponent<Rigidbody>();
                        rb.velocity = firePoint.forward * arrowSpeed;                        

                        countOfBulletsInWeapon--;
                        UpdateAmmo(countOfBulletsInWeapon, countOfBulletsInBackpack);

                        if (shotSound != null)
                        {
                            PlayAudioLocally();
                            gameObject.GetComponent<InventoryManager>().photonView.RPC("PlayAudio", RpcTarget.Others, shotSoundPath);
                        }
                        animation_.Play();
                    }
                    else
                    {
                        Debug.Log("No bullets left in the bow.");
                    }
                }
            }
        }        
        private void PlayAudioLocally()
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = shotSound;
            source.maxDistance = 30f;
            source.spatialBlend = 1f;
            source.volume = 0.1f;
            source.Play();
            Destroy(source, shotSound.length);
        }

        public override void Reload()
        {
            if (countOfBulletsInBackpack > 0 && !isReloading)
            {
                StartCoroutine(ReloadCoroutine());
            }
            else
            {
                Debug.Log("No bullets left in backpack to reload.");
            }
        }
        public void AddBullets(int ammount)
        {
            countOfBulletsInBackpack += ammount;
        }

        private IEnumerator ReloadCoroutine()
        {
            isReloading = true;
            Debug.Log("Reloading...");
            float elapsedTime = 0f;

            while (elapsedTime < reloadTime)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / reloadTime);
                if (reloadProgressImage != null)
                {
                    reloadProgressImage.fillAmount = progress;
                }
                yield return null;
            }

            int bulletsNeeded = maxBulletsInWeapon - countOfBulletsInWeapon;

            if (countOfBulletsInBackpack >= bulletsNeeded)
            {
                countOfBulletsInWeapon += bulletsNeeded;
                countOfBulletsInBackpack -= bulletsNeeded;
            }
            else
            {
                countOfBulletsInWeapon += countOfBulletsInBackpack;
                countOfBulletsInBackpack = 0;
            }

            isReloading = false;
            Debug.Log("Reloaded.");
            UpdateAmmo(countOfBulletsInWeapon, countOfBulletsInBackpack);

            if (reloadProgressImage != null)
            {
                reloadProgressImage.fillAmount = 0;
            }
        }
    }
}
