using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public Button button;
    public static GameManager Instance { get; private set; }
    public event EventHandler OnStateChanged;

    private GameObject LocalPlayerObject;
    public GameObject EscMenu;
    public GameObject GamePlayingClockUI;

    private float startTime;
    private State state;
    private float waitingToStartTime = 1f;
    private float countdownToStartTime = 3f;
    private float gamePlayingTimer = 30f;
    string timerText;
    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver

    }
    private void Awake()
    {
        Instance = this;
        state = State.WaitingToStart;

    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");

    }

    public void EscapeMenuController()
    {

        if (!EscMenu.activeSelf)
        {
            EscMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
        else
        {
            EscMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }


    }
    public void MainMenuScene()
    {
        LocalPlayerObject.GetComponent<PlayerObjectController>().Quit();
        NetworkServer.Destroy(LocalPlayerObject);
    }


    private void Update()
    {
        if (!isServer)
        {
            return;
        }
        switch (state)
        {
            case State.WaitingToStart:
                waitingToStartTime -= Time.deltaTime;
                if (waitingToStartTime < 0f)
                {
                    state = State.CountdownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.CountdownToStart:
                countdownToStartTime -= Time.deltaTime;
                if (countdownToStartTime < 0f)
                {
                    state = State.GamePlaying;
                    GamePlayingClockUI.SetActive(true);
                    startTime = Time.time;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }
    public bool IsCountdownToStartActive()
    {
        return state == State.CountdownToStart;
    }
    public bool IsGameOver()
    {
        return state == State.GameOver;
    }
    public float GetCountdownToStartTimer()
    {
        return countdownToStartTime;
    }
    public string GetGamePlayingTimer()
    {
        float t = Time.time - startTime;
        string minutes = ((int)t / 60).ToString("00");
        string seconds = (t % 60).ToString("00");
        timerText = minutes + ":" + seconds;
        return timerText;
    }
    public bool IsLocalPlayerReady()
    {
        return isLocalPlayer;
    }
}
