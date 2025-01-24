using Objects.Enemies;
using Objects.Weapon;
using Photon.Pun;
using ScriptableObjects.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSword : Weapon
{
    [SerializeField] private SwordData data;

    [SerializeField] private int swordDamage;
    [SerializeField] private float reloadTime;
    
    [SerializeField] private AudioClip attackSound;

    [SerializeField] private Transform firePoint;

    private float lastShotTime;

    public void Initialize()
    {
        data = Resources.Load<SwordData>("Data/Sword");

        swordDamage = data.swordDamage;;
        attackSound = data.attackSound;
        shotTimeout = data.shotTimeout;
       
        firePoint = GameObject.Find("SwordFirePoint").gameObject.transform;

        lastShotTime = -shotTimeout;

        base.Initialize("Sword", swordDamage, false, 0, attackSound, shotTimeout);

        UpdateSwordAmmo("∞");
    }

    public override void Use()
    {
        if (!photonView.IsMine || isReloading) return;

        if (Time.time >= lastShotTime + shotTimeout)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                lastShotTime = Time.time;

                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 50))
                {
                    EnemyModel enemy = hit.collider.gameObject.GetComponent<EnemyModel>();
                    if (enemy != null)
                    {
                        PhotonView targetPhotonView = hit.collider.gameObject.GetComponent<PhotonView>();
                        if (targetPhotonView != null)
                        {
                            targetPhotonView.RPC("TakeDamage", RpcTarget.All, swordDamage);
                        }
                    }
                    //else
                    //{
                    //    Quaternion rotation = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(0, 180, 0);
                    //    Vector3 position = hit.point + hit.normal * 0.06f;
                    //    GameObject decal = PhotonNetwork.Instantiate(decalPrefab.name, position, rotation);
                    //    decal.transform.SetParent(hit.collider.transform);
                    //    PhotonNetwork.Instantiate(hitObjectPrefab.name, decal.transform.position, Quaternion.identity);
                    //}
                }

                if (attackSound != null)
                {
                    PlayAudioLocally();
                    photonView.RPC("PlayAudio", RpcTarget.Others);
                }
                //else
                //{
                //    Debug.Log("No bullets left in the pistol.");
                //}
            }
        }

        //Ray ray = new Ray(transform.position, transform.forward);
        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit, 2.0f))
        //{
        //    if (hit.collider.CompareTag("Enemy"))
        //    {
        //        Enemy enemy = hit.collider.GetComponent<Enemy>();
        //        if (enemy != null)
        //        {
        //            enemy.TakeDamage(damage);
        //            Debug.Log($"Enemy hit! Damage: {damage}");
        //        }
        //    }
        //}
    }
    private void PlayAudioLocally()
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = attackSound;
        source.maxDistance = 20f;
        source.spatialBlend = 1f;
        source.volume = 0.1f;
        source.Play();
        Destroy(source, attackSound.length);
    }

    [PunRPC]
    private void PlayAudio()
    {
        PlayAudioLocally();
    }
}
