﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class TagManager : MonoBehaviour, IPunObservable
{
    public GameState state;
    public int taggersAmount;
    public float startDelay;
    public float roundTime;
    public float timer;
    public List<string> playerIDs = new List<string>();
    public PhotonView pV;
    public string[] playerIDsArray;
    private List<PlayerController> players = new List<PlayerController>();
    public GameObject voteCam;
    public VoteSystem voteSystem;
    public GameObject startRoundSound;

    void Start()
    {
        state = GameState.STARTING;
        StartCoroutine(PreGameCountDown());
        pV = GetComponent<PhotonView>();
    }

    public void StartGame()
    {
        if (state == GameState.FINISHED)
            return;

        state = GameState.RUNNING;
        players = FindObjectsOfType<PlayerController>().ToList();

        foreach (PlayerController player in players)
        {
            player.roleText.gameObject.SetActive(true);
            player.timerText.gameObject.SetActive(true);
        }

        if (players.Count > 1)
        {
            StartRound();
        }
        else
        {
            EndRound();
        }
    }

    private void StartRound()
    {
        if (state == GameState.FINISHED)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < taggersAmount; i++)
            {
                PlayerController randPlayer = players[Random.Range(0, players.Count)];

                if (!randPlayer.isTagger)
                {
                    randPlayer.PhotonTag(transform.position, 0);
                }
                else
                {
                    i--;
                }
            }
            pV.RPC("PlayStartRoundSound", RpcTarget.All);
        }
        timer = roundTime;
        StartCoroutine(CountDown());

        foreach (PlayerController player in players)
        {
            player.SetTagger(player.isTagger);
        }
    }

    [PunRPC]
    void PlayStartRoundSound()
    {
        Instantiate(startRoundSound, transform.position, Quaternion.identity);
    }
    private void EndRound()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].isTagger)
            {
                players[i].Eliminate();
                players.Remove(players[i]);
            }
        }

        if (players.Count > 1)
        {
            StartRound();
        }
        else
        {
            state = GameState.FINISHED;
            print($"{players[0]} won");
            if (players[0] != null)
            {
                string winner = players[0].pV.Owner.NickName;
                Destroy(players[0].gameObject);
                voteCam.SetActive(true);
                voteSystem.PhotonStartVoting();
                if (PhotonNetwork.IsMasterClient)
                {
                    voteSystem.SetWinner(winner);
                }

            }
        }
    }

    private IEnumerator CountDown()
    {
        if (timer <= 0)
            EndRound();

        if (timer >= 0)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            string secondsText = seconds < 10 ? $"0{seconds}" : seconds.ToString();

            foreach (PlayerController player in players)
            {
                if (player.timerText != null)
                    player.timerText.text = $"{minutes}:{secondsText}";
            }
        }

        yield return new WaitForSeconds(1);
        timer--;

        StartCoroutine(CountDown());
    }

    private IEnumerator PreGameCountDown()
    {
        if (state == GameState.STARTING)
        {
            List<PlayerController> pregamePlayers = FindObjectsOfType<PlayerController>().ToList();

            int minutes = Mathf.FloorToInt(startDelay / 60);
            int seconds = Mathf.FloorToInt(startDelay % 60);
            string secText = seconds < 10 ? $"0{seconds}" : seconds.ToString();

            foreach (PlayerController player in pregamePlayers)
            {
                player.pregameTimer.text = $"{minutes}:{secText}";
            }

            if (startDelay < 0)
            {
                foreach (PlayerController player in pregamePlayers)
                {
                    player.pregameTimer.gameObject.SetActive(false);
                }

                StartGame();
            }
            else
            {
                yield return new WaitForSeconds(1);
                startDelay--;
                StartCoroutine(PreGameCountDown());
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(timer);
                stream.SendNext(startDelay);
            }
        }
        if (stream.IsReading)
        {
            timer = (int)stream.ReceiveNext();
            startDelay = (int)stream.ReceiveNext();
        }
    }
}
