using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Goal : MonoBehaviour
{
    public SoccerManager soccerManager;
    PhotonView pv;
    public int teamgoal;
    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Football")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                soccerManager.AddPoint(teamgoal);
                PhotonNetwork.Destroy(other.transform.gameObject);
            }
        }
    }
}
