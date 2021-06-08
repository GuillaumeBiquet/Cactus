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
    [SerializeField] GameObject slider;


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

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            nickNameMenu.SetActive(false);
            createOrJoinRoomMenu.SetActive(true);
            PhotonNetwork.LeaveRoom();
        }
    }

    public void ConnectToMaster(string playerNickname)
    {
        slider.SetActive(true);
        nickNameMenu.SetActive(false);

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = GameSettings.GameVersion;
        PhotonNetwork.NickName = playerNickname;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        slider.SetActive(false);
        createOrJoinRoomMenu.SetActive(true);
        PhotonNetwork.JoinLobby();
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

}
