using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviour
{
    public GameObject enemy;
    
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(enemy.name, new Vector3(-8, 0.5f, -4), Quaternion.identity);
        }
    }
}