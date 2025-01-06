using System;
using Objects.Enemies;
using Photon.Pun;
using UnityEngine;

namespace Objects.Weapon.Fireball
{
    public class Fireball : PlayerShootingFireball
    {
        private GameObject explosionPrefab;
        private GameObject decalPrefab;

        public void Initialize(GameObject explosion, GameObject decal, int damage)
        {
            explosionPrefab = explosion;
            decalPrefab = decal;
            this.damage = damage;
        }

        private void OnCollisionEnter(Collision collision)
        {
            PhotonNetwork.Instantiate(explosionPrefab.name, transform.position, Quaternion.identity);

            if (photonView.IsMine)
            {
                EnemyModel enemy = collision.gameObject.GetComponent<EnemyModel>();
                if (enemy != null)
                {
                    PhotonView targetPhotonView = collision.collider.gameObject.GetComponent<PhotonView>();
                    targetPhotonView.RPC("TakeDamage", RpcTarget.All, damage);
                }
                else
                {
                    ContactPoint contact = collision.contacts[0];
                    Quaternion rotation = Quaternion.LookRotation(contact.normal) * Quaternion.Euler(0, 180, 0);
                    Vector3 position = contact.point + contact.normal * 0.06f; 
                    GameObject decal = PhotonNetwork.Instantiate(decalPrefab.name, position, rotation);
                    decal.transform.SetParent(collision.transform);
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}