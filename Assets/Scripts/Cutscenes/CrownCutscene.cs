using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownCutscene : NetworkBehaviour
{
    [SerializeField] private GameObject cutsceneCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {

        StartCoroutine(PlayCutscene());
    }
    private IEnumerator PlayCutscene()
    {
        cutsceneCamera.SetActive(true);

        // Cutscene code goes here...

        yield return new WaitForSeconds(5f);

        cutsceneCamera.SetActive(false);
        RpcSwitchToPlayerCamera();
    }
    [ClientRpc]
    private void RpcSwitchToPlayerCamera()
    {
        if (isLocalPlayer)
        {
            // Switch back to the player's camera
            Camera.main.enabled = true; // Enable the player's camera
            cutsceneCamera.SetActive(false); // Disable the cutscene camera
        }
    }
}
