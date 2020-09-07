using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSetupCotroller : MonoBehaviour
{
    void Start()
    {
        CreatePlayer();
    }

    void CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "MultiplayerPlayer"), Vector3.zero, Quaternion.identity);
    }
}
