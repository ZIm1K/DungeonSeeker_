using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public GameObject readyPanel; // Панель, що відображається у зоні
    private bool isPlayerInZone = false;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            readyPanel.SetActive(true); // Відображаємо панель
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            readyPanel.SetActive(false); // Ховаємо панель
        }
    }

    public bool IsPlayerInZone()
    {
        return isPlayerInZone;
    }
}
