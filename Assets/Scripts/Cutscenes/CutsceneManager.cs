using Mirror;
using StarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;
public class CutsceneManager : NetworkBehaviour
{
    private TextMeshProUGUI cutsceneTextObject;
    [SyncVar] private int playersInTrigger = 0;
    int playerCountWithoutMonster = 0;
    private bool timelineStarted = false;
    public GameObject timelineCamera;
    private bool isLocalPlayerInTrigger = false;
    private string playerTriggerStr;
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
        cutsceneTextObject = GameObject.Find("CutsceneCounter").GetComponent<TextMeshProUGUI>();
        if (GameObject.FindGameObjectWithTag("Monster"))
        {
            playerCountWithoutMonster = Manager.GamePlayers.Count - 1;

        }
        else
        {
            playerCountWithoutMonster = Manager.GamePlayers.Count;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            cutsceneTextObject.enabled = true;
            if (other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                isLocalPlayerInTrigger = true;
            }
            playersInTrigger++;
            if (isLocalPlayerInTrigger && cutsceneTextObject.gameObject.activeSelf)
            {
                cutsceneTextObject.text = playersInTrigger.ToString() + "/" + playerCountWithoutMonster.ToString();
            }
            if (playersInTrigger > 0)
            {
                if ((playersInTrigger == playerCountWithoutMonster) && isLocalPlayerInTrigger && !timelineStarted)
                {
                    CmdStartTimeline(other.gameObject);
                }
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
            cutsceneTextObject.enabled = false;
        }
    }
   
    [Command(requiresAuthority =false)]
    void CmdStartTimeline(GameObject other)
    {
        RpcStartTimeline(other);
    }
    [ClientRpc]
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