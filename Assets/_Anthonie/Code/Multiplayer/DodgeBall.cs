using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DodgeBall : MonoBehaviour, Photon.Pun.IPunObservable
{
    public int lastTeamThrown;
    public bool killable = true;


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
        }
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
