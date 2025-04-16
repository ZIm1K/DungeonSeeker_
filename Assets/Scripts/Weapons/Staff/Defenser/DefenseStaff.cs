using Inventory;
using Objects.PlayerScripts;
using Objects.Weapon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DefenseStaff : Weapon
{
    [SerializeField] private DefenseStaffItem data;

    [SerializeField] private float reloadTime;
    [SerializeField] private int manaCost;
    [SerializeField] private int defenseHealAmmount;

    private AudioClip attackSound;
    private string shotSoundPath;

    [SerializeField] private Transform firePoint;

    private CharacterModel model;

    private GameObject curAlly;

    public void Initialize(string path)
    {
        data = Resources.Load<DefenseStaffItem>(path);

        model = GetComponent<CharacterModel>();

        manaCost = data.data.manaCost;
        defenseHealAmmount = data.data.defenseHealAmmount;
        shotSoundPath = data.data.useSoundPath;
        attackSound = Resources.Load<AudioClip>(shotSoundPath);

        firePoint = GameObject.Find("StaffFirePoint").gameObject.transform;

        base.Initialize("Defense Staff", 0, false, 0, attackSound, 0);
    }

    public override void Use()
    {
        if (!photonView.IsMine || isReloading) return;

        if (model.Mana < manaCost) return;

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            gameObject.GetComponent<CharacterModel>().durabilDatabase.HealDefense(defenseHealAmmount);

            model.SpendMana(manaCost);

            if (attackSound != null)
            {
                PlayAudioLocally();
                gameObject.GetComponent<InventoryManager>().photonView.RPC("PlayAudio", RpcTarget.Others, shotSoundPath);
            }
        }
    }
    private void PlayAudioLocally()
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = attackSound;
        source.maxDistance = 30f;
        source.spatialBlend = 1f;
        source.volume = 0.1f;
        source.Play();
        Destroy(source, attackSound.length);
    }
}
