using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSetupCotroller : MonoBehaviour
{
    public Transform[] spawns;
    void Start()
    {
        CreatePlayer();
    }

    void CreatePlayer()
    {
        if(spawns.Length > 0)
        {
            int randomSpawnNum = Random.Range(0, spawns.Length - 1);
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "MultiplayerPlayer"), spawns[randomSpawnNum].position, spawns[randomSpawnNum].rotation);
        }
        else
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "MultiplayerPlayer"), Vector3.zero, Quaternion.identity);

        }
    }
}
