using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;

public class CutsceneManager : NetworkBehaviour
{
    private int playersInTrigger = 0;
    private bool timelineStarted = false;
    public GameObject timelineCamera;
    private bool isLocalPlayerInTrigger = false;
    private GameObject cutsceneTextObject;

    private CustomNetworkManager manager;
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
    private void Start()
    {

        cutsceneTextObject = GameObject.Find("CutsceneCounter");


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            cutsceneTextObject.GetComponent<TextMeshProUGUI>().enabled = true;

            if (other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                isLocalPlayerInTrigger = true;
            }
            playersInTrigger++;
            Debug.Log("playerintrigger: " + playersInTrigger);
            if(playersInTrigger > 0)
            {
                if (playersInTrigger == Manager.GamePlayers.Count - 1 && isLocalPlayerInTrigger && !timelineStarted)
                {
                    RpcStartTimeline(other.gameObject);
                }
            }
           



        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (isLocalPlayerInTrigger && cutsceneTextObject.activeSelf)
            {
                int playerCountWithoutMonster = Manager.GamePlayers.Count - 1;
                cutsceneTextObject.GetComponent<TextMeshProUGUI>().text = playersInTrigger.ToString() + "/" + playerCountWithoutMonster.ToString();
            }
        }
        

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                isLocalPlayerInTrigger = false;
            }
            playersInTrigger--;

            cutsceneTextObject.GetComponent<TextMeshProUGUI>().enabled = false;

        }
    }

    //[ClientRpc]
    void RpcStartTimeline(GameObject other)
    {

        timelineStarted = true;
        for (int i = 0; i < playersInTrigger; i++)
        {
            if (Manager.players[i].GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                Manager.players[i].GetComponent<CameraController>().DisableCamera();
            }
        }
        if (timelineCamera != null)
        {
            timelineCamera.SetActive(true);
        }
        StartCoroutine(CutsceneFinish(other));

    }

    IEnumerator CutsceneFinish(GameObject other)
    {
        other.GetComponent<PlayerMovementController>().canMove = false;

        yield return new WaitForSeconds(5);
        if (timelineCamera != null)
        {
            timelineCamera.SetActive(false);
        }
        for (int i = 0; i < playersInTrigger; i++)
        {
            if (Manager.players[i].GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                Manager.players[i].GetComponent<CameraController>().EnableCamera();
            }
        }
        other.GetComponent<PlayerMovementController>().canMove = true;


        Destroy(gameObject);
    }
  

}