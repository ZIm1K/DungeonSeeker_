using Photon.Pun;
using UnityEngine;

public class GameMenuManager : MonoBehaviourPunCallbacks
{
    private Launcher Launcher;

    [Header("PanelSetUp")]
    [SerializeField] GameObject panel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] AudioSource clickSound;

    private bool isMenuActive = false;
    public static bool IsMenuOpen = false;

    private void Start()
    {
        Launcher = Launcher.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isMenuActive)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
    }

    public void OpenMenu()
    {
        clickSound.Play();
        panel.SetActive(true);
        isMenuActive = true;
        IsMenuOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void CloseMenu()
    {
        clickSound.Play();
        panel.SetActive(false);
        isMenuActive = false;
        IsMenuOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        clickSound.Play();
        settingsPanel.SetActive(true);
    }


    public void CloseSettings()
    {
        clickSound.Play();
        settingsPanel.SetActive(false);
    }


    /*public void ExitToMenu()
    {
        if (PhotonNetwork.InRoom)
        {
            Launcher.LeaveRoom();
        }
    }*/
}
