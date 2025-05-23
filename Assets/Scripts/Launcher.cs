using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private TMP_Text roomNameText;

    [SerializeField] private Transform roomList;
    [SerializeField] private GameObject roomButtonPrefab;
    [SerializeField] private Transform playerList;
    [SerializeField] private GameObject playerTextPrefab;

    [SerializeField] private GameObject startGameButton;
    [SerializeField] AudioSource clickSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.OpenMenu("loading");
        }

    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("main");       
    }

    public void OnPlayButton()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public void OnSettingsButton()
    {
        MenuManager.Instance.OpenMenu("settings");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrWhiteSpace(roomNameInputField.text)) return;

        PhotonNetwork.CreateRoom(roomNameInputField.text);

        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        MenuManager.Instance.OpenMenu("room");
        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < playerList.childCount; i++)
        {
            Destroy(playerList.GetChild(i).gameObject);
        }

        foreach (var t in players)
        {
            Instantiate(playerTextPrefab, playerList).GetComponent<PlayerListItem>().SetUp(t);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) 
        {
            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }
    }

    public void LeaveRoom()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogWarning("Not in a room!");
            return;
        }
        Debug.Log("LeaveRoom() called");
        PhotonNetwork.LeaveRoom();
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.OpenMenu("loading");
        }
    }

    private IEnumerator WaitToLeaveRoom()
    {
        float timeout = 5f;
        while (PhotonNetwork.InRoom && timeout > 0f)
        {
            yield return null;
            timeout -= Time.deltaTime;
        }

        if (!PhotonNetwork.InRoom)
        {
            Debug.Log("Room left via coroutine fallback");
            SceneManager.LoadScene(0);
        }
        else
        {
            Debug.LogError("Failed to leave room after timeout!");
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom() called");
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.OpenMenu("main");
        }
    }

    public void PlayClick()
    {
        clickSound.Play();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = $"Error: {message}";
        MenuManager.Instance.OpenMenu("error");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);

        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.OpenMenu("loading");
        }
    }


    public override void OnRoomListUpdate(List<RoomInfo> _roomList)
    {
        for (int i = 0; i < roomList.childCount; i++)
        {
            Destroy(roomList.GetChild(i).gameObject);
        }

        for (int i = 0; i < _roomList.Count; i++)
        {
            if (_roomList[i].RemovedFromList || !_roomList[i].IsOpen) continue;
            Instantiate(roomButtonPrefab, roomList).GetComponent<RoomListItem>().SetUp(_roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        Instantiate(playerTextPrefab, playerList).GetComponent<PlayerListItem>().SetUp(player);
    }
    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        LevelHandler.ResetLevel();
        PhotonNetwork.LoadLevel(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}