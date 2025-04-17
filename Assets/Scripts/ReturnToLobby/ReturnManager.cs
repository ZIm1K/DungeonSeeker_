using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReturnManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI exitCountText;
    public ZoneTrigger exitZoneTrigger;
    public int exitSceneIndex = 1;
    private bool isReadyToExit = false;
    private HashSet<int> readyPlayers = new HashSet<int>();

    private void Start()
    {
        exitCountText.gameObject.SetActive(false);
        UpdateExitText();
    }

    void Update()
    {
        if (exitZoneTrigger.IsPlayerInZone())
        {
            exitCountText.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                ToggleExitReady();
            }
        }
        else
        {
            exitCountText.gameObject.SetActive(false);
        }
    }

    void ToggleExitReady()
    {
        if (!PhotonNetwork.IsConnected) return;

        isReadyToExit = !isReadyToExit;
        photonView.RPC("UpdateExitStatus", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, isReadyToExit);
    }

    [PunRPC]
    void UpdateExitStatus(int playerId, bool isReady)
    {
        if (isReady)
            readyPlayers.Add(playerId);
        else
            readyPlayers.Remove(playerId);

        UpdateExitText();

        if (readyPlayers.Count == PhotonNetwork.PlayerList.Length)
        {
            StartCoroutine(StartExitCountdown());
        }
    }

    void UpdateExitText()
    {
        exitCountText.text = $"Ready to exit: {readyPlayers.Count}/{PhotonNetwork.PlayerList.Length}";
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        readyPlayers.Remove(otherPlayer.ActorNumber);
        UpdateExitText();
    }

    System.Collections.IEnumerator StartExitCountdown()
    {
        int countdown = 3;

        while (countdown > 0)
        {
            if (readyPlayers.Count != PhotonNetwork.PlayerList.Length)
            {
                UpdateExitText();
                yield break;
            }

            exitCountText.text = $"Exiting in {countdown}...";
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        if (readyPlayers.Count == PhotonNetwork.PlayerList.Length)
        {
            PhotonNetwork.LoadLevel(exitSceneIndex);
            LevelHandler.IncreaseLevel();
        }
    }
}