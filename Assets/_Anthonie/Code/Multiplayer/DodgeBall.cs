﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DodgeBall : MonoBehaviour, Photon.Pun.IPunObservable
{
    public int lastTeamThrown;
    public bool killable = true;
    public float startForceY;
    public float startForceZ;
    public PowerUp powerUp;
    public PhotonView pv;
    public SphereCollider[] colliders;
    public float timeToActivate;
    Rigidbody rb;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        colliders = GetComponents<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.up * startForceY);
        rb.AddForce(transform.forward * startForceZ);
    }

    private void Update()
    {
        timeToActivate -= Time.deltaTime;
        if(timeToActivate < 0)
        {
            timeToActivate = 9999999;
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = true;

            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PlayerController playerHit = collision.gameObject.GetComponent<PlayerController>();
            if(playerHit.team == lastTeamThrown || lastTeamThrown == 0)
            {

            }
            else if(killable)
            {
                GameObject.FindObjectOfType<TrefballManager>().CheckIfTeamWon(playerHit.team);
                playerHit.Eliminate();
                killable = false;
            }
        }
        else
        {
            killable = false;
            powerUp.enabled = true;
        }

        if(collision.transform.tag == "Respawn")
        {
            rb.velocity.Equals(Vector3.zero);
            transform.position = GameObject.FindObjectOfType<TrefballManager>().ballSpawn.position;
            transform.rotation = new Quaternion(0, Random.Range(0f, 1f), 0, 1);
            rb.AddForce(transform.up * startForceY);
            rb.AddForce(transform.forward * startForceZ);
        }
    }

    public void SetTeam(int team)
    {
        pv = GetComponent<PhotonView>();
        pv.RPC("PUNSetTeam", RpcTarget.All, team);
    }

    [PunRPC]
    void PUNSetTeam(int team)
    {
        lastTeamThrown = team;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(lastTeamThrown);

        }
        else if (stream.IsReading)
        {
            lastTeamThrown = (int)stream.ReceiveNext();

        }
    }
}
