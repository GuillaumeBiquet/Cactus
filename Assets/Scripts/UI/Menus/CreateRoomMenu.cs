using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{

    [SerializeField] TMP_InputField roomIdInput;

    public void CreateRoom()
    {
        if (!PhotonNetwork.IsConnected || roomIdInput.text == "")
        {
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.MaxPlayers = LobbyManager.GameSettings.MaxPlayers;
        PhotonNetwork.CreateRoom(roomIdInput.text, roomOptions, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("room "  + roomIdInput.text + " was created");
        LobbyManager.Instance.ShowRoomMenu();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("room creation failed: " + message);
    }

}
