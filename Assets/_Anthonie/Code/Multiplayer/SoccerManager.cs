using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.IO;

public class SoccerManager : MonoBehaviour, Photon.Pun.IPunObservable
{
    [Header("Time")]
    public float preRoundTime;
    public float roundTime;
    public float addTimeIfTied;
    [Header("Game")]
    public Transform ballSpawn;
    PhotonView pv;
    int team1points;
    int team2points;
    public Transform[] spawnsTeam1;
    public Transform[] spawnsTeam2;
    PlayerController isMinePlayer;
    bool started = false;
    [Header("Vote")]
    public VoteSystem voteSystem;
    public GameObject voteObj;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!started)
        {
            //preround
            if (PhotonNetwork.IsMasterClient)
            {
                preRoundTime -= Time.deltaTime;

                if(preRoundTime < 0)
                {
                    StartGame();
                }
            }
            if(isMinePlayer != null)
            {
                isMinePlayer.pregameTimer.text = preRoundTime.ToString("#");

            }
            else
            {
                PlayerController[] players = GameObject.FindObjectsOfType<PlayerController>();
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].pV.IsMine)
                    {
                        isMinePlayer = players[i];
                    }
                }
            }

        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                roundTime -= Time.deltaTime;

                if (roundTime < 0)
                {
                    CheckWhoWon();
                }
            }
            if (isMinePlayer != null)
            {
                isMinePlayer.team1PointsText.gameObject.SetActive(true);
                isMinePlayer.team2PointsText.gameObject.SetActive(true);
                if (roundTime < 60)
                {
                    isMinePlayer.pregameTimer.text = roundTime.ToString("#");

                }
                else
                {
                    bool done = false;
                    int mins = 0;
                    float secs = roundTime;
                    for (int i = 0; i < 100; i++)
                    {
                        if (!done)
                        {
                            if (secs - 60 > 0)
                            {
                                secs -= 60;
                                mins++;
                            }
                            else if(secs < 1)
                            {
                                isMinePlayer.pregameTimer.text = mins.ToString() + ":00";
                                done = true;
                            }
                            else if(secs < 9.5)
                            {
                                isMinePlayer.pregameTimer.text = mins.ToString() + ":0" + secs.ToString("#");
                                done = true;
                            }
                            else
                            {
                                isMinePlayer.pregameTimer.text = mins.ToString() + ":" + secs.ToString("#");
                                done = true;
                            }

                        }
                    }
                }
            }

            else
            {
                PlayerController[] players = GameObject.FindObjectsOfType<PlayerController>();
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].pV.IsMine)
                    {
                        isMinePlayer = players[i];
                    }
                }
            }
        }
    }

    
    void StartGame()
    {
        started = true;
        SetTeams();
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Football"), ballSpawn.position, Quaternion.identity);
    }

    public void AddPoint(int goalScored)
    {
        pv.RPC("PUNAddPoint", RpcTarget.All, goalScored);
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Football"), ballSpawn.position, Quaternion.identity);

    }

    [PunRPC]
    public void PUNAddPoint(int goalScored)
    {
        if (goalScored == 1)
        {
            team2points++;
        }
        else if (goalScored == 2)
        {
            team1points++;
        }
        isMinePlayer.team1PointsText.text = team1points.ToString();
        isMinePlayer.team2PointsText.text = team2points.ToString();
    }

    void CheckWhoWon()
    {
        if(team1points == team2points)
        {
            //tied
            roundTime += addTimeIfTied;
        }
        else if(team1points > team2points)
        {
            //team1 won


            pv.RPC("EndGame", RpcTarget.All);
        }
        else
        {
            //team2 won


            pv.RPC("EndGame", RpcTarget.All);
        }
    }

    void SetTeams()
    {
        int team1Amount = 0;
        int team2Amount = 0;
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < players.Length; i++)
        {

            if (team1Amount < team2Amount)
            {
                players[i].TeleportPlayer(spawnsTeam1[Random.Range(0, spawnsTeam1.Length - 1)].position);
                players[i].SetTeam(1, false, 0);
                team1Amount++;
            }
            else
            {
                players[i].TeleportPlayer(spawnsTeam2[Random.Range(0, spawnsTeam2.Length - 1)].position);
                players[i].SetTeam(2, false, 0);
                team2Amount++;
            }

        }
    }

    [PunRPC]
    void EndGame()
    {
        PlayerController[] players = GameObject.FindObjectsOfType<PlayerController>();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].Eliminate();
        }

        voteObj.SetActive(true);
        voteSystem.PhotonStartVoting();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(preRoundTime);
            stream.SendNext(roundTime);
            stream.SendNext(started);
            
        }
        else if (stream.IsReading)
        {
            preRoundTime = (float)stream.ReceiveNext();
            roundTime = (float)stream.ReceiveNext();
            started = (bool)stream.ReceiveNext();
            
        }
    }
}
