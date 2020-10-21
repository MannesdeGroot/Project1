using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class TrefballManager : MonoBehaviour, Photon.Pun.IPunObservable
{
    public float preRoundtime;
    public Transform[] spawnsTeam1;
    public Transform[] spawnsTeam2;
    public Transform ballSpawn;
    int team1Amount;
    int team2Amount;
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
        if(teamWhoDied == 1)
        {
            team1Amount--;
        }
        else if(teamWhoDied == 2)
        {
            team2Amount--;
        }
        if (PhotonNetwork.IsMasterClient)
        {

            if(team1Amount <= 0)
            {
                //team2won
                pv.RPC("EndGameForEveryone", RpcTarget.All);
            }
            else if(team2Amount <= 0)
            {
                //team1won
                pv.RPC("EndGameForEveryone", RpcTarget.All);
            }

        }
    }

    [PunRPC]
    public void EndGameForEveryone()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < players.Length; i++)
        {
            Destroy(players[i].gameObject);

        }
        voteSystemObj.SetActive(true);
        voteSystem.PhotonStartVoting();
    }

    void SetTeams()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].isClamped = true;
            players[i].zClamp = ballSpawn.position.z;

            if(team1Amount < team2Amount)
            {
                players[i].SetTeam(1);
                players[i].TeleportPlayer(spawnsTeam1[Random.Range(0, spawnsTeam1.Length - 1)].position);
                team1Amount++;
            }
            else
            {
                players[i].SetTeam(2);
                players[i].TeleportPlayer(spawnsTeam2[Random.Range(0, spawnsTeam2.Length - 1)].position);
                team2Amount++;
            }
        }
    }

    [PunRPC]
    void StartGame()
    {
        isStarted = true;

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
