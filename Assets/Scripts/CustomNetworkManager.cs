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
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();
    List<PlayerObjectController> players = new List<PlayerObjectController>();

    

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


    public void StartGame(string SceneName)
    {
        SceneLoader.Instance.RpcLoadScene(SceneName);
    }
   

   

    public void SpawnPlayer()
    {
        for (int i = 0; i < GamePlayers.Count; i++)
        {
            players.Add(Instantiate(myPlayer[GamePlayers[i].PlayerSelectedCharacter]));
            var conn = GamePlayers[i].connectionToClient;
            players[i].ConnectionID = GamePlayers[i].ConnectionID;
            players[i].PlayerIdNumber = GamePlayers.Count;
            players[i].PlayerSteamID = GamePlayers[i].PlayerSteamID;
            players[i].PlayerSelectedCharacter = GamePlayers[i].PlayerSelectedCharacter;
            players[i].lobbyID = GamePlayers[i].lobbyID;
            Destroy(GamePlayers[i].gameObject);
            if (GamePlayers[i].gameObject != null)
                NetworkServer.ReplacePlayerForConnection(conn, players[i].gameObject, true);

        }
    }
  
}
