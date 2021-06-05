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

    private void Awake()
    {
        photonView = this.GetComponent<PhotonView>();
        cardController = this.GetComponent<CardController>();
    }

    void OnMouseDown()
    {
        if (!enabled)
            return;


        if (GameManager.GameState == GameState.PlayCardEffectPhase && GameManager.Instance.IsMyTurn())
        {
            if (GameManager.Instance.CardPlayed.Is7or8)
            {
                StartCoroutine( cardController.RevealCardForSeconds(2) );
            }
        }
        else if( GameManager.GameState == GameState.ReplaceCardPhase && GameManager.Instance.IsMyTurn())
        {
            ReplaceCardEvent();
            QuickDiscardEvent();
        }
        positionToReturnTo = transform.position;
    }

    void OnMouseDrag()
    {
        if (!enabled)
            return;

        mousePos = Camera.main.ScreenToWorldPoint( Input.mousePosition );
        mousePos.z = 0;
        transform.position = mousePos;
    }

    void OnMouseUp()
    {
        if (!enabled)
            return;

        if (isOnDiscardPile)
        {
            if (DiscardPileManager.Instance.IsEmpty || cardController.Card.Value != DiscardPileManager.Instance.LastCardValue)
            {
                //TODO : punish player and put card back
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
        // send deck to everyone exept me (master)
        PhotonNetwork.RaiseEvent(EventCode.QUICK_DISCARD, data, RaiseEventOptions.Default, SendOptions.SendReliable);
        DiscardPileManager.Instance.Discard(this.GetComponent<CardController>(), EventCode.QUICK_DISCARD);
    }


    void ReplaceCardEvent()
    {
        object[] data = new object[] { photonView.ViewID };
        // send deck to everyone exept me (master)
        PhotonNetwork.RaiseEvent(EventCode.REPLACE_CARD, data, RaiseEventOptions.Default, SendOptions.SendReliable);
        GameManager.Instance.CurrentPlayer.ReplaceCard(cardController, GameManager.Instance.CardDrawn);
        DiscardPileManager.Instance.Discard(cardController, EventCode.REPLACE_CARD);
    }
}
