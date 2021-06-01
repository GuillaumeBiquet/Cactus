using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMenu : MonoBehaviourPunCallbacks
{

    [SerializeField] GameObject playerDisplayPrefab;
    [SerializeField] Transform content;

    List<PlayerDisplay> playerDisplayList = new List<PlayerDisplay>();


    public override void OnEnable()
    {
        base.OnEnable();
        GetCurrentRoomPlayers();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        for (int i=0; i < playerDisplayList.Count; i++)
        {
            Destroy(playerDisplayList[i].gameObject);
        }
        playerDisplayList.Clear();
    }

    void GetCurrentRoomPlayers() 
    {
        if(PhotonNetwork.CurrentRoom != null && PhotonNetwork.IsConnected)
        {
            foreach(KeyValuePair<int, PhotonPlayer> playerInfo in PhotonNetwork.CurrentRoom.Players)
            {
                InstantiatePlayerDisplay(playerInfo.Value);
            }
        }
    }

    void InstantiatePlayerDisplay(PhotonPlayer photonPlayer) 
    {
        int index = playerDisplayList.FindIndex(x => x.PhotonPlayer == photonPlayer);
        if(index != -1)
        {
            playerDisplayList[index].SetPlayerInfo(photonPlayer);
        }
        else
        {
            PlayerDisplay playerDisplay = Instantiate(playerDisplayPrefab, content).GetComponent<PlayerDisplay>();
            playerDisplay.SetPlayerInfo(photonPlayer);
            playerDisplayList.Add(playerDisplay);
        }

    }

    public override void OnPlayerEnteredRoom(PhotonPlayer newPhotonPlayer)
    {
        Debug.Log("entered");
        InstantiatePlayerDisplay(newPhotonPlayer);
    }

    public override void OnPlayerLeftRoom(PhotonPlayer otherPhotonPlayer)
    {
        int index = playerDisplayList.FindIndex(x => x.PhotonPlayer == otherPhotonPlayer);
        Destroy(playerDisplayList[index].gameObject);
        playerDisplayList.RemoveAt(index);
    }

    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        LeaveRoom();            
        //Todo
        Debug.Log("master left the room");

    }


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(true);
        LobbyManager.Instance.HideRoomMenu();
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(1);
        }
        else
        {
            //Todo
            Debug.Log("not the master client");
        }
    }

}
