using Objects.Enemies;
using Photon.Pun;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient && enemyPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject selectedEnemy = enemyPrefabs[randomIndex];

            PhotonNetwork.Instantiate(selectedEnemy.name, transform.position, transform.rotation);
        }
    }    
}
