using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

public class ReadyManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI readyCountText; 
    public ZoneTrigger zoneTrigger; 
    public int nextSceneIndex = 2;
    protected bool isReady = false; 
    private readonly HashSet<int> readyPlayers = new HashSet<int>();

    private void Start()
    {
        UpdateReadyText();
    }

    void Update()
    {
        if (zoneTrigger.IsPlayerInZone() && Input.GetKeyDown(KeyCode.F))
        {
            ToggleReady();
        }
    }

    protected void ToggleReady()
    {
        if (!PhotonNetwork.IsConnected) return;

        isReady = !isReady; 
        photonView.RPC("UpdateReadyStatus", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, isReady);
    }

    [PunRPC]
    void UpdateReadyStatus(int playerId, bool ready)
    {
        if (ready)
        {
            readyPlayers.Add(playerId);
        }
        else
        {
            readyPlayers.Remove(playerId);
        }

        UpdateReadyText();

        if (readyPlayers.Count == PhotonNetwork.PlayerList.Length)
        {
            StartCoroutine(StartGameCountdown());
        }
    }

    System.Collections.IEnumerator StartGameCountdown()
    {
        int countdown = 3;

        while (countdown > 0)
        {
            if (readyPlayers.Count != PhotonNetwork.PlayerList.Length)
            {
                UpdateReadyText();
                yield break;
            }

            readyCountText.text = $"Starting in {countdown}...";
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        if (readyPlayers.Count == PhotonNetwork.PlayerList.Length)
        {
            if (PlayerViewManager.Instance != null)
            {
                PlayerViewManager.Instance.SavePlayerInventory();
            }

            if (PhotonNetwork.IsMasterClient)
            {
                yield return new WaitForSeconds(1f);
                PhotonNetwork.LoadLevel(nextSceneIndex);
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        readyPlayers.Remove(otherPlayer.ActorNumber);
        UpdateReadyText();
    }

    private void UpdateReadyText()
    {
        readyCountText.text = $"Ready: {readyPlayers.Count}/{PhotonNetwork.PlayerList.Length}";
    }
}
