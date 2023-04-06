using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;

public class PlayerObjectController : NetworkBehaviour
{
    public static PlayerObjectController Instance;

    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName ;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;
    [SyncVar(hook = nameof(PlayerCharacterSelection))] public int PlayerSelectedCharacter;

    public CSteamID lobbyID;


    private CustomNetworkManager manager;
    private CustomNetworkManager Manager
    {
        get
        {
            if(manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

    }

    private void Update()
    {
        if (!isLocalPlayer) { return;}
       
    }
    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            this.Ready = newValue;
        }
        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
    }

    public void ChangeReady()
    {
        if (isOwned)
        {
            CmdSetPlayerReady();
        }
    }


    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }
    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }
    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }
    [Command]
    private void CmdSetPlayerName(string PlayeName)
    {
        this.PlayerNameUpdate(this.PlayerName, PlayeName);
    }
    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
        {
            this.PlayerName = newValue;
        }
        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }
    public void CanStartGame(string sceneName)
    {
        if (isOwned)
        {
            CmdCanStartGame(sceneName);
        }
    }

    [Command]
    public void CmdCanStartGame(string sceneName)
    {
        manager.StartGame(sceneName);
    }

  
   

    [Command(requiresAuthority = false)]
    public void CmdPlayerSelection(int newValue)
    {
        PlayerCharacterSelection(PlayerSelectedCharacter,newValue);
    }
    private void PlayerCharacterSelection(int oldValue, int newValue)
    {
        if (isServer)
        {
            PlayerSelectedCharacter = newValue;
        }
        if (isClient && (oldValue != newValue))
        {
            UpdateCharacterIndex(newValue);
        }
    }
    void UpdateCharacterIndex(int message)
    {
        PlayerSelectedCharacter = message;
    }
    public void Quit()
    {
        //Set the offline scene to null
        manager.offlineScene = "";

        //Make the active scene the offline scene
        SceneManager.LoadScene("MainMenu");

        SteamLobby.Instance.LeaveLobby();

        if (isOwned)
        {
            if (isServer)
            {
                manager.StopHost();
            }
            else
            {
                manager.StopClient();
            }
        }
    }
}
