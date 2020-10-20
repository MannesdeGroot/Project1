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
    PlayerController isMine;
    public float startTime;
    public GameObject walls;
    bool started = false;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        
    }

    private void Update()
    {
        if (!started)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                startTime -= Time.deltaTime;
            }

        
            if (isMine != null)
            {
                isMine.pregameTimer.text = startTime.ToString("#");
            }
            else
            {
                PlayerController[] players = GameObject.FindObjectsOfType<PlayerController>();
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].pV.IsMine)
                    {
                        isMine = players[i];
                    }
                }
            }

            if(startTime < 0)
            {
                StartGame();
            }
        }

    }

    void StartGame()
    {
        started = true;
        isMine.pregameTimer.gameObject.SetActive(false);
        for (int i = 1; i < waypoints.Count; i++)
        {
            waypoints[i].SetActive(false);
            waypoints[i].transform.localScale = Vector3.zero;
        }
        Destroy(walls);
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
            stream.SendNext(startTime);
        }
        else if (stream.IsReading)
        {
            startTime = (float)stream.ReceiveNext();
        }
    }

}
