using Mirror;
using UnityEngine.UI;
using UnityEngine;

public class ButtonSelection : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnButtonSelected))] private bool isButtonSelected = false;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }
    [Command(requiresAuthority = false)]
    public void SetInteractable(bool interactable)
    {
        RPCSetInteractable(interactable);
    }
    [ClientRpc]
    void RPCSetInteractable(bool interactable)
    {
        if (isButtonSelected && button.interactable == true)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = interactable;
        }
    }


    public void OnButtonClick()
    {
        
        var playerSelection = NetworkClient.connection.identity.GetComponent<PlayerSelection>();
        playerSelection.SelectButton(gameObject);
    }

    private void OnButtonSelected(bool oldValue, bool newValue)
    {
        if(isButtonSelected == false)
        {
            isButtonSelected = newValue;
            SetInteractable(!newValue);
        }
        
    }
}
