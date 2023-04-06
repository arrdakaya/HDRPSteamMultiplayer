using Mirror;
using UnityEngine.UI;
using UnityEngine;

public class PlayerSelection : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSelectedButtonChanged))] private GameObject selectedButton = null;

    private ButtonSelection[] buttonSelections;

    private void Start()
    {
        buttonSelections = FindObjectsOfType<ButtonSelection>();
    }

    public void SelectButton(GameObject button)
    {
        if (!isLocalPlayer && selectedButton == button)
        {
            return;
        }   
            CmdSelectButton(button);
        
    }


    [Command]
    private void CmdSelectButton(GameObject button)
    {

        RpcDisableSelectedButton(button);


    }
    [ClientRpc]
    private void RpcDisableSelectedButton(GameObject button)
    {
        selectedButton = button;

        foreach (ButtonSelection buttonSelection in buttonSelections)
        {
            if(buttonSelection.gameObject == button)
            {
                buttonSelection.SetInteractable(false);

            }
        }
    }

    private void OnSelectedButtonChanged(GameObject oldValue, GameObject newValue)
    {
        
            if (oldValue != null)
            {
                oldValue.GetComponent<ButtonSelection>().SetInteractable(true);
            }

            if (newValue != null)
            {
                newValue.GetComponent<ButtonSelection>().SetInteractable(false);
            }
        
    }
}
