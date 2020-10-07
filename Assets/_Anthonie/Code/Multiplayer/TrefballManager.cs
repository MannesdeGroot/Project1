﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class TrefballManager : MonoBehaviour, Photon.Pun.IPunObservable
{
    public float preRoundtime;
    public Transform[] spawnsTeam1;
    public Transform[] spawnsTeam2;
    List<PlayerController> team1Players = new List<PlayerController>();
    List<PlayerController> team2Players = new List<PlayerController>();
    List<string> team1PlayersIDs = new List<string>();
    List<string> team2PlayersIDs = new List<string>();
    PhotonView pv;
    public GameObject ball;
    public VoteSystem voteSystem;
    public GameObject voteSystemObj;
    PlayerController isMinePlayer;
    bool isStarted = false;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].pV.IsMine)
            {
                isMinePlayer = players[i];
            }
        }
    }


    void Update()
    {
        if (!isStarted)
        {
            PreGameTimber();
        }
    }

    void PreGameTimber()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            preRoundtime -= Time.deltaTime;
            if (preRoundtime < 0)
            {
                pv.RPC("StartGame", RpcTarget.All, team1PlayersIDs.ToArray(), team2PlayersIDs.ToArray());
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", ball.name), Vector3.zero, Quaternion.identity);
            }
        }
        isMinePlayer.timerText.text = preRoundtime.ToString("#");
    }

    [PunRPC]
    public void StartGame(string[] team1IDs, string[] team2IDs)
    {
        isStarted = true;
        isMinePlayer.timerText.gameObject.SetActive(false);
        team1Players = new List<PlayerController>();
        team2Players = new List<PlayerController>();
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        DistributePlayers();
        for (int i = 0; i < players.Length; i++)
        {
            for (int j = 0; j < team1IDs.Length; j++)
            {
                if(team1IDs[j] == players[i].pV.ViewID.ToString())
                {
                    team1Players.Add(players[i]);
                }
            }
            for (int j = 0; j < team2IDs.Length; j++)
            {
                if (team2IDs[j] == players[i].pV.ViewID.ToString())
                {
                    team2Players.Add(players[i]);
                }
            }
        }

        for (int i = 0; i < team1Players.Count; i++)
        {
            if (team1Players[i].pV.IsMine)
            {
                team1Players[i].TeleportPlayer(spawnsTeam1[Random.Range(0, spawnsTeam1.Length)].position);
                isMinePlayer.SetTeam(1);
            }
        }
        for (int i = 0; i < team2Players.Count; i++)
        {
            if (team2Players[i].pV.IsMine)
            {
                team2Players[i].TeleportPlayer(spawnsTeam1[Random.Range(0, spawnsTeam1.Length)].position);
                isMinePlayer.SetTeam(2);
            }
        }
    }

    void DistributePlayers()
    {
        team1PlayersIDs = new List<string>();
        team2PlayersIDs = new List<string>();
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < players.Length; i++)
        {
            if(team1PlayersIDs.Count == team2PlayersIDs.Count)
            {
                team1PlayersIDs.Add(players[i].pV.ViewID.ToString());
            }
            else if(team1PlayersIDs.Count < team2PlayersIDs.Count)
            {
                team1PlayersIDs.Add(players[i].pV.ViewID.ToString());
            }
            else
            {
                team2PlayersIDs.Add(players[i].pV.ViewID.ToString());
            }
        }
    }

    void EndGame()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);
        }

        voteSystemObj.SetActive(true);
        voteSystem.PhotonStartVoting();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(preRoundtime);

        }
        else if (stream.IsReading)
        {
            preRoundtime = (float)stream.ReceiveNext();

        }
    }
}
