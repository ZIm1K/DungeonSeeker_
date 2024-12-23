using System.Collections.Generic;
using Enemy.StateMachine;
using Interfaces;
using Objects.PlayerScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace Objects.Enemies
{
    public class EnemyController : MonoBehaviourPunCallbacks, ICanBeKilled
    {
        private StateMachine stateMachine;
        private IState patrolState;
        private IState chaseState;
        private IState attackState;

        private List<PhotonView> photonViews;
        private List<PhotonView> photonViewsOfPlayers;
        private NavMeshAgent agent;
        private GameObject currentTarget;
        private float attackTimer;

        [Header("MVC")]
        [SerializeField] private EnemyView view;

        [Header("Params")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int damage = 10;
        [SerializeField] private float chaseRange = 5f;
        [SerializeField] private float attackRange = 2;
        [SerializeField] private float attackInterval = 1;
       //never used [SerializeField] private float changePositionTime = 5f;
        [SerializeField] private float moveDistance = 10f;

        [Header("Animations")]
        [SerializeField] private Animator animator;

        private EnemyModel model;

        void Start()
        {
            model = gameObject.AddComponent<EnemyModel>();
            model.Initialize(maxHealth, damage, attackRange, attackInterval, view);

            if (!PhotonNetwork.IsMasterClient)
            {
                view.UpdateHealthText(model.Health);
                view.UpdateDamageText(model.Damage);
            }
            else
            {
                view.UpdateHealthText(model.Health);
                view.UpdateDamageText(model.Damage);

                agent = GetComponent<NavMeshAgent>();

                stateMachine = new StateMachine();

                patrolState = new PatrolState(this);
                chaseState = new ChaseState(this);
                attackState = new AttackState(this);

                stateMachine.ChangeState(patrolState);

                photonViews = new List<PhotonView>();
                photonViewsOfPlayers = new List<PhotonView>();
            }
        }

        void Update()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            stateMachine.Update();

            photonViews.Clear();
            photonViewsOfPlayers.Clear();

            photonViews.AddRange(FindObjectsOfType<PhotonView>());

            foreach (PhotonView view in photonViews)
            {
                if (view.GetComponent<PlayerControllerWithCC>() != null)
                {
                    photonViewsOfPlayers.Add(view);
                }
            }

            currentTarget = FindNearestPlayer();

            float distanceToPlayer = Vector3.Distance(gameObject.transform.position, currentTarget.transform.position);

            if (distanceToPlayer <= chaseRange && distanceToPlayer > attackRange)
            {
                if (stateMachine.CurrentState != chaseState)
                {
                    stateMachine.ChangeState(chaseState);
                }
            }
            else if (distanceToPlayer <= attackRange)
            {
                if (stateMachine.CurrentState != attackState)
                {
                    stateMachine.ChangeState(attackState);
                }
            }
            else
            {
                if (stateMachine.CurrentState != patrolState)
                {
                    stateMachine.ChangeState(patrolState);
                }
            }
        }

        private GameObject FindNearestPlayer()
        {
            GameObject nearestPlayer = null;
            float nearestDistance = Mathf.Infinity;

            foreach (PhotonView player in photonViewsOfPlayers)
            {
                if (player == null) continue;

                Transform playerTransform = player.transform;

                if (playerTransform == null) continue;

                float distance = Vector3.Distance(transform.position, playerTransform.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPlayer = player.gameObject;
                }
            }
            return nearestPlayer;
        }

        public Vector3 RandomNavSphere(float distance)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

            randomDirection += transform.position;

            NavMeshHit navHit;

            NavMesh.SamplePosition(randomDirection, out navHit, distance, -1);

            return navHit.position;
        }

        public void Patrol()
        {
            agent.SetDestination(RandomNavSphere(moveDistance));
        }

        public void Chase()
        {
            if (currentTarget != null)
            {
                agent.SetDestination(currentTarget.transform.position);
            }
        }

        public void AttackCurrentTarget()
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
            {
                if (currentTarget != null)
                {
                    PhotonView targetPhotonView = currentTarget.GetComponent<PhotonView>();

                    if (targetPhotonView != null)
                    {
                        Debug.LogWarning("Start working");
                        targetPhotonView.RPC("TakeDamage", targetPhotonView.Owner, model.Damage);
                        attackTimer = attackInterval;
                        Debug.LogWarning("Worked wery well");
                    }
                }
            }
        }

        public bool HasReachedDestination()
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Die()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }
}