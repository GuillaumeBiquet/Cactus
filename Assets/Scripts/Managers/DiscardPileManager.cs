using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class DiscardPileManager : MonoBehaviour
{
    private static DiscardPileManager instance;
    public static DiscardPileManager Instance { get { return instance; } }

    List<Card> discardedCards = new List<Card>();
    bool hasQuickDiscard = false;
    SpriteRenderer spriteRenderer;
    Sprite defaultSprite;
    public bool IsEmpty { get { return discardedCards.Count == 0; } }
    public bool HasQuickDiscard { get { return hasQuickDiscard; } }
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
            if (!hasQuickDiscard)
            {
                hasQuickDiscard = true;
                discardedCards.Add(discardedCard);
                cardController.DestroySelf();
            }
            else
            {
                cardController.ReturnToPos();
            }
        }
        else
        {
            hasQuickDiscard = false;
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

        if (!GameManager.Instance.IsMyTurn())
        {
            Debug.LogError("Not my turn");
            return;
        }

        if(GameManager.GameState == GameState.DrawingPhase)
        {
            if (discardedCards.Count == 0)
            {
                Debug.LogError("Discard pile is empty");
                return;
            }
        }
        else if (GameManager.GameState == GameState.ReplaceCardPhase)
        {
            HelperManager.Instance.HideHelper();
            DiscardCardDrawnEvent();
        }


    }

    void DiscardCardDrawnEvent()
    {
        // send deck to everyone exept me (master)
        PhotonNetwork.RaiseEvent(EventCode.DISCARD_CARD_DRAWN, null, RaiseEventOptions.Default, SendOptions.SendReliable);
        Discard(GameManager.Instance.CardDrawn, EventCode.DISCARD_CARD_DRAWN);
    }


}
