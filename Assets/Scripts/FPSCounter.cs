using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI FPStext;

    float pollingTime = 1f;
    private float time;
    int frameCount;
 

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        frameCount++;
        if(time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);
            FPStext.text = frameRate.ToString() + " FPS";

            time -= pollingTime;
            frameCount = 0;
        }
    }
}
