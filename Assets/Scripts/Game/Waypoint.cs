using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Waypoint : MonoBehaviour
{
    public ParkourManager parkourManager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (other.GetComponent<PlayerController>().pV.IsMine)
            {
                parkourManager.HitWaypoint();

            }
        }
    }
}
