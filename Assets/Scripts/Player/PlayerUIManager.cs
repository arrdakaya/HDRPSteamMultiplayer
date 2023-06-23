using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance;

    public GameObject EscMenu;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }
    private void Update()
    {
       
    }
    public void MainMenuScene()
    {
        PlayerObjectController.Instance.Quit();
    }
    public void EscapeMenuController()
    {
        if (!EscMenu.activeSelf)
        {
            EscMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            EscMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

}

