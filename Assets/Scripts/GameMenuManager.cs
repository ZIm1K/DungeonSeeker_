using Inventory;
using Objects.PlayerScripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviourPunCallbacks
{
    [Header("SettingsPanelSetUp")]
    [SerializeField] GameObject settingsMenuPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] AudioSource clickSound;

    private bool isMenuActive = false;

    private bool IsMouseLockedBeforePause = false;

    private void LateUpdate()
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
    //Settings
    private void SetMenuState(bool isActive)
    {
        clickSound.Play();

        settingsMenuPanel.SetActive(isActive);
        isMenuActive = isActive;       

        gameObject.GetComponent<InventoryManager>().IsOnPause = isActive;
    }

    public void SetSettingsState(bool isActive)
    {
        clickSound.Play();
        settingsPanel.SetActive(isActive);
    }
    public void ExitToMenu()
    {
        DisconectManager.disconectInstance.ChangingScenes(0);
    }
    public void OpenMenu() 
    {
        if (Cursor.lockState == CursorLockMode.Locked) //Saves cursor lock mode (LOCKED/NONE) before opening menu
        {
            IsMouseLockedBeforePause = true;
        }
        else
        {
            IsMouseLockedBeforePause = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameObject.GetComponent<PlayerControllerWithCC>().EnableRotateAndMove(false);
        SetMenuState(true);  
    }
    public void CloseMenu() 
    {
        if (IsMouseLockedBeforePause)   //Settings cursors lock mode by saved boolean
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        Cursor.visible = !IsMouseLockedBeforePause;
        gameObject.GetComponent<PlayerControllerWithCC>().EnableRotateAndMove(IsMouseLockedBeforePause);
        SetMenuState(false); 
    }
}
