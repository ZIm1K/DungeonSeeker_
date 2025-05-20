using Objects.Enemies;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform placeToSpawn;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient && enemyPrefabs.Length > 0)
        {
            StartCoroutine(SpawnEnemy());
        }
    }
    IEnumerator SpawnEnemy() 
    {       
        yield return new WaitForSeconds(3f);
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedEnemy = enemyPrefabs[randomIndex];

        PhotonNetwork.Instantiate(selectedEnemy.name, placeToSpawn.position, transform.rotation);
    }
}
