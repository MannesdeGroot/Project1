using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinGame : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public int minPlayers;
    public int maxPlayers;
    public string roomName;
    public int roomSize;
    public GameObject roomListingPrefab;
    public Transform roomsPannel;
    public InputField roomSizeInput;
    

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
    public void JoinRandom()
    {
        PhotonNetwork.JoinRandomRoom();

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    public void CreateRoom()
    {
        if(roomSize < minPlayers)
        {
            roomSize = minPlayers;
        }
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        if(roomName == "")
        {
            PhotonNetwork.CreateRoom("Room " + Random.Range(0, 1000).ToString(), roomOps);
        }
        else
        {
            PhotonNetwork.CreateRoom(roomName, roomOps);

        }


    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        PhotonNetwork.CreateRoom(roomName + Random.Range(0,1000).ToString(), roomOps);
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
        if(roomSize < minPlayers)
        {
            roomSize = minPlayers;
            roomSizeInput.text = minPlayers.ToString();
        }
        else if(roomSize > maxPlayers)
        {
            roomSize = maxPlayers;
            roomSizeInput.text = maxPlayers.ToString();
        }
    }

    public void JoinLobbyOnClick()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }
}
