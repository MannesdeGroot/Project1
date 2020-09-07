using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinGame : MonoBehaviourPunCallbacks
{
    public bool joinGame = false;
    public bool leaveGame = false;
    void Start()
    {
        
    }

    void Update()
    {
        if (joinGame)
        {
            joinGame = false;
            Join();
        }
        if (leaveGame)
        {
            leaveGame = false;
            Leave();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

    }

    public void Join()
    {
        PhotonNetwork.JoinRandomRoom();

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    void CreateRoom()
    {
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
        PhotonNetwork.CreateRoom("RoomName", roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }
}
