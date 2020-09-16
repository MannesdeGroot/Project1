using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour, Photon.Pun.IPunObservable
{
    public float roundTime;
    public List<string> playerIDs = new List<string>();
    public PhotonView pV;
    void Start()
    {
        pV = transform.GetComponent<PhotonView>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            UpdatePlayerIDs();
            AppointNewTagger();
        }
    }

    public void AppointNewTagger()
    {
        pV.RPC("ChooseTagger", RpcTarget.All, playerIDs[Random.Range(0, playerIDs.Count - 1)]);
    }

    [PunRPC]
    public void ChooseTagger(string idOfPlayer)
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < players.Length; i++)
        {
            if(players[i].pV.ViewID.ToString() == idOfPlayer)
            {
                players[i].PhotonTag(transform.position, 0);
            }
        }
    }

    public void UpdatePlayerIDs()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        playerIDs = new List<string>();
        for (int i = 0; i < players.Length; i++)
        {
            playerIDs.Add(players[i].pV.ViewID.ToString());
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerIDs);
        }
        else if (stream.IsReading)
        {
            playerIDs = (List<string>)stream.ReceiveNext();
        }
    }
}
