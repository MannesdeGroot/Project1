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

    void Start()
    {
        LoadSettings();

        Player[] test = FindObjectsOfType<Player>();

        foreach (Player p in test)
        {
            players.Add(p);
        }

        StartGame();
    }

    protected virtual void StartGame()
    {
        state = GameState.RUNNING;
    }

    protected void EndGame()
    {
        print("Game Over");
    }

    /*private void CountDown()
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
    }*/

    protected virtual void LoadSettings()
    {
        maxPlayers = GameSettings.maxPlayers;
        minPlayers = GameSettings.minPlayers;
    }
}
