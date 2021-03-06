﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame : MonoBehaviour, Photon.Pun.IPunObservable
{
    protected GameState state = GameState.STARTING;
    public int minPlayers, maxPlayers;
    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> asudfhsud = new List<GameObject>();
    public bool shadfksjd;
    private PhotonView pv;

    void Start()
    {
        pv = transform.GetComponent<PhotonView>();
        LoadSettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            pv.TransferOwnership(PhotonNetwork.MasterClient);
            //StartGame();

        }
    }

    public void StartGameActivate()
    {
        pv.RPC("StartGame", RpcTarget.All);
    }

    [PunRPC]
    public virtual void StartGame()
    {
        state = GameState.RUNNING;
        PlayerController[] test = FindObjectsOfType<PlayerController>();

        foreach (PlayerController p in test)
        {
            players.Add(p.gameObject);
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(players);
            stream.SendNext(shadfksjd);
            //stream.SendNext(asudfhsud);
        }
        else if (stream.IsReading)
        {
            //players = (List<GameObject>)stream.ReceiveNext();
            shadfksjd = (bool)stream.ReceiveNext();
            //asudfhsud = (List<GameObject>)stream.ReceiveNext();
        }
    }
}
