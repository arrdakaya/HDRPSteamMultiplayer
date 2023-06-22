using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Timer : NetworkBehaviour
{
    private float startTime;
    public TextMeshProUGUI timeText;
    private CustomNetworkManager manager;
    int allPlayersCount = 0;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }
    void Start()
    {
        allPlayersCount = FindObjectsOfType<PlayerMovementController>().Length;
        if (allPlayersCount == Manager.playerCount)
        {
            Debug.Log("allpayerscount:" + allPlayersCount);
            Debug.Log("managerPlayer Count:" + Manager.playerCount);
            startTime = Time.time;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (allPlayersCount == Manager.GamePlayers.Count(x => x.connectionToClient.isReady))
        {
            RpcStartTimer();
        }
    }

    [ClientRpc]
    private void RpcStartTimer()
    {
       
        float t = Time.time - startTime;
        string minutes = ((int)t / 60).ToString("00");
        string seconds = (t % 60).ToString("00");

        timeText.text = minutes + ":" + seconds;
        
    }
}
