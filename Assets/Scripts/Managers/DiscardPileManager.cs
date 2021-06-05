using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using Photon.Pun;

public class DiscardPileManager : MonoBehaviour
{
    private static DiscardPileManager instance;
    public static DiscardPileManager Instance { get { return instance; } }

    List<Card> discardedCards = new List<Card>();
    SpriteRenderer spriteRenderer;
    Sprite defaultSprite;
    public bool IsEmpty { get { return discardedCards.Count == 0; } }
    public int LastCardValue { get { return discardedCards.Last().Value; } }

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
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
        UpdatePileImage();
    }

    public void Discard(CardController cardController, byte eventCode)
    {
        Card discardedCard = cardController.Card;

        if(eventCode == EventCode.QUICK_DISCARD)
        {
            discardedCards.Add(discardedCard);
            cardController.DestroySelf();
        }
        else
        {
            cardController.Effect();
            discardedCards.Add(discardedCard);
            cardController.DestroySelf();
        }

        UpdatePileImage();
    }

    private void UpdatePileImage()
    {
        if (discardedCards.Count > 0)
        {
            spriteRenderer.sprite = discardedCards.Last().Front;
        }
        else
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    public Card DrawTopCard()
    {
        Card cardToDraw = discardedCards.Last();
        discardedCards.Remove(cardToDraw);
        UpdatePileImage();
        return cardToDraw;
    }



    private void OnMouseDown()
    {
        if (GameManager.GameState != GameState.DrawingPhase)
        {
            Debug.LogError("Not the drawing phase");
            return;
        }

        if (!GameManager.Instance.IsMyTurn())
        {
            Debug.LogError("Not my turn");
            return;
        }

        if (discardedCards.Count == 0)
        {
            Debug.LogError("Discard pile is empty");
            return;
        }

        GameManager.Instance.InstantiateCard(EventCode.DRAW_CARD_FROM_DISCARD_PILE, EventCode.DRAW_TO_DECK, transform.position);

    }

}
