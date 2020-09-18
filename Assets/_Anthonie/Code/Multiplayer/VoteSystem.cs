﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VoteSystem : MonoBehaviour
{
    public List<VoteButton> voteButtons;
    public float voteStartTime;
    float voteTime;
    public bool voting;
    public VoteButton currentVote;

    void Update()
    {
        if (voting)
        {
            Voting();
        }
    }

    public void StartVoting()
    {
        voting = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].voteScreen.SetActive(true);
        }
    }

    void Voting()
    {
        voteTime -= Time.deltaTime;

        

        if (voteTime < 0)
        {
            EndVoting();
        }
    }

    void EndVoting()
    {
        voting = false;
        voteTime = voteStartTime;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        PlayerController[] players = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].voteScreen.SetActive(false);
        }

        VoteButton tempVoteButton = null;
        bool moreThanOne = false;
        string sceneName = "";
        int saveVoteAmount = 0;

        for (int i = 0; i < voteButtons.Count; i++)
        {
            if(tempVoteButton == null)
            {
                tempVoteButton = voteButtons[i];
            }
            else if(tempVoteButton.voteAmount < voteButtons[i].voteAmount)
            {
                tempVoteButton = voteButtons[i];
                moreThanOne = false;
            }
            else if(tempVoteButton.voteAmount == voteButtons[i].voteAmount)
            {
                moreThanOne = true;
                saveVoteAmount = tempVoteButton.voteAmount;
            }
        }

        if (moreThanOne)
        {
            List<VoteButton> tempVoteButtons = new List<VoteButton>();
            for (int i = 0; i < voteButtons.Count; i++)
            {
                if(voteButtons[i].voteAmount == saveVoteAmount)
                {
                    tempVoteButtons.Add(voteButtons[i]);
                }
            }
            sceneName = tempVoteButtons[Random.Range(0, tempVoteButtons.Count - 1)].sceneName;
        }
        else
        {
            sceneName = tempVoteButton.sceneName;
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);


    }
}