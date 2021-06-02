using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class DeckManager : MonoBehaviourPunCallbacks //, IPunObservable
{

    private static DeckManager instance;
    public static DeckManager Instance { get { return instance; } }

    [SerializeField] Sprite[] cardsSprite;
    [SerializeField] Sprite backOfCardSprite;
    [SerializeField] TMP_Text deckText;

    private List<Card> allCards = new List<Card>();
    private List<Card> deckOfCard = new List<Card>();

    private static System.Random random = new System.Random();


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

        CardType cardType = CardType.HEART;
        int cardValue = 0;
        Card card;
        for (int i = 0; i < 52; i++)
        {
            cardType = GetCardType(i);
            cardValue = i + 1;
            card = new Card(i, cardValue % 14, cardType, cardsSprite[i], backOfCardSprite);
            allCards.Add(card);
        }
        deckOfCard = new List<Card>(allCards);

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

    public void DrawCardEvent()
    {
        if (deckOfCard.Count == 0 && !photonView.IsMine ) {
            return;
        }

        if (!RoomManager.Instance.IsMyTurn())
        {
            Debug.LogError("Not my turn");
            return;
        }

        // send event to everyone but me
        PhotonNetwork.RaiseEvent(RoomManager.DRAW_CARD_FROM_DECK, null, RaiseEventOptions.Default, SendOptions.SendReliable);

        DrawTopCard();
    }

    public void ShuffleDeck()
    {
        int n = deckOfCard.Count;
        int nbrOfSuffle = 3;
        while (nbrOfSuffle > 0)
        {
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card value = deckOfCard[k];
                deckOfCard[k] = deckOfCard[n];
                deckOfCard[n] = value;
            }
            nbrOfSuffle--;
        }


        object[] data = new object[] { ListOfCardsToArrayOfIndex(deckOfCard) };
        // send deck to everyone exept me (master)
        PhotonNetwork.RaiseEvent(RoomManager.SET_UP_DECK, data, RaiseEventOptions.Default, SendOptions.SendReliable);

    }

    public void SetUpDeck(object[] data)
    {
        deckOfCard = ArrayOfIndexToListOfCards( (int[]) data[0] );
    }


    public void DrawTopCard()
    {
        Card cardToDraw = deckOfCard.Last();
        RoomManager.Instance.CurrentPlayer.DrawCard(cardToDraw);
        deckOfCard.Remove(cardToDraw);
    }

    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(ListOfCardsToArrayOfIndex(deckOfCard));
        }
        else if (stream.IsReading)
        {
            deckOfCard = ArrayOfIndexToListOfCards((int[])stream.ReceiveNext());
        }
    }*/

    int[] ListOfCardsToArrayOfIndex(List<Card> cards)
    {
        int[] indexes = new int[cards.Count];
        for(int i=0; i < cards.Count; i++)
        {
            indexes[i] = cards[i].Index;
        }
        return indexes;
    }

    List<Card> ArrayOfIndexToListOfCards(int[] indexes)
    {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < indexes.Length; i++)
        {
            cards.Add( allCards[indexes[i]] );
        }
        return cards;
    }
}
