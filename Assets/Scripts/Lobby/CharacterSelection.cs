using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : NetworkBehaviour
{
    public static CharacterSelection Instance;
    public int selectedIndex;
   

  
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }
    private void Start()
    {
        selectedIndex = 2;
    }
   
    public void SelectCharacter(int selectedIndex)
    {
        LobbyController.Instance.LocalPlayerController.CmdPlayerSelection(selectedIndex);
    }
}
