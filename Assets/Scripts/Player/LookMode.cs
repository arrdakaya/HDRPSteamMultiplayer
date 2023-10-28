using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LookMode : NetworkBehaviour
{
    [SerializeField] GameObject flashlightOverlay;
    [SerializeField] FlashlightScript flashlightScript;
    [SerializeField] bool flashLightOn;
    [SerializeField] Light flashLight;
    [SerializeField] private float drainTime = 2;
    // Start is called before the first frame update
    void Start()
    {
        flashLight.GetComponent<Light>().enabled = isLocalPlayer;
        flashLight.enabled = false;
        flashlightOverlay.SetActive(isLocalPlayer);
        //flashlightOverlay.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!flashLightOn && flashlightScript.batteryPower > 0)
            {
                //flashlightOverlay.SetActive(true);
                CMDFlashOn();
            }
            else
            {
                //flashlightOverlay.SetActive(false);
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
        flashlightScript.InvokeRepeating("BatteryDrain", drainTime, drainTime);
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
        flashlightScript.StopDrain();
        flashLightOn = false;
    }
    private void FlasLightSwitchOff()
    {       
        if(flashlightScript.batteryPower <= 0)
        {
            //flashlightOverlay.SetActive(false);
            CMDFlashOff();
        }
    }
}
