using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CardController : MonoBehaviour, IPunInstantiateMagicCallback
{


    Card card;
    Player owner;
    bool isFlipped = false;

    SpriteRenderer spriteRenderer;
    PhotonView photonView;
    Draggable draggableComponent;

    public Card Card { get { return card; } }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        photonView = this.GetComponent<PhotonView>();
        draggableComponent = this.GetComponent<Draggable>();

        byte eventCode = (byte) info.photonView.InstantiationData[0];
        if(eventCode == EventCode.DRAW_CARD_FROM_DECK)
        {
            card = DeckManager.Instance.DrawTopCard();
        } 
        else if (eventCode == EventCode.DRAW_CARD_FROM_DISCARD_PILE)
        {
            card = DiscardPileManager.Instance.DrawTopCard();
        }

        spriteRenderer.sprite = card.Front;
        draggableComponent.enabled = false;
        this.gameObject.name = "CARD_" + card.Type + "_" + card.Value;


        if (GameManager.GameState == GameState.DristibutingPhase)
        {
            GameManager.Instance.CurrentPlayer.AddCard(this);
        }
        else
        {
            GameManager.Instance.SetCardDrawn(this);
        }

    }


    void Update()
    {

        if ((this.transform.eulerAngles.y > 90f || this.transform.eulerAngles.y < -90f) && !isFlipped)
        {
            spriteRenderer.sprite = card.Back;
            isFlipped = true;
        }
        else if ((this.transform.eulerAngles.y > -90f && this.transform.eulerAngles.y < 90f) && isFlipped)
        {
            spriteRenderer.sprite = card.Front;
            isFlipped = false;
        }

    }


    public void SetUpOwner(Player player)
    {
        draggableComponent.enabled = photonView.IsMine;
        transform.SetParent(player.Hand.transform, false);
        owner = player;
    }

    public void DestroySelf()
    {
        owner.RemoveCard(this);
        Destroy(this.gameObject);
    }


    public void Effect()
    {
        if (card.Value == 7 || card.Value == 8)
        {
            Debug.LogError("Play 7, 8");
        }
        else if (card.Value == 9 || card.Value == 10)
        {
            Debug.LogError("Play 9, 10");
        }
        else if (card.Value == 11 || card.Value == 12)
        {
            Debug.LogError("Play Valet, Dame");
        }
        else if (card.Value == 13 && (card.Type == CardType.SPADE || card.Type == CardType.CLUB))
        {
            Debug.LogError("Play black king");
        }
        else
        {
            Debug.LogError("No effect");
        }
        GameManager.Instance.EndTurn();
    }
}
