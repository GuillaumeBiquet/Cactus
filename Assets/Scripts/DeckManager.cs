using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class DeckManager : MonoBehaviourPunCallbacks
{

    private static DeckManager instance;
    public static DeckManager Instance { get { return instance; } }


    [SerializeField] Sprite[] cardsSprite;
    [SerializeField] Sprite backOfCardSprite;
    [SerializeField] GameObject cardPrefab;

    private List<Card> cards = new List<Card>();


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

    // Start is called before the first frame update
    void Start()
    {
        CardType cardType = CardType.HEART;
        int cardValue = 0;
        Card card;
        Vector3 offset = Vector3.zero;
   
        for (int i = 0; i < 52; i++)
        {
            offset.z += 0.1f;
            cardType = GetCardType(i);
            cardValue = i + 1;
            card = new Card(cardValue % 13, cardType, cardsSprite[i], backOfCardSprite);
            cards.Add(card);
        }
    }


    CardType GetCardType(int index) 
    {
        if(index < 13)
        {
            return CardType.HEART;
        }
        else if (index < 13 * 2)
        {
            return CardType.CLUB;
        }
        else if (index < 13 * 3)
        {
            return CardType.DIAMOND;
        }
        else
        {
            return CardType.SPADE;
        }
    }

    public void DrawCard()
    {
        if (cards.Count == 0 && !photonView.IsMine && !RoomManager.Instance.IsMyTurn()) {
            return;
        }
        object[] data = null;
        PhotonNetwork.RaiseEvent(RoomManager.DRAW_CARD_FROM_DECK, data, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }

    public void DrawTopCard()
    {
        Card cardToDraw = cards.Last();
        RoomManager.Instance.CurrentPlayer.DrawCard(cardToDraw);
        cards.Remove(cardToDraw);
    }
}
