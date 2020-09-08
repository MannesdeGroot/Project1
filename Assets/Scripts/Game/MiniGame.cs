using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGame : MonoBehaviour
{
    protected GameState state = GameState.STARTING;
    public int minPlayers, maxPlayers;
    public List<Player> players = new List<Player>();

    void Start()
    {
        LoadSettings();
    }

    public virtual void StartGame()
    {
        state = GameState.RUNNING; 
        Player[] test = FindObjectsOfType<Player>();

        foreach (Player p in test)
        {
            players.Add(p);
        }
    }

    protected void EndGame()
    {
        print("Game Over");
    }

    protected virtual void LoadSettings()
    {
        maxPlayers = GameSettings.maxPlayers;
        minPlayers = GameSettings.minPlayers;
    }
}
