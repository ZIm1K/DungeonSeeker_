using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Objects.PlayerScripts;
using System.ComponentModel;
using static UnityEditor.PlayerSettings;
using Photon.Realtime;

public class AllyController : MonoBehaviourPun
{
    private NavMeshAgent agent;
    private GameObject player;
    private CharacterModel model;
    private int regenInterval;
    private int ammountOfHeal;
    private int maxHealDistanse;
    private Vector3 tpPos;
    private float maxTpDistance;

    public int HealAmmount 
    {
        get { return ammountOfHeal; }
        set { ammountOfHeal = value; }
    }
    public int HealInterval
    {
        get { return regenInterval; }
        set { regenInterval = value; }
    }
    public int HealDistance
    {
        get { return maxHealDistanse; }
        set { maxHealDistanse = value; }
    }
    public float MaxTpDistance
    {
        get { return maxTpDistance; }
        set { maxTpDistance = value; }
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        model = player.GetComponent<CharacterModel>();
        StartCoroutine(HealCharacter());
    }
    void FixedUpdate()
    {
        agent.destination = player.transform.position;
        if (Vector3.Distance(transform.position, player.transform.position) > maxTpDistance) 
        {
            agent.enabled = false;
            agent.transform.position = player.transform.position + player.transform.forward * 2;
            agent.enabled = true;
        }
    }
    private IEnumerator HealCharacter() 
    {
        while (true)
        {
            yield return new WaitForSeconds(regenInterval);
            if (Vector3.Distance(player.transform.position, transform.position) < maxHealDistanse)
            {
                if (model.MaxHealth - model.Health < ammountOfHeal)
                {
                    model.Heal(model.MaxHealth - model.Health);
                }
                else 
                {
                    model.Heal(ammountOfHeal);
                }
            }
        }
    }
    public void PlayAudio(string spawnSound)
    {
        AudioClip clip = Resources.Load<AudioClip>(spawnSound);
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.PlayOneShot(clip);
    }   
}
