using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public GameObject readyPanel; 
    private bool isPlayerInZone = false;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            readyPanel.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            readyPanel.SetActive(false);
        }
    }

    public bool IsPlayerInZone()
    {
        return isPlayerInZone;
    }
}
