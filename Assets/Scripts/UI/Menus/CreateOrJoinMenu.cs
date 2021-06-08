using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class CreateOrJoinMenu : MonoBehaviourPunCallbacks
{

    [SerializeField] TMP_InputField roomIdInput;
    [SerializeField] GameObject joinRoomFailText;

    System.Random random = new System.Random();


    private void Start()
    {
        roomIdInput.onValueChanged.AddListener(OnValueChange);
    }


    #region create room
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.MaxPlayers = LobbyManager.GameSettings.MaxPlayers;
        PhotonNetwork.CreateRoom(GenerateRoomId(), roomOptions, TypedLobby.Default);
    }


    public override void OnCreatedRoom()
    {
        LobbyManager.Instance.ShowRoomMenu();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("room creation failed: " + message);
        CreateRoom();
    }


    #endregion

    
    #region join room

    public void JoinRoom()
    {
        if(roomIdInput.text != "")
        {
            PhotonNetwork.JoinRoom(roomIdInput.text);
        }
    }

    public override void OnJoinedRoom()
    {
        LobbyManager.Instance.ShowRoomMenu();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        joinRoomFailText.SetActive(true);
    }

    void OnValueChange(string text)
    {
        joinRoomFailText.SetActive(false);
    }

    #endregion








    public string GenerateRoomId()
    {
        return random.Next(0, 9999).ToString("D4");
    }

}
