using Mirror;
using System.Collections;
using UnityEngine;

public class CutsceneManager : NetworkBehaviour
{
    private int playersInTrigger = 0;
    private bool timelineStarted = false;
    public GameObject timelineCamera;
    private bool isLocalPlayerInTrigger = false;

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
   
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                isLocalPlayerInTrigger = true;
            }
            playersInTrigger++;
            Debug.Log("playerintrigger: " + playersInTrigger);
            if (playersInTrigger == Manager.players.Count && isLocalPlayerInTrigger && !timelineStarted)
            {
                RpcStartTimeline(other.gameObject);
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
            Debug.Log("playerintrigger: " + playersInTrigger);

        }
    }

    //[ClientRpc]
    void RpcStartTimeline(GameObject other)
    {

        timelineStarted = true;
        for (int i = 0; i < playersInTrigger; i++)
        {
            Debug.Log(Manager.players[i]);
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
        other.GetComponent<PlayerMovementController>().AnimationValueZero();
        yield return new WaitForSeconds(1);
        other.GetComponent<PlayerMovementController>().enabled = false;

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
        other.GetComponent<PlayerMovementController>().enabled = true;

        Destroy(gameObject);
    }
  

}