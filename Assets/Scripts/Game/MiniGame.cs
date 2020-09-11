using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame : MonoBehaviour, Photon.Pun.IPunObservable
{
    protected GameState state = GameState.STARTING;
    public int minPlayers, maxPlayers;
    public List<PlayerController> players = new List<PlayerController>();

    void Start()
    {
        LoadSettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            StartGame();
    }

    public virtual void StartGame()
    {
        state = GameState.RUNNING; 
        PlayerController[] test = FindObjectsOfType<PlayerController>();

        foreach (PlayerController p in test)
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

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(players);
        }
        else if (stream.IsReading)
        {
            players = (List<PlayerController>)stream.ReceiveNext();
        }
    }
}
