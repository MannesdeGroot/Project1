using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Waypoint : MonoBehaviour
{
    public ParkourManager parkourManager;
    public Vector3 scale;
    public float scaleUpSpeed;
    public GameObject waypointSound;

    private void Update()
    {
        if(transform.localScale.y < scale.y)
        {
            transform.localScale += (new Vector3(1, 1, 1) * scaleUpSpeed * Time.deltaTime);

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (other.GetComponent<PlayerController>().pV.IsMine)
            {
                Instantiate(waypointSound, transform.position, Quaternion.identity);
                parkourManager.HitWaypoint();

            }
        }
    }
}
