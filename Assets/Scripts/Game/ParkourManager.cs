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
    public List<GameObject> waypoints = new List<GameObject>();
    int currentWaypoint = 0;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        for (int i = 1; i < waypoints.Count; i++)
        {
            waypoints[i].SetActive(false);
        }
    }

    private void EndGame()
    {
        

        print($"{PhotonNetwork.NickName} won");
        pv.RPC("EndGameForEveryone", RpcTarget.All);
    }

    /*private void OnTriggerEnter(Collider c)
    {
        PlayerController player = c.GetComponent<PlayerController>();

        if (player != null && !gameEnded)
        {
            EndGame(player);
        }
    }*/

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

    public void HitWaypoint()
    {
        waypoints[currentWaypoint].SetActive(false);
        currentWaypoint++;
        if(currentWaypoint == waypoints.Count && !gameEnded)
        {
            EndGame();
        }
        else
        {
            waypoints[currentWaypoint].SetActive(true);

        }

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
