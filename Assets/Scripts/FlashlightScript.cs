using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightScript : MonoBehaviour
{

    [SerializeField]private Image InnerBattery;
    public float batteryPower =1.0f;
    public float batteryAmount = 1.0f;
    [SerializeField] private float drainTime = 2;
    // Start is called before the first frame update
    void OnEnable()
    {
        
        InnerBattery = GameObject.Find("BatteryInner").GetComponent<Image>();
        InvokeRepeating("BatteryDrain", drainTime, drainTime);
    }

    // Update is called once per frame
    void Update()
    {
        batteryPower = Mathf.Clamp(batteryPower, 0, 1);

        InnerBattery.fillAmount = batteryPower;
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (SaveScript.itemAmts[0] > 0 && batteryPower != 1)
            {
                batteryPower += batteryAmount;
                SaveScript.itemAmts[0]--;
            }
        }
        
    }
   
    void BatteryDrain()
    {
        
        if (batteryPower > 0.0f)
        {
            
            batteryPower -= 0.01f;
        }
    }
    public void StopDrain()
    {
        CancelInvoke("BatteryDrain");
    }
}
