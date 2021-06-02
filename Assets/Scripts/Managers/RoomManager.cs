using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private static RoomManager instance;
    public static RoomManager Instance { get { return instance; } }

    public const byte END_PLAYER_TURN = 1;
    public const byte DRAW_CARD_FROM_DECK = 2;
    public const byte PLAY_CARD = 3;
    public const byte SET_UP_DECK = 4;

    List<Player> players = new List<Player>();

    int currentPlayerIndex = 0;

    public List<Player> Players { get { return players; } }
    public Player CurrentPlayer { get { return players[currentPlayerIndex]; } }

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

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }


    void OnEvent(EventData eventData)
    {
        byte eventCode = eventData.Code;
        if(eventCode == END_PLAYER_TURN)
        {
            EndTurn();
        }
        else if (eventCode == DRAW_CARD_FROM_DECK)
        {
            DeckManager.Instance.DrawTopCard();
        }
        else if (eventCode == SET_UP_DECK)
        {
            DeckManager.Instance.SetUpDeck( (object[]) eventData.CustomData );
        }
    }

    void EndTurn()
    {
        currentPlayerIndex++;
        if (currentPlayerIndex >= players.Count)
        {
            currentPlayerIndex = 0;
            //roomTurn++;
        }
    }


    public bool IsMyTurn()
    {
        return CurrentPlayer.PhotonPlayer == PhotonNetwork.LocalPlayer;
    }


    public void SetUpPlayerList()
    {
        foreach (PhotonPlayer photonPlayer in PhotonNetwork.PlayerList)
        {
            GameObject p = (GameObject) photonPlayer.TagObject;
            Debug.LogError("go name = " + p.name);
            players.Add(p.GetComponent<Player>());
        }
    }

}
