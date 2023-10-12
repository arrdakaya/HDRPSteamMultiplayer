using Mirror;
using StarterAssets;
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
    public GameObject monsterCharacter;
    private bool speedUp = false;
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
        Debug.Log("TimerTimerTimer");
        if (GameObject.FindGameObjectWithTag("Monster"))
        {
            monsterCharacter = GameObject.FindGameObjectWithTag("Monster");

        }
        allPlayersCount = FindObjectsOfType<PlayerObjectController>().Length;
        if (isServer)
        {
            startTime = Time.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
            if (isServer)
            {
            CmdStartTimer();
            }
        
        
        
    }
    [Command(requiresAuthority = false)]
    private void CmdStartTimer()
    {
        RpcStartTimer();
    }

    //[ClientRpc]
    private void RpcStartTimer()
    {
       
        float t = Time.time - startTime;
        string minutes = ((int)t / 60).ToString("00");
        string seconds = (t % 60).ToString("00");
        timeText.text = minutes + ":" + seconds;
        if (monsterCharacter != null)
        {
            if (t >= 60.0 && !speedUp)
            {
                Debug.Log("1 dakika oldu");
                monsterCharacter.GetComponent<PlayerMovementController>().MoveSpeed = monsterCharacter.GetComponent<PlayerMovementController>().MoveSpeed * 2;
                monsterCharacter.GetComponent<PlayerMovementController>().SprintSpeed = monsterCharacter.GetComponent<PlayerMovementController>().SprintSpeed * 2;
                speedUp = true;
            }
           
        }
        
    }
}
