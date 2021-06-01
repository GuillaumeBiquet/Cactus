using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomDisplay : MonoBehaviourPunCallbacks
{
    RoomInfo roomInfo;

    public RoomInfo RoomInfo { get { return roomInfo; } }

    public void SetRoomInfo(RoomInfo _roomInfo)
    {
        roomInfo = _roomInfo;
        GetComponent<TMP_Text>().text = roomInfo.Name + ", " + roomInfo.MaxPlayers;
    }

    public void JoinRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        PhotonNetwork.JoinRoom(roomInfo.Name);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("joined room: " + roomInfo.Name);
        LobbyManager.Instance.ShowRoomMenu();
    }

}
