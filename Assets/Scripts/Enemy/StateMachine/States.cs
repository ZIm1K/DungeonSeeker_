using Objects.Enemies;
using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Enemy.StateMachine
{
    public class PatrolState : IState
    {
        private readonly EnemyController enemyController;

        public PatrolState(EnemyController enemyController)
        {
            this.enemyController = enemyController;
        }

        public void Enter()
        {
            enemyController.Patrol();
        }
        public void Execute()
        {
            if (enemyController.HasReachedDestination())
            {
                enemyController.Patrol();
            }
        }
        public void Exit() { }
    }


    public class ChaseState : IState
    {
        private EnemyController controller;

        public ChaseState(EnemyController controller)
        {
            this.controller = controller;
        }

        public void Enter()
        {
            Debug.Log("enter chase state");
        }

        public void Execute()
        {
            controller.Chase();
        }

        public void Exit()
        {
            Debug.Log("exit chase state");
        }
    }

    public class AttackState : IState
    {
        private EnemyController controller;

        public AttackState(EnemyController controller)
        {
            this.controller = controller;
        }

        public void Enter()
        {
            Debug.Log("enter attack state");
        }

        public void Execute()
        {
            controller.AttackCurrentTarget();
        }

        public void Exit()
        {
            Debug.Log("exit attack state");
        }
    }
}