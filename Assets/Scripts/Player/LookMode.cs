using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookMode : NetworkBehaviour
{
    [SerializeField] GameObject flashlightOverlay;
    [SerializeField] bool flashLightOn;
    [SerializeField] Light flashLight;

    // Start is called before the first frame update
    void Start()
    {
        flashLight.GetComponent<Light>().enabled = isLocalPlayer;
        flashLight.enabled = false;
        flashlightOverlay.SetActive(isLocalPlayer);
        flashlightOverlay.SetActive(false);
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!flashLightOn)
            {
                if(isLocalPlayer)
                flashlightOverlay.SetActive(true);

                CMDFlashOn();
            }
            else if(flashLightOn)
            {
                if(isLocalPlayer)
                flashlightOverlay.SetActive(false);
                CMDFlashOff();
            }
        }
        if (flashLightOn)
        {
            
            FlasLightSwitchOff();

        }
    }
    [Command]
    void CMDFlashOn()
    {
        RPCFlashOn();
    }

    [ClientRpc]
    private void RPCFlashOn()
    {
       
        flashLight.enabled = true;
        flashLightOn = true;
        FlasLightSwitchOff();
    }

    [Command]
    void CMDFlashOff()
    {
        RPCFlashOff();
    }

    [ClientRpc]
    private void RPCFlashOff()
    {
        
        flashLight.enabled = false;
        flashlightOverlay.GetComponent<FlashlightScript>().StopDrain();
        flashLightOn = false;
    }


    private void FlasLightSwitchOff()
    {
        
        if(flashlightOverlay.GetComponent<FlashlightScript>().batteryPower <= 0)
        {
            flashlightOverlay.SetActive(false);
            CMDFlashOff();
        }
    }
}
