using Inventory;
using Objects.Enemies;
using Objects.PlayerScripts;
using Objects.Weapon;
using Objects.Weapon.Fireball;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SummonerStaf : Weapon
{
    [SerializeField] private SummonerStaffItem data;

    [SerializeField] private float reloadTime;
    [SerializeField] private int manaCost;

    private string attackSoundPath;

    [SerializeField] private Transform firePoint;

    private CharacterModel model;

    [Header("Ally")]
    [SerializeField] private GameObject allyPrefab;
    [SerializeField] private int allyHealAmmount;
    [SerializeField] private int allySpeed;
    [SerializeField] private int allyhealInterval;
    [SerializeField] private int healDistance;
    [SerializeField] private int tpDistance;

    private GameObject curAlly;

    public void Initialize(string path)
    {
        data = Resources.Load<SummonerStaffItem>(path);

        model = GetComponent<CharacterModel>();

        manaCost = data.data.manaCost;
        allyHealAmmount = data.data.allyHealAmmount;
        allySpeed = data.data.allySpeed;
        allyhealInterval = data.data.healInterval;
        healDistance = data.data.healDistance;
        tpDistance = data.data.tpDistance;
        attackSoundPath = data.data.useSoundPath;
        allyPrefab = data.data.allyPrefab;

        firePoint = GameObject.Find("StaffFirePoint").gameObject.transform;

        base.Initialize("Summoner Staff", 0, false, 0, Resources.Load<AudioClip>(attackSoundPath), 0);
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
        
        if(model.Mana < manaCost) return;

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (curAlly == null)
            {
                curAlly = PhotonNetwork.Instantiate(allyPrefab.name, new Vector3(firePoint.position.x,
                    transform.position.y - 0.5f, firePoint.position.z), firePoint.rotation);
                curAlly.GetComponent<AllyController>().HealAmmount = allyHealAmmount;
                curAlly.GetComponent<AllyController>().HealInterval = allyhealInterval;
                curAlly.GetComponent<AllyController>().HealDistance = healDistance;
                curAlly.GetComponent<AllyController>().MaxTpDistance = tpDistance;
                curAlly.GetComponent<NavMeshAgent>().speed = allySpeed;
                curAlly.GetComponent<AllyController>().PlayAudio(attackSoundPath);
                gameObject.GetComponent<InventoryManager>().photonView.RPC("PlayAudio", RpcTarget.Others, attackSoundPath);
            }
            else
            {
                curAlly.transform.position = new Vector3(firePoint.position.x, transform.position.y - 0.5f, firePoint.position.z);
                curAlly.GetComponent<AllyController>().PlayAudio(attackSoundPath);
                gameObject.GetComponent<InventoryManager>().photonView.RPC("PlayAudio", RpcTarget.Others, attackSoundPath);
            }
            animation_.Play();
            model.SpendMana(manaCost);
        }               
    }
    private void OnDisable()
    {
        if (curAlly != null) 
        {
            PhotonNetwork.Destroy(curAlly.GetComponent<PhotonView>());
        }     
    }
}
