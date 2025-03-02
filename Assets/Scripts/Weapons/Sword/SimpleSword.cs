using Inventory;
using Objects.Enemies;
using Objects.PlayerScripts;
using Objects.Weapon;
using Photon.Pun;
using ScriptableObjects.Weapons;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleSword : Weapon
{
    [SerializeField] private SwordItemData data;

    [SerializeField] private int swordDamage;
    [SerializeField] private float reloadTime;
    
    [SerializeField] private AudioClip attackSound;

    [SerializeField] private Transform firePoint;

    [SerializeField] private float rangeOfAttack;

    private float lastAttackTime;

    public void Initialize()
    {
        data = Resources.Load<SwordItemData>("ScriptableObject/Sword Item");

        swordDamage = data.data.swordDamage;
        attackSound = data.data.attackSound;
        shotTimeout = data.data.shotTimeout;
        rangeOfAttack = data.data.rangeOfAttack;

        firePoint = GameObject.Find("SwordFirePoint").gameObject.transform;

        lastAttackTime = -shotTimeout;

        //UpdateSwordAmmo("∞");

        base.Initialize("Sword", swordDamage, false, 0, attackSound, shotTimeout);
    }

    public override void Use()
    {
        if (!photonView.IsMine || isReloading) return;

        if (Time.time >= lastAttackTime + shotTimeout)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                lastAttackTime = Time.time;

                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, rangeOfAttack))
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
                }

                if (attackSound != null)
                {
                    PlayAudioLocally();
                    gameObject.GetComponent<InventoryManager>().photonView.RPC("PlayAudio", RpcTarget.Others, "FootSteeps/Metal/hit_metal.1");
                }              
            }
        }
    }
    private void PlayAudioLocally()//////////////////////////////////////////////
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = attackSound;
        source.maxDistance = 20f;
        source.spatialBlend = 1f;
        source.volume = 0.1f;
        source.Play();
        Destroy(source, attackSound.length);
    }   
}
