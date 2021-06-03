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
    public List<Card> Cards { get { return cards; } }

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

    public void AddCard(Card card)
    {
        cards.Add(card);
        hand.UpdateCardsPosition(cards);
        GameManager.Instance.EndTurn();
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
        hand.UpdateCardsPosition(cards);
    }

    public void SetUpHand(PlayerHand _hand)
    {
        hand = _hand;
        _hand.Player = this;
        _hand.gameObject.SetActive(true);
    }

}
