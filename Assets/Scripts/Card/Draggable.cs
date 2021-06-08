using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour
{
    Vector3 positionToReturnTo;
    Vector3 mousePos = Vector3.zero;

    PhotonView photonView;
    CardController cardController;

    bool isOnDiscardPile = false;
    bool canDrag = false;
    public Vector3 PositionToReturnTo { get { return positionToReturnTo; } set { positionToReturnTo = value; transform.position = positionToReturnTo; } }

    bool ShouldRevealCard { get { return  (GameManager.Instance.CardPlayed.Is7or8 && cardController.View.IsMine) ||  (GameManager.Instance.CardPlayed.Is9or10 && !cardController.View.IsMine); } }
    bool ShouldSelectCard { get { return !cardController.View.IsMine && GameManager.Instance.CardSelected == null; } }
    bool ShouldExchangeCard { get { return cardController.View.IsMine && GameManager.Instance.CardSelected != null; } }
    bool ShouldPlayCard { get { return GameManager.GameState == GameState.PlayCardEffectPhase && GameManager.Instance.IsMyTurn(); } }
    bool ShouldReplaceCard { get { return GameManager.GameState == GameState.ReplaceCardPhase && GameManager.Instance.IsMyTurn() && cardController.View.IsMine; } }


    private void Awake()
    {
        photonView = this.GetComponent<PhotonView>();
        cardController = this.GetComponent<CardController>();
    }

    void OnMouseDown()
    {
        canDrag = photonView.IsMine;
        if (ShouldPlayCard)
        {
            if (ShouldRevealCard)
            {
                HelperManager.Instance.HideHelper();
                StartCoroutine( cardController.RevealCardForSeconds(2) );
            }
            else if (GameManager.Instance.CardPlayed.IsJackOrQueen || GameManager.Instance.CardPlayed.IsBlackKing)
            {
                if (ShouldSelectCard)
                {
                    if (GameManager.Instance.CardPlayed.IsBlackKing)
                    {
                        cardController.RevealCard();
                    }
                    HelperManager.Instance.ShowHelper("Select one of your card to exchange with");
                    cardController.SelectCard();
                    GameManager.Instance.HideAllPlayerHandsExeptLocal();
                }
                else if (ShouldExchangeCard)
                {
                    HelperManager.Instance.HideHelper();
                    ExchangeCardEvent();
                    GameManager.Instance.CardSelected.UnselectCard();
                    PhotonNetwork.RaiseEvent(EventCode.END_PLAYER_TURN, null, RaiseEventOptions.Default, SendOptions.SendReliable);
                    GameManager.Instance.EndTurn();
                }
            }
            canDrag = false;
        }
        else if( ShouldReplaceCard )
        {
            HelperManager.Instance.HideHelper();
            ReplaceCardEvent();
            //QuickDiscardEvent();
            canDrag = false;
        }
    }

    void OnMouseDrag()
    {
        if (!canDrag)
            return;

        mousePos = Camera.main.ScreenToWorldPoint( Input.mousePosition );
        mousePos.z = 0;
        transform.position = mousePos;
    }

    void OnMouseUp()
    {
        if (!canDrag)
            return;

        if (isOnDiscardPile)
        {
            if (DiscardPileManager.Instance.IsEmpty || cardController.Card.Value != DiscardPileManager.Instance.LastCardValue)
            {
                transform.position = positionToReturnTo;
                GameManager.Instance.InstantiateCard(EventCode.DRAW_CARD_FROM_DECK, EventCode.DRAW_TO_HAND, Vector3.zero, cardController.Owner.ViewID);
            }
            else
            {
                QuickDiscardEvent();
            }

        }
        else
        {
            transform.position = positionToReturnTo;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DiscardPile"))
        {
            isOnDiscardPile = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DiscardPile"))
        {
            isOnDiscardPile = false;
        }
    }

    void QuickDiscardEvent()
    {
        object[] data = new object[] { photonView.ViewID };
        // send deck to everyone
        PhotonNetwork.RaiseEvent(EventCode.QUICK_DISCARD, data, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }


    void ReplaceCardEvent()
    {
        object[] data = new object[] { photonView.ViewID };
        // send deck to everyone exept me (master)
        PhotonNetwork.RaiseEvent(EventCode.REPLACE_CARD, data, RaiseEventOptions.Default, SendOptions.SendReliable);
        GameManager.Instance.CurrentPlayer.ReplaceCard(cardController, GameManager.Instance.CardDrawn);
        DiscardPileManager.Instance.Discard(cardController, EventCode.REPLACE_CARD);
    }


    void ExchangeCardEvent()
    {

        CardController card1 = cardController;
        CardController card2 = GameManager.Instance.CardSelected;
        Player owner1 = card1.Owner;
        Player owner2 = card2.Owner;

        object[] data = new object[] { card1.View.ViewID, card2.View.ViewID };
        // send deck to everyone exept me (master)
        PhotonNetwork.RaiseEvent(EventCode.EXCHANGE_CARD, data, RaiseEventOptions.Default, SendOptions.SendReliable);

        card1.View.TransferOwnership(owner2.PhotonPlayer);
        card2.View.TransferOwnership(owner1.PhotonPlayer);
        owner1.ReplaceCard(card1, card2);
        owner2.ReplaceCard(card2, card1);
    }
}
