using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;
using System.Linq;

public class Player: MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{

    [SerializeField] List<Card> cards = new List<Card>();
    PlayerHand hand;
    PhotonPlayer photonPlayer;


    public static GameObject LocalPlayerInstance;
    public PhotonPlayer PhotonPlayer { get { return photonPlayer; } }
    public PlayerHand Hand { get { return hand; } set { hand = value; } }

    /*public delegate void OnCardsChangedDelegate();
    public OnCardsChangedDelegate onCardsChangedCallback;*/


    void Awake()
    {
        // #Important used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;
        }
        // #Critical we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        // e.g. store this gameobject as this player's charater in Player.TagObject
        info.Sender.TagObject = this.gameObject;
        photonPlayer = info.Sender;
        gameObject.name = photonPlayer.NickName;
        GameManager.NumberOfInstantiatedPlayer++;
    }

    public void DrawCard(Card card)
    {
        cards.Add(card);

        CardController cardController = Instantiate(GameManager.Instance.CardPrefab, hand.transform).GetComponent<CardController>();
        cardController.gameObject.name = card.Value + "_" + card.Type;

        if (!photonView.IsMine)
        {
            cardController.GetComponent<Draggable>().enabled = false;
        }

        cardController.SetUp(card, this);
        FinishTurn();
    }

    public void FinishTurn()
    {
        if(!photonView.IsMine || !RoomManager.Instance.IsMyTurn()) {
            return;
        }
        object[] data = null;
        PhotonNetwork.RaiseEvent(RoomManager.END_PLAYER_TURN, data, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }

    public void SetUpHand(PlayerHand _hand)
    {
        hand = _hand;
        _hand.Player = this;
        _hand.gameObject.SetActive(true);
    }

}
