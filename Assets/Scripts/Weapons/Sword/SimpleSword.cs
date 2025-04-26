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
    [SerializeField] private string attackSoundPath;

    [SerializeField] private Transform firePoint;

    [SerializeField] private float rangeOfAttack;

    private float lastAttackTime;

    private SwordTrigger trigger;

    public void Initialize(string path)
    {
        data = Resources.Load<SwordItemData>(path);

        swordDamage = data.data.swordDamage;
        
        attackSoundPath = data.data.attackSoundPath;
        attackSound = Resources.Load<AudioClip>(attackSoundPath);

        shotTimeout = data.data.shotTimeout;
        rangeOfAttack = data.data.rangeOfAttack;

        firePoint = GameObject.Find("SwordFirePoint").gameObject.transform;

        lastAttackTime = -shotTimeout;

        base.Initialize("Sword", swordDamage, false, 0, attackSound, shotTimeout);
    }
    public void SetTrigger(SwordTrigger trigger) 
    {
        this.trigger = trigger;
        Debug.LogWarning("Triggered");
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

        if (Time.time >= lastAttackTime + shotTimeout + animationClip.length)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                lastAttackTime = Time.time;

                if (trigger.enemysInRange.Count > 0)
                {
                    for (int i = 0; i < trigger.enemysInRange.Count; i++) 
                    {
                        if (trigger.enemysInRange[i] != null)
                        {
                            EnemyModel enemyToAttack = trigger.enemysInRange[i].GetComponent<EnemyModel>();
                            if (enemyToAttack != null)
                            {
                                PhotonView targetPhotonView = trigger.enemysInRange[i].gameObject.GetComponent<PhotonView>();
                                if (targetPhotonView != null)
                                {
                                    targetPhotonView.RPC("TakeDamage", RpcTarget.All, swordDamage);
                                }
                            }
                        }
                        else 
                        {
                            trigger.enemysInRange.RemoveAt(i);
                            i--;
                        }
                    }                    
                }

                if (attackSound != null)
                {
                    PlayAudioLocally();
                    gameObject.GetComponent<InventoryManager>().photonView.RPC("PlayAudio", RpcTarget.Others, attackSoundPath);
                }
                animation_.Play();

                //Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                //RaycastHit hit;
                //if (Physics.Raycast(ray, out hit, rangeOfAttack))
                //{
                //    EnemyModel enemy = hit.collider.gameObject.GetComponent<EnemyModel>();
                //    if (enemy != null)
                //    {
                //        PhotonView targetPhotonView = hit.collider.gameObject.GetComponent<PhotonView>();
                //        if (targetPhotonView != null)
                //        {
                //            targetPhotonView.RPC("TakeDamage", RpcTarget.All, swordDamage);
                //        }
                //    }                   
                //}

                //if (attackSound != null)
                //{
                //    PlayAudioLocally();
                //    gameObject.GetComponent<InventoryManager>().photonView.RPC("PlayAudio", RpcTarget.Others, attackSoundPath);
                //}
                //animation_.Play();
            }
        }
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
}
