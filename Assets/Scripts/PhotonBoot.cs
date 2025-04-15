using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;

public class PhotonBoot : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // Тільки хост вирішує, коли завантажити сцену
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(LoadLobbyAfterDelay());
        }
    }

    IEnumerator LoadLobbyAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        // Синхронне завантаження сцени для всіх гравців
        PhotonNetwork.LoadLevel(2);
    }
}