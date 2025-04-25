using Objects.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTrigger : MonoBehaviour
{
    public List<GameObject> enemysInRange;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyController>() != null && !enemysInRange.Contains(other.gameObject))
        {
            enemysInRange.Add(other.gameObject);
        }                                         
    }   
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyController>() && enemysInRange.Contains(other.gameObject))
        {
            enemysInRange.Remove(other.gameObject);
        }
    }
}
