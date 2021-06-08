using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class RoomMenu : MonoBehaviourPunCallbacks
{

    [SerializeField] GameObject playerDisplayPrefab;
    [SerializeField] Transform playersView;
    [SerializeField] TMP_Text roomId;
    [SerializeField] TMP_Text AtLeast2Players;
    [SerializeField] GameObject StartGameButton;

    List<PlayerDisplay> playerDisplayList = new List<PlayerDisplay>();


    public override void OnEnable()
    {
        base.OnEnable();
        GetCurrentRoomPlayers();
        roomId.text = "# " + PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.IsMasterClient)
        {
            StartGameButton.SetActive(true);
        }
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
            if (PhotonNetwork.PlayerList.Any(p => p.NickName == PhotonNetwork.LocalPlayer.NickName && p != PhotonNetwork.LocalPlayer))
            {
                Debug.LogError("A player with the same nickname is already in this room.");
                Debug.LogError("Restart the game and choose a different nickname to get in this room.");
                LeaveRoom();
            }
            else
            {
                foreach (PhotonPlayer player in PhotonNetwork.PlayerList)
                {
                    InstantiatePlayerDisplay(player);
                }
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
            PlayerDisplay playerDisplay = Instantiate(playerDisplayPrefab, playersView).GetComponent<PlayerDisplay>();
            playerDisplay.SetPlayerInfo(photonPlayer);
            playerDisplayList.Add(playerDisplay);
        }
    }

    public override void OnPlayerEnteredRoom(PhotonPlayer newPhotonPlayer)
    {
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
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(true);
        LobbyManager.Instance.HideRoomMenu();
    }

    public void StartGame()
    {
        if(PhotonNetwork.PlayerList.Length < 2)
        {
            StartCoroutine(ShowTwoPLayerMinMessage());
            return;
        }
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(1);
    }

    IEnumerator ShowTwoPLayerMinMessage()
    {
        AtLeast2Players.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        AtLeast2Players.gameObject.SetActive(false);
    }

}
