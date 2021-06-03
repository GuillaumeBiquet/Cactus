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

    bool isOnDiscardPile = false;

    private void Awake()
    {
        photonView = this.GetComponent<PhotonView>();
    }

    void OnMouseDown()
    {
        if (!enabled)
            return;

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
            object[] data = new object[] { photonView.ViewID };
            // send deck to everyone exept me (master)
            PhotonNetwork.RaiseEvent(EventCode.DISCARD_CARD, data, RaiseEventOptions.Default, SendOptions.SendReliable);

            DiscardPileManager.Instance.Discard(this.GetComponent<CardController>());
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

}
