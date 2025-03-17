using Inventory;
using Objects.Enemies;
using Objects.PlayerScripts;
using Objects.Weapon;
using Objects.Weapon.Fireball;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SummonerStaf : Weapon
{
    [SerializeField] private StaffItem data;

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

    private GameObject curAlly;

    public void Initialize()
    {
        data = Resources.Load<StaffItem>("ScriptableObject/Staff Item");

        model = GetComponent<CharacterModel>();

        manaCost = data.data.manaCost;
        allyHealAmmount = data.data.allyHealAmmount;
        allySpeed = data.data.allySpeed;
        allyhealInterval = data.data.healInterval;
        healDistance = data.data.healDistance;
        attackSoundPath = data.data.useSoundPath;
        allyPrefab = data.data.allyPrefab;

        firePoint = GameObject.Find("StaffFirePoint").gameObject.transform;

        base.Initialize("Staff", 0, false, 0, Resources.Load<AudioClip>(attackSoundPath), 0);
    }

    public override void Use()
    {
        if (!photonView.IsMine || isReloading) return;
        
        if(model.Mana < manaCost) return;

        if (curAlly == null)
        {
            curAlly = PhotonNetwork.Instantiate(allyPrefab.name, new Vector3(firePoint.position.x, 
                transform.position.y - 0.5f, firePoint.position.z), firePoint.rotation);
            curAlly.GetComponent<AllyController>().HealAmmount = allyHealAmmount;
            curAlly.GetComponent<AllyController>().HealInterval = allyhealInterval;
            curAlly.GetComponent<AllyController>().HealDistance = healDistance;
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

        model.SpendMana(manaCost);
                
    }
    private void OnDisable()
    {
        if (curAlly != null) 
        {
            PhotonNetwork.Destroy(curAlly.GetComponent<PhotonView>());
        }     
    }
}
