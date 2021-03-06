﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class TrefballManager : MonoBehaviour, Photon.Pun.IPunObservable
{
    [Header("Time")]
    public float preRoundtime;
    [Header("Game")]
    public Transform[] spawnsTeam1;
    public Transform[] spawnsTeam2;
    public Transform ballSpawn;
    public GameObject ball;
    public int team1Amount;
    public int team2Amount;
    bool isStarted = false;
    PlayerController isMinePlayer;
    PhotonView pv;
    [Header("Vote")]
    public VoteSystem voteSystem;
    public GameObject voteSystemObj;
    [Header("Sounds")]
    public GameObject startRoundSound;


    void Start()
    {
        pv = GetComponent<PhotonView>();
        PlayerController[] players = FindObjectsOfType<PlayerController>();
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
        if(isMinePlayer == null)
        {
            PlayerController[] players = FindObjectsOfType<PlayerController>();
            if(players != null)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].pV.IsMine)
                    {
                        isMinePlayer = players[i];
                    }
                }

            }
        }
        isMinePlayer.pregameTimer.text = preRoundtime.ToString("#");
        if (PhotonNetwork.IsMasterClient)
        {
            preRoundtime -= Time.deltaTime;
            if (preRoundtime < 0)
            {
                SetTeams();
                pv.RPC("StartGame", RpcTarget.All);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", ball.name), ballSpawn.position, new Quaternion(0, Random.Range(0f, 1f), 0, 1));
            }
        }
    }

    public void CheckIfTeamWon(int teamWhoDied)
    {
        pv.RPC("PUNCheckIfTeamWon", RpcTarget.All, teamWhoDied);
    }

    [PunRPC]
    public void PUNCheckIfTeamWon(int teamWhoDied)
    {
        if (teamWhoDied == 1)
        {
            team1Amount--;
        }
        else if (teamWhoDied == 2)
        {
            team2Amount--;
        }
        if (PhotonNetwork.IsMasterClient)
        {

            if (team1Amount <= 0)
            {
                //team2won
                pv.RPC("EndGameForEveryone", RpcTarget.All, "TeamBlue");
            }
            else if (team2Amount <= 0)
            {
                //team1won
                pv.RPC("EndGameForEveryone", RpcTarget.All, "TeamRed");
            }

        }
    }

    [PunRPC]
    public void EndGameForEveryone(string winner)
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);

        }
        voteSystemObj.SetActive(true);
        voteSystem.PhotonStartVoting();

        if (PhotonNetwork.IsMasterClient)
        {
            voteSystem.SetWinner(winner);
        }
    }

    void SetTeams()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < players.Length; i++)
        {

            if(team1Amount < team2Amount)
            {
                players[i].TeleportPlayer(spawnsTeam1[Random.Range(0, spawnsTeam1.Length - 1)].position);
                players[i].SetTeam(1, true, ballSpawn.position.z);
                team1Amount++;
            }
            else
            {
                players[i].TeleportPlayer(spawnsTeam2[Random.Range(0, spawnsTeam2.Length - 1)].position);
                players[i].SetTeam(2, true, ballSpawn.position.z);
                team2Amount++;
            }
            
        }
    }

    [PunRPC]
    void StartGame()
    {
        isStarted = true;
        Instantiate(startRoundSound, transform.position, Quaternion.identity);
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
