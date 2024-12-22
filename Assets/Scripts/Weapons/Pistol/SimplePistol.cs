using System.Collections;
using Objects.Enemies;
using Photon.Pun;
using ScriptableObjects.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Objects.Weapon.Pistol
{
    public class SimplePistol : Weapon
    {
        [SerializeField] private PistolData data;
        
        [SerializeField] private int bullets;
        [SerializeField] private int bulletsInBackpack;
        [SerializeField] private int pistolDamage;
        [SerializeField] private GameObject hitObjectPrefab;
        [SerializeField] private GameObject decalPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private AudioClip shotSound;
        [SerializeField] private float shotTimeout;
        [SerializeField] private float reloadTime;
        [SerializeField] private Image reloadProgressImage;

        public int CountOfBulletsInWeapon => countOfBulletsInWeapon;
        public int CountOfBulletsInBackpack => countOfBulletsInBackpack;
        
        private int countOfBulletsInWeapon;
        private int countOfBulletsInBackpack;
        private float lastShotTime;
        //private bool isReloading = false;

        public void Initialize(Image reloadImage)
        {
            data = Resources.Load<PistolData>("Data/Pistol");
            bullets = data.bullets;
            bulletsInBackpack = data.bulletsInBackpack;
            pistolDamage = data.pistolDamage;
            hitObjectPrefab = data.hitObjectPrefab;
            decalPrefab = data.decalPrefab;
            firePoint = GameObject.Find("PistolFirePoint").gameObject.transform;
            
            //reloadProgressImage = GameObject.Find("ReloadingProgress").GetComponent<Image>();
            reloadProgressImage = reloadImage;

            shotSound = data.shotSound;
            shotTimeout = data.shotTimeout;
            reloadTime = data.reloadTime;
            
            base.Initialize("Pistol", pistolDamage, true, reloadTime, shotSound, shotTimeout);
            countOfBulletsInWeapon = bullets;
            countOfBulletsInBackpack = bulletsInBackpack;
            lastShotTime = -shotTimeout;

            UpdateAmmo(countOfBulletsInWeapon, countOfBulletsInBackpack);

            if (reloadProgressImage != null)
            {
                reloadProgressImage.fillAmount = 0;
            }
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

                        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 100))
                        {
                            EnemyModel enemy = hit.collider.gameObject.GetComponent<EnemyModel>();
                            if (enemy != null)
                            {
                                PhotonView targetPhotonView = hit.collider.gameObject.GetComponent<PhotonView>();
                                if (targetPhotonView != null)
                                {
                                    targetPhotonView.RPC("TakeDamage", RpcTarget.All, damage);
                                }
                            }
                            else
                            {
                                Quaternion rotation = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(0, 180, 0);
                                Vector3 position = hit.point + hit.normal * 0.06f;
                                GameObject decal = PhotonNetwork.Instantiate(decalPrefab.name, position, rotation);
                                decal.transform.SetParent(hit.collider.transform);
                                PhotonNetwork.Instantiate(hitObjectPrefab.name, decal.transform.position, Quaternion.identity);
                            }
                        }

                        countOfBulletsInWeapon--;
                        UpdateAmmo(countOfBulletsInWeapon, countOfBulletsInBackpack);

                        if (shotSound != null)
                        {
                            PlayAudioLocally();
                            photonView.RPC("PlayAudio", RpcTarget.Others);
                        }
                    }
                    else
                    {
                        Debug.Log("No bullets left in the pistol.");
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

        [PunRPC]
        private void PlayAudio()
        {
            PlayAudioLocally();
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

            int bulletsNeeded = 12 - countOfBulletsInWeapon;

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