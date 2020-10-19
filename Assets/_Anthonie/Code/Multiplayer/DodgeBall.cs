using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DodgeBall : MonoBehaviour, Photon.Pun.IPunObservable
{
    public int lastTeamThrown;
    public bool killable = true;
    public Vector3 startForce;
    public PowerUp powerUp;
    public PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        GetComponent<Rigidbody>().AddForce(startForce);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PlayerController playerHit = collision.gameObject.GetComponent<PlayerController>();
            if(playerHit.team == lastTeamThrown)
            {

            }
            else if(killable)
            {
                playerHit.Eliminate();
                killable = false;
            }
        }
        else
        {
            killable = false;
            powerUp.enabled = true;
        }
    }

    public void PhotonSetTeam(int team)
    {
        pv.RPC("SetTeam", RpcTarget.All);
    }

    [PunRPC]
    void SetTeam(int team)
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
