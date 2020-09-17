using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteButton : MonoBehaviour
{
    public PhotonView pV;
    public int voteAmount;
    public Text voteCountText;
    public string sceneName;
    public VoteSystem voteSystem;
    void Start()
    {
        pV = GetComponent<PhotonView>();
        voteSystem.voteButtons.Add(this);
    }

    void Update()
    {
        
    }

    public void VoteActivate()
    {
        pV.RPC("Vote", RpcTarget.All);
        if(voteSystem.currentVote != null)
        {
            voteSystem.currentVote.UnVoteActivate();
        }
        voteSystem.currentVote = this;
    }

    public void UnVoteActivate()
    {
        pV.RPC("UnVote", RpcTarget.All);
    }

    [PunRPC]
    public void Vote()
    {
        voteAmount += 1;
    }

    [PunRPC]
    public void UnVote()
    {
        voteAmount -= 1;
    }

    [PunRPC]
    public void UpdateVoteCountText()
    {
        voteCountText.text = new string('I', voteAmount);
    }



}
