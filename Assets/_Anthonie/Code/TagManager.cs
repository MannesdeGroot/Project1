using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{
    public float roundTime;
    public List<string> playerIDs = new List<string>();
    public PhotonView pV;
    public string[] playerIDsArray;
    void Start()
    {
        pV = transform.GetComponent<PhotonView>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChooseTagger();
        }
    }

    

    
    public void ChooseTagger()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        players[Random.Range(0, players.Length - 1)].PhotonTag(transform.position, 0);


    }

    
}
