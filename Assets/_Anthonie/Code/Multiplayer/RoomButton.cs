using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    public Text nameText;
    public Text amountText;
    public string roomName;
    public int roomSize;
    public int peopleInRoom;

    public void SetRoom()
    {
        nameText.text = roomName;
        amountText.text = peopleInRoom.ToString() + "/" + roomSize.ToString();
    }

    public void JoinRoomOnClick()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
