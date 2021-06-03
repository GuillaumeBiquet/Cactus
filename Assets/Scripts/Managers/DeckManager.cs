using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class DeckManager : MonoBehaviour
{



    private static DeckManager instance;
    public static DeckManager Instance { get { return instance; } }

    [SerializeField] DeckOfCard deckGFX;
    [SerializeField] GameObject cardPrefab;

    private List<Card> allCards = new List<Card>();
    private List<Card> deck = new List<Card>();

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
            card = new Card(i, cardValue % 14, cardType, deckGFX.CardsSprite[i], deckGFX.BackOfCardSprite);
            allCards.Add(card);
        }
        deck = new List<Card>(allCards);

        GetComponent<SpriteRenderer>().sprite = deckGFX.BackOfCardSprite;

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


    public void ShuffleDeck()
    {
        int n = deck.Count;
        int nbrOfSuffle = 3;
        while (nbrOfSuffle > 0)
        {
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
            nbrOfSuffle--;
        }


        object[] data = new object[] { ListOfCardsToArrayOfIndex(deck) };
        // send deck to everyone exept me (master)
        PhotonNetwork.RaiseEvent(EventCode.SET_UP_DECK, data, RaiseEventOptions.Default, SendOptions.SendReliable);

    }

    public void SetUpDeck(object[] data)
    {
        deck = ArrayOfIndexToListOfCards( (int[]) data[0] );
    }


    private void OnMouseDown()
    {
        InstantiateCard();
    }

    public void InstantiateCard()
    {
        if (deck.Count == 0)
        {
            Debug.LogError("No cards left");
            return;
        }

        if (!GameManager.Instance.IsMyTurn())
        {
            Debug.LogError("Not my turn");
            return;
        }

        // send event to everyone but me
        //PhotonNetwork.RaiseEvent(RoomManager.DRAW_CARD_FROM_DECK, null, RaiseEventOptions.Default, SendOptions.SendReliable);
        //DrawTopCard();

        object[] data = new object[] { EventCode.DRAW_CARD_FROM_DECK };
        PhotonNetwork.Instantiate(cardPrefab.name, Vector3.zero, Quaternion.identity, 0, data);
    }

    public Card DrawTopCard()
    {
        Card cardToDraw = deck.Last();
        deck.Remove(cardToDraw);
        return cardToDraw;
    }


    public Card GetCardByIndex(int index)
    {
        return allCards[index];
    }


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
