using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameObject LocalPlayerObject;
    public GameObject EscMenu;
   
    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }
    void Start()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");

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
    public void MainMenuScene()
    {
        PlayerObjectController.Instance.Quit();
        NetworkServer.Destroy(LocalPlayerObject);

    }
}
