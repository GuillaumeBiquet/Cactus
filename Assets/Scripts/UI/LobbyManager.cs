using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    private static LobbyManager instance;
    public static LobbyManager Instance { get { return instance; } }


    [SerializeField] GameSettings gameSettings;
    public static GameSettings GameSettings { get { return Instance.gameSettings; } }


    [SerializeField] GameObject nickNameMenu;
    [SerializeField] GameObject createOrJoinRoomMenu;
    [SerializeField] GameObject roomMenu;


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void ConnectToMaster(string playerNickname)
    {
        Debug.Log("Connecting...");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = GameSettings.GameVersion;
        PhotonNetwork.NickName = playerNickname;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to server.");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from server : " + cause.ToString());
    }


    public void ShowRoomMenu()
    {
        roomMenu.SetActive(true);
        createOrJoinRoomMenu.SetActive(false);
    }

    public void HideRoomMenu()
    {
        roomMenu.SetActive(false);
        createOrJoinRoomMenu.SetActive(true);
    }

    public void HideNicknameMenu()
    {
        nickNameMenu.SetActive(false);
        createOrJoinRoomMenu.SetActive(true);
    }
}
