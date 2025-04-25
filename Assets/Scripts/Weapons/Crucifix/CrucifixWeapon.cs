using Inventory;
using Objects.Enemies;
using Objects.PlayerScripts;
using Objects.Weapon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class CrucifixWeapon : Weapon
{
    [SerializeField] private CrucifixItem data;

    [SerializeField] private float rangeOfAttack;

    private AudioClip attackSound;
    private string attackSoundPath;

    [SerializeField] private Transform firePoint;

    private CharacterModel model;

    public void Initialize(string path)
    {
        data = Resources.Load<CrucifixItem>(path);

        model = GetComponent<CharacterModel>();

        damage = data.data.damage;
        rangeOfAttack = data.data.rangeOfAttack;
               

        attackSoundPath = data.data.attackSoundPath;
        attackSound = Resources.Load<AudioClip>(attackSoundPath);

        firePoint = GameObject.Find("CrucifixFirePoint").gameObject.transform;

        base.Initialize("Crucifix", damage, false, 0, attackSound, 0);
    }

    public override void InitializeAnimation(Animation animation)
    {
        animationClip = data.data.animationClip;
        animation_ = animation;
        animation_.clip = animationClip;
    }
    public async override void Use()
    {
        if (!photonView.IsMine || isReloading) return;

        if (Cursor.lockState == CursorLockMode.Locked)
        {
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
                        animation_.Play();
                        await PlayAnimation(animation_);
                        targetPhotonView.RPC("TakeDamage", RpcTarget.All, damage);
                        if (attackSound != null)
                        {
                            PlayAudioLocally();
                            gameObject.GetComponent<InventoryManager>().photonView.RPC("PlayAudio", RpcTarget.Others, attackSoundPath);
                        }
                        gameObject.GetComponent<WeaponManager>().NullifySlotData(gameObject.GetComponent<WeaponManager>().currentWeaponIndex);
                    }
                }
            }                  
        }
    }

    async Task PlayAnimation(Animation animation) 
    {        
        await Task.Yield(); 

        while (animation.isPlaying)
        {
            await Task.Yield(); 
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
