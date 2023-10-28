using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gamePlayingClockText;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        gamePlayingClockText.text = GameManager.Instance.GetGamePlayingTimer();
    }
}
