using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

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

        byte eventCode = (byte)info.photonView.InstantiationData[0];
        byte whereToCode = (byte)info.photonView.InstantiationData[1];
        if (eventCode == EventCode.DRAW_CARD_FROM_DECK)
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


        if (whereToCode == EventCode.DRAW_TO_HAND)
        {
            int? viewId = (int?)info.photonView.InstantiationData[2];
            if (viewId != null)
            {
                PhotonView view = PhotonView.Find( (int) viewId);
                Player player = view.GetComponent<Player>();
                player.AddCard(this);
            } 
        }
        else
        {
            if (!photonView.IsMine)
            {
                HideCard();
            }
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


    void RevealCard()
    {
        this.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    void HideCard()
    {
        this.transform.eulerAngles = new Vector3(0, 180, 0);
    }


    public IEnumerator RevealCardForSeconds(float seconds)
    {
        RevealCard();
        yield return new WaitForSeconds(seconds);
        HideCard();
        PhotonNetwork.RaiseEvent(EventCode.END_PLAYER_TURN, null, RaiseEventOptions.Default, SendOptions.SendReliable);
        GameManager.Instance.EndTurn();
    }

    public void SetUpOwner(Player player)
    {
        HideCard();
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
        GameManager.GameState = GameState.PlayCardEffectPhase;
        if (card.Is7or8)
        {
            Debug.Log("Play 7, 8");
            GameManager.Instance.CardPlayed = card;
        }
        else if (card.Is9or10)
        {
            Debug.LogError("Play 9, 10");
        }
        else if (card.IsJackOrQueen)
        {
            Debug.LogError("Play Valet, Dame");
        }
        else if (card.IsBlackKing)
        {
            Debug.LogError("Play black king");
        }
        else
        {
            Debug.LogError("No effect");
            GameManager.Instance.EndTurn();
        }

    }
}
