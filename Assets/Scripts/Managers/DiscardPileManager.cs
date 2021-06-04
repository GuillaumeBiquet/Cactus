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

    public void Discard(CardController cardController)
    {
        Card discardedCard = cardController.Card;
        discardedCards.Add(discardedCard);
        cardController.DestroySelf();

        GameManager.GameState = GameState.PlayCardEffectPhase;
        cardController.Effect();

        // if normal discard
        // play card
        // end turn

        // else if quick discard 
        //Check if same value as previous discarded card and play effect
        //if not punish player and put card back

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

        GameManager.Instance.InstantiateCard(EventCode.DRAW_CARD_FROM_DISCARD_PILE, transform.position);

    }

}
