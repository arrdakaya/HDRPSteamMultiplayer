using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUIManager : NetworkBehaviour
{
    public static PlayerUIManager Instance;
    public static GameManager GameManager;

    public bool isEscapePress = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }
    private void Start()
    {
        if(!isLocalPlayer) { return; }
    }
    private void Update()
    {
        if (!isLocalPlayer) { return; }
        EscapeMenu();
    }
    private void EscapeMenu()
    {
        if (SceneManager.GetActiveScene().name == "EarlyMapDesign")
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                isEscapePress = !isEscapePress;
                if (isEscapePress)
                {
                    gameObject.GetComponent<PlayerMovementController>().canMove = false;
                    gameObject.GetComponent<PlayerMovementController>().canCameraMove = false;
                    GameManager.Instance.EscapeMenuController();

                }
                else
                {
                    gameObject.GetComponent<PlayerMovementController>().canMove = true;
                    gameObject.GetComponent<PlayerMovementController>().canCameraMove = true;
                    GameManager.Instance.EscapeMenuController();

                }

            }
        }
    }
   
 

}

