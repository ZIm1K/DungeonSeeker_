using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyKillCount : MonoBehaviour
{
    public int enemiesToKill;
    public int curEnemiesKilled;
    
    public void NewEnemyKilled() 
    {
        if (curEnemiesKilled < enemiesToKill) 
        {
            GetComponent<PhotonView>().RPC("UpdateEnemyKilled", RpcTarget.All);
        }      
    }
    [PunRPC]
    void UpdateEnemyKilled() 
    {
        curEnemiesKilled++;
    }    
    public void NormalizeEnemyToKillLocal() 
    {
        enemiesToKill = Mathf.RoundToInt(enemiesToKill / 3);
        GetComponent<PhotonView>().RPC("UpdateEnemyToKillForAll", RpcTarget.Others, enemiesToKill);
    }
    [PunRPC]
    void UpdateEnemyToKillForAll(int enemiesToKill) 
    {
        this.enemiesToKill = enemiesToKill;
    }
}
