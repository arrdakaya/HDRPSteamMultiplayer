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
        selectedIndex = 0;
    }
   
   
    public void SelectMonster()
    {
        selectedIndex = 1;
        
            LobbyController.Instance.LocalPlayerController.CmdPlayerSelection(selectedIndex);
        

    }


    public void SelectHuman()
    {
        
        selectedIndex = 0;
        
        LobbyController.Instance.LocalPlayerController.CmdPlayerSelection(selectedIndex);
        
    }
}
