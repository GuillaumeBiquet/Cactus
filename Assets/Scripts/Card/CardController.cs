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
    public Player Owner { get { return owner; } }


    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        photonView = this.GetComponent<PhotonView>();
        draggableComponent = this.GetComponent<Draggable>();

        object[] instantiationData = info.photonView.InstantiationData;
        byte eventCode = (byte)instantiationData[0];

        owner = GameManager.Instance.CurrentPlayer;

        if(eventCode == EventCode.DRAW_CARD_FROM_DECK)
        {
            card = DeckManager.Instance.DrawTopCard();
        } 
        else if (eventCode == EventCode.DRAW_CARD_FROM_DISCARD_PILE)
        {
            card = DiscardPileManager.Instance.DrawTopCard();
        }

        card.CardController = this;
        GameManager.Instance.CurrentPlayer.AddCard(card);
        spriteRenderer.sprite = card.Front;
        this.transform.SetParent(owner.Hand.transform, false);
        this.gameObject.name = "CARD_" + card.Type + "_" + card.Value;

        draggableComponent.enabled = photonView.IsMine;

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

    public void DestroySelf()
    {
        owner.RemoveCard(this.card);
        Destroy(this.gameObject);
    }

}
