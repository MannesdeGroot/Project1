using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MiniGame : MonoBehaviour
{
    protected GameState state = GameState.STARTING;
    public int minPlayers, maxPlayers;
    public List<Player> players = new List<Player>();

    [Header("Time")]
    [SerializeField] private bool timed;
    [SerializeField] private bool roundLimit;
    [SerializeField] protected int maxRounds;
    private int currentRound;
    public float matchTime;
    private float matchTimer;
    [SerializeField] private Text timerText;

    [Header("Events")]
    public UnityEvent roundStart;
    public UnityEvent roundEnd;
    public UnityEvent gameEnd;

    void Start()
    {
        LoadSettings();

        timerText.gameObject.SetActive(timed);

        Player[] test = FindObjectsOfType<Player>();

        foreach (Player p in test)
        {
            players.Add(p);
        }

        StartRound();
    }

    void Update()
    {
        if (timed) CountDown();
    }

    protected virtual void StartRound()
    {
        if (state != GameState.RUNNING)
            state = GameState.RUNNING;

        matchTimer = matchTime;
        roundStart.Invoke();
    }

    protected virtual void EndRound()
    {
        if (roundLimit)
        {
            currentRound++;
            if (currentRound >= maxRounds) EndGame();
            return;
        }

        roundEnd.Invoke();
    }

    protected void EndGame()
    {
        print("Game Over");
        gameEnd.Invoke();
    }

    private void CountDown()
    {
        if (state != GameState.RUNNING) return;

        matchTimer -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(matchTimer / 60);
        int seconds = Mathf.FloorToInt(matchTimer % 60);
        string sec = seconds < 10 ? $"0{seconds}" : seconds.ToString();

        if (minutes >= 0 && seconds >= 0)
            timerText.text = $"{minutes}:{sec}";

        if (matchTimer <= 0)
        {
            EndRound();
        }
    }

    protected virtual void LoadSettings()
    {
        maxPlayers = GameSettings.maxPlayers;
        minPlayers = GameSettings.minPlayers;
        timed = GameSettings.timed;
        matchTime = GameSettings.matchTime;
        roundLimit = GameSettings.limitedRounds;
        maxRounds = GameSettings.maxRounds;
    }
}
