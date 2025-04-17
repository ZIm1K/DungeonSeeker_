using Photon.Pun;
using UnityEngine;

namespace Objects.Weapon.Fireball
{
    public class EnemyShooter : MonoBehaviourPun
    {
        [SerializeField] private GameObject fireballPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private GameObject decalPrefab;
        [SerializeField] private int damage = 10;
        [SerializeField] private float fireballSpeed = 10f;
        [SerializeField] private float shootCooldown = 2f;

        private Transform target;
        private float lastShootTime;

        void Start()
        {
            // «находимо гравц€ по тегу
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        void Update()
        {
            if (!photonView.IsMine || target == null) return;

            Vector3 direction = (target.position - firePoint.position).normalized;

            // ѕоворот до гравц€
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);

            if (Time.time > lastShootTime + shootCooldown)
            {
                lastShootTime = Time.time;
                ShootFireball(direction);
            }
        }

        private void ShootFireball(Vector3 direction)
        {
            GameObject fireball = PhotonNetwork.Instantiate(fireballPrefab.name, firePoint.position, Quaternion.LookRotation(direction));
            var fireballScript = fireball.GetComponent<EnemyFireball>();
            fireballScript.Initialize(explosionPrefab, decalPrefab, damage, fireballSpeed);
            fireballScript.Shoot(direction);
        }
    }
}