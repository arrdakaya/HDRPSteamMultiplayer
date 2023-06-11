using Mirror;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : NetworkBehaviour
{
    public static CameraController instance;
    public GameObject playerCamera;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    public void DisableCamera()
    {
        if (isLocalPlayer)
        {
            playerCamera.SetActive(false);
        }
    }

    public void EnableCamera()
    {
        if (isLocalPlayer)
        {
            playerCamera.SetActive(isLocalPlayer);
        }
    }
    public void BlindCamera()
    {
        StartCoroutine(BlindPlayer());
    }

    IEnumerator BlindPlayer()
    {

        this.gameObject.transform.GetChild(0).GetComponent<Volume>().enabled = true;
        yield return new WaitForSeconds(2);
        this.gameObject.transform.GetChild(0).GetComponent<Volume>().enabled = false;

    }
}
