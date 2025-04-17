using System;
using Objects.Enemies;
using Photon.Pun;
using UnityEngine;

namespace Objects.Weapon.Fireball
{
    public class EnemyFireball : MonoBehaviourPun
    {
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private GameObject decalPrefab;
        [SerializeField] private int damage;
        [SerializeField] private float fireballSpeed;
        [SerializeField] private AudioClip shotSound;

        private Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Initialize(GameObject explosion, GameObject decal, int damage, float speed)
        {
            explosionPrefab = explosion;
            decalPrefab = decal;
            this.damage = damage;
            fireballSpeed = speed;
        }

        public void Shoot(Vector3 direction)
        {
            // Set the velocity of the fireball
            rb.velocity = direction * fireballSpeed;

            // Play sound effect locally
            if (shotSound != null)
            {
                AudioSource.PlayClipAtPoint(shotSound, transform.position);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Spawn explosion on collision
            PhotonNetwork.Instantiate(explosionPrefab.name, transform.position, Quaternion.identity);

            if (photonView.IsMine)
            {
                EnemyModel enemy = collision.gameObject.GetComponent<EnemyModel>();
                if (enemy != null)
                {
                    PhotonView targetPhotonView = collision.collider.gameObject.GetComponent<PhotonView>();
                    if (targetPhotonView != null)
                    {
                        targetPhotonView.RPC("TakeDamage", RpcTarget.All, damage);
                    }
                }
                else
                {
                    // If hit something that is not an enemy, spawn a decal
                    ContactPoint contact = collision.contacts[0];
                    Quaternion rotation = Quaternion.LookRotation(contact.normal) * Quaternion.Euler(0, 180, 0);
                    Vector3 position = contact.point + contact.normal * 0.06f;
                    GameObject decal = PhotonNetwork.Instantiate(decalPrefab.name, position, rotation);
                    decal.transform.SetParent(collision.transform);
                }

                // Destroy fireball after impact
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
