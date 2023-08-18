using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : NetworkBehaviour
{
    public static CharacterSelection Instance;
    public Button ReadyButton;

  
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }


    public void SelectCharacter(int selectedIndex)
    {
        if(selectedIndex == 0)
        {
            ReadyButton.interactable = false;
        }
        else
        {
            ReadyButton.interactable = true;
        }
        LobbyController.Instance.LocalPlayerController.CmdPlayerSelection(selectedIndex);
    }
}
