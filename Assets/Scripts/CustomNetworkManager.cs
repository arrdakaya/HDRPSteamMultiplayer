using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> myPlayer;
    PlayerObjectController GamePlayerInstance;
    PlayerObjectController gameplayerInstance;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();
    public List<PlayerObjectController> players = new List<PlayerObjectController>();
    public GameObject objectSpawner;

    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if(SceneManager.GetActiveScene().name == "Lobby")
        {
            GamePlayerInstance = Instantiate(GamePlayerPrefab);
           

            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);
            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);

        }
    }



    public override void ServerChangeScene(string newSceneName)
    {
        if (SceneManager.GetActiveScene().name == "Lobby" && newSceneName.StartsWith("EarlyMapDesign"))
        {
            Debug.Log("gameplayer.count" + GamePlayers.Count);

            for (int i = GamePlayers.Count - 1; i >= 0; i--)
            {
                var conn = GamePlayers[i].connectionToClient;
                gameplayerInstance = Instantiate(myPlayer[GamePlayers[i].PlayerSelectedCharacter], new Vector3(Random.Range(0, -12), 10, Random.Range(40, 50)), Quaternion.identity);
                gameplayerInstance.ConnectionID = GamePlayers[i].ConnectionID;
                gameplayerInstance.PlayerIdNumber = GamePlayers.Count;
                gameplayerInstance.PlayerSteamID = GamePlayers[i].PlayerSteamID;
                gameplayerInstance.PlayerName = GamePlayers[i].PlayerName;
                gameplayerInstance.PlayerSelectedCharacter = GamePlayers[i].PlayerSelectedCharacter;
                gameplayerInstance.lobbyID = GamePlayers[i].lobbyID;
                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject, true);
                players.Add(gameplayerInstance);

            }


        }

        NetworkServer.SpawnObjects();
        base.ServerChangeScene(newSceneName);

    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith("EarlyMapDesign"))
        {
            GameObject objectSpawnSystem = Instantiate(objectSpawner);
            NetworkServer.Spawn(objectSpawnSystem);
        }
    }


    public void StartGame(string SceneName)
    {
        
            ServerChangeScene(SceneName);

        

        //SceneLoader.Instance.RpcLoadScene(SceneName);

    }




}
