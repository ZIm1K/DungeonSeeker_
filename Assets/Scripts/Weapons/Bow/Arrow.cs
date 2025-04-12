using Objects.Enemies;
using Objects.PlayerScripts;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviourPun
{
    private int damage;
    private GameObject decalPrefab;
    public void Initialize(GameObject decal, int damage)
    {
        decalPrefab = decal;
        this.damage = damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine)
        {
            EnemyModel enemy = collision.gameObject.GetComponent<EnemyModel>();
            if (enemy != null)
            {
                PhotonView targetPhotonView = collision.collider.gameObject.GetComponent<PhotonView>();
                targetPhotonView.RPC("TakeDamage", RpcTarget.All, damage);
                Debug.LogWarning("Damaged");
            }           
            else 
            {
                Quaternion rotation = transform.rotation;
                Vector3 position = transform.position;
                GameObject decal = PhotonNetwork.Instantiate(decalPrefab.name, position, rotation);
            }
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
