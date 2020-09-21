using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinGame : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public bool joinGame = false;
    public bool leaveGame = false;
    public string roomName;
    public int roomSize;
    public GameObject roomListingPrefab;
    public Transform roomsPannel;
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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        RemoveRooms();
        foreach(RoomInfo room in roomList)
        {
            ListRoom(room);
        }
    }

    void RemoveRooms()
    {
        for (int i = 0; i < roomsPannel.childCount; i++)
        {
            Destroy(roomsPannel.GetChild(i).gameObject);
        }
    }

    void ListRoom(RoomInfo room)
    {
        if(room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsPannel);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.roomName = room.Name;
            tempButton.roomSize = room.MaxPlayers;
            tempButton.peopleInRoom = room.PlayerCount;
            tempButton.SetRoom();
        }
    }
    public void Join()
    {
        PhotonNetwork.JoinRandomRoom();

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    public void CreateRoom()
    {
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        PhotonNetwork.CreateRoom(roomName, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //CreateRoom();
    }

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnRoomNameChanged(string nameIn)
    {
        roomName = nameIn;
    }

    public void OnRoomSizeChanged(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);
    }

    public void JoinLobbyOnClick()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }
}
