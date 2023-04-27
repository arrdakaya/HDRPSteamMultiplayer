using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float startTime;
    public TextMeshProUGUI timeText;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Time.time - startTime;
        string minutes = ((int)t / 60).ToString("00");
        string seconds = (t % 60).ToString("00");

        timeText.text = minutes + ":" + seconds;
    }
}
