using Mirror;
using UnityEngine;

public class CutsceneManager : NetworkBehaviour
{
    [SerializeField] private Camera cutsceneCamera;
    [SerializeField] private Camera[] playerCameras;
    [SyncVar(hook = nameof(OnCutsceneActiveChanged))] private bool cutsceneActive;
    public static CutsceneManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        cutsceneCamera.enabled = false;
    }

    private void Start()
    {
        if (isServer)
        {
            cutsceneActive = false;
        }
    }
    

    public override void OnStartAuthority() => playerCameras = FindObjectsOfType<Camera>();

    [Command]
    public void CmdSwitchToCutscene()
    {
        cutsceneActive = true;
        RpcSwitchToCutscene(true);
    }

    [ClientRpc]
    private void RpcSwitchToCutscene(bool active)
    {

        foreach (var playerCamera in playerCameras)
        {
            playerCamera.enabled = !active;
        }
        cutsceneCamera.enabled = active;
    }

    [ClientRpc]
    public void RpcSwitchToPlayerCamera()
    {

        foreach (var playerCamera in playerCameras)
        {
            playerCamera.enabled = true;
        }
        cutsceneCamera.enabled = false;
    }

    private void OnCutsceneActiveChanged(bool oldActive, bool newActive)
    {
        if (isLocalPlayer)
        {
            // Disable all player cameras if cutscene is active, enable them otherwise
            foreach (var playerCamera in playerCameras)
            {
                playerCamera.enabled = !newActive;
            }
            cutsceneCamera.enabled = newActive;
        }
    }
}