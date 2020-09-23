using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ParkourManager : MonoBehaviour, Photon.Pun.IPunObservable
{
    private bool gameEnded;
    public GameObject voteCam;
    public VoteSystem voteSystem;
    public PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    private void EndGame(PlayerController winner)
    {
        /*foreach (PlayerController player in FindObjectsOfType<PlayerController>())
        {
            print(1);
            Destroy(player.gameObject);
            voteCam.SetActive(true);
            voteSystem.PhotonStartVoting();
            
        }*/

        print($"{winner} won");
        pv.RPC("EndGameForEveryone", RpcTarget.All);
    }

    private void OnTriggerEnter(Collider c)
    {
        PlayerController player = c.GetComponent<PlayerController>();

        if (player != null && !gameEnded)
        {
            EndGame(player);
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
        voteCam.SetActive(true);
        voteSystem.PhotonStartVoting();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(taggers);
        }
        else if (stream.IsReading)
        {
            //taggers = (List<GameObject>)stream.ReceiveNext();
        }
    }
}
