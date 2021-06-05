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

    List<CardController> cards = new List<CardController>();
    [SerializeField] PlayerHand hand;
    [SerializeField] PlayerUi ui;
    [SerializeField] GameObject hiddingHand;
    PhotonPlayer photonPlayer;


    public static Player LocalPlayerInstance;
    public PhotonPlayer PhotonPlayer { get { return photonPlayer; } }
    public PlayerHand Hand { get { return hand; } set { hand = value; } }
    public PlayerUi Ui { get { return ui; } set { ui = value; } }
    public List<CardController> Cards { get { return cards; } }
    public int ViewID { get { return photonView.ViewID; } }


    void Awake()
    {
        // #Important used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this;
        }
        // #Critical we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }


    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        // e.g. store this gameobject as this player's charater in Player.TagObject
        info.Sender.TagObject = this.gameObject;
        photonPlayer = info.Sender;
        gameObject.name = photonPlayer.NickName;

        ui.SetTarget(this);

        GameManager.NumberOfInstantiatedPlayer++;
    }


    public void InitializeHand()
    {
        for (int i = 0; i < GameManager.NB_CARDS_TO_DISTRIBUTE; i++)
        {
            GameManager.Instance.InstantiateCard(EventCode.DRAW_CARD_FROM_DECK, EventCode.DRAW_TO_HAND, Vector3.zero, ViewID);
        }
    }


    public void AddCard(CardController cardController)
    {
        cardController.SetUpOwner(this);
        cards.Add(cardController);
        hand.UpdateCardsPosition(cards);
    }

    public void RemoveCard(CardController cardControllerToRemove)
    {
        cards.Remove(cardControllerToRemove);
        hand.UpdateCardsPosition(cards);
    }
    
    public void ReplaceCard(CardController cardToReplace, CardController cardToReplaceWith)
    {
        int indexOfCard = cards.IndexOf(cardToReplace);
        cards[indexOfCard] = cardToReplaceWith;
        cardToReplaceWith.SetUpOwner(this);
        hand.UpdateCardsPosition(cards);

        ShowPlayersHand();
    }

    public void HidePlayersHand()
    {
        if (photonView.IsMine)
        {
            foreach (Player player in GameManager.Instance.Players)
            {
                if(player != this)
                {
                    player.hiddingHand.SetActive(true);
                }
            }
        }
    }

    public void ShowPlayersHand()
    {
        if (photonView.IsMine)
        {
            foreach (Player player in GameManager.Instance.Players)
            {
                if (player != this)
                {
                    player.hiddingHand.SetActive(false);
                }
            }
        }
    }



}
