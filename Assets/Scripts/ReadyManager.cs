using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ReadyManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI readyCountText; 
    public ZoneTrigger zoneTrigger; 
    public int nextSceneIndex = 2;
    private bool isReady = false; 
    private int readyPlayers = 0;

    private void Start()
    {
        readyCountText.text = $"Ready: {readyPlayers}/{PhotonNetwork.PlayerList.Length}";
    }

    void Update()
    {
        if (zoneTrigger.IsPlayerInZone() && Input.GetKeyDown(KeyCode.F))
        {
            ToggleReady();
        }
    }

    void ToggleReady()
    {
        if (!PhotonNetwork.IsConnected) return;

        isReady = !isReady; 
        photonView.RPC("UpdateReadyStatus", RpcTarget.All, isReady ? 1 : -1);
    }

    [PunRPC]
    void UpdateReadyStatus(int change)
    {
        readyPlayers += change;
        readyCountText.text = $"Ready: {readyPlayers}/{PhotonNetwork.PlayerList.Length}";

        if (readyPlayers == PhotonNetwork.PlayerList.Length)
        {
            StartCoroutine(StartGameCountdown());
        }
    }

    System.Collections.IEnumerator StartGameCountdown()
    {
        int countdown = 3;

        while (countdown > 0)
        {
            if (readyPlayers != PhotonNetwork.PlayerList.Length)
            {
                readyCountText.text = $"Ready: {readyPlayers}/{PhotonNetwork.PlayerList.Length}";
                yield break;
            }

            readyCountText.text = $"Starting in {countdown}...";
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        if (readyPlayers == PhotonNetwork.PlayerList.Length)
        {
            PhotonNetwork.LoadLevel(nextSceneIndex);
        }
    }
}
