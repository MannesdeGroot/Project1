using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Goal : MonoBehaviour, Photon.Pun.IPunObservable
{
    public SoccerManager soccerManager;
    PhotonView pv;
    public int teamgoal;
    GameObject ball;
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
                pv.RPC("DestroyBall", RpcTarget.All);
                soccerManager.AddPoint(teamgoal);
            }
        }
    }

    [PunRPC]
    void DestroyBall()
    {
        Destroy(GameObject.FindGameObjectWithTag("Football").gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(isTagger);
            
        }
        else if (stream.IsReading)
        {
            //isTagger = (bool)stream.ReceiveNext();
            
        }
    }
}
