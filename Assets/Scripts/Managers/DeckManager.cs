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

    List<Card> allCards = new List<Card>();
    List<Card> deck = new List<Card>();
    SpriteRenderer spriteRenderer;

    private static System.Random random = new System.Random();

    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }

        Card card;
        for (int i = 0; i < 52; i++)
        {
            card = new Card(i, 7, GetCardType(6), deckGFX.CardsSprite[6], deckGFX.BackOfCardSprite);
            //card = new Card(i, (i % 13) + 1, GetCardType(i), deckGFX.CardsSprite[i], deckGFX.BackOfCardSprite);
            allCards.Add(card);
        }
        deck = new List<Card>(allCards);

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = deckGFX.BackOfCardSprite;

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
        if (GameManager.GameState != GameState.DrawingPhase)
        {
            Debug.LogError("Not the drawing phase: " + GameManager.GameState.ToString());
            return;
        }

        if (!GameManager.Instance.IsMyTurn())
        {
            Debug.LogError("Not my turn");
            return;
        }

        if (deck.Count == 0)
        {
            Debug.LogError("No cards left");
            return;
        }

        GameManager.Instance.InstantiateCard(EventCode.DRAW_CARD_FROM_DECK, EventCode.DRAW_TO_DECK, transform.position);
    }



    public Card DrawTopCard()
    {
        Card cardToDraw = deck.Last();
        deck.Remove(cardToDraw);
        return cardToDraw;
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
