using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    public GameObject playerCamera;

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
}
