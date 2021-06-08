using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class EventCode
{
    public const byte END_PLAYER_TURN = 1;
    public const byte DRAW_CARD_FROM_DECK = 2;
    public const byte PLAY_CARD = 3;
    public const byte SET_UP_DECK = 4;
    public const byte DRAW_CARD_FROM_DISCARD_PILE = 5;
    public const byte DISCARD = 6;
    public const byte QUICK_DISCARD = 7;
    public const byte REPLACE_CARD = 8;
    public const byte DRAW_TO_HAND = 9;
    public const byte DRAW_TO_DECK = 10;
    public const byte EXCHANGE_CARD = 11;
    public const byte CACTUS = 12;
    public const byte RELOAD = 13;
    public const byte DISCARD_CARD_DRAWN = 14;
}


public class EventHandler : MonoBehaviour
{

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

        if (eventCode == EventCode.END_PLAYER_TURN)
        {
            GameManager.Instance.EndTurn();
        }
        else if (eventCode == EventCode.SET_UP_DECK)
        {
            DeckManager.Instance.SetUpDeck((object[])eventData.CustomData);
        }
        else if (eventCode == EventCode.QUICK_DISCARD)
        {
            object[] data = (object[])eventData.CustomData;
            PhotonView view = PhotonView.Find((int)data[0]);
            CardController cardToDiscard = view.GetComponent<CardController>();
            DiscardPileManager.Instance.Discard(cardToDiscard, eventCode);
        }
        else if (eventCode == EventCode.REPLACE_CARD)
        {
            object[] data = (object[])eventData.CustomData;
            PhotonView view = PhotonView.Find((int)data[0]);
            CardController cardToReplace = view.GetComponent<CardController>();
            GameManager.Instance.CurrentPlayer.ReplaceCard(cardToReplace, GameManager.Instance.CardDrawn);
            DiscardPileManager.Instance.Discard(cardToReplace, eventCode);
        }
        else if (eventCode == EventCode.EXCHANGE_CARD)
        {
            object[] data = (object[])eventData.CustomData;
            CardController card1 = PhotonView.Find((int)data[0]).GetComponent<CardController>();
            CardController card2 = PhotonView.Find((int)data[1]).GetComponent<CardController>();
            Player owner1 = card1.Owner;
            Player owner2 = card2.Owner;
            owner1.ReplaceCard(card1, card2);
            owner2.ReplaceCard(card2, card1);
        }
        else if (eventCode == EventCode.CACTUS)
        {
            GameManager.Instance.Cactus();
        }
        else if( eventCode == EventCode.RELOAD)
        {
            PhotonNetwork.LoadLevel(1);
        }
        else if (eventCode == EventCode.DISCARD_CARD_DRAWN)
        {
            DiscardPileManager.Instance.Discard(GameManager.Instance.CardDrawn, eventCode);
        }

    }





}
