﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VoteSystem : MonoBehaviour, Photon.Pun.IPunObservable
{
    public List<VoteButton> voteButtons;
    public float voteStartTime;
    float voteTime;
    public bool voting;
    public VoteButton currentVote;
    public PhotonView pv;
    public Text voteTimeText;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartVoting();
        }
        if (voting)
        {
            Voting();
        }
    }

    [PunRPC]
    public void StartVoting()
    {
        voting = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        voteTime = voteStartTime;
        
    }

    public void PhotonStartVoting()
    {
        pv.RPC("StartVoting", RpcTarget.All);
    }
    void Voting()
    {
        voteTime -= Time.deltaTime;
        if(voteTime < 1)
        {
            voteTimeText.text = "0";
        }
        else
        {
            voteTimeText.text = voteTime.ToString("#");
        }
        

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

        if (PhotonNetwork.IsMasterClient)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        }


    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(isTagger);

        }
        else if (stream.IsReading)
        {
            //isTagger = (bool)stream.ReceiveNext();

        }
    }
}
